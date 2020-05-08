using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class ControllerView : MonoBehaviour, IObjectViewComponent, IMapViewComponent
    {
        public const string Path = "Prefabs/RoomObjects/controller";

        [SerializeField] private Renderer _rend;
        [SerializeField] private Renderer _playerRend;
        [SerializeField] private ScaleVisibility _vis;
        [SerializeField] private Collider _collider;
        [SerializeField] private ParticleSystem _ps;

        private Texture2D _texture;
        private Color _controllerWhite;
        private Controller _controller;
        private Material _psMaterial;
        private bool _isMyReservation;

        private Color _myColor;
        private Color _enemyColor;
        public void Init()
        {
            InitTexture();

            _psMaterial = _ps.GetComponent<Renderer>().material;//
            _enemyColor = new Color(1.0f, 0.0f, 0.0f, 0.3f);
            _myColor = new Color(0.0f, 1.0f, 0.0f, 0.3f);
            changeReservationColor(false);
        }

        private void changeReservationColor(bool isMy)
        {
            _isMyReservation = isMy;
            _psMaterial.SetColor("_TintColor", isMy ? _myColor : _enemyColor);
        }

        private void checkReservation()
        {
            var isMyResrvation = false;
            if (_controller.ReservedBy != null)
            {
                isMyResrvation = _controller.ReservedBy.UserId.Equals(Screeps_API.ScreepsAPI.Me.UserId);
            }
            if(isMyResrvation != _isMyReservation)
            {
                changeReservationColor(isMyResrvation);
            }
        }

        public void Load(RoomObject roomObject)
        {
            _controller = roomObject as Controller;

            checkReservation();
            UpdateTexture();
        }

        

        private void InitTexture()
        {
            _texture = new Texture2D(8, 1);
            _texture.filterMode = FilterMode.Point;
            //_rend.materials[1].mainTexture = _texture;
            _rend.materials[1].SetTexture("_BaseColorMap", _texture); // main texture
            _rend.materials[1].SetTexture("_EmissionMap", _texture);
            ColorUtility.TryParseHtmlString("#FDF5E6", out _controllerWhite);


        }

        public void Delta(JSONObject data)
        {
            if (data["level"] == null && data["owner"] == null)
                return;
            UpdateTexture();
        }

        public void Unload(RoomObject roomObject)
        {
        }

        private void UpdateTexture()
        {
            var level = 0;
            for (var i = 0; i < 8; i++)
            {
                if (level < _controller.Level)
                {
                    _texture.SetPixel(i, 1, _controllerWhite);
                } else
                {
                    _texture.SetPixel(i, 1, Color.black);
                }
                level++;
            }
            _texture.Apply();

            if (_controller.Owner != null)
            {
                _playerRend.materials[0].SetTexture("_BaseColorMap", _controller.Owner.Badge); // main texture
                //_playerRend.materials[0].mainTexture = _controller.Owner.Badge;
                _playerRend.materials[0].color = Color.white;
            }
            else
            {
                _playerRend.materials[0].mainTexture = null;
                _playerRend.materials[0].color = Color.grey;
            }
        }

        private void Update()
        {
            if (_controller == null)
                return;
            
            float floor = 0.6f;
            float ceiling = 1.0f;
            float emission = floor + Mathf.PingPong (Time.time * .2f, ceiling - floor);
            Color finalColor = Color.white * emission;
            _rend.materials[1].SetColor ("_EmissionColor", finalColor);

            checkReservation();

            if (_controller.ReservedBy != null && _ps.isStopped)
            {
                _ps.Play();
            }
            else if (_controller.ReservedBy == null && _ps.isPlaying)
            {
                _ps.Stop();
            }
            
       
            /**/
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