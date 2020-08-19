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
    /* 
     * TODO: room claim assistant https://github.com/Esryok/screeps-browser-ext/blob/master/room-claim-assistant.user.js 
     *  this overlay should be enabled when in spawn mode. should it be mixed with prohibited spawn though or should it just enhance it? you can't claim prohibited rooms, so we probably just want to enhance it.
     *  render mineral type, high in the air, this assistant onlky renders minerals for 2 source rooms, we might want to render a small scale mineral indicator for 1 source rooms, not sure how to indicate density then
     *  render a floating status text to indicate a "status" or recommendation
     *  recommend if it has two sources and a controller, nobody else owns it,
     *  and user hasn't already claimed
     *  could probably use a flags enum.
     *  do we render the SVG icon for minerals, or do we render our model? both?
     *  
     *  colors
     *  .claim-assist { pointer-events: none; }
        .claim-assist.not-recommended { background: rgba(192, 192, 50, 0.3); } #AAAAAA
        .claim-assist.recommended { background: rgba(25, 255, 25, 0.2); }
        .claim-assist.owned { background: rgba(50, 50, 255, 0.2); }
        .claim-assist.signed { background: rgba(255, 128, 0, 0.35); }
        .claim-assist.prohibited { background: rgba(255, 50, 50, 0.2); }
        .room-prohibited { display: none; }
     */
    public class RespawnManager : BaseSingleton<RespawnManager>
    {

        [SerializeField] private ToolChooser _toolChooser = default;
        [SerializeField] private ChooseConstructionSite _chooseConstruction = default;
        [SerializeField] private LostSpawnPopup _lostSpawnPopup = default;
        [SerializeField] private RespawnWarningPopup _respawnWarningPopup = default;
        [SerializeField] private RoomChooser _roomChooser = default;

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
            try
            {
                WorldStatusUpdater.Instance.OnWorldStatusChanged -= OnWorldStatusChanged;
                _lostSpawnPopup.OnCancel -= LostSpawnPopupCancelClicked;
                _lostSpawnPopup.OnRespawn -= LostSpawnPopupRespawnClicked;
                PlaceFirstSpawn.Instance.OnFirstSpawnPlaced -= FirstSpawnPlaced;

                _respawnWarningPopup.OnCancel -= RespawnWarningPopupCancelClicked;
                _respawnWarningPopup.OnRespawn -= RespawnWarningPopupRespawnClicked;
            }
            catch (MissingSingletonException ex)
            {
                // This is okay, we are being destroyed, but why are we being destroyed? due to scene load?
            }
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
            if (WorldStatusUpdater.Instance.WorldStatus == WorldStatus.Empty)
            {
                // "trigger" a fake respawn
                WorldStatusUpdater.Instance.SetWorldStatus(WorldStatus.Empty);
                return;
            }

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

                        _toolChooser?.SelectTool(ToolType.Selection);
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

                    if (previous == WorldStatus.None)
                    {
                        _toolChooser?.SelectTool(ToolType.Selection);
                    }
                    else
                    {
                        _toolChooser?.SelectTool(ToolType.Spawn);
                    }

                    _roomChooser.GetAndChooseRandomWorldStartRoom();

                    // Get respawn prohibited rooms and cache them

                    break;
                default:
                    break;
            }

            Debug.Log($"[WorldStatus] {previous} => {current}");
        }
    }
}