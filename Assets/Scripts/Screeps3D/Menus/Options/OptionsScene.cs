using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Screeps3D.Menus.Options
{
    public class OptionsScene : MonoBehaviour
    {
        public GameObject OptionsCanvas;
        private void Awake()
        {
            OptionsCanvas.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OptionsCanvas.SetActive(!OptionsCanvas.activeSelf);
            }
        }
    }
}
