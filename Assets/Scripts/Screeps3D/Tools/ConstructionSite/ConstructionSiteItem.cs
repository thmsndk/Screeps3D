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

        public void SetAvailable(int used, int max, bool unlimited = false)
        {
            var available = max - used;
            var canConstruct = available > 0;
            var color = canConstruct ? "green" : "#BEBEBE";
            var text = unlimited ? "Available" : $"Available: {available} / {max}";
            this.available.text = $"<color={color}>{text}</color>";
            // TODO: sneak peak next rcl?

            if (toggle != null)
            {
                toggle.interactable = canConstruct;
            }
        }
    }
}
