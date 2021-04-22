using Common;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class TerminalView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleAxes _energyDisplay = default;
        [SerializeField] private Renderer _storageContents = default;
        private Terminal _terminal;

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
            _terminal.Store.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            List<string> resources = new List<string>(_terminal.Store.Keys);

            int height = 1000;
            int width = 10;
            float yStep = 0.01f;

            int resourceIndex = 0;
            float step = 1000 * (float)System.Math.Round(_terminal.Store[resources[resourceIndex]] / _terminal.TotalCapacity, 3);
            Debug.LogError("energy amount " + (float)System.Math.Round(_terminal.Store[resources[resourceIndex]] / _terminal.TotalCapacity, 3));
            float nextResourceAt = step;
            Color color = GetColorForResource(resources[resourceIndex]);

            Texture2D texture = new Texture2D(width, height);
            // Debug.LogError("Resources to draw " + resources.Count);
            // Debug.LogError("Current " + resources[resourceIndex] + "[" + color.ToString() + "] till " + nextResourceAt.ToString());
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
                        nextResourceAt = y + 100 * (float)System.Math.Round(_terminal.Store[resources[resourceIndex]] / _terminal.TotalCapacity, 2);
                    }
                    color = GetColorForResource(resources[resourceIndex]);
                    // Debug.LogError("Current " + resources[resourceIndex] + "[" + color.ToString() + "] till " + nextResourceAt.ToString());
                }

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
            _terminal = roomObject as Terminal;
            AdjustScale();
            _storageContents.materials[0].SetFloat("EmissionStrength", 1.75f);
            _storageContents.materials[0].SetTexture("EmissionTexture", this.CreateStorageTexture());
        }

        public void Delta(JSONObject data)
        {
            AdjustScale();
            _storageContents.materials[0].SetFloat("EmissionStrength", 1.75f);
            _storageContents.materials[0].SetTexture("EmissionTexture", this.CreateStorageTexture());
        }

        public void Unload(RoomObject roomObject)
        {
        }

        private void AdjustScale()
        {
            if (_terminal != null)
            {
                _energyDisplay.SetVisibility(_terminal.TotalResources / _terminal.TotalCapacity);
            }
        }
    }
}