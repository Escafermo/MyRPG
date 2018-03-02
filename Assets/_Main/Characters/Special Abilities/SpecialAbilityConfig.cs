using System.Collections;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public abstract class SpecialAbilityConfig : ScriptableObject
    {
        [Header("Special Ability General")]
        [SerializeField] float energyCost = 50f;
        [SerializeField] GameObject particleSystem;
        [SerializeField] AudioClip[] audioClips;
        [SerializeField] AnimationClip[] animationClips;

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

        public AnimationClip GetRandomAnimationClip()
        {
            return animationClips[Random.Range(0, animationClips.Length)];
        }

    }


}
