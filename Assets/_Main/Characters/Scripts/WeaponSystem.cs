using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters
{
	public class WeaponSystem : MonoBehaviour 
	{
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";
        const string ATTACK_TRIGGER = "Attack";

        float lastHitTime;

        [SerializeField] WeaponConfig currentWeaponConfig;
        [SerializeField] float playerBaseDamage = 10f;

        Character character;
        Animator animator;
        GameObject target;
        GameObject weaponObject;

        void Start ()
        {
            character = GetComponent<Character>();
            animator = GetComponent<Animator>();

            PickupWeapon(currentWeaponConfig); 
            SetAttackAnimation(); 
            
        }
	
	
		void Update () {
			
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

            print("put this in player hand " + weaponToUse +"  "+ weaponObject +"   "+ weaponPrefab);

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

       

        public void AttackTarget(GameObject targetToAttack)
        {
            target = targetToAttack;
            print("attacking " + targetToAttack);
            // TODO use a repeat attack co-routine
        }

        void AttackTarget()
        {
            if (Time.time - lastHitTime > currentWeaponConfig.GetAttackRate())
            {
                SetAttackAnimation();
                animator.SetTrigger(ATTACK_TRIGGER);
                //enemy.TakeDamage(CalculateDamage());
                lastHitTime = Time.time;
            }
        }

        float CalculateDamage()
        {
            return playerBaseDamage + currentWeaponConfig.GetAdditionalDamage();
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
            animator = GetComponent<Animator>();
            var animatorOverrideController = character.GetAnimatorOverrideController();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimationClip();  // TODO remove const
        }

    }
}
