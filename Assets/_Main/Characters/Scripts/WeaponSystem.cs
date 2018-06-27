using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace RPG.Characters
{
	public class WeaponSystem : MonoBehaviour 
	{
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";
        const string ATTACK_TRIGGER = "Attack";

        float lastHitTime;

        [SerializeField] WeaponConfig currentWeaponConfig;
        [SerializeField] float baseDamage = 10f;
        [SerializeField] AudioClip[] arrayOfAttackClips;

        Character character;
        Animator animator;
        GameObject target;
        GameObject weaponObject;
        AudioSource characterAudioSource;

        void Start ()
        {
            character = GetComponent<Character>();
            animator = GetComponent<Animator>();
            characterAudioSource = character.GetComponent<AudioSource>();

            PickupWeapon(currentWeaponConfig); 
            SetAttackAnimation(); 
        }

        void Update()
        {
            bool isTargetDead;
            bool isTargetOutOfRange;
            if (target == null)
            {
                isTargetDead = false;
                isTargetOutOfRange = false;
            }
            else
            {
                isTargetDead = target.GetComponent<HealthSystem>().healthAsPercentage <= Mathf.Epsilon;
                isTargetOutOfRange = Vector3.Distance(transform.position, target.transform.position) > currentWeaponConfig.GetAttackRange();
            }

            float characterHealth = GetComponent<HealthSystem>().healthAsPercentage;
            bool characterIsDead = (characterHealth <= Mathf.Epsilon);
            if (characterIsDead || isTargetOutOfRange || isTargetDead)
            {
                StopAllCoroutines();
            }

        }

        public WeaponConfig GetCurrentWeapon()
        {
            return currentWeaponConfig;
        }

        public void PickupWeapon(WeaponConfig weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = weaponToUse.GetWeaponPrefab();
            GameObject weaponHoldingHand = RequestHoldingHand();
            Destroy(weaponObject); // Empty hands
            weaponObject = Instantiate(weaponPrefab, weaponHoldingHand.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
        }

        GameObject RequestHoldingHand()
        {
            var weaponHoldingHand = GetComponentsInChildren<HandWeapon>();
            int numberOfHoldingHands = weaponHoldingHand.Length;
            Assert.AreNotEqual(numberOfHoldingHands, 0, "No holding hand found, add script to hand");
            Assert.IsFalse(numberOfHoldingHands > 1, "Multiple holding hands found, remove script from hand(s)");
            return weaponHoldingHand[0].gameObject;
        }

        public void StopAttacking()
        {
            animator.StopPlayback();
            StopAllCoroutines();
        }

        public void AttackTarget(GameObject targetToAttack)
        {
            target = targetToAttack;
            StartCoroutine(AttackTargetRepeatedly());
        }

        IEnumerator AttackTargetRepeatedly()
        {
            bool attackerIsAlive = GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;
            bool targetIsAlive = target.GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;

            while(attackerIsAlive && targetIsAlive)
            {
                AnimationClip animationClip = currentWeaponConfig.GetAttackAnimationClip();
                float animationClipTime = animationClip.length / character.GetAnimationSpeedMultiplier();
                float timeToWait = animationClipTime + currentWeaponConfig.GetTimeBetweenAnimationCycles();

                //float attackRate = currentWeaponConfig.GetTimeBetweenAnimationCycles();
                //float timeToWait = attackRate * character.GetAnimationSpeedMultiplier();
                bool isTimeToAttackAgain = Time.time - lastHitTime > timeToWait;

                if (isTimeToAttackAgain)
                {
                    AttackTargetOnce();
                    lastHitTime = Time.time;
                }
                yield return new WaitForSeconds(timeToWait);
            }
        }

        void AttackTargetOnce()
        {
            transform.LookAt(target.transform);
            animator.SetTrigger(ATTACK_TRIGGER);
            float damageDelay = currentWeaponConfig.GetWeaponDelay();
            SetAttackAnimation();
            PlayRandomAttackSound();
            StartCoroutine(DamageAfterDelay(damageDelay));
        }

        IEnumerator DamageAfterDelay(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            HealthSystem thisHealthSystem = target.GetComponent<HealthSystem>();
            thisHealthSystem.TakeDamage(CalculateDamage());
        }


        float CalculateDamage()
        {
            return baseDamage + currentWeaponConfig.GetAdditionalDamage();
            // TODO reinstance CRITICAL HIT
            //bool isCriticalHit = UnityEngine.Random.Range(0f, 1f) <= criticalHitChance;
            //float damageBeforeCritical = (playerBaseDamage + currentWeaponConfig.GetAdditionalDamage());
            //if (isCriticalHit)
            //{
            //    criticalHitParticleSystem.Play();
            //    return damageBeforeCritical * criticalHitMultiplier;
            //}
            //else
            //{
            //    return damageBeforeCritical;
            //}
        }

        void SetAttackAnimation()
        {
            if (!character.GetAnimatorOverrideController())
            {
                Debug.Break();
                Debug.LogAssertion("Provide " + gameObject + " with an animator override controller");
            }
            var animatorOverrideController = character.GetAnimatorOverrideController();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimationClip();  // TODO remove const
        }

        void PlayRandomAttackSound()
        {
            //if (arrayOfAttackClips[0] != null)
            //{
                var clip = arrayOfAttackClips[UnityEngine.Random.Range(0, arrayOfAttackClips.Length)];
                characterAudioSource.PlayOneShot(clip); // Play on top of other sounds
            //}
        }
    }
}
