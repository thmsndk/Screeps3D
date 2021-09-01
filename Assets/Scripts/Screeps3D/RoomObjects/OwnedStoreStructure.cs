using Microsoft.VisualBasic;
using System.Collections.Generic;
using Common;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    public class OwnedStoreStructure : OwnedStructure, IStoreObject
    {
        public float TotalCapacity { get; set; }
        public float TotalResources { get; set; }

        public Dictionary<string, float> Store { get; private set; }
        public Dictionary<string, float> Capacity { get; private set; }

        public OwnedStoreStructure()
        {
            Store = new Dictionary<string, float>();
            Capacity = new Dictionary<string, float>();
        }

        public Texture2D CreateResourceTexture(string resource)
        {
            int height = 1;
            int width = 1;
            Texture2D texture = new Texture2D(width, height);
            texture.SetPixel(1, 1, Constants.GetComplexResourceColor(resource));
            texture.Apply();
            return texture;
        }

        public Texture2D CreateStoreTexture(bool excludeEnergy = false)
        {
            var storeCopy = this.Store;
            var storeCapacity = this.TotalResources;

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

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);

            UnpackUtility.Store(this, data);
        }
    }
}