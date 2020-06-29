using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Screeps3D.Menus.Respawn
{
    public class RespawnWarningPopup : MonoBehaviour
    {
        [SerializeField] private Button _cancelButton = default;
        [SerializeField] private Button _respawnButton = default;

        public Action OnCancel;
        public Action OnRespawn;
        private void Start()
        {
            _cancelButton.onClick.AddListener(CancelClicked);
            _respawnButton.onClick.AddListener(RespawnClicked);
        }

        private void RespawnClicked()
        {
            OnRespawn?.Invoke();
        }
        private void CancelClicked()
        {
            OnCancel?.Invoke();
        }

        private void OnDestroy()
        {
            _cancelButton.onClick.RemoveListener(CancelClicked);
            _respawnButton.onClick.RemoveListener(RespawnClicked);
        }
    }
}
