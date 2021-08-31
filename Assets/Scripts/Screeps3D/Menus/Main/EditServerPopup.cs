using Assets.Scripts.Screeps_API;
using Screeps_API;
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
    public class EditServerPopup : MonoBehaviour
    {
        [SerializeField] private Button _cancelButton = default;
        [SerializeField] private Button _okButton = default;

        [SerializeField] private TMP_InputField _key = default;
        [SerializeField] private TMP_InputField _name = default;
        [SerializeField] private TMP_InputField _host = default;
        [SerializeField] private TMP_InputField _port = default;
        [SerializeField] private TMP_InputField _path = default;

        [SerializeField] private Toggle _persistCredentials = default;

        [SerializeField] private GameObject _tokenAndLabel = default;
        [SerializeField] private TMP_InputField _token = default;

        [SerializeField] private GameObject _usernameAndLabel = default;
        [SerializeField] private TMP_InputField _username = default;

        [SerializeField] private GameObject _passwordAndLabel = default;
        [SerializeField] private TMP_InputField _password = default;

        [SerializeField] private Toggle _usesSSL = default;

        private IScreepsServer server;

        public Action OnCancel;
        public Action OnOkClicked;
        private void Start()
        {
            _cancelButton.onClick.AddListener(CancelClicked);
            _okButton.onClick.AddListener(OkClicked);
        }

        public void SetServer(IScreepsServer server)
        {
            this.server = server;

            _key.text = server.Key;
            _name.text = server.Name;
            _host.text = server.Address.HostName;
            _port.text = server.Address.Port;
            _path.text = server.Address.Path;
            _usesSSL.isOn = server.Address.Ssl;

            
            if (server.Official)
            {
                _key.readOnly = true;
                _name.readOnly = true;
                _host.readOnly = true;
                _port.readOnly = true;
                _path.readOnly = true;
                //_usesSSL // can't mark this read only
                _tokenAndLabel.gameObject.SetActive(true);
                _usernameAndLabel.gameObject.SetActive(false);
                _passwordAndLabel.gameObject.SetActive(false);
            }
            else
            {
                _key.readOnly = false;
                _name.readOnly = false;
                _host.readOnly = false;
                _port.readOnly = false;
                _path.readOnly = false;
                //_usesSSL // can't mark this read only

                _tokenAndLabel.gameObject.SetActive(false);
                _usernameAndLabel.gameObject.SetActive(true);
                _passwordAndLabel.gameObject.SetActive(true);
            }

            _token.text = server.Credentials.Token;

            _username.text = server.Credentials.Email;
            _password.text = server.Credentials.Password;

        }

        private void OkClicked()
        {
            var oldKey = this.server.Key;

            server.Key = _key.text;
            server.Name = _name.text;
            server.Address.HostName = _host.text;
            server.Address.Port = _port.text;
            server.Address.Path = _path.text;
            server.Address.Ssl = _usesSSL.isOn;

            server.Credentials.Token = _token.text;

            server.Credentials.Email = _username.text;
            server.Credentials.Password = _password.text;

            SS3UnifiedCredentials.SaveServer(this.server, oldKey, _persistCredentials.isOn);

            this.gameObject.SetActive(false);

            OnOkClicked?.Invoke();
        }
        private void CancelClicked()
        {
            this.gameObject.SetActive(false);

            OnCancel?.Invoke();
        }

        private void OnDestroy()
        {
            _cancelButton.onClick.RemoveListener(CancelClicked);
            _okButton.onClick.RemoveListener(OkClicked);
        }
    }
}
