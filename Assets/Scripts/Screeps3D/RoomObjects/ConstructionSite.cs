namespace Screeps3D.RoomObjects
{
    /*
     * {
	        "type": "constructionSite",
	        "room": "W8N3",
	        "x": 11,
	        "y": 47,
	        "structureType": "lab",
	        "user": "ef2aa8bdb56fce9",
	        "progress": 0,
	        "progressTotal": 50000,
	        "_id": "8711a35914ff67f",
	        "meta": {
		        "revision": 0,
		        "created": 1,
		        591994E+12,
		        "version": 0
	        },
	        "$loki": 1628680
        }
     */
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
        }
    }
}