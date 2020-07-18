using Assets.Scripts.Screeps3D.Tools.ConstructionSite;
using UnityEngine;

namespace Screeps3D.Tools.Selection
{
    public class PlaceFirstSpawnTool : MonoBehaviour
    {
        private void Start()
        {
            ToolChooser.Instance.OnToolChange += OnToolChange;
        }
        
        private void OnToolChange(ToolType toolType)
        {
            var activated = toolType == ToolType.Spawn;
            
            PlaceFirstSpawn.Instance.enabled = activated;
        }
    }
}