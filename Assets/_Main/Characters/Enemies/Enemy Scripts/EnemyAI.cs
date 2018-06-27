using UnityEngine;
using System.Collections;

namespace RPG.Characters
{
    [RequireComponent(typeof (WeaponSystem))]
    [RequireComponent(typeof(HealthSystem))]
    [RequireComponent(typeof(Character))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float moveToPlayerRadius = 10f;
        [SerializeField] float moveToPlayerSecondRadius = 50f; //TODO make enemy have a bigger chase radius after seeign the player for the 1st time
        [SerializeField] WaypointContainer patrolPath;
        [SerializeField] float waypointTolerance = 2f;
        [SerializeField] float waypointDwellTime;

        enum State { idle, attack, patrol, heal, chase, flee, warn} // TODO implement heal, flee, warn
        State state = State.idle;

        float currentWeaponRange;
        float distanceToPlayer;
        int nextWaypointIndex;
        float patrolTimeCounter = 0f;
        bool timeToMove = false;

        PlayerControl player;
        Character character;
        AudioSource characterAudioSource;

        private void Start()
        {
            player = FindObjectOfType<PlayerControl>();
            character = GetComponent<Character>();
        }

        private void Update()
        {
            IsTimeToMove();

            WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetAttackRange();

            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

            bool inWeaponRange = distanceToPlayer <= currentWeaponRange;
            bool inMoveToPlayerRange = distanceToPlayer > currentWeaponRange && distanceToPlayer <= moveToPlayerRadius;
            bool outOfRange = distanceToPlayer > moveToPlayerRadius;

            //if (distanceToPlayer > moveToPlayerRadius && state != State.patrol)
            if (outOfRange && character.IsCharacterAlive())
            {
                StopAllCoroutines();
                weaponSystem.StopAttacking();
                character.isWalking();
                StartCoroutine(Patrol());
            }
            //if (distanceToPlayer <= moveToPlayerRadius && state != State.chase)
            if (inMoveToPlayerRange && character.IsCharacterAlive())
            {
                StopAllCoroutines();
                weaponSystem.StopAttacking();
                character.isRunning();
                StartCoroutine(ChasePlayer());
            }
            //if (distanceToPlayer <= currentWeaponRange && state != State.attack)
            if (inWeaponRange && character.IsCharacterAlive())
            {
                StopAllCoroutines();
                state = State.attack;
                weaponSystem.AttackTarget(player.gameObject);
            }
        }
        
        IEnumerator ChasePlayer()
        {
            state = State.chase;

            while (distanceToPlayer >= currentWeaponRange)
            {
                character.SetDestination(player.transform.position);
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator Patrol()
        {
            state = State.patrol;

            while (patrolPath != null)
            {
                Vector3 nextWaypointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
                character.SetDestination(nextWaypointPos);
                if (timeToMove)
                {
                    CycleWaypointWhenClose(nextWaypointPos);
                    timeToMove = false;
                    patrolTimeCounter = 0f;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        void IsTimeToMove()
        {
            if (patrolTimeCounter >= waypointDwellTime)
            {
                timeToMove = true;
            }
            patrolTimeCounter += Time.deltaTime;
        }

        void CycleWaypointWhenClose(Vector3 nextWaypointPosition)
        {
            if (Vector3.Distance(transform.position,nextWaypointPosition) <= waypointTolerance)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
            }
        }

        void OnDrawGizmos()
        {
            //Draw attack gizmos
            Gizmos.color = new Color(255f, 0f, 0f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, moveToPlayerRadius);

            Gizmos.color = new Color(0f, 0f, 255f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, currentWeaponRange);
        }
    }
}