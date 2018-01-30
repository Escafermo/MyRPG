using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    [SerializeField] AudioClip audioClip;
    [SerializeField] AudioSource audioSource;

    [SerializeField] Collider thisCollider;

    private void Start()
    {
        audioSource = GameObject.FindObjectOfType<AudioSource>();
    }

    private void OnTriggerEnter(Collider thisCollider)
    {
        if (thisCollider.tag == "Player")
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        } else
        {
            return;
        }

    }
}
