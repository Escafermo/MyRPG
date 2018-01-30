using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = ("RPG_Weapon"))]
public class Weapons : ScriptableObject {

    public Transform gripTransform;

	[SerializeField] GameObject weaponPrefab;
	[SerializeField] AnimationClip attackAnimation;


    public GameObject GetWeaponPrefab()
    {
        return weaponPrefab;
    }
}
