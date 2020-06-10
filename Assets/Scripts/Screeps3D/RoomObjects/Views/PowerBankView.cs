using Common;
using System.Collections;
using UnityEngine;
using Screeps_API;

namespace Screeps3D.RoomObjects.Views
{
    public class PowerBankView : MonoBehaviour, IObjectViewComponent, IMapViewComponent
    {

        public const string Path = "Prefabs/RoomObjects/powerBankMV";

        [SerializeField] private GameObject destroyed = default;
        [SerializeField] private ScaleVisibility _pbMapView = default;
        [SerializeField] private Collider _collider = default;
        [SerializeField] private MeshRenderer _base;
        [SerializeField] private MeshRenderer _p1;
        [SerializeField] private MeshRenderer _p2;
        [SerializeField] private MeshRenderer _p3;
        [SerializeField] private MeshRenderer _p4;
        [SerializeField] private MeshRenderer _p5;
        [SerializeField] private MeshRenderer _p6;
        [SerializeField] private MeshRenderer _p7;
        [SerializeField] private MeshRenderer _p8;
        [SerializeField] private MeshRenderer _p9;
        [SerializeField] private MeshRenderer _p10;
        [SerializeField] private ParticleSystem _ps;
        private PowerBank _powerBank;
        private GameObject spawnedDebris;
        private IEnumerator _despawnDebris;
        private long _lastTickUpdate;
        private float _decayLeft = 0;

        public void Init()
        {
        }


        private void setPowerDisplay() {
            float power = _powerBank.Store["power"];
            _p1.materials[0].SetFloat("EmissionStrength", (power > 0) ? .6f : 0);
            _p2.materials[0].SetFloat("EmissionStrength", (power > 1000) ? .6f : 0);
            _p3.materials[0].SetFloat("EmissionStrength", (power > 2000) ? .6f : 0);
            _p4.materials[0].SetFloat("EmissionStrength", (power > 3000) ? .6f : 0);
            _p5.materials[0].SetFloat("EmissionStrength", (power > 4000) ? .6f : 0);
            _p6.materials[0].SetFloat("EmissionStrength", (power > 5000) ? .6f : 0);
            _p7.materials[0].SetFloat("EmissionStrength", (power > 6000) ? .6f : 0);
            _p8.materials[0].SetFloat("EmissionStrength", (power > 7000) ? .6f : 0);
            _p9.materials[0].SetFloat("EmissionStrength", (power > 8000) ? .6f : 0);
            _p10.materials[0].SetFloat("EmissionStrength", (power > 9000) ? .6f : 0);
        }

        private void startStopEmission(string startStop) {
            if(_ps != null) {
                if(startStop == "start" && !_ps.isPlaying) {
                    _ps.Play();
                    return;
                }
                if(startStop == "stop" && _ps.isPlaying) {
                    _ps.Stop();
                    return;
                }
            }
        }

        public void Load(RoomObject roomObject)
        {
            _powerBank = roomObject as PowerBank;
            setPowerDisplay();
            _lastTickUpdate = ScreepsAPI.Time;
        }

        private void Update() {

            startStopEmission((_powerBank != null && _powerBank.Shown) ? "start" : "stop");            
            if (_powerBank == null )
            {
                return;
            }


            if (_ps != null && !_ps.isPlaying) {
                    _ps.Play();
            }

            
            var percentage = _powerBank.Store["power"] / _powerBank.PowerCapacity;
            var minVisibility = 0.001f; /*to keep it visible and selectable, also allows the resource to render again when regen hits*/
            float visibility = percentage == 0 ? minVisibility : percentage;
            _pbMapView.SetVisibility(visibility);


            long now = ScreepsAPI.Time;
            if(_lastTickUpdate < now) {
                _lastTickUpdate = now;

                float leftToTick = Mathf.Max(0, _powerBank.NextDecayTime - now);
                _decayLeft = Mathf.Round((float)leftToTick / _powerBank.maxTTL * 100f) / 100f;
                if (_ps != null)
                {
                    var m = _ps.main;
                    m.maxParticles = Mathf.RoundToInt(1000 * _decayLeft);
                    var e = _ps.emission;
                    e.rateOverTime = Mathf.RoundToInt(250 * _decayLeft);
                }
            }
        }

        public void Delta(JSONObject data)
        {
            if (_powerBank == null)
            {
                return;
            }
        }

        public void Unload(RoomObject roomObject)
        {
            // perhaps make this a couroutine? seems like the debris where instantly removed upon unload
            // Destroy(spawnedDebris);
            // if (_powerBank.Hits <= 0 || hits == 1)
            // {
            //     spawnedDebris = Instantiate(destroyed, transform.position, transform.rotation);
            //     Destroy(gameObject); // Would really like to spawn this just before the powerBank is hidden.
            //     _despawnDebris = DespawnDebris();
            //     StartCoroutine(_despawnDebris); // This coroutine never triggered again.
            // }
        }

        // private IEnumerator DespawnDebris()
        // {

        //     while (spawnedDebris != null)
        //     {
        //         Debug.Log("waiting to despawn");
        //         yield return new WaitForSeconds(30);
        //         Debug.Log("Should be despawning");
        //         Destroy(spawnedDebris);
        //         spawnedDebris = null;
        //         StopCoroutine(_despawnDebris);
        //         _despawnDebris = null;
        //     }
        // }
        
        // IMapViewComponent *****************
        public int roomPosX { get; set; }
        public int roomPosY { get; set; }
        public void Show()
        {
            _pbMapView.Show();
            _collider.enabled = false;
        }
        public void Hide()
        {
            _pbMapView.Hide();
            _collider.enabled = true;
        }
    }
}
