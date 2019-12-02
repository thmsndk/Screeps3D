using Common;
using UnityEngine;
using Screeps3D.Rooms.Views;

namespace Screeps3D.RoomObjects.Views
{
    public class SourceKeeperLairView : MonoBehaviour, IObjectViewComponent, IMapViewComponent
    {
        public const string Path = "Prefabs/RoomObjects/keeperLair";
        
        [SerializeField] private ScaleVisibility _vis;
        private SourceKeeperLair _sourceKeeperLair;
        

        public void Init()
        {
        }
        
        public void Load(RoomObject roomObject)
        {
            _sourceKeeperLair = roomObject as SourceKeeperLair;
            var terrainView = _sourceKeeperLair.Room.View.GetComponentInChildren<TerrainView>();
            if (terrainView)
                terrainView.addLair(_sourceKeeperLair.X, _sourceKeeperLair.Y);
        }

        public void Delta(JSONObject data)
        {
        }

        public void Unload(RoomObject roomObject)
        {
        }
        
        // IMapViewComponent *****************
        public int roomPosX { get; set; }
        public int roomPosY { get; set; }
        public void Show()
        {
            _vis.Show();
        }
        public void Hide()
        {
            _vis.Hide();
        }
    }
}