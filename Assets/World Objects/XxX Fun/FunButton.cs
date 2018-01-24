using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunButton : MonoBehaviour {

    private AudioSource audioSource;

    private void OnCollisionEnter(Collision collision)
    {
        audioSource.Play();

        transform.position += new Vector3(0, +17, 0);
    }

    void Start ()
    {
        audioSource = GetComponent<AudioSource>();
    }


    void Update () {
		
	}
}
