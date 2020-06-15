using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class ControllerView : MonoBehaviour, IObjectViewComponent, IMapViewComponent
    {
        public const string Path = "Prefabs/RoomObjects/controller";

        [SerializeField] private Renderer _badge = default;
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
        enum Ownership {
            Me,
            Enemy,
            None
        }
        
        private void setReservation() {
            if(_controller?.ReservedBy?.Badge != null) {
                _badge.materials[0].SetColor("EmissionColor", new Color(0.7f, 0.7f, 0.7f, 1f));
                _badge.materials[0].SetTexture("EmissionTexture", _controller.Owner.Badge);
                _badge.materials[0].SetFloat("EmissionStrength", 5);
                _ownership = _controller.ReservedBy.UserId.Equals(Screeps_API.ScreepsAPI.Me.UserId) ? Ownership.Me : Ownership.Enemy;
            }
        }

        private void setOwnership() {
            if (_controller?.Owner?.Badge != null) {
                _badge.materials[0].SetColor("EmissionColor", new Color(0.7f, 0.7f, 0.7f, 1f));
                _badge.materials[0].SetTexture("EmissionTexture", _controller.Owner.Badge);
                _badge.materials[0].SetFloat("EmissionStrength", 5);
                _ownership = _controller.Owner.UserId.Equals(Screeps_API.ScreepsAPI.Me.UserId) ? Ownership.Me : Ownership.Enemy;
            }
        }

        private void setParticleSystemColor() {
            var isMy = false;
            var psMain = _ps.main;
            psMain.startColor = new Color(0.6f, 0.6f, 0.6f, 0.0f);
            if(_ownership == Ownership.None) {
                return;
            }            
            psMain.startColor = _ownership == Ownership.Me ? new Color(0.5f, 1.000f, 0.5f, 0.0f) : new Color(1.000f, 0.33f, 0.33f, 0.0f);
        }
        
        private void customizeController() {
            _ownership = Ownership.None;
            _badge.materials[0].SetFloat("EmissionStrength", 0);
            setReservation();
            setOwnership();
            setParticleSystemColor();
        }
       
        public void Init()
        {
            // loadOwnerTexture();
            customizeController();
        }

        public void Load(RoomObject roomObject)
        {
            _controller = roomObject as Controller;
            customizeController();
        }

        public void Delta(JSONObject data)
        {
            updateProgress();
            updateLevel();
            customizeController();
        }

        public void Unload(RoomObject roomObject)
        {
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
            _l1.materials[0].SetColor("EmissionColor", new Color(1f,1f,1f,0f));
            _l2.materials[0].SetColor("EmissionColor", new Color(1f,1f,1f,0f));
            _l3.materials[0].SetColor("EmissionColor", new Color(1f,1f,1f,0f));
            _l4.materials[0].SetColor("EmissionColor", new Color(1f,1f,1f,0f));
            _l5.materials[0].SetColor("EmissionColor", new Color(1f,1f,1f,0f));
            _l6.materials[0].SetColor("EmissionColor", new Color(1f,1f,1f,0f));
            _l7.materials[0].SetColor("EmissionColor", new Color(1f,1f,1f,0f));
            _l8.materials[0].SetColor("EmissionColor", new Color(1f,1f,1f,0f));

            _l1.materials[0].SetFloat("EmissionStrength", _controller.Level >= 1 ? 3 : 0);
            _l2.materials[0].SetFloat("EmissionStrength", _controller.Level >= 2 ? 3 : 0);
            _l3.materials[0].SetFloat("EmissionStrength", _controller.Level >= 3 ? 3 : 0);
            _l4.materials[0].SetFloat("EmissionStrength", _controller.Level >= 4 ? 3 : 0);
            _l5.materials[0].SetFloat("EmissionStrength", _controller.Level >= 5 ? 3 : 0);
            _l6.materials[0].SetFloat("EmissionStrength", _controller.Level >= 6 ? 3 : 0);
            _l7.materials[0].SetFloat("EmissionStrength", _controller.Level >= 7 ? 3 : 0);
            _l8.materials[0].SetFloat("EmissionStrength", _controller.Level >= 8 ? 3 : 0);
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