namespace Screeps3D.RoomObjects
{
    public class ConstructionSite : OwnedRoomObject, IProgress
    {
        public float Progress { get; set; }
        public float ProgressMax { get; set; }
        public string StructureType { get; set; }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);
            UnpackUtility.Progress(this, data);

            var typeData = data["structureType"];
            if (typeData != null)
                StructureType = typeData.str;
            
            ProgressMax = 1;
            if (Constants.ConstructionCost.ContainsKey(StructureType))
                ProgressMax = Constants.ConstructionCost[StructureType];
        }
    }
}