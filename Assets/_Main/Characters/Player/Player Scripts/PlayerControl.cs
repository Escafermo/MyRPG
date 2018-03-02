using UnityEngine;
using UnityEngine.Assertions;
using RPG.CameraUI;

namespace RPG.Characters
{
    public class PlayerControl : MonoBehaviour
    {
        Character character;
        EnemyAI enemy;
        SpecialAbilities abilities;
        WeaponSystem weaponSystem;

        CameraRaycaster cameraRaycaster;
        [SerializeField] float timeBeforeWalk;

        [Header("Damage")]
        [Range(.1f, 1f)] [SerializeField] float criticalHitChance = 0.1f;
        [Range(1.1f, 2f)] [SerializeField] float criticalHitMultiplier = 1.25f;
        [SerializeField] ParticleSystem criticalHitParticleSystem; // Because there a two Particle Systems

        void Start()
        {
            character = GetComponent<Character>();
            abilities = GetComponent<SpecialAbilities>();

            RegisterForMouseEvents();

            weaponSystem = GetComponent<WeaponSystem>();
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
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.notifyNewDestinationObservers += FindNewDestination;
            cameraRaycaster.notifyNewEnemyObservers += FindNewEnemy;
        }

        void FindNewEnemy(EnemyAI currentEnemy)
        {
            this.enemy = currentEnemy;

            if (Input.GetMouseButton(0) && IsEnemyInRange(enemy))
            {
                weaponSystem.AttackTarget(enemy.gameObject);
            }
            else if (Input.GetMouseButtonDown(2))
            {
                abilities.AttemptSpecialAbility(0);
            }
        }

        bool IsEnemyInRange(EnemyAI enemy)
        {
            float distanceToEnemy = (enemy.transform.position - transform.position).magnitude;
            return distanceToEnemy <= weaponSystem.GetCurrentWeapon().GetAttackRange();
        }


        void FindNewDestination(Vector3 destination) // Set destination to click pos
        {
            if (Time.fixedTime > timeBeforeWalk && Input.GetMouseButton(0)) //Delay for waking up animation
            {
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

