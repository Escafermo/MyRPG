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

        public override void AttachComponent(GameObject gameObjectToAttach)
        {
            var behaviourComponent = gameObjectToAttach.AddComponent<AOEBehaviour>();
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent;
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
