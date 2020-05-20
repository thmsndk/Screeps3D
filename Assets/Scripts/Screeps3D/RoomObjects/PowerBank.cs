using System.Collections.Generic;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    /*
       {
           "_id":"5ec2b6b1bdd9e5f1ebcb7467",
           "type":"powerBank",
           "x":6,
           "y":13,
           "room":"E30N54",
           "store":{"power":1208},
           "hits":2000000,
           "hitsMax":2000000,
           "decayTime":1,834327E+07
       }
    */

    public class PowerBank : StoreStructure, IDecay
    {
        public float NextDecayTime { get; set; }

        public float maxTTL = 5000;
        public float maxCapacity = 10000;

        /// <summary>
        /// The maximum power a bank can spawn with
        /// </summary>
        public float PowerCapacity { get; set; }

        internal PowerBank()
        {
        }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);
            UnpackUtility.Decay(this, data);
            UnpackUtility.Store(this, data);
            this.TotalCapacity = maxCapacity;
        }
    }
}