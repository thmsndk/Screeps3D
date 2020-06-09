using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Screeps_API.ConsoleClientAbuse;
using Screeps3D.Effects;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class CreepBeamView: MonoBehaviour, IObjectViewComponent
    {
        
        [SerializeField] private Transform _rotationRoot = default;
        private static readonly Dictionary<string, BeamConfig> BeamConfigs = new Dictionary<string, BeamConfig>
        {   
            // HORSE 0.3f -> 0.7f
            // {"attack", new BeamConfig(Color.red, 0.7f, 0.3f)},
            {"rangedAttack", new BeamConfig(Color.blue, 0.7f, 0.3f)},
            {"rangedMassAttack", new BeamConfig(Color.blue, 0.7f, 0.3f)}, // RMA is an AOE effect, not a beam. should really be in another view
            {"rangedHeal", new BeamConfig(Color.green, 0.7f, 0.3f)},
            {"repair", new BeamConfig(Color.yellow, 0.7f, 0.3f)},
            {"build", new BeamConfig(Color.yellow, 0.7f, 0.3f)},    
            {"upgradeController", new BeamConfig(Color.yellow, 0.7f, 1f)}
        };

        private IActionObject _creep;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _creep = roomObject as IActionObject;
        }

        public void Delta(JSONObject data)
        {
            var beam = BeamConfigs.FirstOrDefault(c => _creep.Actions.ContainsKey(c.Key) && !_creep.Actions[c.Key].IsNull);
            if (beam.Value == null) {
                (_creep as Creep).actionTarget = null;
                return;
            }
            
            var action = _creep.Actions[beam.Key];
            var _cRO = _creep as Creep;
            (_creep as Creep).actionTarget = PosUtility.Convert(action, _cRO.Room);

            Debug.Log(beam.Key);
            Debug.Log(data.ToString());
            switch (beam.Key)
            {
                // In BumpView for now
                // case "attack":
                //     EffectsUtility.Attack(_creep as RoomObject, (Vector3)(_creep as Creep).actionTarget);
                //     break;
                case "rangedMassAttack":
                    EffectsUtility.ElectricExplosion(_creep as RoomObject);
                    break;
                default:
                    EffectsUtility.Beam(_creep as RoomObject, action, beam.Value);
                    break;
            }

            // StartCoroutine(Beam.Draw(_creep, _creep.Actions[beam.Key], _lineRenderer, beam.Value));
            
        }

        public void Unload(RoomObject roomObject)
        {
        }
    }
}
