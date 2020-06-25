using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class ControllerView : MonoBehaviour, IObjectViewComponent, IMapViewComponent
    {
        public const string Path = "Prefabs/RoomObjects/controller";

        [SerializeField] private Renderer _badge = default;
        [SerializeField] private Texture _unownedBadge = default;
        [SerializeField] private Renderer _core = default;
        [SerializeField] private Renderer _progressRenderer = default;
        [SerializeField] private Renderer _l1 = default;
        [SerializeField] private Renderer _l2 = default;
        [SerializeField] private Renderer _l3 = default;
        [SerializeField] private Renderer _l4 = default;
        [SerializeField] private Renderer _l5 = default;
        [SerializeField] private Renderer _l6 = default;
        [SerializeField] private Renderer _l7 = default;
        [SerializeField] private Renderer _l8 = default;
        [SerializeField] private ScaleVisibility _progressScale = default;
        [SerializeField] private ScaleVisibility _vis = default;
        [SerializeField] private Collider _collider = default;
        [SerializeField] private ParticleSystem _ps = default;
        private Controller _controller;
        private Ownership _ownership;
        private string _owner;
        private float _levelDecayTick = 0;
        private Color _defaultEmissionColor = new Color(0.8f, 0.8f, 0.8f, 0);
        private float _eStr = 0.7f;
        private Color _decayColor = new Color(1.000f, 0.33f, 0.33f, 0.0f);
        private int level = 0;
        enum Ownership {
            Me,
            Enemy,
            None
        }
        private bool ownerHasChanged() {
            // check for reservation change/expiry
            if(_controller?.ReservedBy?.UserId != null) {
                return _owner != _controller.ReservedBy.UserId;
            }
            // check for owner change
            if(_controller?.Owner?.UserId != null) {
                return _owner != _controller.Owner.UserId;
            }
            // no owner, no reservation -> check if we had owner
            return _owner != "None";
                                   
        }
        private void setReservation() {
            if(_controller?.ReservedBy?.UserId != null) {
                _badge.materials[0].SetTexture("EmissionTexture", _controller.ReservedBy.Badge);
                _badge.materials[0].SetFloat("EmissionStrength", 0.1f);
                _ownership = _controller.ReservedBy.UserId.Equals(Screeps_API.ScreepsAPI.Me.UserId) ? Ownership.Me : Ownership.Enemy;
                _owner = _controller.ReservedBy.UserId;
            }
        }

        private void setOwnership() {
            if (_controller?.Owner?.UserId != null) {
                _badge.materials[0].SetTexture("EmissionTexture", _controller.Owner.Badge);
                _badge.materials[0].SetFloat("EmissionStrength", 0.1f);
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
            _core.materials[1].SetFloat("EmissionStrength", _eStr);
        }
        
        private void customizeController() {
            _owner = "None";
            setReservation();
            setOwnership();
            setParticleSystemColor();
            if(_owner == "None") {
                _badge.materials[0].SetTexture("EmissionTexture", _unownedBadge);
                _badge.materials[0].SetFloat("EmissionStrength", 0.01f);
            } 
        }
        private void updateProgress(bool isDecaying) {
            float scale = 1f;
            if(_controller.Level < 8) {
                scale = _controller.Progress / _controller.ProgressMax;
            }
            _progressScale.SetVisibility(scale);
            _progressRenderer.materials[0].SetColor("EmissionColor", isDecaying ? _decayColor : _defaultEmissionColor);
        }

        private void updateLevel(bool isDecaying) {
            // so it matches the levels properly, without playing +1/-1 on indexing
            // i know it's ugly
            Renderer[] levels = { null, _l1, _l2, _l3, _l4, _l5, _l6, _l7, _l8 };
            float ePower = _controller?.Owner?.Badge == null ? 0 : _eStr;
            for(int i = 1; i < levels.Length; i++) {
                var eColor = _defaultEmissionColor;

                if(i == _controller.Level && isDecaying) {
                    eColor = _decayColor;
                }

                levels[i].materials[0].SetFloat("EmissionStrength", _controller.Level >= i ? ePower : 0);
                levels[i].materials[0].SetColor("EmissionColor", eColor);                 
            }
        }

       
        public void Init()
        {            
            _ownership = Ownership.None;
            _owner = "None";
            _badge.materials[0].SetFloat("EmissionStrength", 1);

            customizeController();
        }

        public void Load(RoomObject roomObject)
        {
            _controller = roomObject as Controller;            
            _ownership = Ownership.None;
            _owner = "None";
            _levelDecayTick = _controller.DowngradeTime;
            _badge.materials[0].SetFloat("EmissionStrength", 1);
            _progressRenderer.materials[0].SetFloat("EmissionStrength", _eStr);

            customizeController();
        }

        public void Delta(JSONObject data)
        {
            bool _isDecaying = _controller.DowngradeTime == _levelDecayTick;
            updateProgress(_isDecaying);
            updateLevel(_isDecaying);
            if(!ownerHasChanged()) {
                return;
            }
            customizeController();
            _levelDecayTick = _controller.DowngradeTime;
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