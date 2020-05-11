using Common;
using UnityEngine;

namespace Screeps3D.Rooms.Views
{
    internal class RoadNetworkView : MonoBehaviour, IRoomViewComponent
    {
        [SerializeField] private RoadView _roadPrototype = default;
        public RoadView[,] roads = new RoadView[50, 50];
        private Room _room;

        public void Init(Room room)
        {
            _room = room;
            _room.MapStream.OnData += OnMapData;
        }

        private void OnMapData(JSONObject data)
        {
            var roadObj = data["r"];
            if (roadObj != null)
            {
                UnpackRoads(roadObj);
            }
        }

        private void UnpackRoads(JSONObject roadObj)
        {
            foreach (var posArray in roadObj.list)
            {
                var x = (int) posArray.list[0].n;
                var y = (int) posArray.list[1].n;
                if (roads[x, y] == null)
                {
                    Scheduler.Instance.Add(() => AssignRoad(x, y));
                }
            }
            // TODO: Cull destroyed roads
        }

        private void AssignRoad(int x, int y)
        {
            roads[x, y] = Instantiate(_roadPrototype.gameObject).GetComponent<RoadView>();
            roads[x, y].Init(this, x, y);
            roads[x, y].transform.SetParent(transform, false);
            roads[x, y].transform.localPosition = new Vector3(x, 0, 49 - y);
        }

        /*public static string Decompress(string strData) {
            var data = Encoding.UTF8.GetBytes(strData);
            var output = new MemoryStream();
            using(var compressedStream = new MemoryStream(data))
            using(var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                zipStream.CopyTo(output);
                zipStream.Close();
                output.Position = 0;
                return output.ToString();
            }
        }*/
    }
}