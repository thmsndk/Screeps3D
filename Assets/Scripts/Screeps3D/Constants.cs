using System.Collections.Generic;
using UnityEngine;

namespace Screeps3D
{
    public static class Constants
    {
        public const string InvaderUserId = "2";
        public const string SourceKeeperUserId = "3";

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
        public const string TypeInvaderCore = "invaderCore";
        public const string TypeNuke = "nuke";




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

        // TODO: this kinda belongsin Screeps_API theese flag colors are API specific.
        public enum FlagColor
        {
            Red = 1,
            Purple = 2,
            Blue = 3,
            Cyan = 4,
            Green = 5,
            Yellow = 6,
            Orange = 7,
            Brown = 8,
            Grey = 9,
            White = 10
        }

        public static readonly Dictionary<int, Color> FlagColors = new Dictionary<int, Color>
        {
            {(int)FlagColor.Red, new Color(.95f, .262f, .218f)},
            {(int)FlagColor.Purple, new Color(0.6117f, 0.1529411764705882f, 0.6901960784313725f)},
            {(int)FlagColor.Blue, new Color(0.1294117647058824f, 0.5882352941176471f, 0.9529411764705882f)},
            {(int)FlagColor.Cyan, new Color(0, 0.73725490196078432f, .83137f)},
            {(int)FlagColor.Green, new Color(0.2980392156862745f, 0.6862745098039216f, 0.3137254901960784f)},
            {(int)FlagColor.Yellow, new Color(1, 0.9215686274509804f, 0.2313725490196078f)},
            {(int)FlagColor.Orange, new Color(1, 0.596078431372549f, 0)},
            {(int)FlagColor.Brown, new Color(0.4745098039215686f, 0.3333333333333333f, 0.2823529411764706f)},
            {(int)FlagColor.Grey, new Color(0.6196078431372549f, 0.6196078431372549f, 0.6196078431372549f)},
            {(int)FlagColor.White, new Color(1f, 1f, 1f)},
        };

        public static class CreepBodyPartColors
        {
            public static readonly Color Move;
            public static readonly Color Work;
            public static readonly Color Attack;
            public static readonly Color RangedAttack;
            public static readonly Color Heal;
            public static readonly Color Tough;
            public static readonly Color Claim;
            public static readonly Color Carry;

            static CreepBodyPartColors()
            {
                ColorUtility.TryParseHtmlString("#A9B7C6", out Move);
                ColorUtility.TryParseHtmlString("#FFE56D", out Work);
                ColorUtility.TryParseHtmlString("#F93842", out Attack);
                ColorUtility.TryParseHtmlString("#5D80B2", out RangedAttack);
                ColorUtility.TryParseHtmlString("#65FD62", out Heal);
                ColorUtility.TryParseHtmlString("#FFFFFF", out Tough);
                ColorUtility.TryParseHtmlString("#B99CFB", out Claim);
                ColorUtility.TryParseHtmlString("#777777", out Carry);
            }
        }

        public static class CreepBodyPartBoostColors
        {
            public static readonly Color BOOST_TYPE_UH_UO;
            public static readonly Color BOOST_TYPE_KH_KO;
            public static readonly Color BOOST_TYPE_LH_LO;
            public static readonly Color BOOST_TYPE_ZH_ZO;
            public static readonly Color BOOST_TYPE_GH_GO;

            static CreepBodyPartBoostColors()
            {
                ColorUtility.TryParseHtmlString("#50D7F9", out BOOST_TYPE_UH_UO);
                ColorUtility.TryParseHtmlString("#A071FF", out BOOST_TYPE_KH_KO);
                ColorUtility.TryParseHtmlString("#00F4A2", out BOOST_TYPE_LH_LO);
                ColorUtility.TryParseHtmlString("#FDD388", out BOOST_TYPE_ZH_ZO);
                ColorUtility.TryParseHtmlString("#FFFFFF", out BOOST_TYPE_GH_GO);
            }
        }

        public static readonly Dictionary<float, float> MineralDensity = new Dictionary<float, float>
        {
            {1, 15000},
            {2, 35000},
            {3, 70000},
            {4, 100000}
        };

        public static readonly Dictionary<string, Color> ResourceColors = new Dictionary<string, Color> {
            {"other", new Color32(204, 204, 204, 255)},
            {"energy", new Color32(118, 93, 0, 255)},
            {"power", new Color32(255, 0, 0, 255)},
            // MINERALS
            {Constants.BaseMineral.Hydrogen, new   Color32(205,205,205,255)},
            {Constants.BaseMineral.Oxygen, new   Color32(205,205,205,255)},
            {Constants.BaseMineral.Utrium, new   Color32(80,215,249,255)},
            {Constants.BaseMineral.Keanium, new   Color32(160,113,255,255)},
            {Constants.BaseMineral.Lemergium, new   Color32(0,244,162,255)},
            {Constants.BaseMineral.Zynthium, new   Color32(253,211,136,255)},
            {Constants.BaseMineral.Catalyst, new   Color32(255,119,119,255)},
            // DEPOSITS
            {Constants.BaseDeposit.Biomass, new   Color32(38,110,0,255)},
            {Constants.BaseDeposit.Metal, new   Color32(128,58,0,255)},
            {Constants.BaseDeposit.Mist, new   Color32(97,0,128,255)},
            {Constants.BaseDeposit.Silicon, new   Color32(0,102,128,255)}
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

        public static class BaseDeposit
        {
            public const string Silicon = "silicon";
            public const string Metal = "metal";
            public const string Biomass = "biomass";
            public const string Mist = "mist";
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

        public static int NUKE_ROOM_RANGE = 10;
        public static int NUKE_TRAVEL_TICKS = 50000;
        public static int CONTROLLER_RESERVE_MAX = 5000;

        public static float DEPOSIT_EXHAUST_MULTIPLY = 0.001f;
        public static float DEPOSIT_EXHAUST_POW = 1.2f;
        public static int DEPOSIT_DECAY_TIME = 50000;
    }
}