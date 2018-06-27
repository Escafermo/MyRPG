using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG_Weapon"))]
    public class WeaponConfig : ScriptableObject
    {
        public Transform gripTransform;

        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip[] attackAnimations;
        [SerializeField] float timeBetweenAnimationCycles = 0.5f;
        [SerializeField] float attackRange = 2f;
        [SerializeField] float additionalDamage = 10f;
        [SerializeField] float damageDelay = .5f;

        public float GetTimeBetweenAnimationCycles()
        {
            return timeBetweenAnimationCycles;
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
            var attackAnimation = attackAnimations[UnityEngine.Random.Range(0, attackAnimations.Length)];
            attackAnimation.events = new AnimationEvent[0]; // So that asset packs cannot cause crashes/bugs, we remove animation events
            return attackAnimation;
        }

        public float GetAdditionalDamage()
        {
            return additionalDamage;
        }

        public float GetWeaponDelay()
        {
            return damageDelay;
        }

        
    }
}
