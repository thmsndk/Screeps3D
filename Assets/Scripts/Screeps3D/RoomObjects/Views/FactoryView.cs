using Common;
using UnityEngine;
using System.Linq;
using Screeps_API;
using System.Collections.Generic;

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

        private void setLevelDisplay()
        {
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

        private bool isRawResource(string thing)
        {
            string[] rawResources = {
                "U", "L", "K", "Z", "O", "H","X","energy", "G"
            };
            return rawResources.Contains(thing);
        }

        private bool isPackedResource(string thing)
        {
            var packedResources = new List<string> {
                //"utrium_bar",
                //"lemergium_bar",
                //"zynthium_bar",
                //"keanium_bar",
                "ghodium_melt",
                "oxidant",
                "reductant",
                "purifier",
                "battery"
            };
            return thing.IndexOf("bar") >= 0 || packedResources.IndexOf(thing) > -1;
        }

        private Color resourceToColor(string resource)
        {
            switch (resource)
            {
                case Constants.BaseMineral.Utrium:
                case "utrium_bar":
                    return new Color32(80, 215, 249, 255);
                case Constants.BaseMineral.Lemergium:
                case "lemergium_bar":
                    return new Color32(0, 244, 162, 255);
                case Constants.BaseMineral.Zynthium:
                case "zynthium_bar":
                    return new Color32(253, 211, 136, 255);
                case Constants.BaseMineral.Keanium:
                case "keanium_bar":
                    return new Color32(160, 113, 255, 255);
                case Constants.BaseMineral.Oxygen:
                case "oxidant":
                    return new Color32(160, 185, 175, 255);
                case Constants.BaseMineral.Hydrogen:
                case "reductant":
                    return new Color32(160, 180, 185, 255);
                case Constants.BaseMineral.Catalyst:
                case "purifier":
                    return new Color32(255, 119, 119, 255);
                case "G":
                case "ghodium_melt":
                    return new Color32(205, 205, 205, 255);
                case "energy":
                case "battery":
                    return new Color32(195, 160, 15, 255);

                // Mystical chain
                case "condensate":
                case "concentrate":
                case "extract":
                case "spirit":
                case "emanation":
                case "essence":
                    return new Color32(85, 0, 95, 255);

                // Electronical chain
                case "wire":
                case "switch":
                case "transistor":
                case "microchip":
                case "circuit":
                case "device":
                    return new Color32(15, 110, 135, 255);

                // biological chain
                case "cell":
                case "phlegm":
                case "tissue":
                case "muscle":
                case "organoid":
                case "organism":
                    return new Color32(0, 140, 0, 255);

                // mechanical chain
                case "alloy":
                case "tube":
                case "fixtures":
                case "frame":
                case "hydraulics":
                case "machine":
                    return new Color32(70, 50, 0, 255);

                default: // liquid and two other commodities
                    return new Color32(205, 205, 205, 255);
            }
        }


        public void Delta(JSONObject data)
        {
            // Delta data{"store":{"energy":2644,"battery":4449},"actionLog":{"produce":{"x":21,"y":19,"resourceType":"energy"}},"cooldownTime":1,940481E+07}
            AdjustScale();
            _isOnCooldown = _factory.CooldownTime > ScreepsAPI.Time;
            _lightningRing.enabled = false;

            if (!data.HasField("actionLog") || !data["actionLog"].HasField("produce") || data["actionLog"]["produce"]["resourceType"] == null)
            {
                showProduction("none");
                return;
            };
            var product = data["actionLog"]["produce"]["resourceType"].str;

            ////string product = DebugProduceRendering();

            _lightningRing.enabled = true;
            showProduction(product);
            return;
        }

        private int produceIndex = 0;
        private string DebugProduceRendering()
        {
            var renderQueue = new System.Collections.Generic.List<string> {
                "battery",
                "energy",
                "O",
                "oxidant",
                "purifier",
                "condensate",
                "wire",
                "cell",
                "alloy"
            };

            showProduction("none");

            if (renderQueue.Count == produceIndex)
            {
                produceIndex = 0;
            }

            var product = renderQueue[produceIndex];
            produceIndex++;
            NotifyText.Message($"<size=40><b>{product}</b></size>", resourceToColor(product));

            return product;

        }

        public void Unload(RoomObject roomObject)
        {
            _factory = null;
        }

        private void showProduction(string product)
        {
            if (product == "none")
            {
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

            if (isRawResource(product))
            {
                _rawProduct.enabled = true;
                _rawProduct.materials[0].SetColor("EmissionColor", c);
                return;
            }

            if (isPackedResource(product))
            {
                _packedProduct.enabled = true;
                _packedProduct.materials[0].SetColor("EmissionColor", c);
                return;
            }

            _commodityProduct.enabled = true;
            _commodityProduct.materials[1].SetColor("EmissionColor", c);
        }

        private void Update()
        {
            if (_factory == null)
                return;

            if (_isOnCooldown)
            {
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