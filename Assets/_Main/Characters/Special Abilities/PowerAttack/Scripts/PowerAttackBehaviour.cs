using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	public class PowerAttackBehaviour : SpecialAbilityBehaviour
	{
        Player player;

        void Start()
        {
            player = GetComponent<Player>();
        }

        public override void Use(AbilityUseParameters useParameters)
        {
            DealDamage(useParameters);
            PlayParticleEffect();
            PlaySound();
        }

        private void DealDamage(AbilityUseParameters useParameters)
        {
            float damageToDeal = useParameters.baseDamage + (config as PowerAttackConfig).GetExtraDamage();

            useParameters.target.TakeDamage(damageToDeal);
        }

    }
}
