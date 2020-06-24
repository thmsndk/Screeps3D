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

                switch (_mineralObject.ResourceType)
                {
                    case Constants.BaseMineral.Hydrogen:
                        // cdcdcd hsv 0 0 80
                        // mineralcolor = new Color32(205,205,205,255);
                        // mineralcolor = Random.ColorHSV(0f, 0f, 0f, 0f, 0.80f, 0.80f);
                        hue = 0f;
                        saturation = 0f;
                        val = 0.8f;
                        alpha = 0.8f;
                        break;
                    case Constants.BaseMineral.Oxygen:
                        // cdcdcd hsv 0 0 80
                        mineralcolor = new Color32(205,205,205,255);
                        // mineralcolor = Random.ColorHSV(0f, 0f, 0f, 0f, 0.80f, 0.80f);
                        break;
                    case Constants.BaseMineral.Utrium:
                        // 50d7f9 hsv 192 68 98
                        mineralcolor = new Color32(80,215,249,255);
                        // mineralcolor = Random.ColorHSV(192f / 359f, 192f / 359f, , 0.68f, 0.98f, 0.98f);
                        break;
                    case Constants.BaseMineral.Keanium:
                        // #a071ff hsv 260 56 100
                        mineralcolor = new Color32(160,113,255,255);
                        // mineralcolor = Random.ColorHSV(260f / 359f, 260f / 359f, 0.56f, 0.56f, 1f, 1f);
                        break;
                    case Constants.BaseMineral.Lemergium:
                        // should be lime-greenish #00f4a2 hsv 160 100 96
                        mineralcolor = new Color32(0,244,162,255);
                        // mineralcolor = Random.ColorHSV(160f / 359f, 160f / 359f, 1f, 1f, 0.96f, 0.96f);
                        break;
                    case Constants.BaseMineral.Zynthium:
                        // Should be sand/yellow fdd388 hsv 38 46 99
                        mineralcolor = new Color32(253,211,136,255);
                        // mineralcolor = Random.ColorHSV(38f / 359f, 38f / 359f, 0.46f, 0.46f, 0.99f, 0.99f);
                        break;
                    case Constants.BaseMineral.Catalyst:
                        // Catalyst should be red ff7777 hsv 0 53 100
                        mineralcolor = new Color32(255,119,119,255);
                        // mineralcolor = Random.ColorHSV(0f, 0f, 0.5f, 0.5f, 1f, 1f);
                        break;
                }

                // _mineral.materials[0].SetColor("BaseColor", (Color)mineralcolor * 0.3f);
                // _mineral.materials[0].SetFloat("EmissionEV", 0.3f);
                // _light.color = mineralcolor;
                // _light.intensity = 3f;
                // _mineral.GetComponentInChil
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