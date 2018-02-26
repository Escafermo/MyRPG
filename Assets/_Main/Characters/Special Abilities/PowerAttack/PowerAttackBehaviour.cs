using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	public class PowerAttackBehaviour : MonoBehaviour , ISpecialAbility
	{
        PowerAttackConfig config;
        Player player;
        AudioSource audioSource = null;

        void Start()
        {
            player = GetComponent<Player>();
            audioSource = player.GetComponent<AudioSource>();
        }

        public void SetConfig(PowerAttackConfig configToSet)
        {
            this.config = configToSet;
        }


        public void Use(AbilityUseParameters useParameters)
        {
            DealDamage(useParameters);
            PlayParticleSystem();
        }

        private void PlayParticleSystem()
        {
            var thisGameObject = Instantiate(config.GetParticleSystemPrefab(), transform.position, Quaternion.identity);
            ParticleSystem myParticleSystem = thisGameObject.GetComponent<ParticleSystem>();
            myParticleSystem.Play();
            Destroy(thisGameObject, myParticleSystem.main.duration);
        }

        private void DealDamage(AbilityUseParameters useParameters)
        {
            float damageToDeal = useParameters.baseDamage + config.GetExtraDamage();

            useParameters.target.TakeDamage(damageToDeal);

            PlaySound();
        }

        private void PlaySound()
        {
            audioSource.clip = config.GetAudioClip();
            audioSource.Play();
        }
    }
}
