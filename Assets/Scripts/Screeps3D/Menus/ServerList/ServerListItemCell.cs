using UnityEngine;
using System.Collections;
using Tacticsoft;
using UnityEngine.UI;
using Screeps_API;
using System;

namespace Screeps3D.Menus.ServerList
{
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


        internal void Update(ServerCache server)
        {
            ServerNameLabel.text = server.Name ?? server.Address.HostName; // TODO: perhaps a tooltip on hover with server address?
            ServerAddressHostLabel.text = server.Address.HostName;
            ServerAddressPortLabel.text = server.Address.Port;
            ServerAddressSSLToggle.isOn = server.Address.Ssl;
            //UserCountLabel.text = server.UserCount
            LikesLabel.text = server.LikeCount.ToString();
            //PackageVersionLabel.text = server.PackageVersion
        }

        //private int m_numTimesBecameVisible;
        //public void NotifyBecameVisible() {
        //    m_numTimesBecameVisible++;
        //    m_visibleCountText.text = "# rows this cell showed : " + m_numTimesBecameVisible.ToString();
        //}

    }
}
