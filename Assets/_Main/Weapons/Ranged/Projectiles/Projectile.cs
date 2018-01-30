using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core; // TODO consider re-wiring;

namespace RPG.Weapon
{
    public class Projectile : MonoBehaviour
    {

        [SerializeField] float projectileSpeed;
        [SerializeField] GameObject projectileShooter;

        float damageCaused;
        const float DESTROY_DELAY = 0.01f;

        public void SetShooter(GameObject shooter)
        {
            this.projectileShooter = shooter;
        }

        public void SetDamage(float damage)
        {
            damageCaused = damage;
        }

        public float GetDefaultLaunchSpeed()
        {
            return projectileSpeed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            var layerCollidedWith = collision.gameObject.layer;
            var layerOfShooter = projectileShooter.layer;
            if (projectileShooter && layerCollidedWith != layerOfShooter)
            {
                DealDamage(collision);
            }
            else
            {
                Destroy(gameObject, DESTROY_DELAY);
            }
        }

        private void DealDamage(Collision collision)
        {
            Component damageableComponent = collision.gameObject.GetComponent(typeof(IDamageable));

            if (damageableComponent)
            {
                (damageableComponent as IDamageable).TakeDamage(damageCaused);
            }
            Destroy(gameObject, DESTROY_DELAY);
        }
    }
}