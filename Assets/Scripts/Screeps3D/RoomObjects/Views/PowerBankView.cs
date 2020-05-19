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
        [SerializeField] private Animation _spinner;
        [SerializeField] private ParticleSystem _ps;
        private PowerBank _powerBank;
        private GameObject spawnedDebris;
        private IEnumerator _despawnDebris;
        private long _lastTickUpdate;
        private float _decayProgress = 0;

        public void Init()
        {
        }


        private void setPowerDisplay() {

            float power = _powerBank.Store["power"];
            _p1.materials[0].SetFloat("EmissionStrength", (power > 0) ? 6 : 0);
            _p2.materials[0].SetFloat("EmissionStrength", (power > 1000) ? 6 : 0);
            _p3.materials[0].SetFloat("EmissionStrength", (power > 2000) ? 6 : 0);
            _p4.materials[0].SetFloat("EmissionStrength", (power > 3000) ? 6 : 0);
            _p5.materials[0].SetFloat("EmissionStrength", (power > 4000) ? 6 : 0);
            _p6.materials[0].SetFloat("EmissionStrength", (power > 5000) ? 6 : 0);
            _p7.materials[0].SetFloat("EmissionStrength", (power > 6000) ? 6 : 0);
            _p8.materials[0].SetFloat("EmissionStrength", (power > 7000) ? 6 : 0);
            _p9.materials[0].SetFloat("EmissionStrength", (power > 8000) ? 6 : 0);
            _p10.materials[0].SetFloat("EmissionStrength", (power > 9000) ? 6 : 0);
        }

        public void Load(RoomObject roomObject)
        {
            _powerBank = roomObject as PowerBank;
            setPowerDisplay();
            _lastTickUpdate = ScreepsAPI.Time;
        }

        private void Update() {
            
            if (_powerBank == null || !_powerBank.Shown)
            {
                if (_ps != null)
                {
                    _ps.Stop();
                }
                return;
            }

            if (_ps != null && !_ps.isPlaying) {
                    _ps.Play();
            }

            long now = ScreepsAPI.Time;
            if(_lastTickUpdate < now) {
                _lastTickUpdate = now;

                float leftToTick = Mathf.Max(0, _powerBank.NextDecayTime - now);
                _decayProgress = Mathf.Round((float)leftToTick / _powerBank.maxTTL * 100f) / 100f;
                Debug.LogError("new decay progress " + _decayProgress.ToString());
                if (_ps != null)
                {
                    Debug.LogError("update m.maxParticles to 1000 * " + _decayProgress.ToString());
                    var m = _ps.main;
                    m.maxParticles = Mathf.RoundToInt(1000 * _decayProgress);
                    var e = _ps.emission;
                    e.rateOverTime = Mathf.RoundToInt(250 * _decayProgress);
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
            // _powerScaleVisibility.Show();
            // _collider.enabled = false;
        }
        public void Hide()
        {
            // _powerScaleVisibility.Hide();
            // _collider.enabled = true;
        }
    }
}
