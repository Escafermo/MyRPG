using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("Special Ability / AOE Attack"))]
    public class AOEConfig : SpecialAbilityConfig 
	{
        [Header("AOE Attack")]
        [SerializeField] float radius = 10f;
        [SerializeField] float damageToEachTarget = 100f;

        public override SpecialAbilityBehaviour GetBehaviourComponent(GameObject behaviourToAttachTo)
        {
            return behaviourToAttachTo.AddComponent<AOEBehaviour>();
        }


        public float GetRadius()
        {
            return radius;
        }

        public float GetDamageToEachTarget()
        {
            return damageToEachTarget;
        }

	}
}
