using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Characters
{
	public class AOEBehaviour : SpecialAbilityBehaviour
	{

        // (config as AOEConfig) is a CASTER to specify the kind of config we are talking about

        PlayerControl player;

        void Start()
        {
            player = GetComponent<PlayerControl>();
        }

        public override void Use(GameObject target)
        {
            DealRadialDamage();
            PlayParticleEffect();
            PlayAbilitySound();
            PlayAbilityAnimation();
        }

        private void DealRadialDamage()
        {
            //Static sphere cast for targets:
            RaycastHit[] hitArray = Physics.SphereCastAll(transform.position, (config as AOEConfig).GetRadius(), Vector3.up, (config as AOEConfig).GetRadius());
            print("AOE");
            foreach (RaycastHit hit in hitArray)
            {
                var healthSystem = hit.collider.gameObject.GetComponent<HealthSystem>();
                //var enemy = hit.collider.gameObject.GetComponent<Enemy>();
                bool hitPlayer = hit.collider.gameObject.GetComponent<PlayerControl>();

                if (healthSystem != null/* && enemy != null*/ && !hitPlayer)
                {
                    float damageToDeal = (config as AOEConfig).GetDamageToEachTarget();
                    healthSystem.TakeDamage(damageToDeal);
                    //enemy.TakeDamage(damageToDeal);
                }
            }
        }

        
    }
}
