using UnityEngine;
using System.Collections;
using Tacticsoft;
using UnityEngine.UI;
using Screeps_API;
using System;
using UnityEngine.Events;

namespace Screeps3D.Menus.ServerList
{
    [System.Serializable]
    public class OnServerSelected : UnityEvent<ServerCache> { }

    //Inherit from TableViewCell instead of MonoBehavior to use the GameObject
    //containing this component as a cell in a TableView
    public class ServerListItemCell : TableViewCell
    {
        public Image OnlineIndicator;
        public Text ServerNameLabel;
        public Text ServerAddressHostLabel;
        public Text ServerAddressPortLabel;
        public Toggle ServerAddressSSLToggle;
        public Text UserCountLabel;
        public Text LikesLabel;
        public Text PackageVersionLabel;
        
        public OnServerSelected onServerSelected;

        private ServerCache server;
        private Image buttonImage;

        void Start()
        {
            buttonImage = GetComponent<Image>();
        }

        public void Selected()
        {
            if (onServerSelected != null)
            {
                onServerSelected.Invoke(server);
            }
        }

        internal void SetServer(ServerCache server)
        {
            this.server = server;

            if (buttonImage != null) {
                buttonImage.color = server.Selected ? UnityEngine.Random.ColorHSV() : Color.white;
            }
            
            OnlineIndicator.color = server.Online.HasValue ? server.Online.Value ? Color.green : Color.red : Color.yellow;

            ServerNameLabel.text = server.Name ?? server.Address.HostName; // TODO: perhaps a tooltip on hover with server address?

            ServerAddressHostLabel.text = server.Address.HostName;
            ServerAddressPortLabel.text = server.Address.Port;
            ServerAddressSSLToggle.isOn = server.Address.Ssl;

            UserCountLabel.text = server.Users.ToString();

            if (!server.Official)
            {
                LikesLabel.text = server.LikeCount.ToString();
                foreach (Transform child in LikesLabel.transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
            else
            {
                LikesLabel.text = string.Empty;
                foreach (Transform child in LikesLabel.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }


            PackageVersionLabel.text = server.Version;
        }
    }
}
