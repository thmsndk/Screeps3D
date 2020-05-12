using Screeps3D;
using Screeps3D.RoomObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Screeps3D.Tools.Selection.Subpanels
{
    public class CreepBodyPart : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _part = default;
        [SerializeField] private Image _boost = default;

        private CreepPart creepPart;
        

        private void Start()
        {
            
        }

        internal void Load(CreepPart part)
        {
            creepPart = part;

            SetBodyPartType(part);

            // boost
            SetBoost(part.Boost);

            // hitpoint
            ScaleHitpoints(part);
        }
        internal void Delta(CreepPart part)
        {
            ScaleHitpoints(part);

            SetBoost(part.Boost);
        }

        private void ScaleHitpoints(CreepPart part)
        {
            _part.fillAmount = part.Hits / 100f;
        }

        private void SetBoost(string boost)
        {
            // shard2 E25N18 has upgraders with boosts, so that can be used for testing
            if (string.IsNullOrEmpty(boost))
            {
                _boost.color = Color.clear;
                return;
            }

            if (boost.Contains("U"))
            {
                _boost.color = Constants.CreepBodyPartBoostColors.BOOST_TYPE_UH_UO;
            }
            else if (boost.Contains("K"))
            {
                _boost.color = Constants.CreepBodyPartBoostColors.BOOST_TYPE_KH_KO;
            }
            else if (boost.Contains("L"))
            {
                _boost.color = Constants.CreepBodyPartBoostColors.BOOST_TYPE_LH_LO;
            }
            else if (boost.Contains("Z"))
            {
                _boost.color = Constants.CreepBodyPartBoostColors.BOOST_TYPE_ZH_ZO;
            }
            else if (boost.Contains("G"))
            {
                _boost.color = Constants.CreepBodyPartBoostColors.BOOST_TYPE_GH_GO;
            }
        }

        private void SetBodyPartType(CreepPart part)
        {
            var color = Color.clear;

            switch (part.Type)
            {
                case "move":
                    color = Constants.CreepBodyPartColors.Move;
                    break;
                case "work":
                    color = Constants.CreepBodyPartColors.Work;
                    break;
                case "attack":
                    color = Constants.CreepBodyPartColors.Attack;
                    break;
                case "ranged_attack":
                    color = Constants.CreepBodyPartColors.RangedAttack;
                    break;
                case "heal":
                    color = Constants.CreepBodyPartColors.Heal;
                    break;
                case "tough":
                    color = Constants.CreepBodyPartColors.Tough;
                    break;
                case "claim":
                    color = Constants.CreepBodyPartColors.Claim;
                    break;
                case "carry":
                    color = Constants.CreepBodyPartColors.Carry;
                    break;
            }

            this._part.color = color;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // TODO: raise tooltip event, with tooltip data. should it update while hovering?
            Debug.Log($"Hovering {creepPart.Type} {creepPart.Hits}/100");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log($"Leaving {creepPart.Type} {creepPart.Hits}/100");
        }
    }
}
