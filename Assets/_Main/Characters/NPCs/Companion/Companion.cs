using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    //[RequireComponent(typeof(WeaponSystem))]
    [RequireComponent(typeof(Character))]
    public class Companion : MonoBehaviour 
	{
        [SerializeField] float moveToPlayerRadius = 10f;
        [SerializeField] float reInstanceCompanionRadius = 30f;
        [SerializeField] float healRate = 20f;

        enum State { idle, heal, chase, flee, warn, attack } // TODO implement heal, flee, warn
        State state = State.idle;

        float currentWeaponRange;
        float distanceToPlayer;
        float healTime = 0f;
        bool isTimeToHeal = false;

        PlayerControl player;
        Character character;
        SpecialAbilities abilities;
        WeaponSystem weaponSystem;
        EnemyAI enemy;

        public bool companionAttack = false;

        private void Start()
        {
            player = FindObjectOfType<PlayerControl>();
            character = GetComponent<Character>();
            abilities = GetComponent<SpecialAbilities>();
            weaponSystem = GetComponent<WeaponSystem>();
        }

        private void Update()
        {
            IsTimeToHeal();

            //WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
            //currentWeaponRange = weaponSystem.GetCurrentWeapon().GetAttackRange();

            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            //distanceToEnemy = Vector3.Distance(enem)

            bool inMoveToPlayerRange = distanceToPlayer > moveToPlayerRadius;
            bool outOfRange = distanceToPlayer > reInstanceCompanionRadius;
            bool nearPlayer = distanceToPlayer <= moveToPlayerRadius;

            //if (distanceToPlayer <= moveToPlayerRadius && state != State.chase)
            if (inMoveToPlayerRange && character.IsCharacterAlive())
            {
                StopAllCoroutines();
                //weaponSystem.StopAttacking();
                character.isRunning();
                StartCoroutine(ChasePlayer());
            }
            if (isTimeToHeal && character.IsCharacterAlive())
            {
                StopAllCoroutines();
                StartCoroutine(HealPlayer());
            }
            if (companionAttack && character.IsCharacterAlive())
            {

                StopAllCoroutines();
                character.isRunning();
                StartCoroutine(MoveAndAttack(enemy));
            }
            //if (nearPlayer && character.IsCharacterAlive())
            //{
            //    StopCoroutine(ChasePlayer());
            //    state = State.idle;
            //}
            //if (outOfRange && character.IsCharacterAlive())
            //{
            //    Destroy(gameObject, 1f);
            //    Instantiate (FindObjectOfType<Companion>().gameObject,player.transform.position,Quaternion.identity);
            //}
        }

       IEnumerator ChasePlayer()
       {
            state = State.chase;

            while (distanceToPlayer >= moveToPlayerRadius)
            {
                character.SetDestination(player.transform.position);
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator HealPlayer()
        {
            state = State.heal;

            abilities.AttemptSpecialAbility(0, player.gameObject);

            isTimeToHeal = false;
            healTime = 0f;

            yield return new WaitForEndOfFrame();
        }

        void IsTimeToHeal()
        {
            healTime += Time.deltaTime;

            if (healTime >= healRate)
            {
                isTimeToHeal = true;
            }
        }


        private void OnDrawGizmos()
        {
            //Draw attack gizmos
            Gizmos.color = new Color(255f, 0f, 0f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, moveToPlayerRadius);

            Gizmos.color = new Color(0f, 0f, 255f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, currentWeaponRange);

        }
        
        public void GetCompanionEnemy (EnemyAI myEnemy)
        {
            enemy = myEnemy;
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
            state = State.attack;

            yield return (StartCoroutine(MoveToTarget(enemy.gameObject)));
            weaponSystem.AttackTarget(enemy.gameObject);
        }


        //IEnumerator MoveAndSpecialAttack(EnemyAI enemy)
        //{
        //    yield return (StartCoroutine(MoveToTarget(enemy.gameObject)));
        //    abilities.AttemptSpecialAbility(0, enemy.gameObject);
        //}

        bool IsTargetInRange(GameObject target)
        {
            float distanceToEnemy = (target.transform.position - transform.position).magnitude;
            return distanceToEnemy <= weaponSystem.GetCurrentWeapon().GetAttackRange();
        }

    }
}
