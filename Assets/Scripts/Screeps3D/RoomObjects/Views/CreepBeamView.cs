using System.Collections.Generic;
using System.Linq;
using Screeps3D.Effects;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class CreepBeamView: MonoBehaviour, IObjectViewComponent
    {
        private static readonly Dictionary<string, BeamConfig> BeamConfigs = new Dictionary<string, BeamConfig>
        {   
            {"rangedAttack", new BeamConfig(Color.blue, 0.3f, 0.3f)},
            {"rangedMassAttack", new BeamConfig(Color.blue, 0.3f, 0.3f)},
            {"rangedHeal", new BeamConfig(Color.green, 0.3f, 0.3f)},
            {"repair", new BeamConfig(Color.yellow, 0.3f, 0.3f)},
            {"build", new BeamConfig(Color.yellow, 0.3f, 0.3f)},
            {"upgradeController", new BeamConfig(Color.yellow, 0.3f, 1f)}
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
            EffectsUtility.Beam(_creep as RoomObject, _creep.Actions[beam.Key], beam.Value);
            // StartCoroutine(Beam.Draw(_creep, _creep.Actions[beam.Key], _lineRenderer, beam.Value));
            
        }

        public void Unload(RoomObject roomObject)
        {
        }
    }
}
