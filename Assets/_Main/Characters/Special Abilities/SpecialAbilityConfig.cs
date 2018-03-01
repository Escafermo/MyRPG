using System.Collections;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    //public struct AbilityUseParameters
    //{
    //    public IDamageable target;
    //    public float baseDamage;
        
    //    public AbilityUseParameters(IDamageable thisTarget , float thisBaseDamage)
    //    {
    //        this.target = thisTarget;
    //        this.baseDamage = thisBaseDamage;
    //    }
    //}

    public abstract class SpecialAbilityConfig : ScriptableObject
    {
        [Header("Special Ability General")]
        [SerializeField] float energyCost = 50f;
        [SerializeField] GameObject particleSystem = null;
        [SerializeField] AudioClip[] audioClips = null;

        protected SpecialAbilityBehaviour behaviour;

        abstract public SpecialAbilityBehaviour GetBehaviourComponent(GameObject gameObjectToAttach);

        public void AttachAbilityTo(GameObject gameObjectToAttach)
        {
            SpecialAbilityBehaviour behaviourComponent = GetBehaviourComponent(gameObjectToAttach);
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent;
        }

        public void Use(GameObject target)
        {
            behaviour.Use(target);
        }

        public float GetEnergyCost()
        {
            return energyCost;
        }
        
        public GameObject GetParticleSystemPrefab()
        {
            return particleSystem;
        }

        public AudioClip GetRandomAudioClip()
        {
            return audioClips[Random.Range(0,audioClips.Length)];
        }

	}


}
