using Common;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class ContainerView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleAxes _containerStoreDisplay = default;
        [SerializeField] private Renderer _containerStore = default;
        private Container _container;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _container = roomObject as Container;
            AdjustScale();
            UpdateStore();
        }

        public void Delta(JSONObject data)
        {
            AdjustScale();
            UpdateStore();
        }

        public void Unload(RoomObject roomObject)
        {
        }
        private void UpdateStore()
        {

            var storeTexture = CreateStoreTexture();
            _containerStore.materials[0].SetFloat("xSize", .2f);
            _containerStore.materials[0].SetFloat("ySize", .2f);
            _containerStore.materials[0].SetFloat("EmissionStrength", .05f);
            _containerStore.materials[0].SetTexture("EmissionTexture", storeTexture);
        }

        private void AdjustScale()
        {
            if (_container != null)
            {
                _containerStoreDisplay.SetVisibility(_container.TotalResources / _container.TotalCapacity);
            }
        }

        public Texture2D CreateStoreTexture(bool excludeEnergy = false)
        {
            var storeCopy = _container.Store;
            var storeCapacity = _container.TotalResources;

            if (excludeEnergy)
            {
                if (storeCopy.ContainsKey("energy"))
                {
                    storeCapacity = -storeCopy["energy"];
                }
            }
            storeCopy.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            List<string> resources = new List<string>(storeCopy.Keys);

            int height = 1000;
            int width = 10;
            float yStep = 0.01f;

            int resourceIndex = 0;
            float percent = (float)System.Math.Round(storeCopy[resources[resourceIndex]] / storeCapacity, 3);
            float nextResourceAt = 1000 * percent;
            Color color = Constants.GetComplexResourceColor(resources[resourceIndex]);

            Texture2D texture = new Texture2D(width, height);
            // Debug.LogError("Resources to draw " + resources.Count);
            // Debug.LogError("Current " + resources[resourceIndex] + " [" + color.ToString() + "][" + this.Store[resources[resourceIndex]] + "][" + this.TotalResources + "][" + percent + "][" + nextResourceAt.ToString() + "]");
            for (int y = 0; y < height; y++)
            {
                if (y >= nextResourceAt)
                {
                    resourceIndex += 1;
                    if (resourceIndex >= resources.Count)
                    {
                        resourceIndex -= 1;
                        nextResourceAt = height + 1;
                    }
                    else
                    {
                        if (excludeEnergy && resources[resourceIndex] == "energy")
                        {
                            resourceIndex += 1;
                            continue;
                        }
                        percent = (float)System.Math.Round(storeCopy[resources[resourceIndex]] / storeCapacity, 3);
                        nextResourceAt = y + 1000 * percent;
                    }

                    color = Constants.GetComplexResourceColor(resources[resourceIndex]);
                    // Debug.LogError("Current " + resources[resourceIndex] + " [" + color.ToString() + "][" + this.Store[resources[resourceIndex]] + "][" + this.TotalResources + "][" + percent + "][" + nextResourceAt.ToString() + "]");
                }

                for (int x = 0; x < Mathf.CeilToInt(width); x++)
                {
                    texture.SetPixel(Mathf.CeilToInt(x), Mathf.CeilToInt(y), color);
                }
            }
            texture.Apply();
            return texture;
        }
    }
}