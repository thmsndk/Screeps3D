using Common;
using UnityEngine;
using System.Linq;
using Screeps_API;

namespace Screeps3D.RoomObjects.Views
{
    public class FactoryView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleAxes _energyDisplay = default;
        [SerializeField] private Renderer _base = default;
        [SerializeField] private Renderer _lightningRing = default;
        [SerializeField] private Renderer _rawProduct = default;
        [SerializeField] private Renderer _packedProduct = default;
        [SerializeField] private Renderer _commodityProduct = default;
        [SerializeField] private ParticleSystem _ps = default;
        [SerializeField] private Renderer _l1 = default;
        [SerializeField] private Renderer _l2 = default;
        [SerializeField] private Renderer _l3 = default;
        [SerializeField] private Renderer _l4 = default;
        [SerializeField] private Renderer _l5 = default;

        private Factory _factory;
        private bool _isOnCooldown;
        // TODO: we also need the mineral on the location to get regen time if we want to do something specific in regards to that

        private void setLevelDisplay() {
            int level = _factory.Level != null ? (int)_factory.Level : 0;
            _l1.materials[0].SetFloat("EmissionStrength", level >= 1 ? 0.5f : 0.1f);
            _l2.materials[0].SetFloat("EmissionStrength", level >= 2 ? 0.5f : 0.1f);
            _l3.materials[0].SetFloat("EmissionStrength", level >= 3 ? 0.5f : 0.1f);
            _l4.materials[0].SetFloat("EmissionStrength", level >= 4 ? 0.5f : 0.1f);
            _l5.materials[0].SetFloat("EmissionStrength", level >= 5 ? 0.5f : 0.1f);

            _l1.materials[0].SetColor("EmissionColor", Color.white);
            _l2.materials[0].SetColor("EmissionColor", Color.white);
            _l3.materials[0].SetColor("EmissionColor", Color.white);
            _l4.materials[0].SetColor("EmissionColor", Color.white);
            _l5.materials[0].SetColor("EmissionColor", Color.white);
        }

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _factory = roomObject as Factory;
            _factory.LevelMax = 5;
            _isOnCooldown = _factory.CooldownTime > ScreepsAPI.Time;
            _base.materials[0].SetFloat("EmissionStrength", 0f);

            _ps.Stop();
            _rawProduct.enabled = false;
            _packedProduct.enabled = false;
            _commodityProduct.enabled = false;
            _lightningRing.enabled = false;

            setLevelDisplay();
            AdjustScale();
        }

        private bool isRawResource(string thing) {
            string[] rawResources = {
                "U", "L", "K", "Z", "O", "H","X","energy", "G"
            };
            return rawResources.Contains(thing);
        }

        private bool isPackedResource(string thing) {
            return thing.IndexOf("bar") >= 0 || thing == "purifier" || thing == "ghodium_melt" || thing == "battery";
        }

        private Color resourceToColor(string resource) {
            switch(resource) {
                case "U":
                case "utrium_bar":
                    return new Color32(80,215,249,255);
                case "L":
                case "lemergium_bar":
                    return new Color32(0,244,162,255);
                case "Z":
                case "zynthium_bar":
                    return new Color32(253,211,136,255);
                case "K":
                case "keanium_bar":
                    return new Color32(160,113,255,255);
                case "O":
                case "oxidant":
                    return new Color32(160, 185, 175, 255);
                case "H":
                case "reductant":
                    return new Color32(160, 180, 185,255);
                case "X":
                case "purifier":
                    return new Color32(255,119,119,255);
                case "G":
                case "ghodium_melt":
                    return new Color32(205,205,205,255);
                case "energy":
                case "battery":
                    return new Color32(195,160,15,255);

                case "condensate":
                case "concentrate":
                case "extract":
                case "spirit":
                case "emanation":
                case "essence":
                    return new Color32(85, 0, 95,255);

                case "wire":
                case "switch":
                case "transistor":
                case "microchip":
                case "circuit":
                case "device":
                    return new Color32(15, 110, 135,255);

                case "cell":
                case "phlegm":
                case "tissue":
                case "muscle":
                case "organoid":
                case "organism":
                    return new Color32(0, 140, 0,255);

                case "alloy":
                case "tube":
                case "fixtures":
                case "frame":
                case "hydraulics":
                case "machine":
                    return new Color32(70, 50, 0,255);

                default:
                    return new Color32(205,205,205,255);
            }
        }


        public void Delta(JSONObject data)
        { 
            // Delta data{"store":{"energy":2644,"battery":4449},"actionLog":{"produce":{"x":21,"y":19,"resourceType":"energy"}},"cooldownTime":1,940481E+07}
            AdjustScale();
            _isOnCooldown = _factory.CooldownTime > ScreepsAPI.Time;
            _lightningRing.enabled = false;

            if(!data.HasField("actionLog") || !data["actionLog"].HasField("produce") || data["actionLog"]["produce"]["resourceType"] == null) {
                showProduction("none");
                return;
            };
            var product = data["actionLog"]["produce"]["resourceType"].str;  
            _lightningRing.enabled = true;
            showProduction(product);
            return;           
        }

        public void Unload(RoomObject roomObject)
        {
            _factory = null;
        }

        private void showProduction(string product) {
            if (product == "none") {
                _ps.Stop();
                _rawProduct.enabled = false;
                _packedProduct.enabled = false;
                _commodityProduct.enabled = false;
                _base.materials[2].SetFloat("EmissionStrength", 0f);
                return;
            }
            
            Color32 c = resourceToColor(product);

            var psMain = _ps.main;
            psMain.startColor = (Color)c;
            _ps.Play();

            _base.materials[2].SetColor("EmissionColor", c);
            _lightningRing.materials[0].SetColor("EmissionColor", c);

            if(isRawResource(product)) {
                _rawProduct.enabled = true;
                _rawProduct.materials[0].SetColor("EmissionColor", c);
                return;
            } 

            if(isPackedResource(product)) {
                _packedProduct.enabled = true;
                _packedProduct.materials[0].SetColor("EmissionColor", c);
                return;
            }
            
            _commodityProduct.enabled = false;
            _commodityProduct.materials[1].SetColor("EmissionColor", c);
        }

        private void Update()
        {
            if (_factory == null)
                return;

            if(_isOnCooldown) {
                _base.materials[2].SetFloat("EmissionStrength", 0.3f + Mathf.PingPong(Time.time, 0.2f));
            }

            // TODO: actions, like creep
        }
        private void AdjustScale()
        {
            if (_factory != null)
            {
                _energyDisplay.SetVisibility(_factory.TotalResources / _factory.TotalCapacity);
            }
        }
    }
}