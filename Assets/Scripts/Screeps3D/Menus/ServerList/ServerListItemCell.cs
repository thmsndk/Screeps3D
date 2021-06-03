using UnityEngine;
using System.Collections;
using Tacticsoft;
using UnityEngine.UI;
using Screeps_API;
using System;
using UnityEngine.Events;
using System.Text;
using System.Linq;
using Assets.Scripts.Screeps3D;

namespace Screeps3D.Menus.ServerList
{
    [System.Serializable]
    public class OnServerSelected : UnityEvent<IScreepsServer> { }

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

        public Text ShardNames;

        public TooltipTrigger tooltipTrigger;

        public BadgeAndLabel badge;

        public OnServerSelected onServerSelected;
        public IScreepsServer Server { get; private set; }
        
        private Image buttonImage;

        void Start()
        {
            buttonImage = GetComponent<Image>();
        }

        public void Selected()
        {
            if (onServerSelected != null)
            {
                onServerSelected.Invoke(Server);
            }
        }

        internal void SetServer(IScreepsServer server)
        {
            this.Server = server;
           
            OnlineIndicator.color = server.Online.HasValue ? server.Online.Value ? Color.green : Color.red : Color.yellow;

            ServerNameLabel.text = server.Name ?? server.Address.HostName; // TODO: perhaps a tooltip on hover with server address?
            //ShardNames.text = string.Join(" ", server.Meta.ShardNames) + Environment.NewLine + string.Join(" ", server.Meta.Features);
            var sb = new StringBuilder();

            sb.AppendLine(@$"<b>Shards</b>: {string.Join(" ", server.Meta.ShardNames)}");


            if (server.Meta.Features.Count > 0)
            {
                sb.AppendLine("<b>Features</b>:");
            }

            foreach (var item in server.Meta.Features.OrderBy(f => f.Key))
            {
                var feature = item.Key.Trim('"');
                var version = item.Value.Trim('"');
                sb.AppendLine($"{feature}: {version}");
            }

            tooltipTrigger.Content = sb.ToString();

            ServerAddressHostLabel.text = $"{server.Address.Http()}";
            //ServerAddressPortLabel.text = server.Address.Port;
            //ServerAddressSSLToggle.isOn = server.Address.Ssl;

            UserCountLabel.text = server.Meta.Users.ToString();

            if (!server.Official)
            {
                LikesLabel.text = server.Meta.LikeCount.ToString();
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

            PackageVersionLabel.text = server.Meta.Version;

            this.badge.SetOwner(server.Meta.Me);
        }

        internal void SetSelectedState(IScreepsServer server)
        {
            if (buttonImage != null)
            {
                buttonImage.color = this.Server == server ? UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f) : Color.white;
            }
        }
    }
}
