using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Characters
{
	public class SelfHealBehaviour : SpecialAbilityBehaviour
	{
        Player player = null;

        void Start()
        {
            player = GetComponent<Player>();
        }

        public override void Use(AbilityUseParameters useParameters)
        {
            Heal(useParameters);
            PlayParticleEffect();
            PlaySound();
        }

        private void Heal(AbilityUseParameters useParameters)
        {
            player.Heal((config as SelfHealConfig).GetHealAmount());
        }

    }
}
