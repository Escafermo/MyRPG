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

        Player player;

        void Start()
        {
            player = GetComponent<Player>();
        }

        public override void Use(AbilityUseParameters useParameters)
        {
            DealRadialDamage(useParameters);
            PlayParticleEffect();
            PlaySound();
        }

        private void DealRadialDamage(AbilityUseParameters useParameters)
        {
            //Static sphere cast for targets:
            RaycastHit[] hitArray = Physics.SphereCastAll(transform.position, (config as AOEConfig).GetRadius(), Vector3.up, (config as AOEConfig).GetRadius());
            print("AOE");
            foreach (RaycastHit hit in hitArray)
            {
                var damageable = hit.collider.gameObject.GetComponent<IDamageable>();
                var enemy = hit.collider.gameObject.GetComponent<Enemy>();
                //bool hitPlayer = hit.collider.gameObject.GetComponent<Player>();

                if (damageable != null && enemy != null /*&& !hitPlayer*/)
                {
                    float damageToDeal = useParameters.baseDamage + (config as AOEConfig).GetDamageToEachTarget();
                    damageable.TakeDamage(damageToDeal);
                }
            }
        }

        
    }
}
