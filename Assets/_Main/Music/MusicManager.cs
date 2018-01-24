using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    [SerializeField] AudioClip audioClip;
    [SerializeField] AudioSource audioSource;

    [SerializeField] Collider thisCollider;

    float colliderIndex = 0f;

    private void Start()
    {
        audioSource = GameObject.FindObjectOfType<AudioSource>();
    }

    private void OnTriggerEnter(Collider thisCollider)
    {
        audioSource.clip = audioClip;
        audioSource.Play();

    }




}
