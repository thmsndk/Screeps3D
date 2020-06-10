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
        [SerializeField] private TMP_Text typeLabel = default;

        public void SetType(string type)
        {
            typeLabel.text = type;
        }
    }
}
