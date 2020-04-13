using System;
using Screeps3D.RoomObjects;
using Screeps_API;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class CreepBodyPanel : LinePanel
    {
        [SerializeField] private GridLayoutGroup _bodyParts;
        [SerializeField] private Toggle _bodyPartPrefab;

        private RoomObject _roomObject;
        private ICreepBody _creep;

        public override string Name
        {
            get { return "CreepBody"; }
        }

        public override Type ObjectType
        {
            get { return typeof(ICreepBody); }
        }

        public override void Load(RoomObject roomObject)
        {
            _creep = roomObject as ICreepBody;

            _roomObject = roomObject;
            _roomObject.OnDelta += OnDelta;

            // TODO: use the objectfactory so we don't instantiate objects all the time.
            DestroyBodyParts();

            for (int i = 0; i < _creep.Body.Parts.Count; i++)
            {
                CreepPart part = _creep.Body.Parts[i];

                var toggle = Instantiate(_bodyPartPrefab, _bodyParts.transform);

                var color = Color.clear;

                switch (part.Type)
                {
                    case "move":
                        color = Constants.CreepBodyPartColors.Move;
                        break;
                    case "work":
                        color = Constants.CreepBodyPartColors.Work;
                        break;
                    case "attack":
                        color = Constants.CreepBodyPartColors.Attack;
                        break;
                    case "ranged_attack":
                        color = Constants.CreepBodyPartColors.RangedAttack;
                        break;
                    case "heal":
                        color = Constants.CreepBodyPartColors.Heal;
                        break;
                    case "tough":
                        color = Constants.CreepBodyPartColors.Tough;
                        break;
                    case "claim":
                        color = Constants.CreepBodyPartColors.Claim;
                        break;
                    case "carry":
                        color = Constants.CreepBodyPartColors.Carry;
                        break;
                }

                var colors = toggle.colors;
                
                colors.normalColor = color;
                colors.selectedColor = UnityEngine.Random.ColorHSV(); // use for boosted?
                toggle.colors = colors;

                toggle.name = $"{i} {part.Type}";
                //toggle.isOn = color == SelectedColor;
                //this.ScaleToggleButton(toggle, toggle.isOn);
            }
        }

        private void DestroyBodyParts()
        {
            foreach (Transform child in _bodyParts.transform)
            {
                var toggle = child.GetComponent<Toggle>();
                toggle?.onValueChanged.RemoveAllListeners();

                Destroy(child.gameObject);
            }
        }

        private void OnDelta(JSONObject obj)
        {
            // TODO: how do we update the correct bodypart? - index should be preserved, we should be able to loop parts and look up data
            foreach (Transform child in _bodyParts.transform)
            {
                // TODO: look up data in body parts relative to index
                // TODO: scale image based on hitpoints
            }
        }

        private void OnTick(long obj)
        {
            //UpdateLabel();
        }

        public override void Unload()
        {
            DestroyBodyParts();

            _creep = null;
            _roomObject.OnDelta -= OnDelta;
            _roomObject = null;
        }

        ////private void UpdateLabel()
        ////{
        ////    if (_decay.NextDecayTime == 0f)
        ////    {
        ////        Hide();
        ////        return;
        ////    }

        ////    _label.text = string.Format("{0:n0}", _decay.NextDecayTime - _decay.Room.GameTime);
        ////}
        
        //// For adjusting body part sizes
        ////protected void AdjustSize(string partType, float min, float flex)
        ////{
        ////    var amount = 0f;
        ////    foreach (var part in creep.Body.Parts)
        ////    {
        ////        if (part.Type != partType)
        ////            continue;
        ////        amount += part.Hits;
        ////    }

        ////    var scaleAmount = 0f;
        ////    if (amount > 0)
        ////    {
        ////        scaleAmount = (amount / 5000) * flex + min;
        ////    }

        ////    _partDisplay.transform.localScale = Vector3.one * scaleAmount;
        ////}
    }
}