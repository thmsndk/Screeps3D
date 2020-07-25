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

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _creep = roomObject as Creep;
        }

        public void Delta(JSONObject data)
        {
            _shouldBump = false;
            _bumping = true;
            _animating = true;
            _actionEffect = false;
            _creep.ActionTarget = null;
            Color beamColor = Color.black;

            List<string> aKeys = _creep.Actions.Keys.ToList();
            for(int i = 0; i < aKeys.Count; i++) {
                string k = aKeys[i];
                if(_creep.Actions[k].IsNull) {
                    continue;
                }
                switch(k) {
                    case "rangedMassAttack": 
                        EffectsUtility.ElectricExplosion(_creep as RoomObject);
                        break;
                    // Bump Stuff
                    case "attack":
                        _shouldBump = true;
                        doParticles(k, new Color32(255, 45, 0, 255));
                        break;
                    case "harvest":
                        _shouldBump = true;
                        doParticles(k, new Color32(255, 111, 111, 255));
                        break;
                    case "reserveController":
                        _shouldBump = true;
                        doParticles(k, new Color32(255, 111, 111, 255));
                        break;
                    case "attackController":
                        _shouldBump = true;
                        doParticles(k, new Color32(255, 111, 111, 255));
                        break;
                    // Healed - just aura
                    case "healed":
                        doParticles(k, new Color32(65, 140, 65, 255));
                        break;
                    // Beam Stuff
                    case "rangedAttack": 
                        _creep.ActionTarget = PosUtility.Convert(_creep.Actions[k], _creep.Room);
                        doArcBeam((Vector3)_creep.ActionTarget, Color.blue);
                        // doBeam(_creep.Actions[k] , new BeamConfig(Color.blue, 0.3f, 0.3f));
                        doParticles(k, new Color32(0, 45, 255, 255));
                        break;
                    case "rangedHeal":
                        _creep.ActionTarget = PosUtility.Convert(_creep.Actions[k], _creep.Room); 
                        // doBeam(_creep.Actions[k], new BeamConfig(Color.green, 0.3f, 0.3f)); 
                        doArcBeam((Vector3)_creep.ActionTarget, Color.green);
                        break;
                    case "repair": 
                        _creep.ActionTarget = PosUtility.Convert(_creep.Actions[k], _creep.Room); 
                        // doBeam(_creep.Actions[k], new BeamConfig(Color.yellow, 0.3f, 0.3f));
                        doArcBeam((Vector3)_creep.ActionTarget, Color.yellow);
                        break;
                    case "build": 
                        _creep.ActionTarget = PosUtility.Convert(_creep.Actions[k], _creep.Room); 
                        // doBeam(_creep.Actions[k], new BeamConfig(Color.yellow, 0.3f, 0.3f));
                        doArcBeam((Vector3)_creep.ActionTarget, Color.yellow);
                        break;
                    case "upgradeController": 
                        _creep.ActionTarget = PosUtility.Convert(_creep.Actions[k], _creep.Room); 
                        // doBeam(_creep.Actions[k], new BeamConfig(Color.yellow, 0.3f, 1f));
                        doArcBeam((Vector3)_creep.ActionTarget, Color.yellow); 
                        break;                        
                }
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
            targetLocalPos.y = 0.3f;
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

        private void doArcBeam(Vector3 target, Color beamColor) {
            EffectsUtility.ArcBeam(_creepRoot.transform, target, new BeamConfig(beamColor, 0f, 0f));
        }

        private void doParticles(string particleType, Color32 auraColor) {
            var target = _creep.Actions[particleType];
            switch(particleType) {
                case "rangedAttack":
                    EffectsUtility.RangedAttackHit((Vector3)_creep.ActionTarget);                    
                    break;
                case "attack":
                    _creep.ActionTarget = PosUtility.Convert(target, _creep.Room);
                    EffectsUtility.Attack((_creepRoot.transform.position + (Vector3)_creep.ActionTarget) / 2);
                    break;
                case "heal":
                    _creep.ActionTarget = PosUtility.Convert(target, _creep.Room);
                    // no effect on healing crep, effect applied to healed creep
                    break;
                case "healed":
                    EffectsUtility.Heal(_creep as RoomObject);
                    break;
                case "reserveController":
                    _creep.ActionTarget = PosUtility.Convert(target, _creep.Room);
                    EffectsUtility.Reserve((Vector3)_creep.ActionTarget);
                    break;
                case "harvest":
                    _creep.ActionTarget = PosUtility.Convert(target, _creep.Room);
                    EffectsUtility.Harvest((Vector3)_creep.ActionTarget);
                    break;
            }
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