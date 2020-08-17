using Screeps_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Screeps3D.Menus.RecentBattlesListPopup
{
    public class RecentBattlesListPopup : MonoBehaviour
    {
        [SerializeField] private RecentBattlesListPopupTableViewController _tableViewController;

        private void Awake()
        {


        }

        private void Start()
        {
            ScreepsAPI.Warpath.OnClassificationsUpdated += ClassificationsUpdated;
        }

        private void OnEnable()
        {
            ClassificationsUpdated();
            ScreepsAPI.Warpath.OnClassificationsUpdated += ClassificationsUpdated;
        }

        private void OnDisable()
        {
            if (ScreepsAPI.Warpath != null)
            {
                ScreepsAPI.Warpath.OnClassificationsUpdated -= ClassificationsUpdated;
            }
        }

        private void OnDestroy()
        {
            if (ScreepsAPI.Warpath != null)
            {
                ScreepsAPI.Warpath.OnClassificationsUpdated -= ClassificationsUpdated;
            }
        }

        private void ClassificationsUpdated()
        {
            Debug.Log("ClassificationsUpdated");
            if (ScreepsAPI.Warpath != null)
            {
                var battles = ScreepsAPI.Warpath.Rooms;
                _tableViewController.UpdateList(battles);
                //Debug.Log(battles.Count + " battles after refresh");
            }
        }
    }
}