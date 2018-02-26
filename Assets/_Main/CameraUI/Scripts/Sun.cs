using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CameraUI
{
	public class Sun : MonoBehaviour 
	{
        float timeScale = 0.36f; // Needs to be 0.36
	
		void Update ()
        {
            float angleThisFrame = Time.deltaTime * timeScale;

            transform.RotateAround(transform.position, Vector3.forward, angleThisFrame);

        }
	}
}
