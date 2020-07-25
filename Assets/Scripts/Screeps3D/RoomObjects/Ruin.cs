using Screeps_API;
using Screeps3D.RoomObjects.Views;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    /**
     * {
	        "_id": "5e91a19d3a405e7d83c5462d",
	        "type": "ruin",
	        "room": "W14S36",
	        "x": 17,
	        "y": 6,
	        "structure": {
		        "id": "5e91894e3d05c456772035b9",
		        "type": "rampart",
		        "hits": 0,
		        "hitsMax": 3E+08,
		        "user": "2"
	        },
	        "destroyTime": 2,
	        396669E+07,
	        "decayTime": 2,
	        403374E+07,
	        "user": "2",
	        "store": {}
        }

     */
    public class Ruin : PlaceHolderRoomObject /*OwnedStoreStructure*/, ICooldownObject, IDecay
    {
        public float NextDecayTime { get; set; }

        public float Cooldown { get; set; }

        public string StructureId { get; set; }
        public string StructureType { get; set; }
        ////public string StructureUserId { get; set; }
        ////public ScreepsUser StructureOwner { get; set; }

        public float DestroyTime { get; set; }

        private MeshFilter meshFilter { get; set; }

        internal override void Unpack(JSONObject data, bool initial)
        {
            if (data.keys.Count == 0)
            {
                return;
            }

            base.Unpack(data, initial);

            UnpackUtility.Cooldown(this, data);
            UnpackUtility.Decay(this, data);

            var destroyData = data["destroyTime"];

            if (destroyData != null)
            {
                this.DestroyTime = data["destroyTime"].n;
            }

            var structure = data["structure"];

            if (structure != null)
            {
                UnpackUtility.HitPoints(this, structure);

                var structureIdData = structure["_id"];
                if (structureIdData != null)
                {
                    this.StructureId = structureIdData.str;
                }

                var structureTypeData = structure["type"];
                if (structureTypeData != null)
                {
                    this.StructureType = structureTypeData.str;
                }

                UnpackUtility.Owner(this, structure);
                //var userData = structure["user"];
                //if (userData != null)
                //{
                //    this.StructureUserId = userData.str;
                //    this.StructureOwner = ScreepsAPI.UserManager.GetUser(userData.str);
                //}
            }
        }

        protected internal override void AssignView()
        {
            if (Shown && View == null)
            {
                var type = this.Type;

                switch(this.StructureType) {                    
                    case Constants.TypeExtension:
                    case Constants.TypeInvaderCore:
                    case Constants.TypePowerBank:
                    case Constants.TypeSpawn:
                    case Constants.TypeTower:
                    case Constants.TypeLab:
                    case Constants.TypeRampart:
                    case Constants.TypeFactory:
                    case Constants.TypeStorage:
                    case Constants.TypeTerminal:
                    case Constants.TypeContainer:
                    case Constants.TypeLink:
                    case Constants.TypePowerSpawn:
                    case Constants.TypeObserver:
                    case Constants.TypeExtractor:
                    case Constants.TypeConstructedWall:
                    case Constants.TypeRoad:
                    case Constants.TypeNuker:
                        this.Type = $"ruins/ruin_{this.StructureType}";
                        break;
                    default: 
                        this.Type = this.StructureType;
                        break;
                }
                
                View = ObjectViewFactory.Instance.NewView(this);

                this.Type = type + " ( " + this.StructureType + " )";

                if (View)
                {
                    View.Load(this);
                }
            }
        }
    }
}