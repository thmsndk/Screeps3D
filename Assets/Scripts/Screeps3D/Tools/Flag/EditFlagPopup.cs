using Screeps_API;
using Screeps3D.RoomObjects;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screeps3D.Tools.Selection
{
    // TODO: flag templates, common used colors / configurations and start names might be useful

    public class EditFlagPopup : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _flagName;
        [SerializeField] private TMP_Text _flagNameWarning;
        [SerializeField] private FlagColorToggle _primaryFlagColor;
        [SerializeField] private FlagColorToggle _secondaryFlagColor;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _okButton;

        private bool _newFlag;
        private Flag _flag;

        public Action OnFlagCreated;

        public Action OnFlagColorChanged;

        public Action OnCancel;

        private Constants.FlagColor _initialPrimaryColor;
        private Constants.FlagColor _initialSecondaryColor;
        private int _initialX;
        private int _initialY;

        private void Start()
        {
            _cancelButton.onClick.AddListener(CancelClicked);
            _okButton.onClick.AddListener(OkClicked);
            _flagName.onDeselect.AddListener(FlagNameDeselect);

            _primaryFlagColor.OnColorChange += PrimaryColorChange;
            _secondaryFlagColor.OnColorChange += SecondaryColorChange;
        }
        private void OnDestroy()
        {
            _cancelButton.onClick.RemoveListener(CancelClicked);
            _okButton.onClick.RemoveListener(OkClicked);
            _flagName.onDeselect.RemoveListener(FlagNameDeselect);

            _primaryFlagColor.OnColorChange -= PrimaryColorChange;
            _secondaryFlagColor.OnColorChange -= SecondaryColorChange;
        }

        public void Load(Flag flag, bool newFlag = false)
        {
            ////Debug.Log($"flag loaded create {newFlag}=> {_flag?.Room?.ShardName}/{_flag?.Room?.RoomName}");
            _flag = flag;
            _newFlag = newFlag;
            _flagNameWarning.gameObject.SetActive(false);

            _flagName.readOnly = !newFlag;

            if (newFlag)
            {
                GenerateUniqueFlagName();
            }
            else
            {
                _flag.PauseDeltaUpdates = true;
                _flagName.text = flag.Name;
            }

            _initialX = _flag.X;
            _initialY = _flag.Y;

            _initialPrimaryColor = (Constants.FlagColor)_flag.PrimaryColor;
            _initialSecondaryColor = (Constants.FlagColor)_flag.SecondaryColor;

            _primaryFlagColor.SetColor(_initialPrimaryColor);
            _secondaryFlagColor.SetColor(_initialSecondaryColor);
        }

        private void GenerateUniqueFlagName()
        {
            ScreepsAPI.Http.GenerateUniqueFlagName(_flag.Room.ShardName, onSuccess: jsonString =>
            {
                var result = new JSONObject(jsonString);

                var ok = result["ok"];

                if (ok.n == 1)
                {
                    var flagName = result["name"];
                    _flagName.text = flagName.str;
                    _flag.Name = _flagName.text;
                }
            });
        }

        public void Save()
        {
            if (_newFlag || (_flag.X != _initialX || _flag.Y != _initialY))
            {
                ScreepsAPI.Http.CreateFlag(
                    _flag.Room.ShardName,
                    _flag.Room.RoomName,
                    _flag.X,
                    _flag.Y,
                    _flag.Name,
                    _flag.PrimaryColor,
                    _flag.SecondaryColor,
                    onSuccess: jsonString =>
                    {
                        var result = new JSONObject(jsonString);

                        var ok = result["ok"];

                        if (ok.n == 1)
                        {
                            OnFlagCreated?.Invoke();
                        }
                        else
                        {
                            // error
                        }
                    });
            }
            else if ((int)_initialPrimaryColor != _flag.PrimaryColor || (int)_initialSecondaryColor != _flag.SecondaryColor)
            {
                ScreepsAPI.Http.ChangeFlagColor(
                    _flag.Room.ShardName,
                    _flag.Room.RoomName,
                    _flag.Name,
                    _flag.PrimaryColor,
                    _flag.SecondaryColor,
                    onSuccess: jsonString =>
                    {
                        var result = new JSONObject(jsonString);

                        var ok = result["ok"];

                        if (ok.n == 1)
                        {
                            OnFlagColorChanged?.Invoke();
                        }
                        else
                        {
                            // error
                        }
                    });
            }

            _flag.PauseDeltaUpdates = false;

        }

        private void PrimaryColorChange(Constants.FlagColor flagColor)
        {
            _flag.PrimaryColor = (int)flagColor;
        }
        private void SecondaryColorChange(Constants.FlagColor flagColor)
        {
            _flag.SecondaryColor = (int)flagColor;
        }

        private void FlagNameDeselect(string text)
        {
            if (_flag.Name != _flagName.text)
            {
                _flagNameWarning.gameObject.SetActive(false);
            }
            _flag.Name = _flagName.text;
        }

        private void CheckUniqueFlag(string text, Action<bool> isUnique)
        {
            // POST https://screeps.com/api/game/check-unique-flag-name
            // Request: {"name":"Flag1","shard":"shard3"}
            // Response: {"error":"name exists"} || {"ok":1}
            ScreepsAPI.Http.CheckUniqueFlag(
                    _flag.Room.ShardName,
                    _flag.Name,
                    onSuccess: jsonString =>
                    {
                        var result = new JSONObject(jsonString);

                        var ok = result["ok"];
                        var error = result["error"];
                        ////Debug.Log(result);
                        if (error != null && !error.IsNull && error.str == "name exists")
                        {
                            _flagNameWarning.SetText($"{text} is already in use, press OK again to confirm.");
                            _flagNameWarning.gameObject.SetActive(true);
                            isUnique(false);
                        }
                        else
                        {
                            isUnique(true);
                        }
                    });
        }

        private void CancelClicked()
        {
            _flag.PrimaryColor = (int)_initialPrimaryColor;
            _flag.SecondaryColor = (int)_initialSecondaryColor;

            _flag.PauseDeltaUpdates = false;
            OnCancel?.Invoke();
        }

        private void OkClicked()
        {
            ////Debug.Log("OkClicked");

            if (_flagNameWarning.gameObject.activeSelf)
            {
                ////Debug.Log("Save");
                _flagNameWarning.gameObject.SetActive(false);
                Save();

                return;
            }

            CheckUniqueFlag(_flagName.text, (isUnique) =>
            {
                if (!isUnique)
                {

                    if (_flagNameWarning.gameObject.activeSelf)
                    {
                        ////Debug.Log("warning");
                        return;
                    }
                }

                Save();
            });
        }
    }
}