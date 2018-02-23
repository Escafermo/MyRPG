//using System;
using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
//using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

// TODO consider re-wiring:
using RPG.CameraUI;
using RPG.Core;
using RPG.Weapon;
using System;

namespace RPG.Characters
{
    public class Player : MonoBehaviour, IDamageable
    {
        const string DEATH_TRIGGER = "Death";
        const string ATTACK_TRIGGER = "Attack";

        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float playerBaseDamage = 10f;
        
        [SerializeField] Weapon.Weapons weaponInHand = null;
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        Animator animator;

        //Temporarily serialized for debugging TODO
        [SerializeField] SpecialAbilityConfig[] abilities;

        private float currentHealthPoints;
        private float lastHitTime = 0f;
        private AudioSource audioSource;

        CameraRaycaster cameraRaycaster = null;

        [SerializeField] AudioClip[] arrayOfHitClips;
        [SerializeField] AudioClip[] arrayOfDeathClips;

        void Start()
        {
            RegisterForMouseClick();
            GetMaxHealth();
            PutWeaponInHand();
            OverrideAnimatorController();
            abilities[0].AttachComponent(gameObject);
            audioSource = GetComponent<AudioSource>();
        }

        public void TakeDamage(float damage)
        {
            bool playerDies = (currentHealthPoints - damage <= 0); // Must ask before Reducing Health
            ReduceHealth(damage);
            if (damage > 0) // Stop hit sound when heal - TODO find better solution
            {
                PlayRandomHitSound();
            }
            if (playerDies) 
            {
               StartCoroutine(KilllPlayer());
            }
        }

        IEnumerator KilllPlayer()
        {
              animator.SetTrigger(DEATH_TRIGGER);

              PlayRandomDeathSound();
              yield return new WaitForSecondsRealtime(audioSource.clip.length + 2f);

              SceneManager.LoadScene(0);
        }

        private void ReduceHealth(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
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
            cameraRaycaster.notifyNewEnemyObservers += FindNewEnemy;
        }

        void FindNewEnemy(Enemy enemy)
        {
            if (Input.GetMouseButton(0) && IsEnemyInRange(enemy))
            {
                AttackEnemy(enemy);
            }
            else if (Input.GetMouseButtonDown(2) && IsEnemyInRange(enemy))
            {
                AttemptSpecialAbility(0,enemy);   
            }

        }

        private void AttemptSpecialAbility(int abilityIndex , Enemy enemy)
        {
            var energyComponent = GetComponent<Energy>();
            var energyCost = abilities[abilityIndex].GetEnergyCost();

            if (energyComponent.IsEnergyAvailable(energyCost)) // TODO read from specialized scriptable object
            {
                energyComponent.ConsumeEnergy(energyCost);
                var abilityParameters = new AbilityUseParameters(enemy, playerBaseDamage);
                abilities[abilityIndex].Use(abilityParameters);
            }
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



        private bool IsEnemyInRange(Enemy enemy)
        {
            float distanceToEnemy = (enemy.transform.position - transform.position).magnitude;
            return distanceToEnemy <= weaponInHand.GetAttackRange();
        }

        private void AttackEnemy(Enemy enemy)
        {
            if (Time.time - lastHitTime > weaponInHand.GetAttackRate())
            {
                animator.SetTrigger(ATTACK_TRIGGER);
                enemy.TakeDamage(playerBaseDamage);
                lastHitTime = Time.time;
            }
        }

        private void PlayRandomHitSound()
        {
            audioSource.clip = arrayOfHitClips[UnityEngine.Random.Range(0, 5)];
            audioSource.Play();
        }

        private void PlayRandomDeathSound()
        {
            audioSource.clip = arrayOfDeathClips[UnityEngine.Random.Range(0, 3)];
            audioSource.Play();
        }
    }
}

//[SerializeField] const int enemyLayerNumber = 9;
//void OnMouseClick(RaycastHit raycastHit, int layerHit)
//{
//    if (layerHit == enemyLayerNumber)
//    {
//        var enemy = raycastHit.collider.gameObject;
//        if (IsEnemyInRange(enemy))
//        {
//            AttackEnemy(enemy);
//        }
//    }
//}