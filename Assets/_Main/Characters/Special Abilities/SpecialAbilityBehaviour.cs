﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public abstract class SpecialAbilityBehaviour : MonoBehaviour
    {
        const float PARTICLE_DELAY = 6f;

        protected SpecialAbilityConfig config; // Only Children can see it and set it

        public abstract void Use(AbilityUseParameters useParameters);

        public void SetConfig(SpecialAbilityConfig configToSet)
        {
            config = configToSet;
        }

        protected void PlayParticleEffect()
        {
            var particleSystemObject = config.GetParticleSystemPrefab();
            Vector3 particlesPos = new Vector3(0f, 1f, 0f); // Position relative to player
            var thisParticleSystemObject = Instantiate(
                particleSystemObject, 
                transform.position + particlesPos, 
                particleSystemObject.transform.rotation
                );
            thisParticleSystemObject.transform.parent = transform;
            ParticleSystem myParticleSystem = thisParticleSystemObject.GetComponent<ParticleSystem>();

            myParticleSystem.Play();
            StartCoroutine(DestroyParticleWhenFinished(thisParticleSystemObject));
        }

        IEnumerator DestroyParticleWhenFinished (GameObject particlePrefab)
        {
            while (particlePrefab.GetComponent<ParticleSystem>().isPlaying)
            {
                yield return new WaitForSeconds(PARTICLE_DELAY);
            }
            Destroy(particlePrefab);
            yield return new WaitForEndOfFrame();
        }

        protected void PlaySound()
        {
            AudioSource audioSource = GetComponentInParent<AudioSource>();
            AudioClip audioClip = config.GetRandomAudioClip(); // TODO change to random clip
            audioSource.PlayOneShot(audioClip);
        }


    }
}
