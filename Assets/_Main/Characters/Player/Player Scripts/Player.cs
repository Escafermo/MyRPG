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
using System;

namespace RPG.Characters
{
    public class Player : MonoBehaviour/*, IDamageable*/
    {
        // Constants
        //const string DEATH_TRIGGER = "Death";
        const string ATTACK_TRIGGER = "Attack";
        //const string HIT_TRIGGER = "Hit";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";

        //Animations & Particles
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        Animator animator;
        [SerializeField] ParticleSystem criticalHitParticleSystem = null; // Because there a two Particle Systems

        //Raycastimg
        CameraRaycaster cameraRaycaster = null;

        //Health
        //[SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float playerBaseDamage = 10f;
        //private float currentHealthPoints;
        float timeLastPlayerHit = 0f;

        //Damage
        Enemy enemy = null;
        GameObject weaponObject;
        [SerializeField] Weapons currentWeaponConfig = null;
        [SerializeField] SpecialAbilityConfig[] abilities;
        private float lastHitTime = 0f;
        [Range(.1f, 1f)] [SerializeField] float criticalHitChance = 0.1f;
        [Range(1.1f, 2f)] [SerializeField] float criticalHitMultiplier = 1.25f;

        //Audio
        //AudioSource audioSource;
        //[SerializeField] AudioClip[] arrayOfHitClips;
        //[SerializeField] AudioClip[] arrayOfDeathClips;

        void Start()
        {
            //audioSource = GetComponent<AudioSource>();

            RegisterForMouseClick();
            //GetMaxHealth();
            PickupWeapon(currentWeaponConfig);
            SetAttackAnimation();
            AttachAbilities();
        }

        void Update()
        {
            var healthPercentage = GetComponent<HealthSystem>().healthAsPercentage;
            if (healthPercentage > Mathf.Epsilon)
            {
                ScanForAbilityKeyPress();
            }
        }

        private void AttachAbilities()
        {
            for (int i = 0; i < abilities.Length; i++)
            {
                abilities[i].AttachAbilityTo(gameObject);
            }
        }

        private void ScanForAbilityKeyPress()
        {
            for (int i = 0; i < abilities.Length; i++)
            {
                string j = (i + 1).ToString(); // Array starts at 0, button starts at 1
                if (Input.GetKeyDown(j))
                {
                    AttemptSpecialAbility(i);
                }
            }
        }

        //public void TakeDamage(float damageAmount)
        //{
        //    bool playerDies = (currentHealthPoints - damageAmount <= 0); // Must ask before Reducing Health
        //    bool isTimeToGetHit = Time.time - timeLastPlayerHit > UnityEngine.Random.Range(2f, 5f); // Always take damage, but only play animation and hit sound after 2f-5f delay

        //    ReduceHealth(damageAmount);

        //    if (damageAmount > 0 && isTimeToGetHit) // Stop hit sound when heal - TODO find better solution
        //    {
        //        animator.SetTrigger(HIT_TRIGGER);
        //        //PlayRandomHitSound();
        //        timeLastPlayerHit = Time.time;
        //    }
        //    if (playerDies)
        //    {
        //        StartCoroutine(KilllPlayer());
        //    }
        //}

        //public void Heal(float healAmount)
        //{
        //    currentHealthPoints = Mathf.Clamp(currentHealthPoints + healAmount, 0f, maxHealthPoints);
        //}

        //IEnumerator KilllPlayer()
        //{
        //      animator.SetTrigger(DEATH_TRIGGER);

        //      //PlayRandomDeathSound();
        //      yield return new WaitForSecondsRealtime(audioSource.clip.length + 2f);

        //      SceneManager.LoadScene(0);
        //}

        //private void ReduceHealth(float damage)
        //{
        //    currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
        //}

        //public float healthAsPercentage
        //{
        //    get
        //    {
        //        return currentHealthPoints / (float)maxHealthPoints;
        //    }
        //}

        //private void GetMaxHealth()
        //{
        //    currentHealthPoints = maxHealthPoints;
        //}

        private void RegisterForMouseClick()
        {
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.notifyNewEnemyObservers += FindNewEnemy;
        }

        void FindNewEnemy(Enemy currentEnemy)
        {
            this.enemy = currentEnemy;

            if (Input.GetMouseButton(0) && IsEnemyInRange(enemy))
            {
                AttackEnemy();
            }
            else if (Input.GetMouseButtonDown(2))
            {
                AttemptSpecialAbility(0);   
            }
        }

        private void AttemptSpecialAbility(int abilityIndex)
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

        private void SetAttackAnimation()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimationClip();  // TODO remove const
        }


        private GameObject RequestHoldingHand()
        {
            var weaponHoldingHand = GetComponentsInChildren<HandWeapon>();
            int numberOfHoldingHands = weaponHoldingHand.Length;
            Assert.AreNotEqual(numberOfHoldingHands, 0, "No holding hand found, add script to hand");
            Assert.IsFalse(numberOfHoldingHands > 1, "Multiple holding hands found, remove script from hand(s)");
            return weaponHoldingHand[0].gameObject;
        }

        public void PickupWeapon(Weapons weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = weaponToUse.GetWeaponPrefab();
            GameObject weaponHoldingHand = RequestHoldingHand();
            Destroy(weaponObject); // Empty hands
            weaponObject = Instantiate(weaponPrefab, weaponHoldingHand.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
            print("put this in player hand " + weaponToUse);
        }

        private bool IsEnemyInRange(Enemy enemy)
        {
            float distanceToEnemy = (enemy.transform.position - transform.position).magnitude;
            return distanceToEnemy <= currentWeaponConfig.GetAttackRange();
        }

        private void AttackEnemy()
        {
            if (Time.time - lastHitTime > currentWeaponConfig.GetAttackRate())
            {
                SetAttackAnimation();
                animator.SetTrigger(ATTACK_TRIGGER);
                //enemy.TakeDamage(CalculateDamage());
                lastHitTime = Time.time;
            }
        }

        private float CalculateDamage()
        {
            bool  isCriticalHit = UnityEngine.Random.Range(0f, 1f) <= criticalHitChance;
            float damageBeforeCritical = (playerBaseDamage + currentWeaponConfig.GetAdditionalDamage());
            if (isCriticalHit)
            {
                criticalHitParticleSystem.Play();
                return damageBeforeCritical * criticalHitMultiplier;
            }
            else
            {
                return damageBeforeCritical;
            }
        }

        //private void PlayRandomHitSound()
        //{
        //    audioSource.clip = arrayOfHitClips[UnityEngine.Random.Range(0, 5)];
        //    audioSource.Play();
        //}

        //private void PlayRandomDeathSound()
        //{
        //    audioSource.clip = arrayOfDeathClips[UnityEngine.Random.Range(0, 3)];
        //    audioSource.Play();
        //}
    }
}

