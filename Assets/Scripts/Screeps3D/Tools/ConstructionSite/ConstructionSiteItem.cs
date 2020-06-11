using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Screeps3D.Tools.ConstructionSite
{
    public class ConstructionSiteItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text name = default;
        [SerializeField] private TMP_Text available = default;
        [SerializeField] private TMP_Text description = default;

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
            var color = used < max ? "green" : "#BEBEBE";
            this.available.text = $"<color={color}>Available: {available}</color>";
        }
    }
}
