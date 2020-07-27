using Screeps3D.RoomObjects.Views;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    /*
     * https://docs.screeps.com/api/#Deposit
        {
	        "_id": "5ec7fcb6c57ac60118c7508d",
	        "type": "deposit",
	        "depositType": "silicon",
	        "x": 22,
	        "y": 16,
	        "room": "W31N20",
	        "harvested": 7584,
	        "decayTime": 1,
	        850442E+07,
	        "cooldownTime": 1,
	        845446E+07
        }
        
        hmmm is there a "lastCooldown" property? can it be calculated based on harvested and the cooldown time?

        Cooldown	0.001 * totalHarvested ^ 1.2	
        Decay	50,000 ticks after appearing or last harvest operation

        We could show the number of times harvested by calculating backwards from harvested and comparing with cooldownTime?
     */
    public class Deposit : Structure, IDepositCooldown, IDecay
    {
        public string DepositType { get; set; }

        public float NextDecayTime { get; set; }

        public int Harvested { get; set; }
        public float CooldownTime { get; set; }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);

            if (initial)
            {
                var depositType = data["depositType"];
                if (depositType != null)
                {
                    DepositType = depositType.str;
                }

                Initialized = true;
            }

            // TODO: Decay is a max of 50k ticks, we can render a progressbar based on this info. and if no "max decay" is specified. it can do what it normally does.
            UnpackUtility.Decay(this, data);

            var coolDownTimeData = data["cooldownTime"];
            if (coolDownTimeData != null)
            {
                this.CooldownTime = coolDownTimeData.n;
            }

            var harvestedData = data["harvested"];
            if (harvestedData != null)
            {
                this.Harvested = (int)harvestedData.n;
            }
        }

        protected internal override void AssignView()
        {
            if (Shown && View == null)
            {
                var type = this.Type;

                string dType = null;

                switch (this.DepositType)
                {
                    case Constants.BaseDeposit.Silicon:
                        dType = "silicon";
                        break;
                    case Constants.BaseDeposit.Metal:
                        dType = "metal";
                        break;
                    case Constants.BaseDeposit.Biomass:
                        dType = "biomass";
                        break;
                    case Constants.BaseDeposit.Mist:
                        dType = "mist";
                        break;
                }

                this.Type = dType == null ? "placeholder" : $"Deposits/{dType}";
                View = ObjectViewFactory.Instance.NewView(this);
                
                this.Type = dType;

                if (View)
                {
                    View.Load(this);
                }
            }
        }
    }
}