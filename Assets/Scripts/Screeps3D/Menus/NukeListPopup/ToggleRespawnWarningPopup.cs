using Assets.Scripts.Screeps3D.Menus.Respawn;
using Screeps3D.Menus.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Screeps3D.Menus.NukeListPopup
{
    public class ToggleRespawnWarningPopup : MainMenuItem
    {
        public RespawnWarningPopup Popup;
        public override string Description
        {
            get { return "Respawn"; }
        }

        public override void Invoke()
        {
            Popup.gameObject.SetActive(!Popup.gameObject.activeSelf);
        }
    }
}
