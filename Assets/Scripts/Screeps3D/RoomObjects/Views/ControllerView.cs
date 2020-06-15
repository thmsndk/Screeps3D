using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class ControllerView : MonoBehaviour, IObjectViewComponent, IMapViewComponent
    {
        public const string Path = "Prefabs/RoomObjects/controller";

        [SerializeField] private Renderer _badge = default;
        [SerializeField] private Renderer _core = default;
        [SerializeField] private Renderer _l1 = default;
        [SerializeField] private Renderer _l2 = default;
        [SerializeField] private Renderer _l3 = default;
        [SerializeField] private Renderer _l4 = default;
        [SerializeField] private Renderer _l5 = default;
        [SerializeField] private Renderer _l6 = default;
        [SerializeField] private Renderer _l7 = default;
        [SerializeField] private Renderer _l8 = default;
        [SerializeField] private ScaleVisibility _progress = default;
        [SerializeField] private ScaleVisibility _vis = default;
        [SerializeField] private Collider _collider = default;
        [SerializeField] private ParticleSystem _ps = default;
        private Controller _controller;
        private Ownership _ownership;
        private string _owner;
        private Color32 _defaultEmissionColor = new Color(0.7f, 0.7f, 0.7f, 0f);
        private int level = 0;
        enum Ownership {
            Me,
            Enemy,
            None
        }
        private bool ownerHasChanged() {
            // check for reservation change/expiry
            if(_controller?.ReservedBy?.Badge != null) {
                return _owner != _controller.ReservedBy.UserId;
            }
            // check for owner change
            if(_controller?.Owner?.Badge != null) {
                return _owner != _controller.Owner.UserId;
            }
            // no owner, no reservation -> check if we had owner
            return _owner != "None";
                                   
        }
        private void setReservation() {
            if(_controller?.ReservedBy?.Badge != null) {
                _badge.materials[0].SetColor("EmissionColor", _defaultEmissionColor);
                _badge.materials[0].SetTexture("EmissionTexture", _controller.ReservedBy.Badge);
                _badge.materials[0].SetFloat("EmissionStrength", 5);
                _ownership = _controller.ReservedBy.UserId.Equals(Screeps_API.ScreepsAPI.Me.UserId) ? Ownership.Me : Ownership.Enemy;
                _owner = _controller.ReservedBy.UserId;
            }
        }

        private void setOwnership() {
            if (_controller?.Owner?.Badge != null) {
                _badge.materials[0].SetColor("EmissionColor", _defaultEmissionColor);
                _badge.materials[0].SetTexture("EmissionTexture", _controller.Owner.Badge);
                _badge.materials[0].SetFloat("EmissionStrength", 5f);
                _ownership = _controller.Owner.UserId.Equals(Screeps_API.ScreepsAPI.Me.UserId) ? Ownership.Me : Ownership.Enemy;
                _owner = _controller.Owner.UserId;
            }
        }

        private void setParticleSystemColor() {
            var isMy = false;
            var psMain = _ps.main;
            Color color = _defaultEmissionColor;
            if(_ownership != Ownership.None) {
                color = _ownership == Ownership.Me ? new Color(0.5f, 1.000f, 0.5f, 0.0f) : new Color(1.000f, 0.33f, 0.33f, 0.0f);
            }            
            psMain.startColor = color;
            _core.materials[1].SetColor("EmissionColor", color);
            _core.materials[1].SetFloat("EmissionStrength", 5f);
        }
        
        private void customizeController() {
            _owner = "None";
            setReservation();
            setOwnership();
            setParticleSystemColor();
        }
        private void updateProgress() {
            float scale = 1f;
            if(_controller.Level == 8) {
                _progress.SetVisibility(scale);
                return;
            }
            scale = _controller.Progress / _controller.ProgressMax;
            _progress.SetVisibility(scale);
        }

        private void updateLevel() {
            Renderer[] levels = { _l1, _l2, _l3, _l4, _l5, _l6, _l7, _l8 };
            float ePower = _controller?.Owner?.Badge == null ? 0 : 3;
            for(int i = 0; i < levels.Length; i++) {
                levels[i].materials[0].SetColor("EmissionColor", _defaultEmissionColor);
                levels[i].materials[0].SetFloat("EmissionStrength", _controller.Level >= i ? ePower : 0);
            }
        }

       
        public void Init()
        {            
            _ownership = Ownership.None;
            _owner = "None";
            _badge.materials[0].SetFloat("EmissionStrength", 0);

            customizeController();
        }

        public void Load(RoomObject roomObject)
        {
            _controller = roomObject as Controller;            
            _ownership = Ownership.None;
            _owner = "None";
            _badge.materials[0].SetFloat("EmissionStrength", 0);

            customizeController();
        }

        public void Delta(JSONObject data)
        {
            updateProgress();
            updateLevel();
            if(!ownerHasChanged()) {
                return;
            }
            customizeController();
        }

        public void Unload(RoomObject roomObject)
        {
        }

        private void Update()
        {
            if (_controller == null)
                return;
        }
        
        // IMapViewComponent *****************
        public int roomPosX { get; set; }
        public int roomPosY { get; set; }
        public void Show()
        {
            _vis.Show();
            _collider.enabled = false;
            _ps.Stop();
        }
        public void Hide()
        {
            _vis.Hide();
            _collider.enabled = true;
        }
    }
}