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
        Companion companion;

        [SerializeField] float timeBeforeWalk;
        [SerializeField] GameObject particleEffectForClick;

        Vector3 particleEffectPos;

        public bool isAttacking = false;

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
            companion = FindObjectOfType<Companion>();

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
            if (IsTargetInRange(enemy.gameObject))
            {
                weaponSystem.AttackTarget(enemy.gameObject);
            }
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
            else if (Input.GetButtonDown("CompanionAttack"))
            {
                StartCoroutine(CompanionAttack(enemy));
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

        IEnumerator CompanionAttack(EnemyAI companionEnemy)
        {
            companion.GetCompanionEnemy(companionEnemy);
            companion.companionAttack = true;
            yield return new WaitForEndOfFrame();
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
                PlayClickParticleEffect(destination);
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

        void PlayClickParticleEffect(Vector3 destination)
        {
            Vector3 offset = new Vector3(0, .25f, 0f);
            
            GameObject unique = GameObject.FindGameObjectWithTag("Respawn");
            if (!unique)
            {
                var thisParticleSystemObject = Instantiate(
                particleEffectForClick,
                destination + offset,
                particleEffectForClick.transform.rotation
                );
                ParticleSystem myParticleSystem = thisParticleSystemObject.GetComponent<ParticleSystem>();
                myParticleSystem.Play();
                StartCoroutine(DestroyParticleWhenFinished(thisParticleSystemObject));
            }
        }

        IEnumerator DestroyParticleWhenFinished(GameObject particlePrefab)
        {
            while (particlePrefab.GetComponent<ParticleSystem>().isPlaying)
            {
                yield return new WaitForSeconds(.5f);
            }
            Destroy(particlePrefab);
            yield return new WaitForEndOfFrame();
        }


    }
}

