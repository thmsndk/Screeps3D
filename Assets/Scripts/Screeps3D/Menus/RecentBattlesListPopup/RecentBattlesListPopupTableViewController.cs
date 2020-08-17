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

namespace Assets.Scripts.Screeps3D.Menus.RecentBattlesListPopup
{
    //An example implementation of a class that communicates with a TableView
    public class RecentBattlesListPopupTableViewController : MonoBehaviour, ITableViewDataSource
    {
        public TextMeshProUGUI HeaderLabel;
        public RecentBattlesListPopupListItemCell m_cellPrefab;
        public TableView m_tableView;

        public int m_numRows;
        private int m_numInstancesCreated = 0;

        public OnBattleSelected onBattleSelected;
        private List<Warpath.WarpathRoom> _battles;

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
            return _battles?.Count ?? 0;
        }

        //Will be called by the TableView to know what is the height of each row
        public float GetHeightForRowInTableView(TableView tableView, int row)
        {
            return ((RectTransform)m_cellPrefab.transform).rect.height;
        }

        //Will be called by the TableView when a cell needs to be created for display
        public TableViewCell GetCellForRowInTableView(TableView tableView, int row)
        {
            var cell = tableView.GetReusableCell(m_cellPrefab.reuseIdentifier) as RecentBattlesListPopupListItemCell;
            if (cell == null)
            {
                cell = Instantiate(m_cellPrefab) as RecentBattlesListPopupListItemCell;
                cell.name = "RecentBattlesListPopupListItemCell_" + (++m_numInstancesCreated).ToString();
                //cell.onSelected.AddListener(OnSelected);
            }

            var battle = _battles[row];

            cell.SetCellItem(battle);
            return cell;
        }

        #endregion

        #region Table View event handlers

        internal void UpdateList(List<Warpath.WarpathRoom> battles)
        {
            Debug.Log("RecentBattles.UpdateList called");
            var sb = new StringBuilder();
            foreach (var shardBattles in battles.GroupBy(n => n.Shard))
            {
                sb.Append($"<b>{shardBattles.Key}:</b> {shardBattles.Count()}  ");
            }

            HeaderLabel.text = sb.ToString();

            _battles = battles.OrderByDescending(n => n.LastPvpTime).ToList();
            m_tableView.ReloadData();
        }


        #endregion

        private void OnSelected(Warpath.WarpathRoom battle)
        {
            onBattleSelected?.Invoke(battle);
        }
    }
}