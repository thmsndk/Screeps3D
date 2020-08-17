using Screeps_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Screeps3D
{
    public class BadgeAndLabel : MonoBehaviour
    {
        public TextMeshProUGUI Name;
        public Image Badge;
        public Image BadgeMask;

        public void SetOwner(ScreepsUser user)
        {
            if (user != null)
            {
                Name.enabled = true;
                Badge.enabled = true;
                Name.text = string.Format("{0}", user.Username);
                if (user.Badge != null)
                {
                    Badge.sprite = Sprite.Create(user.Badge,
                        new Rect(0.0f, 0.0f, BadgeManager.BADGE_SIZE, BadgeManager.BADGE_SIZE), new Vector2(.5f, .5f));
                }
                else
                {
                    Badge.enabled = false;
                }
            }
            else
            {
                Name.enabled = false;
                Badge.enabled = false;
            }
        }
    }
}
