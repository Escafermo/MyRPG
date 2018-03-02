using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	public class WaypointContainer : MonoBehaviour 
	{

        // Draw spheres and lines between them
        private void OnDrawGizmos()
        {
            Vector3 firstWaypointPosition = transform.GetChild(0).position;
            Vector3 previousWaypointPosition = firstWaypointPosition;
            foreach (Transform waypoint in transform)
            {
                Gizmos.DrawSphere(waypoint.position, .2f);
                Gizmos.DrawLine(previousWaypointPosition, waypoint.position);
                previousWaypointPosition = waypoint.position;

            }
            Gizmos.DrawLine(previousWaypointPosition, firstWaypointPosition);
        }
    }
}
