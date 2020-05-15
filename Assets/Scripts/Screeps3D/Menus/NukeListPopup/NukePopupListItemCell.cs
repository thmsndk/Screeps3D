using UnityEngine;
using System.Collections;
using Tacticsoft;
using UnityEngine.UI;
using Screeps_API;
using System;
using UnityEngine.Events;
using Screeps3D.World.Views;
using Screeps3D;
using TMPro;

namespace Assets.Scripts.Screeps3D.Menus.NukeListPopup
{
    [Serializable]
    public class OnNukeSelected : UnityEvent<NukeMissileOverlay> { }

    //Inherit from TableViewCell instead of MonoBehavior to use the GameObject
    //containing this component as a cell in a TableView
    public class NukePopupListItemCell : TableViewCell
    {
        public Image Progress;

        public Text ImpactRealTime;

        public Text LaunchRoom;
        public Text LaunchTime;

        public BadgeAndLabel LaunchRoomOwner;
        

        public Text ImpactRoom;
        public Text ImpactTime;

        public BadgeAndLabel ImpactRoomOwner;

        public Text TicksLeft;
        public Text ETAEarly;
        public Text ETALate;


        public OnNukeSelected onSelected;

        private NukeMissileOverlay nuke;
        private Image buttonImage;

        void Start()
        {
            buttonImage = GetComponent<Image>();
        }

        public void Selected()
        {
            if (onSelected != null)
            {
                onSelected.Invoke(nuke);
            }
        }

        internal void SetCellItem(NukeMissileOverlay nuke)
        {
            this.nuke = nuke;

            LaunchRoom.text = nuke.LaunchRoom?.RoomName;
            LaunchTime.text = nuke.InitialLaunchTick.ToString();

            // TODO: we need to queue a map-stats lookup if we can't find it. but what about rate limits?
            var launchRoomInfo = MapStatsUpdater.Instance.GetRoomInfo(nuke.LaunchRoom?.RoomName);

            LaunchRoomOwner.SetOwner(launchRoomInfo?.User);

            ImpactRoom.text = nuke.ImpactRoom?.RoomName;
            ImpactTime.text = nuke.LandingTime.ToString();

            var impactRoomInfo = MapStatsUpdater.Instance.GetRoomInfo(nuke.ImpactRoom?.RoomName);

            ImpactRoomOwner.SetOwner(impactRoomInfo?.User);

            TicksLeft.text = (nuke.LandingTime - ScreepsAPI.Time).ToString();

            ETAEarly.text = nuke.EtaEarly.ToString();
            ETALate.text = nuke.EtaLate.ToString();

            Progress.fillAmount = nuke.Progress;// / 100f;

            //if (buttonImage != null)
            //{
            //    buttonImage.color = server.Selected ? UnityEngine.Random.ColorHSV() : Color.white;
            //}

            //OnlineIndicator.color = server.Online.HasValue ? server.Online.Value ? Color.green : Color.red : Color.yellow;

            //ServerNameLabel.text = server.Name ?? server.Address.HostName; // TODO: perhaps a tooltip on hover with server address?

            //ServerAddressHostLabel.text = server.Address.HostName;
            //ServerAddressPortLabel.text = server.Address.Port;
            //ServerAddressSSLToggle.isOn = server.Address.Ssl;

            //UserCountLabel.text = server.Users.ToString();

            //if (!server.Official)
            //{
            //    LikesLabel.text = server.LikeCount.ToString();
            //    foreach (Transform child in LikesLabel.transform)
            //    {
            //        child.gameObject.SetActive(true);
            //    }
            //}
            //else
            //{
            //    LikesLabel.text = string.Empty;
            //    foreach (Transform child in LikesLabel.transform)
            //    {
            //        child.gameObject.SetActive(false);
            //    }
            //}


            //PackageVersionLabel.text = server.Version;
        }

        private void Update()
        {
            var impactTimeSpan = nuke.EtaEarly - DateTime.Now;
            ImpactRealTime.text = string.Format("{0:D2}:{1:D2}:{2:D2}", impactTimeSpan.Hours, impactTimeSpan.Minutes, impactTimeSpan.Seconds);
        }
    }
}
