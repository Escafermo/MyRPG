using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public struct AbilityUseParameters
    {
        public IDamageable target;
        public float baseDamage;
        
        public AbilityUseParameters(IDamageable thisTarget , float thisBaseDamage)
        {
            this.target = thisTarget;
            this.baseDamage = thisBaseDamage;
        }
    }

    public abstract class SpecialAbilityConfig : ScriptableObject
    {
        [Header("Special Ability General")]
        [SerializeField] float energyCost = 50f;
        [SerializeField] GameObject particleSystem = null;
        [SerializeField] AudioClip audioClip = null;

        protected ISpecialAbility behaviour;

        abstract public void AttachComponent(GameObject gameObjectToAttach);

        public void Use(AbilityUseParameters useParameters)
        {
            behaviour.Use(useParameters);
        }


        public float GetEnergyCost()
        {
            return energyCost;
        }
        
        public GameObject GetParticleSystemPrefab()
        {
            return particleSystem;
        }

        public AudioClip GetAudioClip()
        {
            return audioClip;
        }

	}

    public interface ISpecialAbility
    {
        void Use(AbilityUseParameters useParameters);
    }


}
