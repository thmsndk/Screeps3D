using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Screeps3D.Menus.NukeListPopup
{
    public class NukeListPopup : MonoBehaviour
    {
        [SerializeField]private NukePopupListTableViewController _nukeListTableViewController;

        private void Awake()
        {
            var nukes = NukeMonitor.Instance.Nukes;

            // TODO: wire up so we listen on updates, e.g. when nukemonitor refreshes the nuke list
            Debug.Log(nukes.Count + " nukes!!!!!!");

            NukeMonitor.Instance.OnNukesRefreshed += NukesRefreshed;
        }

        private void OnDestroy()
        {
            NukeMonitor.Instance.OnNukesRefreshed -= NukesRefreshed;
        }

        private void NukesRefreshed()
        {
            var nukes = NukeMonitor.Instance.Nukes;
            _nukeListTableViewController.UpdateList(nukes);
            Debug.Log(nukes.Count + " nukes!!!!!! after refresh");
        }
    }
}