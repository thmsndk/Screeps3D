using Common;
using Screeps_API;
using UnityEngine;
using UnityEngine.UI;

namespace Screeps3D.Player
{
    public class PerformanceView : MonoBehaviour
    {
        [SerializeField] private ScaleAxes _cpuMeter = default;
        [SerializeField] private ScaleAxes _memMeter = default;
        
        private Color _baseColor = new Color(0, 0.65f, .5f);
        private Image _cpuImage;
        private Image _memImage;

        private void Start()
        {
            _cpuImage = _cpuMeter.GetComponent<Image>();
            _memImage = _memMeter.GetComponent<Image>();
        }
        
        private void Update()
        {
            var cpuPercent = 0f;
            var cpuColor = _baseColor;
            var memPercent = 0f;
            var memColor = _baseColor;
            if (ScreepsAPI.IsConnected && ScreepsAPI.Me != null)
            {
                cpuPercent = (float) ScreepsAPI.Monitor.CPU / ScreepsAPI.Me.Cpu;
                cpuColor = ModifyColor(cpuPercent);
                memPercent = (float) ScreepsAPI.Monitor.Memory / 2048000;
                memColor = ModifyColor(memPercent);
            }
            
            _cpuMeter.SetVisibility(cpuPercent);
            _cpuImage.color = Color.Lerp(_cpuImage.color, cpuColor, Time.deltaTime * 5);
            _memMeter.SetVisibility(memPercent);
            _memImage.color = Color.Lerp(_memImage.color, memColor, Time.deltaTime * 5);
        }

        private Color ModifyColor(float percent)
        {
            // var over = percent > 1;
            // return new Color(over ? 1 : .3f + percent * .6f, over ? 0.3f : 0.65f, .55f);
            return new Color(.3f + percent * .7f, 0.6f + percent * .22f, .4f + percent * .1f);
        }
    }
}