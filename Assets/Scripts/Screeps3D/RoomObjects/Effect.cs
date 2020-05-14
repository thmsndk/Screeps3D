using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Screeps3D.RoomObjects
{
    // TODO: should probably move it to a "shared" place
    public enum EffectType
    {
        // https://docs.screeps.com/api/#Constants
        PWR_GENERATE_OPS = 1,
        PWR_OPERATE_SPAWN = 2,
        PWR_OPERATE_TOWER = 3,
        PWR_OPERATE_STORAGE = 4,
        PWR_OPERATE_LAB = 5,
        PWR_OPERATE_EXTENSION = 6,
        PWR_OPERATE_OBSERVER = 7,
        PWR_OPERATE_TERMINAL = 8,
        PWR_DISRUPT_SPAWN = 9,
        PWR_DISRUPT_TOWER = 10,
        PWR_DISRUPT_SOURCE = 11,
        PWR_SHIELD = 12,
        PWR_REGEN_SOURCE = 13,
        PWR_REGEN_MINERAL = 14,
        PWR_DISRUPT_TERMINAL = 15,
        PWR_OPERATE_POWER = 16,
        PWR_FORTIFY = 17,
        PWR_OPERATE_CONTROLLER = 18,
        PWR_OPERATE_FACTORY = 19,

        EFFECT_INVULNERABILITY = 1001,
        EFFECT_COLLAPSE_TIMER = 1002,
    }

    // TODO: should probably move it to a "shared" place
    public enum PowerType
    {
        // https://docs.screeps.com/api/#Constants
        PWR_GENERATE_OPS = 1,
        PWR_OPERATE_SPAWN = 2,
        PWR_OPERATE_TOWER = 3,
        PWR_OPERATE_STORAGE = 4,
        PWR_OPERATE_LAB = 5,
        PWR_OPERATE_EXTENSION = 6,
        PWR_OPERATE_OBSERVER = 7,
        PWR_OPERATE_TERMINAL = 8,
        PWR_DISRUPT_SPAWN = 9,
        PWR_DISRUPT_TOWER = 10,
        PWR_DISRUPT_SOURCE = 11,
        PWR_SHIELD = 12,
        PWR_REGEN_SOURCE = 13,
        PWR_REGEN_MINERAL = 14,
        PWR_DISRUPT_TERMINAL = 15,
        PWR_OPERATE_POWER = 16,
        PWR_FORTIFY = 17,
        PWR_OPERATE_CONTROLLER = 18,
        PWR_OPERATE_FACTORY = 19,

        EFFECT_INVULNERABILITY = 1001,
        EFFECT_COLLAPSE_TIMER = 1002,
    }

    public struct EffectDto
    {
        public EffectType Effect { get; set; } // TODO: should be an enum that we translate when mapping this is a code for a specific effect
        public PowerType Power { get; set; } // this seems to be the same as effect?
        public int EndTime { get; set; }
        public int Duration { get; set; }

        public EffectDto(int effect, int power, int endTime, int duration )
        {
            this.Effect = (EffectType)effect;
            this.Power = (PowerType)power;
            this.EndTime = endTime;
            this.Duration = duration;

        }
    };
}
