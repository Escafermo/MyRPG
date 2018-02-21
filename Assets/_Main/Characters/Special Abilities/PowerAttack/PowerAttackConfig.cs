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

        public override void AttachComponent(GameObject gameObjectToAttach)
        {
            var behaviourComponent = gameObjectToAttach.AddComponent<PowerAttackBehaviour>();
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent;
        }

        public float GetExtraDamage()
        {
            return extraDamage;
        }

	}
}
