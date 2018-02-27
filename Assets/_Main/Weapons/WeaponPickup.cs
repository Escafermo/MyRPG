using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

namespace RPG.Weapon
{
    [ExecuteInEditMode]
	public class WeaponPickup : MonoBehaviour 
	{
        [SerializeField] Weapons weaponConfig;
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

        void InstantiateWeapon()
        {
            var weapon = weaponConfig.GetWeaponPrefab();
            weapon.transform.position = Vector3.zero;
            Instantiate(weapon, gameObject.transform);
        }

        private void OnTriggerEnter() // May be slow TODO find 
        {
            FindObjectOfType<Player>().PickupWeapon(weaponConfig);
            audioSource.PlayOneShot(pickupSound);
        }
    }
}
