using Common;
using System.Collections;
using UnityEngine;
using Screeps_API;

namespace Screeps3D.RoomObjects.Views
{
    public class PowerBankView : MonoBehaviour, IObjectViewComponent, IMapViewComponent
    {

        public const string Path = "Prefabs/RoomObjects/powerBankMV";

        // [SerializeField] private GameObject destroyed = default;
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
        [SerializeField] private ParticleSystem _psEnergy;
        [SerializeField] private ParticleSystem _psSmoke;
        private PowerBank _powerBank;
        private GameObject spawnedDebris;
        private IEnumerator _despawnDebris;
        private long _lastTickUpdate;
        private float _decayLeft = 0;
        private bool destroyed = false;

        public void Init()
        {
        }


        private void setPowerDisplay() {
            _base.materials[0].SetFloat("EmissionStrength", destroyed ? 0 : 2);
            if(_powerBank != null) {
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
            } else {
                _base.materials[0].SetFloat("EmissionStrength", 0);
                _p1.materials[0].SetFloat("EmissionStrength", 0);
                _p2.materials[0].SetFloat("EmissionStrength", 0);
                _p3.materials[0].SetFloat("EmissionStrength", 0);
                _p4.materials[0].SetFloat("EmissionStrength", 0);
                _p5.materials[0].SetFloat("EmissionStrength", 0);
                _p6.materials[0].SetFloat("EmissionStrength", 0);
                _p7.materials[0].SetFloat("EmissionStrength", 0);
                _p8.materials[0].SetFloat("EmissionStrength", 0);
                _p9.materials[0].SetFloat("EmissionStrength", 0);
                _p10.materials[0].SetFloat("EmissionStrength",0);
            }
        }

        private void energyEmissionControl(string startStop) {
            if(_psEnergy == null) {
                return;
            }
            if(destroyed) {
                return;
            }
            _psSmoke.Stop();
            if(startStop == "start" && !destroyed && !_psEnergy.isPlaying) {
                _psEnergy.Play();
                return;
            }

            if(startStop == "stop" && _psEnergy.isPlaying) {
                _psEnergy.Stop();
                return;
            }          
        }
        private void smokeEmissionControl(string startStop) {
            if(_psSmoke == null) {
                return ;
            }
            if(!destroyed) {
                return;
            }
            _psEnergy.Stop();
            if(startStop == "start" && !_psSmoke.isPlaying) {
                _psSmoke.Play();
                return;
            }
            if(startStop == "stop" && _psSmoke.isPlaying) {
                _psSmoke.Stop();
                return;
            }
        }

        public void Load(RoomObject roomObject)
        {
            _powerBank = roomObject as PowerBank;
            setPowerDisplay();
            _lastTickUpdate = ScreepsAPI.Time;
        }

        private void Update() {
            string state = (_powerBank != null && _powerBank.Shown) ? "start" : "stop";
            energyEmissionControl(state);
            smokeEmissionControl(state);
        }

        public void Delta(JSONObject data)
        {
            if (_powerBank == null)
            {
                return;
            }

            destroyed = _powerBank.Hits < 1;
            setPowerDisplay();
            if (_psEnergy != null && !_psEnergy.isPlaying) {
                _psEnergy.Play();
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
                if (_psEnergy != null && _psEnergy.isPlaying)
                {
                    var m = _psEnergy.main;
                    m.maxParticles = Mathf.RoundToInt(500 * _decayLeft);
                    var e = _psEnergy.emission;
                    e.rateOverTime = Mathf.RoundToInt(250 * _decayLeft);
                }
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
