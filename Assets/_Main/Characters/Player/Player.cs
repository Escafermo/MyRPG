using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.Assertions;

// TODO consider re-wiring:
using RPG.CameraUI; 
using RPG.Core; 
using RPG.Weapon;

namespace RPG.Characters
{
    public class Player : MonoBehaviour, IDamageable
    {

        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float playerDamage = 10f;
        
        [SerializeField] Weapon.Weapons weaponInHand;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        Animator animator;

        [SerializeField] const int enemyLayerNumber = 9;

        private float currentHealthPoints;
        private float lastHitTime = 0f;

        CameraRaycaster cameraRaycaster = null;

        void Start()
        {
            RegisterForMouseClick();
            GetMaxHealth();
            PutWeaponInHand();
            OverrideAnimatorController();
        }

        public void TakeDamage(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            if (currentHealthPoints <= 0)
            {
                //Destroy(gameObject);
                //Debug.LogError("Player dead");
            }
        }

        public float healthAsPercentage
        {
            get
            {
                return currentHealthPoints / (float)maxHealthPoints;
            }
        }

        private void GetMaxHealth()
        {
            currentHealthPoints = maxHealthPoints;
        }

        private void RegisterForMouseClick()
        {
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
        }

        private void OverrideAnimatorController()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController["DEFAULT ATTACK"] = weaponInHand.GetAttackAnimationClip();  // TODO remove const
        }

        private void PutWeaponInHand()
        {
            var weaponPrefab = weaponInHand.GetWeaponPrefab();
            GameObject weaponHoldingHand = RequestHoldingHand();
            var weapon = Instantiate(weaponPrefab, weaponHoldingHand.transform);
            weapon.transform.localPosition = weaponInHand.gripTransform.localPosition;
            weapon.transform.localRotation = weaponInHand.gripTransform.localRotation;
        }

        private GameObject RequestHoldingHand()
        {
            var weaponHoldingHand = GetComponentsInChildren<HandWeapon>();
            int numberOfHoldingHands = weaponHoldingHand.Length;
            Assert.AreNotEqual(numberOfHoldingHands, 0, "No holding hand found, add script to hand");
            Assert.IsFalse(numberOfHoldingHands > 1, "Multiple holding hands found, remove script from hand(s)");
            return weaponHoldingHand[0].gameObject;
        }

        void OnMouseClick(RaycastHit raycastHit, int layerHit)
        {
            if (layerHit == enemyLayerNumber)
            {
                var enemy = raycastHit.collider.gameObject;
                if (IsEnemyInRange(enemy))
                {
                    AttackEnemy(enemy);
                }
            }
        }

        private bool IsEnemyInRange(GameObject target)
        {
            float distanceToEnemy = (target.transform.position - transform.position).magnitude;
            return distanceToEnemy <= weaponInHand.GetAttackRange();
        }

        private void AttackEnemy(GameObject target)
        {
            var enemyComponent = target.GetComponent<Enemy>();
            if (Time.time - lastHitTime > weaponInHand.GetAttackRate())
            {
                animator.SetTrigger("Attack"); // TODO Make const
                enemyComponent.TakeDamage(playerDamage);
                lastHitTime = Time.time;
            }
        }
    }
}