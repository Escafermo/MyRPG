using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour, IDamageable
{

    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] float damagePerShot = 9f;

    [SerializeField] float moveToPlayerRadius = 10f;
    [SerializeField] float attackPlayerRadius = 5f;
    [SerializeField] float chasePlayerRadius = 50f; //TODO make enemy have a bigger chase radius after seeign the player for the 1st time

    [SerializeField] GameObject projectileToUse;
    [SerializeField] GameObject projectileSpawnSocket;
    [SerializeField] float projectileFrequency = 1f;
    [SerializeField] Vector3 verticalAimOffset = new Vector3(0,1f,0);

    private float currentHealthPoints = 100;
    private bool isAttacking = false;

    AICharacterControl aiCharacter = null;
    GameObject player = null;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        aiCharacter = GetComponent<AICharacterControl>();

        currentHealthPoints = maxHealthPoints;
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position,transform.position);
        
        if (distanceToPlayer <= attackPlayerRadius && !isAttacking)
        {
            isAttacking = true;
            InvokeRepeating("SpawnProjectile", 0f, projectileFrequency); // TODO switch to coroutines
        }
        if (distanceToPlayer > attackPlayerRadius)
        {
            isAttacking = false;
            CancelInvoke();
        }

        if (distanceToPlayer <= moveToPlayerRadius)
        {
            aiCharacter.SetTarget(player.transform);
        }
        else
        {
            aiCharacter.SetTarget(null);
        }

    }

    
    public void TakeDamage(float damage)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
        if (currentHealthPoints <= 0)
        {
            Destroy(gameObject);
        }
    }

    public float healthAsPercentage
    {
        get
        {
            return currentHealthPoints / (float)maxHealthPoints;
        }
    }

    public void SpawnProjectile ()
    {
        GameObject newProjectile = Instantiate(projectileToUse, projectileSpawnSocket.transform.position, Quaternion.identity);
        Projectile projectileComponent = newProjectile.GetComponent<Projectile>();

        projectileComponent.SetDamage(damagePerShot);

        Vector3 unitVectorToPlayer = ((player.transform.position+(verticalAimOffset)) - projectileSpawnSocket.transform.position).normalized;
        float thisProjectileSpeed = projectileComponent.projectileSpeed;
        newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * thisProjectileSpeed;
    }

    private void OnDrawGizmos()
    {
        //Draw attack gizmos
        Gizmos.color = new Color(255f, 0f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, moveToPlayerRadius);

        Gizmos.color = new Color(0f, 0f, 255f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, attackPlayerRadius);
    }


}
