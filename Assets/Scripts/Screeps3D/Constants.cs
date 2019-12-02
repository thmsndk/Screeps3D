using System.Collections.Generic;
using UnityEngine;

namespace Screeps3D
{
    public static class Constants
    {
        public const string TypeStorage = "storage";
        public const string TypeExtension = "extension";
        public const string TypeSpawn = "spawn";
        public const string TypeCreep = "creep";
        public const string TypeTower = "tower";
        public const string TypeController = "controller";
        public const string TypeTerminal = "terminal";
        public const string TypeContainer = "container";
        public const string TypeLink = "link";
        public const string TypeRampart = "rampart";
        public const string TypeConstruction = "constructionSite";
        public const string TypeLab = "lab";
        public const string TypeConstructedWall = "constructedWall";
        public const string TypeNuker = "nuker";
        public const string TypeMineral = "mineral";
        public const string TypePowerSpawn = "powerSpawn";
        public const string TypeSource = "source";
        public const string TypeTombstone = "tombstone";
        public const string TypeResource = "energy";
        public const string TypeSourceKeeperLair = "keeperLair";
        public const string TypePowerBank = "powerBank";
        public const string TypePowerCreep = "powerCreep";
        public const string TypePortal = "portal";
        public const string TypeRoad = "road";
        public const string TypeObserver = "observer";
        public const string TypeExtractor = "extractor";
        public const string TypeFactory = "factory";
        public const string TypeDeposit = "deposit";
        public const string TypeRuin = "ruin";
        public const string TypeInvaderCore = "invadercore";
        



        public const float ShardHeight = 100;

        public static readonly Dictionary<int, float> ControllerLevels = new Dictionary<int, float>
        {
            {1, 200},
            {2, 45000},
            {3, 135000},
            {4, 405000},
            {5, 1215000},
            {6, 3645000},
            {7, 10935000}
        };

        public static readonly Dictionary<string, bool> ContactActions = new Dictionary<string, bool>
        {
            {"attack", true},
            {"heal", true},
            {"harvest", true},
            {"reserveController", true},
            {"rangedAttack", false},
            {"rangedHeal", false},
            {"build", false},
            {"repair", false},
            {"upgradeController", false}
        };
        
        public static readonly Dictionary<int, Color> FlagColors = new Dictionary<int, Color>
        {
            {1, new Color(.95f, .262f, .218f)},
            {2, new Color(0.6117f, 0.1529411764705882f, 0.6901960784313725f)},
            {3, new Color(0.1294117647058824f, 0.5882352941176471f, 0.9529411764705882f)},
            {4, new Color(0, 0.73725490196078432f, .83137f)},
            {5, new Color(0.2980392156862745f, 0.6862745098039216f, 0.3137254901960784f)},
            {6, new Color(1, 0.9215686274509804f, 0.2313725490196078f)},
            {7, new Color(1, 0.596078431372549f, 0)},
            {8, new Color(0.4745098039215686f, 0.3333333333333333f, 0.2823529411764706f)},
            {9, new Color(0.6196078431372549f, 0.6196078431372549f, 0.6196078431372549f)},
            {10, new Color(1f, 1f, 1f)},
        };

        public static readonly Dictionary<string, float> ConstructionCost = new Dictionary<string, float>
        {
            {"spawn", 15000},
            {"extension", 3000},
            {"road", 300},
            {"constructedWall", 1},
            {"rampart", 1},
            {"link", 5000},
            {"storage", 30000},
            {"tower", 5000},
            {"observer", 8000},
            {"powerSpawn", 100000},
            {"extractor", 5000},
            {"lab", 50000},
            {"terminal", 100000},
            {"container", 5000},
            {"nuker", 100000},
        };
        
        public static readonly Dictionary<float, float> MineralDensity = new Dictionary<float, float>
        {
            {1, 15000},
            {2, 35000},
            {3, 70000},
            {4, 100000}
        };

        public static class BaseMineral
        {
            public const string Hydrogen = "H";
            public const string Oxygen = "O";
            public const string Utrium = "U";
            public const string Keanium = "K";
            public const string Lemergium = "L";
            public const string Zynthium = "Z";
            public const string Catalyst = "X";
        }
        // TODO: factory update? https://github.com/screeps/storage/blob/b045531aca745f0942293bd32e0bdb5813bc12e2/lib/db.js#L55
        // https://github.com/screeps/common/blob/c5b52a2550444e6e30599c51eff3a601c983267e/lib/constants.js#L384-L387
        // might need to verify that all resources are in fact present.
        //   const depositTypes = [C.RESOURCE_SILICON, C.RESOURCE_METAL, C.RESOURCE_BIOMASS, C.RESOURCE_MIST];
        public static readonly HashSet<string> ResourcesAll = new HashSet<string>()
        {
            "energy", 
            "power", 
            "ops",
            "H",
            "O",
            "U",
            "K",
            "L",
            "Z",
            "X",
            "G",

            "silicon",
            "metal",
            "biomass",
            "mist",

            "OH",
            "ZK",
            "UL",

            "UH",
            "UO",
            "KH",
            "KO",
            "LH",
            "LO",
            "ZH",
            "ZO",
            "GH",
            "GO",

            "UH2O",
            "UHO2",
            "KH2O",
            "KHO2",
            "LH2O",
            "LHO2",
            "ZH2O",
            "ZHO2",
            "GH2O",
            "GHO2",

            "XUH2O",
            "XUHO2",
            "XKH2O",
            "XKHO2",
            "XLH2O",
            "XLHO2",
            "XZH2O",
            "XZHO2",
            "XGH2O",
            "XGHO2",
            
            "utrium_bar",
            "lemergium_bar",
            "zynthium_bar",
            "keanium_bar",
            "ghodium_melt",
            "oxidant",
            "reductant",
            "purifier",
            "battery",

            "composite",
            "crystal",
            "liquid",

            "wire",
            "switch",
            "transistor",
            "microchip",
            "circuit",
            "device",

            "cell",
            "phlegm",
            "tissue",
            "muscle",
            "organoid",
            "organism",

            "alloy",
            "tube",
            "fixtures",
            "frame",
            "hydraulics",
            "machine",

            "condensate",
            "concentrate",
            "extract",
            "spirit",
            "emanation",
            "essence",

        }; 
    }
}