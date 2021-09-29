using System.Numerics;
using Common;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class StorageView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private ScaleAxes _storeDisplay = default;
        [SerializeField] private Renderer _storageStore = default;
        [SerializeField] private Renderer _storageStoreTop = default;

        private Storage _storage;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _storage = roomObject as Storage;
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

            _storage.UpdateStoreTexture();

            _storageStore.materials[0].SetFloat("ySize", _storage.TotalResources / _storage.TotalCapacity);
            _storageStore.materials[0].SetFloat("EmissionStrength", .05f);
            _storageStore.materials[0].SetTexture("EmissionTexture", _storage._storeTexture);

            _storageStoreTop.materials[0].SetFloat("xSize", .2f);
            _storageStoreTop.materials[0].SetFloat("ySize", .2f);
            _storageStoreTop.materials[0].SetFloat("EmissionStrength", .05f);
            _storageStoreTop.materials[0].SetTexture("EmissionTexture", _storage._storeTexture);
        }

        private void AdjustScale()
        {
            if (_storage != null && _storeDisplay != null)
            {
                _storeDisplay.SetVisibility(_storage.TotalResources / _storage.TotalCapacity);
            }
        }
    }
}