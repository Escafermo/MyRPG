using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Player : MonoBehaviour, IDamageable {

    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] float playerDamage = 10f;
    [SerializeField] float attackRate = 0.5f;
    [SerializeField] float attackRange = 2f;
    
    [SerializeField] const int enemyLayerNumber = 9;
    
    private float currentHealthPoints;
    private float lastHitTime = 0f;

    CameraRaycaster cameraRaycaster = null;
    GameObject currentTarget;

    void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        cameraRaycaster.notifyMouseClickObservers += OnMouseClick;

        currentHealthPoints = maxHealthPoints;
    }

    void OnMouseClick(RaycastHit raycastHit, int layerHit)
    {
        if (layerHit == enemyLayerNumber)
        {
            var enemy = raycastHit.collider.gameObject;

            if ((enemy.transform.position - transform.position).magnitude > attackRange)
            {
                return;
            }

            currentTarget = enemy;

            var enemyComponent = enemy.GetComponent<Enemy>();
            if(Time.time - lastHitTime > attackRate)
            {
                enemyComponent.TakeDamage(playerDamage);
                lastHitTime = Time.time;
            }
        }
    }

    public float  healthAsPercentage
    {
        get
        {
            return currentHealthPoints / (float)maxHealthPoints;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
        if (currentHealthPoints <= 0)
        {
            //Destroy(gameObject);
            Debug.LogError("Player dead");
        }
    }


}
