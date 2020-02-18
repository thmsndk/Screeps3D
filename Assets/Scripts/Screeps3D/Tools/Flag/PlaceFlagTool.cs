using UnityEngine;

namespace Screeps3D.Tools.Selection
{
    public class PlaceFlagTool : MonoBehaviour
    {
        private void Start()
        {
            ToolChooser.Instance.OnToolChange += OnToolChange;
        }
        
        private void OnToolChange(ToolType toolType)
        {
            var activated = toolType == ToolType.Flag;
            PlaceFlag.Instance.enabled = activated;
        }
    }
}