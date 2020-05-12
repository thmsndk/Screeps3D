using Screeps3D.Menus.Options;
using UnityEngine;

namespace Screeps3D.Menus.Main
{
    public class OpenOptions : MainMenuItem
    {
        [SerializeField] private OptionsMenu _options = default;
        
        public override string Description
        {
            get { return "Options"; }
        }

        public override void Invoke()
        {
            _options.Toggle();
        }
    }
}