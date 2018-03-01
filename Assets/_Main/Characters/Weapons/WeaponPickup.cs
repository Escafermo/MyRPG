using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

namespace RPG.Characters
{
    [ExecuteInEditMode]
	public class WeaponPickup : MonoBehaviour 
	{
        [SerializeField] WeaponConfig weaponConfig;
        [SerializeField] AudioClip pickupSound;

        AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        void Update ()
        {
            if (!Application.isPlaying)
            {
                DestroyChildren();
                InstantiateWeapon();
            }
		}

        void DestroyChildren()
        {
            foreach(Transform child in transform) // Since transform is a IEnumerable, this allows to run through the children
            {
                DestroyImmediate(child.gameObject);
            }
        }

        void InstantiateWeapon() // For Editor runtime
        {
            var weapon = weaponConfig.GetWeaponPrefab();
            weapon.transform.position = Vector3.zero;
            Instantiate(weapon, gameObject.transform);
        }

        private void OnTriggerEnter(Collider thisCollider) // May be slow TODO find 
        {
            GameObject thisGameObject = thisCollider.gameObject;
            WeaponSystem weaponSystem = thisGameObject.GetComponent<WeaponSystem>();
            weaponSystem.PickupWeapon(weaponConfig);
            audioSource.PlayOneShot(pickupSound);
        }
    }
}
