using Screeps3D;
using Screeps3D.RoomObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Screeps3D.Tools.Selection.Subpanels
{
    public class CreepBodyPart : MonoBehaviour
    {
        [SerializeField] private Image _part;
        [SerializeField] private Image _boost;

        

        private void Start()
        {
            
        }

        internal void Load(CreepPart part)
        {
            SetBodyPartType(part);

            // boost
            SetBoost(part.Boost);

            // hitpoint
        }

        private void SetBoost(string boost)
        {
            // shard2 E25N18 has upgraders with boosts, so that can be used for testing
            if (string.IsNullOrEmpty(boost))
            {
                _boost.color = Color.clear;
                return;
            }

            if (boost.Contains("UH") || boost.Contains("UO"))
            {
                _boost.color = Constants.CreepBodyPartBoostColors.BOOST_TYPE_UH_UO;
            }
            else if (boost.Contains("KH") || boost.Contains("KO"))
            {
                _boost.color = Constants.CreepBodyPartBoostColors.BOOST_TYPE_KH_KO;
            }
            else if (boost.Contains("LH") || boost.Contains("LO"))
            {
                _boost.color = Constants.CreepBodyPartBoostColors.BOOST_TYPE_LH_LO;
            }
            else if (boost.Contains("ZH") || boost.Contains("ZO"))
            {
                _boost.color = Constants.CreepBodyPartBoostColors.BOOST_TYPE_ZH_ZO;
            }
            else if (boost.Contains("GH") || boost.Contains("GO"))
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
    }
}
