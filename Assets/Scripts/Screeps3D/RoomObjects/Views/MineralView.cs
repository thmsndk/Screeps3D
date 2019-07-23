using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    internal class MineralView : ObjectView, IMapViewComponent
    {
        public const string Path = "Prefabs/RoomObjects/mineral";

        [SerializeField] private Renderer _mineral;
        [SerializeField] private Collider _collider;
        [SerializeField] private ScaleVisibility _vis;
        //[SerializeField] private Transform _rotationRoot;

        private Quaternion _rotTarget;
        private Vector3 _posTarget;
        private Vector3 _posRef;
        private Mineral _mineralObject;

        internal override void Load(RoomObject roomObject)
        {
            base.Load(roomObject);
            _mineralObject = roomObject as Mineral;
            var mineralcolor = _mineral.material.color;
            // TODO: should color change based on density aswell? e.g. MORE green / less green
            // we could vary value for brigther or darker colors
            // saturation can vary the color aswell
            switch (_mineralObject.ResourceType)
            {
                case Constants.BaseMineral.Hydrogen:
                    // cdcdcd hsv 0 0 80
                    mineralcolor = Random.ColorHSV(0f, 0f, 0f, 0f, 0.80f, 0.80f);
                    break;
                case Constants.BaseMineral.Oxygen:
                    // cdcdcd hsv 0 0 80
                    mineralcolor = Random.ColorHSV(0f, 0f, 0f, 0f, 0.80f, 0.80f);
                    break;
                case Constants.BaseMineral.Utrium:
                    // 50d7f9 hsv 192 68 98
                    mineralcolor = Random.ColorHSV(192f / 359f, 192f / 359f, 0.68f, 0.68f, 0.98f, 0.98f);
                    break;
                case Constants.BaseMineral.Keanium:
                    // #a071ff hsv 260 56 100
                    mineralcolor = Random.ColorHSV(260f / 359f, 260f / 359f, 0.56f, 0.56f, 1f, 1f);
                    break;
                case Constants.BaseMineral.Lemergium:
                    // should be lime-greenish #00f4a2 hsv 160 100 96

                    mineralcolor = Random.ColorHSV(160f / 359f, 160f / 359f, 1f, 1f, 0.96f, 0.96f);
                    break;
                case Constants.BaseMineral.Zynthium:
                    // Should be sand/yellow fdd388 hsv 38 46 99
                    mineralcolor = Random.ColorHSV(38f / 359f, 38f / 359f, 0.46f, 0.46f, 0.99f, 0.99f);
                    break;
                case Constants.BaseMineral.Catalyst:
                    // Catalyst should be red ff7777 hsv 0 53 100
                    mineralcolor = Random.ColorHSV(0f, 0f, 0.5f, 0.5f, 1f, 1f);
                    break;
            }

            _mineral.material.color = mineralcolor;
            //_body.material.mainTexture = _mineral.Owner.Badge;

            _rotTarget = transform.rotation;
            _posTarget = roomObject.Position;
        }

        internal override void Delta(JSONObject data)
        {
            base.Delta(data);

            //var posDelta = _posTarget - RoomObject.Position;

            //if (posDelta.sqrMagnitude > .01)
            //{
            //    _posTarget = RoomObject.Position;
            //} 
        }

        private void Update()
        {
            if (_mineralObject == null)
                return;
            //transform.localPosition = Vector3.SmoothDamp(transform.localPosition, _posTarget, ref _posRef, .5f);
            //_rotationRoot.transform.rotation = Quaternion.Slerp(_rotationRoot.transform.rotation, _tombstone.Rotation, Time.deltaTime * 5);
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