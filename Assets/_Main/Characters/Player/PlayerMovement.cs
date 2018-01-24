using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AICharacterControl))]
[RequireComponent(typeof(ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
    ThirdPersonCharacter thirdPersonPlayer = null;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster cameraRaycaster = null;
    AICharacterControl aiCharacter = null;
    GameObject walkTarget = null;
    Vector3 currentClickDestination;

    [SerializeField] const int buttonCursorNumber = 5;
    [SerializeField] const int walkCursorNumber = 8;
    [SerializeField] const int targetCursorNumber = 9;

    bool isInDirectMode = false; // Direct mode = keyboard or gamepad | Indirect mode = mouse

    void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        thirdPersonPlayer = GetComponent<ThirdPersonCharacter>();
        currentClickDestination = transform.position;
        aiCharacter = GetComponent<AICharacterControl>();
        walkTarget = new GameObject("walkTarget");

        cameraRaycaster.notifyMouseClickObservers += ProcessMouseClick;
    }

    void ProcessMouseClick(RaycastHit raycastHit, int layerHit)
    {
        switch (layerHit)
        {
            case targetCursorNumber:
                GameObject enemy = raycastHit.collider.gameObject;
                aiCharacter.SetTarget(enemy.transform);
                break;
            case walkCursorNumber:
                walkTarget.transform.position = raycastHit.point;
                aiCharacter.SetTarget(walkTarget.transform);
                break;
            case buttonCursorNumber:
                break;
            default:
                break;
        }
    }

    //private void FixedUpdate()
    //{
    //    if (Input.GetKeyDown(KeyCode.G)) // Press G to change between mouse and gamepad. TODO: Allow player to remap this key
    //    {
    //        aiCharacter.SetTarget(transform); // Clear the last set ClickTarget
    //        isInDirectMode = !isInDirectMode; // Toggle
    //    }

    //    if (isInDirectMode)
    //    {
    //        cameraRaycaster.notifyMouseClickObservers -= ProcessMouseClick;
    //        ProcessDirectMovement();
    //    }
    //    else
    //    {
    //        return;
    //    }
    //}


    //TODO Process direct player movement
    //private void ProcessDirectMovement ()
    //{
    //    float h = CrossPlatformInputManager.GetAxis("Horizontal"); // Could be CROSSPLATFORM
    //    float v = CrossPlatformInputManager.GetAxis("Vertical");

    //    // calculate camera relative direction to move:
    //    Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
    //    Vector3 moveVector = v * cameraForward + h * Camera.main.transform.right;

    //    thirdPersonPlayer.Move(moveVector, false, false);
    //}

    //private void OnDrawGizmos()
    //{
    //    //Draw move gizmos
    //    Gizmos.color = Color.black;
    //    Gizmos.DrawLine(transform.position, currentClickDestination); // Get a line from current position to the click target
    //    Gizmos.DrawSphere(currentClickDestination, 0.1f);
    //    Gizmos.DrawSphere(clickPoint, 0.15f);

    //    //Draw attack gizmos
    //    Gizmos.color = new Color(0f, 0f, 255f, 0.5f);
    //    Gizmos.DrawWireSphere(transform.position, attackMoveStopRadius);
    //}
}

