using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	public class CompanionHealBehaviour : SpecialAbilityBehaviour
    {
            PlayerControl player = null;

            void Start()
            {
                player = FindObjectOfType<PlayerControl>();
            }

            public override void Use(GameObject target)
            {
                Heal(target);
                PlayParticleEffect();
                PlayAbilitySound();
                PlayAbilityAnimation();
            }

            private void Heal(GameObject target)
            {
                var playerHealth = player.GetComponent<HealthSystem>();
                playerHealth.Heal((config as CompanionHealConfig).GetHealAmount());
            }
    }
}
