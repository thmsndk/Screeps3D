using UnityEngine;
using System.Collections;
using Tacticsoft;
using Screeps_API;
using System;
using UnityEngine.Events;
using Screeps3D.Menus.ServerList;
using Screeps3D.World.Views;
using System.Collections.Generic;
using System.Linq;
using Screeps3D.Rooms;
using UnityEngine.UI;
using System.Text;
using TMPro;

namespace Assets.Scripts.Screeps3D.Menus.NukeListPopup
{
    //An example implementation of a class that communicates with a TableView
    public class NukePopupListTableViewController : MonoBehaviour, ITableViewDataSource
    {
        public TextMeshProUGUI HeaderLabel;
        public NukePopupListItemCell m_cellPrefab;
        public TableView m_tableView;

        public int m_numRows;
        private int m_numInstancesCreated = 0;

        public OnNukeSelected onNukeSelected;
        private List<NukeMonitor.NukeData> _nukes;

        //Register as the TableView's delegate (required) and data source (optional)
        //to receive the calls

        private void Start()
        {
            m_tableView.dataSource = this;
        }

        

        #region ITableViewDataSource

        //Will be called by the TableView to know how many rows are in this table
        public int GetNumberOfRowsForTableView(TableView tableView)
        {
            // Should return the amount of servers in the list
            return _nukes?.Count ?? 0;
        }

        //Will be called by the TableView to know what is the height of each row
        public float GetHeightForRowInTableView(TableView tableView, int row)
        {
            return ((RectTransform)m_cellPrefab.transform).rect.height;
        }

        //Will be called by the TableView when a cell needs to be created for display
        public TableViewCell GetCellForRowInTableView(TableView tableView, int row)
        {
            var cell = tableView.GetReusableCell(m_cellPrefab.reuseIdentifier) as NukePopupListItemCell;
            if (cell == null)
            {
                cell = Instantiate(m_cellPrefab) as NukePopupListItemCell;
                cell.name = "NukePopupListItemCell_" + (++m_numInstancesCreated).ToString();
                //cell.onSelected.AddListener(OnSelected);
            }

            var nuke = _nukes[row];

            cell.SetCellItem(nuke);
            return cell;
        }

        #endregion

        #region Table View event handlers

        internal void UpdateList(Dictionary<string, List<NukeMonitor.NukeData>> nukes)
        {
            var sb = new StringBuilder();
            foreach (var shardNukes in nukes.OrderBy(n => n.Key))
            {
                sb.Append($"<b>{shardNukes.Key}:</b> {shardNukes.Value.Count}  ");
            }

            HeaderLabel.text = sb.ToString();

            _nukes = nukes.SelectMany(n => n.Value).OrderBy(n => n.EtaEarly).ToList();
            m_tableView.ReloadData();
        }


        #endregion

        private void OnSelected(NukeMonitor.NukeData nuke)
        {
            onNukeSelected?.Invoke(nuke);
        }
    }
}