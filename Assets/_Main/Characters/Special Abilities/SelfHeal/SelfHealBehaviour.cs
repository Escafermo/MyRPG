using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Characters
{
	public class SelfHealBehaviour : MonoBehaviour , ISpecialAbility
	{
        SelfHealConfig config = null;
        Player player = null;
        AudioSource audioSource = null;

        void Start()
        {
            player = GetComponent<Player>();
            audioSource = player.GetComponent<AudioSource>();
        }

        public void SetConfig(SelfHealConfig configToSet)
        {
            this.config = configToSet;
        }

        public void Use(AbilityUseParameters useParameters)
        {
            Heal(useParameters);
            PlayParticleEffect();
        }

        private void PlayParticleEffect()
        {
            var thisGameObject = Instantiate(config.GetParticleSystemPrefab(), transform.position, Quaternion.identity);
            thisGameObject.transform.parent = transform;
            ParticleSystem myParticleSystem = thisGameObject.GetComponent<ParticleSystem>();
            myParticleSystem.Play();
            Destroy(thisGameObject, myParticleSystem.main.duration);
        }

        private void Heal(AbilityUseParameters useParameters)
        {
            player.Heal(config.GetHealAmount());
            PlaySound();
        }

        private void PlaySound()
        {
            audioSource.clip = config.GetAudioClip();
            audioSource.Play();
        }
    }
}
