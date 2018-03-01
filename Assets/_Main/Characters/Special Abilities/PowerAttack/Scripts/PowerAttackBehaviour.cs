using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	public class PowerAttackBehaviour : SpecialAbilityBehaviour
	{
        PlayerControl player;

        void Start()
        {
            player = GetComponent<PlayerControl>();
        }

        public override void Use(GameObject target)
        {
            DealDamage(target);
            PlayParticleEffect();
            PlaySound();
        }

        private void DealDamage(GameObject target)
        {
            var healthSystem = target.GetComponent<HealthSystem>();

            float damageToDeal = (config as PowerAttackConfig).GetExtraDamage();

            healthSystem.TakeDamage(damageToDeal);
        }

    }
}
