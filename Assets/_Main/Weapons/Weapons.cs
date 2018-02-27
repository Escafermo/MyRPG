using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Weapon
{
    [CreateAssetMenu(menuName = ("RPG_Weapon"))]
    public class Weapons : ScriptableObject
    {
        public Transform gripTransform;

        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip attackAnimation;
        [SerializeField] float attackRate = 0.5f;
        [SerializeField] float attackRange = 2f;
        [SerializeField] float additionalDamage = 10f;

        public float GetAttackRate()
        {
            // TODO consider whether we take animation time into account
            return attackRate;
        }

        public float GetAttackRange()
        {
            return attackRange;
        }

        public GameObject GetWeaponPrefab()
        {
            return weaponPrefab;
        }

        public AnimationClip GetAttackAnimationClip()
        {
            RemoveAnimationEvents();
            return attackAnimation;
        }

        // So that asset packs cannot cause crashes/bugs
        private void RemoveAnimationEvents()
        {
            attackAnimation.events = new AnimationEvent[0];
        }

        public float GetAdditionalDamage()
        {
            return additionalDamage;
        }

        
    }
}
