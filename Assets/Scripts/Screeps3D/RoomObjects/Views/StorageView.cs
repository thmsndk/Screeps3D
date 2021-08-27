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

        private Storage _storage;

        public void Init()
        {
        }
        public void Load(RoomObject roomObject)
        {
            _storage = roomObject as Storage;
            AdjustScale();
            _storageContents.materials[0].SetFloat("EmissionStrength", .01f);
            _storageContents.materials[0].SetTexture("EmissionTexture", _storage.CreateStorageTexture());

            _storageContents.materials[1].SetFloat("EmissionStrength", .01f);
            _storageContents.materials[1].SetTexture("EmissionTexture", _storage.getLastStoreKeyTexture());
        }

        public void Delta(JSONObject data)
        {
            AdjustScale();
            _storageContents.materials[0].SetFloat("EmissionStrength", .01f);
            _storageContents.materials[0].SetTexture("EmissionTexture", _storage.CreateStorageTexture());

            _storageContents.materials[1].SetFloat("EmissionStrength", .01f);
            _storageContents.materials[1].SetTexture("EmissionTexture", _storage.getLastStoreKeyTexture());
        }

        public void Unload(RoomObject roomObject)
        {
        }

        private void AdjustScale()
        {
            if (_storage != null)
            {
                _energyDisplay.SetVisibility(_storage.TotalResources / _storage.TotalCapacity);
            }
        }
    }
}