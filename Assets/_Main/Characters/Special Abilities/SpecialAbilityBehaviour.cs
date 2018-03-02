using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public abstract class SpecialAbilityBehaviour : MonoBehaviour
    {
        const float PARTICLE_DELAY = 6f;
        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";

        protected SpecialAbilityConfig config; // Only Children can see it and set it

        public abstract void Use(GameObject target = null); // null to be configured by each override method

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

        protected void PlayAbilitySound()
        {
            AudioSource audioSource = GetComponentInParent<AudioSource>();
            AudioClip audioClip = config.GetRandomAudioClip(); 
            audioSource.PlayOneShot(audioClip);
        }

        protected void PlayAbilityAnimation()
        {
            AnimatorOverrideController animatorOverrideController = GetComponent<Character>().GetAnimatorOverrideController();
            Animator animator = GetComponentInParent<Animator>();
            AnimationClip animationClip = config.GetRandomAnimationClip();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = animationClip;
            animator.SetTrigger(ATTACK_TRIGGER);
            if (!animatorOverrideController)
            {
                Debug.Break();
                Debug.LogAssertion("Provide " + gameObject + " with an animator override controller");
            }
        }
    }
}
