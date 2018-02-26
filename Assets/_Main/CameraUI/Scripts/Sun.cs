using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CameraUI
{
	public class Sun : MonoBehaviour 
	{
        float timeScale = 3.6f; // Needs to be 3.6
	
		void Update ()
        {
            float angleThisFrame = Time.deltaTime * timeScale;

            transform.RotateAround(transform.position, Vector3.forward, angleThisFrame);

        }
	}
}
