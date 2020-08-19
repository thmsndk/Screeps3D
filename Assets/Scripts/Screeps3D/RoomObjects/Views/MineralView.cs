using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    internal class MineralView : MonoBehaviour, IObjectViewComponent, IMapViewComponent
    {
        public const string Path = "Prefabs/RoomObjects/mineral";

        [SerializeField] private Renderer _mineralMesh = default;
        [SerializeField] private Collider _collider = default;
        [SerializeField] private ScaleVisibility _vis = default;

        private Quaternion _rotTarget;
        private Vector3 _posTarget;
        private Vector3 _posRef;
        private Mineral _mineralObject;
        private Light _light;
        private float _currentEmission;

        public void Init()
        {
            _currentEmission = 0f;
        }

        public void Load(RoomObject roomObject)
        {
            _mineralObject = roomObject as Mineral;
            _light = (Light)GetComponentInChildren(typeof(Light));
            if (_mineralObject != null)
            {

                var mineralcolor = new Color32(255,255,255,255);
                // TODO: should color change based on density aswell? e.g. MORE green / less green
                // we could vary value for brigther or darker colors
                // saturation can vary the color aswell

                float hue = 0f;
                float saturation = 0f;
                float val = 0f;
                float alpha = 0f;
                mineralcolor = Constants.ResourceColors[_mineralObject.ResourceType];
            }

            _rotTarget = transform.rotation;
            _posTarget = roomObject.Position;

            // Move mineral up "above" the terrain
            transform.localPosition = roomObject.Position + (Vector3.up * 0.3f);
            _currentEmission = 0f;
            _mineralMesh.materials[0].SetFloat("EmissionEV", _currentEmission);
        }

        public void Delta(JSONObject data)
        {
            if(_mineralObject.ResourceCapacity != 0) {
                _currentEmission = _mineralObject.ResourceAmount / _mineralObject.ResourceCapacity;
            } else {
                _currentEmission = 0f;
            }
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
            _collider.enabled = false;
        }
        public void Hide()
        {
            _vis.Hide();
            _collider.enabled = true;
        }
    }
}