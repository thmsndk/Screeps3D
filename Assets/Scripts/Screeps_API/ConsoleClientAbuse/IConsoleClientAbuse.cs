using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Screeps_API;

namespace Assets.Scripts.Screeps_API.ConsoleClientAbuse
{
    interface IConsoleClientAbuse
    {
        void Abuse(ScreepsConsole.ConsoleMessage message);
    }
}
