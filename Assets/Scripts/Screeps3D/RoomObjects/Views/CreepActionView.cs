using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Screeps3D.Effects;

namespace Screeps3D.RoomObjects.Views
{
    public class CreepActionView : MonoBehaviour, IObjectViewComponent
    {

        [SerializeField] private Transform _creepRoot = default;
        private Creep _creep;
        private Vector3 _bumpRef;
        private bool _shouldBump;
        private bool _bumping;
        private bool _actionEffect;
        private bool _animating;

        // possible actions from Delta JSONdata: 
        // attack               bump + sparks
        // attacked             n/a
        // heal                 n/a
        // rangedHeal           beam 
        // healed               aura (green)
        // rangedAttack         beam
        // rangedMassAttack     aura
        // harvest              bump + sparks
        // repair               beam
        // build                beam
        // upgradeController    beam
        // reserveController    bump + aura(violet)
        // say                  text
        private static readonly Dictionary<string, bool> BumpConfig = new Dictionary<string, bool>
        {
            {"rangedAttack", false},
            {"rangedMassAttack", false}, // RMA is an AOE effect, not a beam. should really be in another view
            {"rangedHeal", false},
            {"repair", false},
            {"build", false},
            {"heal", false},
            {"attack", true},
            {"harvest", true},
            {"reserveController", true}
        };
        private static readonly Dictionary<string, BeamConfig> BeamConfigs = new Dictionary<string, BeamConfig>
        {   
            // HORSE 0.3f -> 0.7f
            {"rangedAttack", new BeamConfig(Color.blue, 0.7f, 0.3f)},
            {"rangedHeal", new BeamConfig(Color.green, 0.7f, 0.3f)},
            {"repair", new BeamConfig(Color.yellow, 0.7f, 0.3f)},
            {"build", new BeamConfig(Color.yellow, 0.7f, 0.3f)},
            {"upgradeController", new BeamConfig(Color.yellow, 0.7f, 1f)}
        };

        private static readonly Dictionary<string, Color32> AuraConfigs = new Dictionary<string, Color32> 
        {
            {"rangedMassAttack", new Color32(255, 255, 255, 0)},
            {"attack", new Color32(255, 111, 111, 0)},
            {"healed", new Color32(65, 140, 65, 0)},
            {"harvest", new Color32(255, 111, 111, 0)},
            {"reserveController", new Color32(255, 111, 111, 0)}
        };

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _creep = roomObject as Creep;
        }

        public void Delta(JSONObject data)
        {
            // if (_creep.BumpPosition == default(Vector3))
            //     return;

            _bumping = true;
            _animating = true;
            _actionEffect = false;
            _creep.actionTarget = null;

            var beam = BeamConfigs.FirstOrDefault(c => _creep.Actions.ContainsKey(c.Key) && !_creep.Actions[c.Key].IsNull);
            if (beam.Value != null) {
                var target = _creep.Actions[beam.Key];
                _creep.actionTarget = PosUtility.Convert(target, _creep.Room);
                doBeam(target, beam.Value);
            }

            var aura = AuraConfigs.FirstOrDefault(c => _creep.Actions.ContainsKey(c.Key) && !_creep.Actions[c.Key].IsNull);
            if (aura.Key != null) {
                doAura(aura.Key, aura.Value);
            }
            
            var shouldBump = BumpConfig.FirstOrDefault(c => _creep.Actions.ContainsKey(c.Key) && !_creep.Actions[c.Key].IsNull);
            _shouldBump = false;
            if(shouldBump.Value != null) {
                _shouldBump = shouldBump.Value;
            }
        }

        public void Unload(RoomObject roomObject)
        {
            _creep = null;
        }

        private void doBump() {

            var bumpCreep = _creep as IBump;

            var localBase = Vector3.zero;
            var targetLocalPos = localBase;
            var speed = .2f;
            if (_bumping)
            {
                // either half forward (creep is having -z as forward - reasons...)
                targetLocalPos = Vector3.back * .5f;
                speed = .1f;
            }
            // creep IS rotated towards source/action so we just need to go forward via Z, and do not care about X axis
            targetLocalPos.x = 0f;
            targetLocalPos.y = 0f;
            _creepRoot.transform.localPosition =
                Vector3.SmoothDamp(_creepRoot.transform.localPosition, targetLocalPos, ref _bumpRef, speed);
            var sqrMag = (_creepRoot.transform.localPosition - targetLocalPos).sqrMagnitude;  

            // if(_bumping && sqrMag < .005f && !_actionEffect) {
            //     EffectsUtility.Attack(_creep as RoomObject, bumpCreep.BumpPosition);
            //     _actionEffect = true;
            // }

            if (sqrMag < .0001f)
            {
                if (_bumping) {
                    _bumping = false;
                    // _actionEffect = false;
                }
                else {
                    _animating = false;
                }
            }
        }

        private void doBeam(JSONObject target, BeamConfig beamCfg) {
            EffectsUtility.Beam(_creep as RoomObject, target, beamCfg);
        }

        private void doAura(string auraType, Color32 auraColor) {
            var target = _creep.Actions[auraType];
            switch(auraType) {
                case "rangedMassAttack":
                    EffectsUtility.ElectricExplosion(_creep as RoomObject);
                    break;
                case "attack":
                    _creep.actionTarget = PosUtility.Convert(target, _creep.Room);
                    EffectsUtility.Attack((_creep as IBump).BumpPosition);
                    break;
                case "heal":
                    _creep.actionTarget = PosUtility.Convert(target, _creep.Room);
                    // no effect on healing crep, effect applied to healed creep
                    break;
                case "healed":
                    EffectsUtility.Heal(_creep as RoomObject);
                    break;
                case "reserveController":
                    _creep.actionTarget = PosUtility.Convert(target, _creep.Room);
                    EffectsUtility.Reserve((Vector3)_creep.actionTarget);
                    break;
                case "harvest":
                    _creep.actionTarget = PosUtility.Convert(target, _creep.Room);
                    EffectsUtility.Harvest((Vector3)_creep.actionTarget);
                    break;
            }
        }

        private void doAttack() {
            // 
        }

        private void Update()
        {
            if (_creep == null || !_animating)
                return;

            if(_shouldBump) {
                doBump();
            }
        }
    }
}