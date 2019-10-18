using UnityEngine;
using System.Collections;
using Tacticsoft;
using Screeps_API;
using System;
using UnityEngine.Events;

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

        public OnServerSelected onServerSelected;

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
            return _servers?.Count ?? 0;
        }

        //Will be called by the TableView to know what is the height of each row
        public float GetHeightForRowInTableView(TableView tableView, int row)
        {
            return ((RectTransform) m_cellPrefab.transform).rect.height;
        }

        //Will be called by the TableView when a cell needs to be created for display
        public TableViewCell GetCellForRowInTableView(TableView tableView, int row)
        {
            var cell = tableView.GetReusableCell(m_cellPrefab.reuseIdentifier) as ServerListItemCell;
            if (cell == null)
            {
                cell = GameObject.Instantiate(m_cellPrefab) as ServerListItemCell;
                cell.name = "ServerListItemCell_" + (++m_numInstancesCreated).ToString();
                cell.onServerSelected.AddListener(OnServerSelected);
            }

            var server = _servers[row];

            cell.SetServer(server);
            return cell;
        }

        #endregion

        #region Table View event handlers

        internal void UpdateServerList(CacheList servers)
        {
            _servers = servers; // Temporary to get something rendered, we should have a proper "serverlist" object without cache
            m_tableView.ReloadData();
        }

        #endregion

        private void OnServerSelected(ServerCache server)
        {
            onServerSelected?.Invoke(server);
        }
    }
}