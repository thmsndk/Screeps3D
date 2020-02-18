using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Screeps_API;
using Screeps3D.Menus.Options;
using Screeps3D.RoomObjects;
using Screeps3D.RoomObjects.Views;
using Screeps3D.Rooms.Views;
using UnityEngine;

namespace Screeps3D.Tools.Selection
{
    public class EditFlag : BaseSingleton<EditFlag>
    {
        [SerializeField] private EditFlagPopup _editFlagPopup;

        private void Start()
        {
            _editFlagPopup.OnFlagCreated += FlagCreated;
            _editFlagPopup.OnFlagColorChanged += FlagColorChanged;
            _editFlagPopup.OnCancel += CancelFlagEdit;
        }

        private void OnDestroy()
        {
            _editFlagPopup.OnFlagCreated -= FlagCreated;
            _editFlagPopup.OnFlagColorChanged -= FlagColorChanged;
            _editFlagPopup.OnCancel -= CancelFlagEdit;
        }

        private void CancelFlagEdit()
        {
            ToggleEditFlagPopup(false);
        }
        private void FlagColorChanged()
        {
            ToggleEditFlagPopup(false);
        }

        private void FlagCreated()
        {
            ToggleEditFlagPopup(false);
        }

        public void ShowEditDialog(Flag flag)
        {
            this.enabled = true;
            ToggleEditFlagPopup(true);
            _editFlagPopup.Load(flag);
        }

        public void DeleteFlag(Flag flag)
        {
            PlayerInput.AskQuestion($"Are you sure you want to delete\n{flag.Name}", (bool yes) => {
                if (yes)
                {
                    ScreepsAPI.Http.RemoveFlag(
                    flag.Room.ShardName,
                    flag.Room.RoomName,
                    flag.Name,
                    onSuccess: jsonString =>
                    {
                        var result = new JSONObject(jsonString);

                        var ok = result["ok"];

                        if (ok.n == 1)
                        {
                            flag.HideObject(flag.Room);
                            flag.DetachView();
                            flag.Room.Objects.Remove(flag.Name); // TODO: we do this because flags are not removed if they get deleted by AI or another client/api call
                        }
                        else
                        {
                            // error
                        }
                    });
                }
            });
        }

        private void ToggleEditFlagPopup(bool? active = null)
        {
            if (!active.HasValue)
            {
                active = _editFlagPopup.gameObject.activeSelf;
            }

            this.enabled = active.Value;

            _editFlagPopup?.gameObject?.SetActive(active.Value);
        }

        private void OnDisable()
        {
            ToggleEditFlagPopup(false);
        }
    }
}