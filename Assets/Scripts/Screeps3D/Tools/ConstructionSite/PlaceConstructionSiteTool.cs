using Assets.Scripts.Screeps3D.Tools.ConstructionSite;
using UnityEngine;

namespace Screeps3D.Tools.Selection
{
    public class PlaceConstructionSiteTool : MonoBehaviour
    {
        private void Start()
        {
            ToolChooser.Instance.OnToolChange += OnToolChange;
        }
        
        private void OnToolChange(ToolType toolType)
        {
            var activated = toolType == ToolType.Construction;
            
            ChooseConstructionSite.Instance.enabled = activated;
            PlaceConstructionSite.Instance.enabled = activated;
        }
    }
}