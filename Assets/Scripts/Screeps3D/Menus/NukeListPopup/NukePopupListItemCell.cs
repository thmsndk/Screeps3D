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
            LaunchTime.text = $"Tick {nuke.InitialLaunchTick.ToString()}"; 

            // TODO: we need to queue a map-stats lookup if we can't find it. but what about rate limits?
            var launchRoomInfo = MapStatsUpdater.Instance.GetRoomInfo(nuke.LaunchRoom?.RoomName);

            LaunchRoomOwner.SetOwner(launchRoomInfo?.User);

            ImpactRoom.text = nuke.ImpactRoom?.RoomName;

            ImpactTime.text = $"Tick {nuke.LandingTime.ToString()}";

            var impactRoomInfo = MapStatsUpdater.Instance.GetRoomInfo(nuke.ImpactRoom?.RoomName);

            ImpactRoomOwner.SetOwner(impactRoomInfo?.User);

            ETAEarly.text = nuke.EtaEarly.ToString();
            ETALate.text = nuke.EtaLate.ToString();

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
            var initialLaunchTick = Math.Max(nuke.LandingTime - Constants.NUKE_TRAVEL_TICKS, 0);
            var progress = (float)(ScreepsAPI.Time - initialLaunchTick) / Constants.NUKE_TRAVEL_TICKS;
            Progress.fillAmount = progress;// / 100f;

            var ticksLeft = (nuke.LandingTime - ScreepsAPI.Time);
            TicksLeft.text = $"Ticks remaining {ticksLeft.ToString()}";

            var impactTimeSpan = nuke.EtaEarly - DateTime.Now;
            ImpactRealTime.text = $"Impact in {Environment.NewLine}{impactTimeSpan.Days:D2}d {impactTimeSpan.Hours:D2}h {impactTimeSpan.Minutes:D2}m {impactTimeSpan.Seconds:D2}s";
        }
    }
}
