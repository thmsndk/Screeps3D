using Assets.Scripts.Screeps_API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Screeps3D.Main
{
    public class ChooseSS3UnifiedCredentialsFileLocationPopup : MonoBehaviour
    {
        [SerializeField] private Button _cancelButton = default;
        [SerializeField] private Button _okButton = default;

        [SerializeField] private TMP_Dropdown _validFileLocations = default;

        public Action OnCancel;
        public Action OnOkClicked;
        private void Start()
        {
            _cancelButton.onClick.AddListener(CancelClicked);
            _okButton.onClick.AddListener(OkClicked);

            _validFileLocations.ClearOptions();
            var validLocations = SS3UnifiedCredentials.GetValidConfigPaths().Where(path => path.EndsWith(".yml")).ToList();

            _validFileLocations.AddOptions(validLocations); ;
        }

        private void OkClicked()
        {
            var selectedConfigFile = _validFileLocations.options[_validFileLocations.value];

            SS3UnifiedCredentials.SetConfigFile(selectedConfigFile.text);

            OnOkClicked?.Invoke();
        }
        private void CancelClicked()
        {
            OnCancel?.Invoke();
        }

        private void OnDestroy()
        {
            _cancelButton.onClick.RemoveListener(CancelClicked);
            _okButton.onClick.RemoveListener(OkClicked);
        }
    }
}
