using UnityEngine;
using RPG.CameraUI;
using System.Collections;

namespace RPG.Characters
{
    public class PlayerControl : MonoBehaviour
    {
        Character character;
        EnemyAI enemy;
        SpecialAbilities abilities;
        WeaponSystem weaponSystem;

        [SerializeField] float timeBeforeWalk;

        //bool isTargetOutOfRange;

        //[Header("Damage")]
        //[Range(.1f, 1f)] [SerializeField] float criticalHitChance = 0.1f;
        //[Range(1.1f, 2f)] [SerializeField] float criticalHitMultiplier = 1.25f;
        //[SerializeField] ParticleSystem criticalHitParticleSystem; // Because there a two Particle Systems

        void Start()
        {
            character = GetComponent<Character>();
            abilities = GetComponent<SpecialAbilities>();
            weaponSystem = GetComponent<WeaponSystem>();

            RegisterForMouseEvents();
        }

        void Update()
        {
            var healthPercentage = GetComponent<HealthSystem>().healthAsPercentage;
            if (healthPercentage > Mathf.Epsilon)
            {
                ScanForAbilityKeyPress();
            }
        }

        void RegisterForMouseEvents()
        {
            CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.notifyNewDestinationObservers += FindNewDestination;
            cameraRaycaster.notifyNewEnemyObservers += FindNewEnemy;
        }

        void FindNewEnemy(EnemyAI enemy)
        {
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                weaponSystem.AttackTarget(enemy.gameObject);
            }
            else if (Input.GetMouseButton(0) && !IsTargetInRange(enemy.gameObject))
            {
                StartCoroutine(MoveAndAttack(enemy));
            }
            else if (Input.GetMouseButtonDown(2) && IsTargetInRange(enemy.gameObject))
            {
                abilities.AttemptSpecialAbility(0,enemy.gameObject); // TODO Targeting the enemy whe clicked on
            }
            else if (Input.GetMouseButtonDown(2) && !IsTargetInRange(enemy.gameObject))
            {
                StartCoroutine(MoveAndSpecialAttack(enemy));
            }
        }

        IEnumerator MoveToTarget(GameObject target)
        {
            while (!IsTargetInRange(target))
            {
                character.SetDestination(target.transform.position);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
        }

        IEnumerator MoveAndAttack(EnemyAI enemy)
        {
            yield return (StartCoroutine(MoveToTarget(enemy.gameObject)));
            weaponSystem.AttackTarget(enemy.gameObject);
        }

        IEnumerator MoveAndSpecialAttack(EnemyAI enemy)
        {
            yield return (StartCoroutine(MoveToTarget(enemy.gameObject)));
            abilities.AttemptSpecialAbility(0, enemy.gameObject);
        }

        bool IsTargetInRange(GameObject target)
        {
            float distanceToEnemy = (target.transform.position - transform.position).magnitude;
            return distanceToEnemy <= weaponSystem.GetCurrentWeapon().GetAttackRange();
        }

        void FindNewDestination(Vector3 destination) // Set destination to click pos
        {
            if (Time.fixedTime > timeBeforeWalk && Input.GetMouseButton(0) && character.IsCharacterAlive()) //Delay for waking up animation
            {
                weaponSystem.StopAttacking();
                character.SetDestination(destination);
            }
        }

        void ScanForAbilityKeyPress()
        {
            for (int i = 0; i < abilities.GetNumberOfAbilities(); i++)
            {
                string j = (i + 1).ToString(); // Array starts at 0, button starts at 1
                if (Input.GetKeyDown(j))
                {
                    abilities.AttemptSpecialAbility(i);
                }
            }
        }
    }
}

