using Assets.Scripts.Screeps3D.Tools.Selection;
using Common;
using Screeps_API;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    public class ConstructedWall : Structure, IHitpointsObject, IButtons
    {
        public List<IRoomObjectPanelButton> GetButtonActions()
        {
            /*
             * <!-- ngIf: !Game.readOnly && (Room.roomController.user == Game.player 
             * || Room.roomController.user == 0 && Game.player == Me()._id) && (Room.selectedObject.type != 'constructedWall' 
             * || !Room.selectedObject.decayTime 
             * || Room.selectedObject.user) -->
             */
            var controller = this.Room.Objects.SingleOrDefault(ro => ro.Value.Type == Constants.TypeController).Value as Controller;
            var myRoom = ScreepsAPI.Me.UserId == controller?.Owner?.UserId;
            if (!myRoom)
            {
                return new List<IRoomObjectPanelButton>();
            }

            return new List<IRoomObjectPanelButton>
            {
                new SelectionRoomObjectButton<ConstructedWall>("Destroy this structure", (structure) => {
                    // Question
                    // Accept => destroy
                    PlayerInput.AskQuestion($"Are you sure you want to delete\n{structure.Type} {structure.Id}", (bool yes) => {
                    if (yes)
                    {
                        ScreepsAPI.Http.DestroyStructureObjectIntent(
                        structure.Room.ShardName,
                        structure.Room.RoomName,
                        structure.Id,
                        ScreepsAPI.Me.UserId,
                        onSuccess: jsonString =>
                        {
                            var result = new JSONObject(jsonString);

                            var ok = result["ok"];

                            if (ok.n == 1)
                            {
                                structure.HideObject(structure.Room);
                                structure.DetachView();
                            }
                            else
                            {
                                // error
                            }
                        });
                    }
                    });
                }),
            };
        }
    }
}