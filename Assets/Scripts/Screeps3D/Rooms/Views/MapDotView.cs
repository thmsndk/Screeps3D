using Common;
using UnityEngine;

namespace Screeps3D.Rooms.Views
{
    public class MapDotView : MonoBehaviour
    {
        public const string Path = "Prefabs/mapDot";
        
        [SerializeField] private ScaleVisibility _vis = default;
        [SerializeField] private MeshRenderer _rend = default;
        private MapView _mapView;

        public int X { get; private set; }
        public int Y { get; private set; }

        public Color Color
        {
            get { return _rend.material.color; }
            set {
                _rend.material.SetColor("_BaseColor", value);
                //_rend.material.color = value;
            }
        }

        private void Start()
        {
            _vis.OnFinishedAnimation += OnFinishedAnimation;
        }

        private void OnFinishedAnimation(bool shown)
        {
            if (!shown && _mapView)
            {
                PoolLoader.Return(Path, gameObject);
                _mapView.RemoveAt(X, Y);
            }
        }
        
        public void Show()
        {
            _vis.Show();
        }

        public void Hide()
        {
            _vis.Hide();
        }

        public void Load(int x, int y, MapView mapView)
        {
            X = x;
            Y = y;
            _mapView = mapView;
            transform.position = PosUtility.Convert(x, y, mapView.Room);
        }
    }
}