using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class SourceView : MonoBehaviour, IObjectViewComponent, IMapViewComponent
    {
        public const string Path = "Prefabs/RoomObjects/source";

        [SerializeField] private ScaleVisibility _vis;
        private Source _source;
        

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _source = roomObject as Source;
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