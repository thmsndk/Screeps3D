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
using TMPro;
using Assets.Scripts.Screeps_API.ConsoleClientAbuse;

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
        public Text UserCountLabel;
        public Text LikesLabel;
        public Text PackageVersionLabel;

        public TooltipTrigger tooltipTrigger;

        public TooltipTrigger BadgeTooltip;

        public TooltipTrigger WarningTooltip;

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
            // set GCL
            //gclLevel.text = server.Meta.GlobalControlLevel.ToString();
            //gclLevel.enabled = server.Meta.GlobalControlLevel >= 1;


            var worldStatusColor = "#FFFFFF";
            var worldStatusDescription = string.Empty;
            switch (server.Meta.WorldStatus)
            {
                case WorldStatus.None:
                    break;
                case WorldStatus.Normal:
                    worldStatusColor = "#32a852"; //green
                    worldStatusDescription = " - Everything is fine";
                    break;
                case WorldStatus.Lost:
                    worldStatusColor = "#f3f70c"; // yellow
                    worldStatusDescription = " - Currently have no spawns";
                    break;
                case WorldStatus.Empty:
                    worldStatusColor = "#d11111"; // red
                    worldStatusDescription = " - Respawned, place spawn";
                    break;
                default:
                    break;
            }

            BadgeTooltip.Header = $"<size=25><b><color={worldStatusColor}>{server.Meta.WorldStatus}</color></b></size>";
            BadgeTooltip.Content = $"{worldStatusDescription}\n\n<b>Global Control Level:</b> {server.Meta.GlobalControlLevel}";


            var warningMessage = new StringBuilder();

            if (!server.Official && server.Online.HasValue && server.Online.Value)
            {
                if (server.Meta.Version != null && server.Meta.Version.Contains("xxscreeps"))
                {
                    warningMessage.AppendLine("<b>xxscreeps</b> is not fully supported yet, some features might not work.");
                }

                var hasAdminUtils = server.Meta.Features.ContainsKey("screepsmod-admin-utils");

                if (!hasAdminUtils)
                {
                    warningMessage.AppendLine("<b>screepsmod-admin-utils</b> is required on the server for a proper experience");
                }

                var hasAuthMod = server.Meta.Features.ContainsKey("screepsmod-auth");

                if (!hasAuthMod)
                {
                    warningMessage.AppendLine("<b>screepsmod-auth</b> is required on the server to be able to connect!");
                }

                if (warningMessage.Length > 0)
                {
                    WarningTooltip.gameObject.SetActive(true);
                    WarningTooltip.Content = warningMessage.ToString();
                }
                else
                {
                    WarningTooltip.gameObject.SetActive(false);
                }
            }
            else
            {
                WarningTooltip.gameObject.SetActive(false);
            }
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
