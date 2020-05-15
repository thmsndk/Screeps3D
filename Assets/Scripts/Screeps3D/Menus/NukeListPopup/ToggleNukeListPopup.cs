using Screeps3D.Menus.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Screeps3D.Menus.NukeListPopup
{
    public class ToggleNukeListPopup : MainMenuItem
    {
        public NukeListPopup Popup;
        public override string Description
        {
            get { return "Toggle Nuke List"; }
        }

        public override void Invoke()
        {
            Popup.gameObject.SetActive(!Popup.gameObject.activeSelf);
        }
    }
}
