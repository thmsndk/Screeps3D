using System.Collections.Generic;
using Common;
using Screeps_API;
using UnityEngine;
using Screeps3D.RoomObjects.Views;

namespace Screeps3D.Rooms.Views
{
    /*
         {
             "w":[],                                                                // walls
             "r":[[40,31],[36,2],[40,30],[40,33],[37,3],[38,4],[39,34],[38,5]],     // road
             "pb":[],                                                               // powerbank
             "p":[],                                                                // power
             "s":[[35,4],[42,36]],                                                  // source
             "c":[[9,33]],                                                          // controller
             "m":[[29,45]],                                                         // mineral
             "k":[],                                                                // keeper
             "cd74623069821ed":[[32,6],[35,3],[41,40],[41,37],[10,33]],             // player
             "f4b532d08c3952a":[[32,18],[33,17],[32,16],[31,15],[33,16]]            // player
         }
     */
    
    public class MapView : MonoBehaviour, IRoomViewComponent
    {
        public Room Room { get; private set; }

        private MapDotView[,] _dots = new MapDotView[50, 50];
        private List<MapDotView> _dotList = new List<MapDotView>();
        private List<SourceKeeperLairView> _lairList = new List<SourceKeeperLairView>();

        public void Init(Room room)
        {
            Room = room;
            Room.MapStream.OnData += OnMapData;
            Room.OnShowObjects += OnShowObjects;
        }

        private void OnShowObjects(bool show)
        {
            if (show)
            {
                ClearDots();
                foreach (var lair in _lairList)
                    lair.Hide();
            }
            else
            {   
                foreach (var lair in _lairList)
                    lair.Show();
            }
        }

        private void OnMapData(JSONObject data)
        {
            ClearDots();

            if (Room.ShowingObjects)
                return;
            
            foreach (var key in data.keys)
            {
                // player
                if (key.Length > 2)
                    SpawnDots(key, data[key].list);

                else if (key.Equals("k"))
                    SpawnLairs(data[key].list);
            }
        }
        
        private void SpawnLairs(List<JSONObject> list)
        {
            foreach (var numArray in list)
            {
                var x = (int) numArray.list[0].n;
                var y = (int) numArray.list[1].n;
                var pos = PosUtility.Convert(x, y, Room);

                if (_lairList.FindIndex(l => l.transform.position == pos) != -1)
                    continue;

                var lair = PoolLoader.Load(SourceKeeperLairView.Path);
                var lairView = lair.GetComponent<SourceKeeperLairView>();

                lairView.Load(null);
                lairView.transform.position = PosUtility.Convert(x, y, Room);
                lairView.Show();

                _lairList.Add(lairView);
            }
        }
        
        private void SpawnDots(string key, List<JSONObject> list)
        {
            Color randomEnemyColor;
            if (!GameManager.Instance.PlayerColors.TryGetValue(key, out randomEnemyColor))
            {
                randomEnemyColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                GameManager.Instance.PlayerColors.Add(key, randomEnemyColor);
            }


            var color = key == ScreepsAPI.Me.UserId ? Color.green : randomEnemyColor;
            foreach (var numArray in list)
            {
                var x = (int) numArray.list[0].n;
                var y = (int) numArray.list[1].n;
                var view = _dots[x, y];
                if (!view || view.Color != color)
                {
                    var go = PoolLoader.Load(MapDotView.Path);
                    view = go.GetComponent<MapDotView>();
                }
                    
                view.Load(x, y, this);
                view.Color = color;
                view.Show();
                _dots[x, y] = view;
                _dotList.Add(view);
            }
        }

        private void ClearDots()
        {
            foreach (var dot in _dotList)
                dot.Hide();
            _dotList.Clear();
        }

        public void RemoveAt(int x, int y)
        {
            _dots[x, y] = null;
        }

        
    }
}