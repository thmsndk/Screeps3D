using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Screeps3D.Tools.ConstructionSite
{
    /// <summary>
    /// Responsible for showing available construction sites for placement
    /// </summary>
    class ChooseConstructionSite
    {
        // TODO: list all buildable roomobject types, do we use reflection on all roomobjects? no reason, we can just iterate the "constants" we need to define amount of structures anyway.
        // TODO: calculate how many structures of the type is currently in the room, how many are yours, and how many are someone elses?
        // TODO: how do we mark the amount we can have based on RCL, do we just define a constant lookup table? can perhaps use LastOrDefault based on current RCL https://docs.screeps.com/api/#Constants
        private const int AVAILABLE = 2500; // 2500 seems to be an indicator of not showing available amount.

        public readonly Dictionary<string, List<int>> CONTROLLER_STRUCTURES = new Dictionary<string, List<int>>
            {
                
                { "spawn",              new List<int>{0, 1, 1, 1, 1, 1, 1, 2, 3 } },
                { "extension",          new List<int>{0, 0, 5, 10, 20, 30, 40, 50, 60 } },
                { "link",               new List<int>{0, 0, 0, 0, 0, 2, 3, 4, 6 } },
                { "road",               new List<int>{AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE } },
                { "constructedWall",    new List<int>{0, 0, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE } },
                { "rampart",            new List<int>{0, 0, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE } },
                { "storage",            new List<int>{0, 0, 0, 0, 1, 1, 1, 1, 1 } },
                { "tower",              new List<int>{0, 0, 0, 1, 1, 2, 2, 3, 6 } },
                { "observer",           new List<int>{0, 0, 0, 0, 0, 0, 0, 0, 1 } },
                { "powerSpawn",         new List<int>{0, 0, 0, 0, 0, 0, 0, 0, 1 } },
                { "extractor",          new List<int>{0, 0, 0, 0, 0, 0, 1, 1, 1 } },
                { "terminal",           new List<int>{0, 0, 0, 0, 0, 0, 1, 1, 1 } },
                { "lab",                new List<int>{0, 0, 0, 0, 0, 0, 3, 6, 10 } },
                { "container",          new List<int>{5, 5, 5, 5, 5, 5, 5, 5, 5 } },
                { "nuker",              new List<int>{0, 0, 0, 0, 0, 0, 0, 0, 1 } },
                { "factory",            new List<int>{0, 0, 0, 0, 0, 0, 0, 1, 1 } },
            };

        public ChooseConstructionSite()
        {

        }
    }


}
