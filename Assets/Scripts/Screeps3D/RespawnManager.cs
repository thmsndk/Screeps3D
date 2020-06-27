using System;
using System.Collections.Generic;
using Common;
using Screeps3D.Rooms;
using Screeps_API;
using UnityEngine;
using System.Linq;
using Screeps3D.Player;
using System.Collections;
using System.Text.RegularExpressions;
using Screeps3D.Tools;
using Assets.Scripts.Screeps3D.Tools.ConstructionSite;
using Assets.Scripts.Screeps3D.Menus.Respawn;

namespace Screeps3D
{
    // TODO: make sure it does not toggle on "red" when mode is normal
    public class RespawnManager : BaseSingleton<RespawnManager>
    {

        [SerializeField] private ToolChooser _toolChooser = default;
        [SerializeField] private ChooseConstructionSite _chooseConstruction = default;
        [SerializeField] private LostSpawnPopup _lostSpawnPopup = default;
        [SerializeField] private RespawnWarningPopup _respawnWarningPopup = default;

        private void Awake()
        {
            WorldStatusUpdater.Instance.OnWorldStatusChanged += OnWorldStatusChanged;
            PlaceFirstSpawn.Instance.OnFirstSpawnPlaced += FirstSpawnPlaced;
        }

        private void FirstSpawnPlaced()
        {
            WorldStatusUpdater.Instance.SetWorldStatus(WorldStatus.Normal);
        }

        private void Start()
        {
            _lostSpawnPopup.OnCancel += LostSpawnPopupCancelClicked;
            _lostSpawnPopup.OnRespawn += LostSpawnPopupRespawnClicked;

            _respawnWarningPopup.OnCancel += RespawnWarningPopupCancelClicked;
            _respawnWarningPopup.OnRespawn += RespawnWarningPopupRespawnClicked;
        }

        private void OnDestroy()
        {
            WorldStatusUpdater.Instance.OnWorldStatusChanged -= OnWorldStatusChanged;
            _lostSpawnPopup.OnCancel -= LostSpawnPopupCancelClicked;
            _lostSpawnPopup.OnRespawn -= LostSpawnPopupRespawnClicked;
            PlaceFirstSpawn.Instance.OnFirstSpawnPlaced -= FirstSpawnPlaced;

            _respawnWarningPopup.OnCancel -= RespawnWarningPopupCancelClicked;
            _respawnWarningPopup.OnRespawn -= RespawnWarningPopupRespawnClicked;
        }

        private void LostSpawnPopupCancelClicked()
        {
            _lostSpawnPopup?.gameObject?.SetActive(false);
        }

        private void LostSpawnPopupRespawnClicked()
        {
            _lostSpawnPopup?.gameObject?.SetActive(false);
            Respawn();
        }

        private void RespawnWarningPopupCancelClicked()
        {
            _respawnWarningPopup?.gameObject?.SetActive(false);
        }

        private void RespawnWarningPopupRespawnClicked()
        {
            _respawnWarningPopup?.gameObject?.SetActive(false);
            Respawn();
        }

        private static void Respawn()
        {
            ScreepsAPI.Http.Respawn((jsonResponse) =>
            {
                var result = new JSONObject(jsonResponse);
                var ok = result["ok"];

                if (ok != null && ok.n == 1)
                {
                    WorldStatusUpdater.Instance.SetWorldStatus(WorldStatus.Empty);
                }
            });
        }

        private void OnWorldStatusChanged(WorldStatus previous, WorldStatus current)
        {
            switch (current)
            {
                case WorldStatus.None:
                    break;
                case WorldStatus.Normal:
                    if (previous == WorldStatus.Empty)
                    {
                        _toolChooser?.Show(ToolType.Flag);
                        _toolChooser?.Show(ToolType.Construction);
                        _toolChooser?.Hide(ToolType.Spawn);
                    }
                    break;
                case WorldStatus.Lost:
                    if (previous != WorldStatus.Lost)
                    {
                        _lostSpawnPopup?.gameObject?.SetActive(true);
                    }

                    break;
                case WorldStatus.Empty:
                    _toolChooser?.Hide(ToolType.Flag);
                    _toolChooser?.Hide(ToolType.Construction);
                    _toolChooser?.Show(ToolType.Spawn);

                    // Get respawn prohibited rooms and cache them

                    break;
                default:
                    break;
            }

            Debug.Log($"[WorldStatus] {previous} => {current}");
        }
    }
}