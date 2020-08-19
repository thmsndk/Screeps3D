using Assets.Scripts.Common.SettingsManagement;
using Common;
using Screeps_API;
using Screeps3D;
using Screeps3D.World.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TODO: fix odd pixelation / white edges on badges

namespace Assets.Scripts.Screeps3D.World.Views
{
    public class OwnerControllerLevelView : MonoBehaviour, IWorldOverlayViewComponent
    {
        [Setting("Overlay/Owner View", "Show roomname")]
        private static bool showRoomName = true;

        private static float overlayHeight = 10f;

        private static float overlayCameraHeightThreshold = -200;
        [Setting("Overlay/Owner View", "Camera Angle Threshold")]
        public static float overlayCameraAngleThreshold = 0.25f;

        private OwnerControllerLevelData data;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GameObject _badgeScaledContent;
        [SerializeField] private Image _badge;
        [SerializeField] private Image _badgeMask;
        [SerializeField] private TMP_Text _username;
        [SerializeField] private TMP_Text _roomName;
        [SerializeField] private TMP_Text _controllerLevel;
        [SerializeField] private Image _controllerLevelImage;

        private int? _roomLevel = 0;

        [Setting("Overlay/Owner View", "Camera Height Threshold")]
        public static float OverlayCameraHeightThreshold {
            get => overlayCameraHeightThreshold * -1;
            set => overlayCameraHeightThreshold =  value * -1;
        }

        public void Init(object o)
        {
            this.data = o as OwnerControllerLevelData;
            // TODO: we need the room position and move it there
            //arcRenderer.point1.transform.position = Overlay.LaunchRoom.Position + new Vector3(25, 0, 25); // Center of the room, because we do not know where the nuke is, could perhaps scan for it and correct it?
            this.gameObject.transform.position = data.Room.Position + new Vector3(25, overlayHeight, 25); // Center of the room
            _roomName.text = data.RoomInfo.RoomName;
            UpdateLevel();
            var user = data.RoomInfo.User;


            // TODO: do zomething based on player height / zoom level

            if (OverlayShouldBeShown(user))
            {
                _roomName.enabled = showRoomName;
                _canvas.gameObject.SetActive(true);
                UpdateUser(user);
                ScaleBadge();
            }
            else
            {
                _canvas.gameObject.SetActive(false);
            }

        }

        private void UpdateLevel()
        {
            if (_roomLevel != data.RoomInfo.Level)
            {
                _roomLevel = data.RoomInfo.Level;

                _controllerLevel.text = data.RoomInfo.Level?.ToString() ?? string.Empty;

                switch (_roomLevel)
                {
                    case 8: _controllerLevelImage.fillAmount = 1f; break;
                    case 7: _controllerLevelImage.fillAmount = 0.875f; break;
                    case 6: _controllerLevelImage.fillAmount = 0.75f; break;
                    case 5: _controllerLevelImage.fillAmount = 0.625f; break;
                    case 4: _controllerLevelImage.fillAmount = 0.5f; break;
                    case 3: _controllerLevelImage.fillAmount = 0.375f; break;
                    case 2: _controllerLevelImage.fillAmount = 0.25f; break;
                    case 1: _controllerLevelImage.fillAmount = 0.125f; break;
                    default:
                        _controllerLevelImage.fillAmount = 0f;
                        break;
                }
            }
        }

        private void ScaleBadge()
        {
            float scale = 1f;
            int level = data.RoomInfo.Level ?? 1;
            if (data.RoomInfo.IsReserved)
            {
                level = 1;
                //_badge.color.a = 0.5f; // TODO: make it alpha when reserved
            }

            switch (level)
            {
                case 7: scale = 0.9f; break;
                case 6: scale = 0.8f; break;
                case 5: scale = 0.75f; break;
                case 4: scale = 0.7f; break;
                case 3: scale = 0.6f; break;
                case 2: scale = 0.55f; break;
                case 1: scale = 0.5f; break;
                default:
                    break;
            }

            if (_badgeScaledContent.transform.localScale.x != scale)
            {
                _badgeScaledContent.transform.localScale = new Vector3(scale, scale, 1f); 
            }
        }

        private void UpdateUser(ScreepsUser user)
        {
            if (_username.text != user.Username)
            {
                // TODO: should we cache the sprite? the badge will exist multiple places, 
                _badge.sprite = Sprite.Create(user.Badge,
                            new Rect(0.0f, 0.0f, BadgeManager.BADGE_SIZE, BadgeManager.BADGE_SIZE), new Vector2(.5f, .5f));

                _username.text = user.Username; 
            }
        }

        private bool OverlayShouldBeShown(ScreepsUser user)
        {
            return user != null && data.Room.Shown;
        }



        private void Update()
        {
            // if pivot.localRotation.x is 0.70 we are looking top down, 0 is at room level / in the ground, we need to determine some sort of treshhold
            // boom.position.z needs to be < - 200
            //Debug.Log(CameraRig.Position);
            

            if (OverlayShouldBeShown(data?.RoomInfo?.User) && CameraHasCorrectHeight())
            {
                _roomName.enabled = showRoomName;
                UpdateUser(data.RoomInfo.User);
                ScaleBadge();
                UpdateLevel();
                _canvas.gameObject.SetActive(true);
                //this.transform.LookAt(Camera.main.transform.position);
                //this.transform.rotation = Quaternion.LookRotation(Camera.main.transform.position) * Quaternion.Euler(0, 0, 90);
                //Debug.Log(Camera.main.transform.rotation);


                // TOP DOWN camera, Pivot Rotation.X = 90, y = 0 Z = 0
                // TOP DOWN LOOKING EAST X = 90 Z = -90
                // SOUTH  X = 90 Z = 175
                // WEST = X = 90 Z = 90
                // We need the absolute value, cause it goes negative when we move the other way
                // Pivot.X controls the angle, if it is 58.5 and we are looking east Z = 0 and Y = 90
                //var cameraAtCanvasHeight = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, this.transform.position.z);
                //Debug.Log(Vector3.Angle(this.transform.position, cameraAtCanvasHeight));


                // W ---- N --- E
                //    y -50 ---- y = 50

                //else if (Input.GetKeyDown(KeyCode.H))
                //{
                //    // Rotates corect against  east // rotating things messes up the height / angle check though might need to be refined
                //    _canvas.transform.localRotation *= Quaternion.AngleAxis(90, Vector3.back);
                //}

            }
            else
            {

                _canvas.gameObject.SetActive(false);
            }
        }

        private static bool CameraHasCorrectHeight()
        {
            return /*CameraRig.PivotLocalRotation.x > overlayCameraAngleThreshold &&*/ CameraRig.BoomLocalPosition.z < overlayCameraHeightThreshold;
        }

        // TODO: we need an update trigger for when new data is recieved.
    }
}
