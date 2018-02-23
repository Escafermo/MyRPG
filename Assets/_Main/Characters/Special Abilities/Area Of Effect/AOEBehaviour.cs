using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
	public class AOEBehaviour : MonoBehaviour , ISpecialAbility
	{
        AOEConfig config;

        public void SetConfig(AOEConfig configToSet)
        {
            this.config = configToSet;
        }

        public void Use(AbilityUseParameters useParameters)
        {
            //Static sphere cast for targets:
            RaycastHit[] hitArray = Physics.SphereCastAll(transform.position, config.GetRadius(), Vector3.up,config.GetRadius());

            foreach (RaycastHit hit in hitArray)
            {
                var damageable = hit.collider.gameObject.GetComponent<IDamageable>();
                var enemy = hit.collider.gameObject.GetComponent<Enemy>();

                if (damageable != null && enemy != null)
                {
                    float damageToDeal = useParameters.baseDamage + config.GetDamageToEachTarget();
                    damageable.TakeDamage(damageToDeal);
                }
            }
        }
    }
}
