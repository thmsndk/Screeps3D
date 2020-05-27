using Assets.Scripts.Screeps3D;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class FlagView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private MeshRenderer rend = default;
        private Flag _flag;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _flag = roomObject as Flag;
        }

        public void Delta(JSONObject data)
        {
        }

        public void Unload(RoomObject roomObject)
        {
            _flag = null;
        }

        private void Update()
        {
            if (_flag == null)
                return;

            var primary = Constants.FlagColors[_flag.PrimaryColor];
            var secondary = Constants.FlagColors[_flag.SecondaryColor];

            var currentPrimaryColor = rend.material.GetColor(ShaderKeys.FlagShader.PrimaryColor);
            var currentSecondaryColor = rend.material.GetColor(ShaderKeys.FlagShader.SecondaryColor);

            // TODO: compare if we need to update the color
            // https://docs.unity3d.com/ScriptReference/Mathf.Approximately.html

            rend.material.SetColor(ShaderKeys.FlagShader.PrimaryColor, Color.Lerp(currentPrimaryColor, primary, Time.deltaTime));
            rend.material.SetColor(ShaderKeys.FlagShader.SecondaryColor, Color.Lerp(currentSecondaryColor, secondary, Time.deltaTime));
        }
    }
}