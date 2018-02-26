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

        public override void AttachComponent(GameObject gameObjectToAttach)
        {
        var behaviourComponent = gameObjectToAttach.AddComponent<SelfHealBehaviour>();
        behaviourComponent.SetConfig(this);
        behaviour = behaviourComponent;
        }

        public float GetHealAmount()
        {
            return healAmount;
        }

        
    }
}
