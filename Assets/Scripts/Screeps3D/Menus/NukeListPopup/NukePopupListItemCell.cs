using UnityEngine;
using System.Collections;
using Tacticsoft;
using UnityEngine.UI;
using Screeps_API;
using System;
using UnityEngine.Events;
using Screeps3D.World.Views;

namespace Assets.Scripts.Screeps3D.Menus.NukeListPopup
{
    [Serializable]
    public class OnNukeSelected : UnityEvent<NukeMissileOverlay> { }

    //Inherit from TableViewCell instead of MonoBehavior to use the GameObject
    //containing this component as a cell in a TableView
    public class NukePopupListItemCell : TableViewCell
    {
        public Text LaunchRoom;
        public Text LaunchTime;
        public Text ImpactRoom;
        public Text ImpactTime;
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

            ImpactRoom.text = nuke.ImpactRoom?.RoomName;
            ImpactTime.text = nuke.LandingTime.ToString();

            TicksLeft.text = (nuke.LandingTime - ScreepsAPI.Time).ToString();

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
    }
}
