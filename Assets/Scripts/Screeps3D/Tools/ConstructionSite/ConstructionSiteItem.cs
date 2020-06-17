using Screeps3D.RoomObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Screeps3D.Tools.ConstructionSite
{
    public class ConstructionSiteItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text name = default;
        [SerializeField] private TMP_Text available = default;
        [SerializeField] private TMP_Text description = default;

        private Toggle toggle;

        private void Awake()
        {
            this.toggle = GetComponent<Toggle>();
        }

        public void SetName(string value)
        {
            this.name.text = value;
        }

        public void SetDescription(string value)
        {
            this.description.text = value;
        }

        public void SetAvailable(int used, int max, bool unlimited = false, Controller controller = null, int nextRclUpgrade = 0)
        {
            var available = max - used;
            var canConstruct = available > 0;
            var color = canConstruct ? "green" : "#BEBEBE";
            var text = unlimited ? "Available" : $"Available: {available} / {max}";

            if (controller == null || controller.Level == 0 && max == 0)
            {
                color = "#BEBEBE";
                text = "No controller";
            }
            else if (max == 0)
            {
                text = $"RCL {nextRclUpgrade} required";
            }

            this.available.text = $"<color={color}>{text}</color>";

            if (toggle != null)
            {
                toggle.interactable = canConstruct;
            }
        }
    }
}
