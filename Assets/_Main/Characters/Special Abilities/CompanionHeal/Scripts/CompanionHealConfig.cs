using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("Special Ability / CompanionHeal"))]
    public class CompanionHealConfig : SpecialAbilityConfig
    {
            [Header("Heal")]
            [SerializeField] float healAmount = 10f;

            public override SpecialAbilityBehaviour GetBehaviourComponent(GameObject behaviourToAttachTo)
            {
                return behaviourToAttachTo.AddComponent<CompanionHealBehaviour>();
            }

            public float GetHealAmount()
            {
                return healAmount;
            }
    }
}
