using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Characters
{
	public class AOEBehaviour : MonoBehaviour , ISpecialAbility
	{
        AOEConfig config;
        Player player;
        AudioSource audioSource = null;

        void Start()
        {
            player = GetComponent<Player>();
            audioSource = player.GetComponent<AudioSource>();
        }


        public void SetConfig(AOEConfig configToSet)
        {
            this.config = configToSet;
        }

        public void Use(AbilityUseParameters useParameters)
        {
            DealRadialDamage(useParameters);
            PlayParticleEffect();
            PlaySound();
        }

        private void PlayParticleEffect()
        {
            var thisGameObject = Instantiate(config.GetParticleSystemPrefab(), transform.position,Quaternion.identity);
            ParticleSystem myParticleSystem = thisGameObject.GetComponent<ParticleSystem>();
            myParticleSystem.Play();
            Destroy(thisGameObject,myParticleSystem.main.duration);
        }



        private void DealRadialDamage(AbilityUseParameters useParameters)
        {
            //Static sphere cast for targets:
            RaycastHit[] hitArray = Physics.SphereCastAll(transform.position, config.GetRadius(), Vector3.up, config.GetRadius());
            print("AOE");
            foreach (RaycastHit hit in hitArray)
            {
                var damageable = hit.collider.gameObject.GetComponent<IDamageable>();
                var enemy = hit.collider.gameObject.GetComponent<Enemy>();
                //bool hitPlayer = hit.collider.gameObject.GetComponent<Player>();

                if (damageable != null && enemy != null /*&& !hitPlayer*/)
                {
                    float damageToDeal = useParameters.baseDamage + config.GetDamageToEachTarget();
                    damageable.TakeDamage(damageToDeal);
                }
            }
        }

        private void PlaySound()
        {
            audioSource.clip = config.GetAudioClip();
            audioSource.Play();
        }
    }
}
