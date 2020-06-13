using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Screeps3D
{
    public class ShaderKeys
    {
        public class HDRPLit
        {
            public const string Color = "_BaseColor";
            public const string Texture = "_BaseColorMap";
        }

        public class HDRPDecal
        {
            public const string Color = "_BaseColor";
            public const string Texture = "_BaseColorMap";
        }

        public class FlagShader
        {
            public const string PrimaryColor = "PrimaryColor";
            public const string SecondaryColor = "SecondaryColor";

        }

    }
}
