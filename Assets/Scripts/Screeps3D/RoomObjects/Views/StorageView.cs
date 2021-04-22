using Common;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class StorageView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleAxes _energyDisplay = default;
        [SerializeField] private Renderer _storageContents = default;

        private Storage _energyObject;

        public void Init()
        {
        }

        private Color GetColorForResource(string resource)
        {
            Color c = Color.red;
            if (!Constants.ResourceColors.TryGetValue(resource, out c))
            {
                c = Constants.ResourceColors["other"];
            }
            return c;
        }
        private Texture2D CreateStorageTexture()
        {
            _energyObject.Store.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            List<string> resources = new List<string>(_energyObject.Store.Keys);

            int height = 1000;
            int width = 10;
            float yStep = 0.01f;

            int resourceIndex = 0;
            float nextResourceAt = 0f + 1000 * (float)System.Math.Round(_energyObject.Store[resources[resourceIndex]] / _energyObject.TotalCapacity, 3);

            Texture2D texture = new Texture2D(width, height);
            Debug.LogError("Resources to draw " + resources.Count);
            for (int y = 0; y < height; y++)
            {
                if (y >= nextResourceAt)
                {
                    Debug.LogError("Changing resource");
                    resourceIndex += 1;
                    if (resourceIndex >= resources.Count)
                    {
                        resourceIndex -= 1;
                        nextResourceAt = height + 1;
                    }
                    else
                    {
                        nextResourceAt = y + 100 * (float)System.Math.Round(_energyObject.Store[resources[resourceIndex]] / _energyObject.TotalCapacity, 2);
                    }
                    Debug.LogError("Current " + resources[resourceIndex] + " till " + nextResourceAt.ToString());
                }

                Color color = GetColorForResource(resources[resourceIndex]);
                for (int x = 0; x < Mathf.CeilToInt(width); x++)
                {
                    texture.SetPixel(Mathf.CeilToInt(x), Mathf.CeilToInt(y), color);
                }
            }
            texture.Apply();
            return texture;
        }

        public void Load(RoomObject roomObject)
        {
            _energyObject = roomObject as Storage;
            _storageContents.materials[0].SetFloat("EmissionStrength", 1.75f);
            // _storageContents.materials[0].SetColor("EmissionColor", Color.white);
            _storageContents.materials[0].SetTexture("EmissionTexture", this.CreateStorageTexture());
            AdjustScale();
        }

        public void Delta(JSONObject data)
        {
            AdjustScale();
            // _storageContents.materials[0].SetColor("EmissionColor", Color.white);
            _storageContents.materials[0].SetFloat("EmissionStrength", 1.75f);
            _storageContents.materials[0].SetTexture("EmissionTexture", this.CreateStorageTexture());
            // Debug.LogError("EnergyObject " + string.Join(",", keyList.ToArray()));
        }

        public void Unload(RoomObject roomObject)
        {
        }

        private void AdjustScale()
        {
            if (_energyObject != null)
            {
                _energyDisplay.SetVisibility(_energyObject.TotalResources / _energyObject.TotalCapacity);
            }
        }
    }
}