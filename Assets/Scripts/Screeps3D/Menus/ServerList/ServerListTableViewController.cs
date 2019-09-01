using UnityEngine;
using System.Collections;
using Tacticsoft;
using Screeps_API;
using System;

namespace Screeps3D.Menus.ServerList
{
    //An example implementation of a class that communicates with a TableView
    public class ServerListTableViewController : MonoBehaviour, ITableViewDataSource
    {
        public ServerListItemCell m_cellPrefab;
        public TableView m_tableView;

        public int m_numRows;
        private int m_numInstancesCreated = 0;
        private CacheList _servers;

        //Register as the TableView's delegate (required) and data source (optional)
        //to receive the calls
        void Start() {
            m_tableView.dataSource = this;
        }

        #region ITableViewDataSource

        //Will be called by the TableView to know how many rows are in this table
        public int GetNumberOfRowsForTableView(TableView tableView) {
            // Should return the amount of servers in the list
            return _servers.Count;
        }

        //Will be called by the TableView to know what is the height of each row
        public float GetHeightForRowInTableView(TableView tableView, int row) {
            return (m_cellPrefab.transform as RectTransform).rect.height;
        }

        //Will be called by the TableView when a cell needs to be created for display
        public TableViewCell GetCellForRowInTableView(TableView tableView, int row) {
            var cell = tableView.GetReusableCell(m_cellPrefab.reuseIdentifier) as ServerListItemCell;
            if (cell == null) {
                cell = GameObject.Instantiate(m_cellPrefab) as ServerListItemCell;
                cell.name = "ServerListItemCell_" + (++m_numInstancesCreated).ToString();
            }
            // how the hell am I supposed to get the servername? do I access a data source by row?

            var server = _servers[row];

            cell.Update(server);
            return cell;
        }

        #endregion

        #region Table View event handlers

        //Will be called by the TableView when a cell's visibility changed
        public void TableViewCellVisibilityChanged(int row, bool isVisible) {
            //Debug.Log(string.Format("Row {0} visibility changed to {1}", row, isVisible));
            //if (isVisible) {
            //    var cell = m_tableView.GetCellAtRow(row) as ServerListItemCell;
            //    //cell.NotifyBecameVisible();
            //}
        }

        internal void UpdateServerList(CacheList servers)
        {
            this._servers = servers; // Temporary to get something rendered, we should have a proper "serverlist" object without cache
            m_tableView.ReloadData();
        }

        #endregion

    }
}
