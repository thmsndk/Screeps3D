using Screeps3D.RoomObjects;
using System.Collections.Generic;
using UnityEngine;

namespace Screeps3D.Rooms.Views
{
    /// <summary>
    /// This component is used both by RoadNetworkView, the roomobject for road in the room you subscribe for, and when placing a constructionsite.
    /// </summary>
    internal class RoadView : MonoBehaviour, IObjectViewComponent
    {
        private int _x;
        private int _y;
        private RoadNetworkView _roadNetworkView;
        private Dictionary<string, Renderer> _offshoots;

        private Road _road;

        public void Init(RoadNetworkView roadNetworkView, int x, int y)
        {
            InitializeOffshoots();

            this._x = x;
            this._y = y;
            this._roadNetworkView = roadNetworkView;
            CheckNeighbors();
        }

        private void InitializeOffshoots()
        {
            if (_offshoots == null)
            {
                _offshoots = new Dictionary<string, Renderer>();
                foreach (var renderer in GetComponentsInChildren<Renderer>())
                {
                    _offshoots[renderer.gameObject.name] = renderer;
                }
            }
        }

        private void CheckNeighbors()
        {
            var foundOffshoot = false;
            for (var xDelta = -1; xDelta <= 1; xDelta++)
            {
                for (var yDelta = -1; yDelta <= 1; yDelta++)
                {
                    var rx = _x + xDelta;
                    var ry = _y + yDelta;
                    if (xDelta == 0 && yDelta == 0)
                        continue;
                    if (_roadNetworkView.roads[rx, ry] == null)
                        continue;
                    var key = xDelta.ToString() + yDelta.ToString();
                    _offshoots[key].enabled = true;
                    foundOffshoot = true;
                }
            }
            if (!foundOffshoot)
            {
                _offshoots["base"].enabled = true;
            }
        }

        public void ShowBase(bool enabled)
        {
            _offshoots["base"].enabled = enabled;
        }

        void IObjectViewComponent.Init()
        {
            // road roomobjects initialized should not be rendered, they are already rendered by the RoadNetworkView.
            InitializeOffshoots();
        }

        void IObjectViewComponent.Load(RoomObject roomObject)
        {
            _road = roomObject as Road;

            foreach (var keyValuePair in _offshoots)
            {
                keyValuePair.Value.enabled = false;
            }

            //CheckNeighbors(); we can't check neighbours yet, we don't have a roadnetwork in roomobject mode
        }

        void IObjectViewComponent.Delta(JSONObject data)
        {
            
        }

        void IObjectViewComponent.Unload(RoomObject roomObject)
        {
            
        }

        private void Update()
        {
            if (_road == null)
            {
                return;
            }

            // TODO: update x,y with the position of the road, not relevent before we fix "checkneighbours" for placing csites.
            
        }
    }
}