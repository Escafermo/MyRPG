using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("Special Ability / Power Attack"))]
    public class PowerAttackConfig : SpecialAbilityConfig
	{

        [Header("Power Attack")]
        [SerializeField] float extraDamage = 10f;

        public override SpecialAbilityBehaviour GetBehaviourComponent(GameObject behaviourToAttachTo)
        {
            return behaviourToAttachTo.AddComponent<PowerAttackBehaviour>();
        }

        public float GetExtraDamage()
        {
            return extraDamage;
        }

	}
}
