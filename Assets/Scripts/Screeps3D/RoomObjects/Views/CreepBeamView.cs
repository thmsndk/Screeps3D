using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Screeps_API.ConsoleClientAbuse;
using Screeps3D.Effects;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class CreepBeamView: MonoBehaviour, IObjectViewComponent
    {
        private static readonly Dictionary<string, BeamConfig> BeamConfigs = new Dictionary<string, BeamConfig>
        {   
            // HORSE 0.3f -> 0.7f
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
            if (beam.Value == null) return;
            var action = _creep.Actions[beam.Key];

            switch (beam.Key)
            {
                case "rangedMassAttack":
                    Debug.Log(beam.Key);
                    Debug.Log(data.ToString());
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
