using UnityEngine;
using System.Collections;

namespace RPG.Characters
{
    [RequireComponent(typeof (WeaponSystem))]
    [RequireComponent(typeof(HealthSystem))]
    [RequireComponent(typeof(Character))]
    public class EnemyAI : MonoBehaviour
    {
        //[SerializeField] float damagePerShot = 9f;

        [SerializeField] float moveToPlayerRadius = 10f;
        [SerializeField] float moveToPlayerSecondRadius = 50f; //TODO make enemy have a bigger chase radius after seeign the player for the 1st time
        [SerializeField] WaypointContainer patrolPath;
        [SerializeField] float waypointTolerance = 2f;

        //[SerializeField] GameObject projectileToUse;
        //[SerializeField] GameObject projectileSpawnSocket;
        //[SerializeField] float projectileFrequency = 1f;
        //[SerializeField] float projectileFrequencyVariation = 0.1f;
        //[SerializeField] Vector3 verticalAimOffset = new Vector3(0, 1f, 0);

        enum State { idle, attack, patrol, heal, chase, flee, warn} // TODO implement heal, flee, warn
        State state = State.idle;

        float currentWeaponRange;
        float distanceToPlayer;
        int nextWaypointIndex;

        PlayerControl player;
        Character character;

        private void Start()
        {
            player = FindObjectOfType<PlayerControl>();
            character = GetComponent<Character>();
        }

        private void Update()
        {
            WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetAttackRange();

            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

            if (distanceToPlayer > moveToPlayerRadius && state != State.patrol)
            {
                StopAllCoroutines();
                StartCoroutine(Patrol());
            }
            if (distanceToPlayer <= moveToPlayerRadius && state != State.chase)
            {
                StopAllCoroutines();
                StartCoroutine(ChasePlayer());
            }
            if (distanceToPlayer <= currentWeaponRange && state != State.attack)
            {
                StopAllCoroutines();
                state = State.attack;
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

            while (true)
            {
                Vector3 nextWaypointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
                character.SetDestination(nextWaypointPos);
                CycleWaypointWhenClose(nextWaypointPos);
                yield return new WaitForSeconds(0.5f); // TODO parameterise for 0.5f
            }

        }

        void CycleWaypointWhenClose(Vector3 nextWaypointPosition)
        {
            if (Vector3.Distance(transform.position,nextWaypointPosition) <= waypointTolerance)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
            }
        }


        //// TODO Separate out Shooter firing logic insto separate class
        //public void FireProjectile()
        //{
        //    GameObject newProjectile = Instantiate(projectileToUse, projectileSpawnSocket.transform.position, Quaternion.identity);
        //    Projectile projectileComponent = newProjectile.GetComponent<Projectile>();
        //    projectileComponent.SetDamage(damagePerShot);
        //    projectileComponent.SetShooter(gameObject);

        //    Vector3 unitVectorToPlayer = ((player.transform.position + (verticalAimOffset)) - projectileSpawnSocket.transform.position).normalized;
        //    float thisProjectileSpeed = projectileComponent.GetDefaultLaunchSpeed();
        //    newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * thisProjectileSpeed;
        //}

        private void OnDrawGizmos()
        {
            //Draw attack gizmos
            Gizmos.color = new Color(255f, 0f, 0f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, moveToPlayerRadius);

            Gizmos.color = new Color(0f, 0f, 255f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, currentWeaponRange);
        }


    }
}