using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("Special Ability / SelfHeal"))]
    public class SelfHealConfig : SpecialAbilityConfig 
	{
        [Header("SelfHeal")]
        [SerializeField] float healAmount = 10f;

        public override SpecialAbilityBehaviour GetBehaviourComponent(GameObject behaviourToAttachTo)
        {
            return behaviourToAttachTo.AddComponent<SelfHealBehaviour>();
        }

        public float GetHealAmount()
        {
            return healAmount;
        }

        
    }
}
