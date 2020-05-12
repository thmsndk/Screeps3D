using System;
using Screeps3D.RoomObjects;
using Screeps_API;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Screeps3D.Tools.Selection.Subpanels;
using Common;

namespace Screeps3D.Tools.Selection.Subpanels
{
    // Stats https://github.com/Arcath/screeps-tools/blob/master/src/tools/creep-designer.tsx#L82
    // TODO: info button that displays a tooltip with stats on hover?
    public class CreepBodyPanel : LinePanel
    {
        [SerializeField] private TMP_Text _BodyPartCountLabel = default;
        [SerializeField] private GridLayoutGroup _bodyParts = default;
        [SerializeField] private CreepBodyPart _bodyPartPrefab = default;

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

            ////SetTestBodyParts(_creep);

            _BodyPartCountLabel.SetText($"{_creep.Body.Parts.Count} parts");

            for (int i = 0; i < _creep.Body.Parts.Count; i++)
            {
                CreepPart part = _creep.Body.Parts[i];

                var bodyPart = Instantiate(_bodyPartPrefab, _bodyParts.transform);

                bodyPart.Load(part);

                bodyPart.name = $"{i} {part.Type}";
                //toggle.isOn = color == SelectedColor;
                //this.ScaleToggleButton(toggle, toggle.isOn);
            }
        }

        private void SetTestBodyParts(ICreepBody creep)
        {
            creep.Body.Parts.Clear();
            creep.Body.Parts.Add(new CreepPart { Type = "move" });
            creep.Body.Parts.Add(new CreepPart { Type = "move", Boost = "ZO" });

            creep.Body.Parts.Add(new CreepPart { Type = "work" });
            creep.Body.Parts.Add(new CreepPart { Type = "work", Boost = "UH" });
            creep.Body.Parts.Add(new CreepPart { Type = "work", Boost = "LH" });
            creep.Body.Parts.Add(new CreepPart { Type = "work", Boost = "ZH" });
            creep.Body.Parts.Add(new CreepPart { Type = "work", Boost = "GH" });

            creep.Body.Parts.Add(new CreepPart { Type = "attack" });
            creep.Body.Parts.Add(new CreepPart { Type = "attack", Boost = "UH" });

            creep.Body.Parts.Add(new CreepPart { Type = "ranged_attack" });
            creep.Body.Parts.Add(new CreepPart { Type = "ranged_attack", Boost = "KO" });

            creep.Body.Parts.Add(new CreepPart { Type = "heal" });
            creep.Body.Parts.Add(new CreepPart { Type = "heal", Boost = "LO" });

            creep.Body.Parts.Add(new CreepPart { Type = "tough" });
            creep.Body.Parts.Add(new CreepPart { Type = "tough", Boost = "GO" });

            creep.Body.Parts.Add(new CreepPart { Type = "claim" });

            creep.Body.Parts.Add(new CreepPart { Type = "carry" });
            creep.Body.Parts.Add(new CreepPart { Type = "carry", Boost = "KH" });

            for (int i = creep.Body.Parts.Count; i < 50; i++)
            {
                creep.Body.Parts.Add(new CreepPart { Type = "claim" });
            }
        }

        private void SetTestBoostsAndHitpoints(ICreepBody creep)
        {
            foreach (var part in creep.Body.Parts)
            {
                part.Hits = UnityEngine.Random.Range(0, 100);
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
            ////SetTestBoostsAndHitpoints(_creep);

            // TODO: how do we update the correct bodypart? - index should be preserved, we should be able to loop parts and look up data
            foreach (Transform child in _bodyParts.transform)
            {
                // TODO: look up data in body parts relative to index
                // TODO: scale image based on hitpoints
            }

            for (int i = 0; i < _bodyParts.transform.childCount; i++)
            {
                var child = _bodyParts.transform.GetChild(i);

                var part = _creep.Body.Parts[i];

                var bodyPart = child.GetComponent<CreepBodyPart>();
                
                bodyPart?.Delta(part);
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