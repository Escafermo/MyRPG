using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI; // TODO consider re-wiring;

namespace RPG.Characters
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class CharacterMovement : MonoBehaviour
    {
        NavMeshAgent agent = null;
        Animator myAnimator = null; // TODO consider animationSpeedMultipler (animator.speed is being set to moveSpeedMultiplier)
        Rigidbody myRigidbody = null;

        [SerializeField] float timeBeforeWalk;
        [SerializeField] float stoppingDistance = 1f;
        [SerializeField] float moveSpeedMultiplier = 1f;
        [SerializeField] float animationSpeedMultiplier = 1.5f;
        [SerializeField] float movingTurnSpeed = 360;
        [SerializeField] float stationaryTurnSpeed = 180;
        [SerializeField] float moveThreshold = 1f;

        float turnAmount;
        float forwardAmount;

        void Start()
        {
            CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            agent = GetComponent<NavMeshAgent>();
            myAnimator = GetComponent<Animator>();
            myRigidbody = GetComponent<Rigidbody>();

            agent.updateRotation = false;
            agent.updatePosition = true;
            agent.stoppingDistance = stoppingDistance;

            cameraRaycaster.notifyNewDestinationObservers += FindNewDestination;
            cameraRaycaster.notifyNewEnemyObservers += FindNewEnemy;

            myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        private void Update()
        {
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                this.Move(agent.desiredVelocity);
            }
            else
            {
                this.Move(Vector3.zero);
            }
        }

        public void Move(Vector3 move) // Move to destination
        {
            SetForwardAndTurn(move);
            ApplyExtraTurnRotation();
            UpdateAnimator();
        }

        public void OnAnimatorMove()
        {
            //we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (/*m_IsGrounded && */Time.deltaTime > 0)
            {
                Vector3 velocity = (myAnimator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;
                // animator.deltaPositin is what should be the position last frame, then multiplied by moveSpeedMultipler
                // we preserve the existing y part of the current velocity.
                velocity.y = myRigidbody.velocity.y;
                myRigidbody.velocity = velocity;
            }
        }

        public void CharacterDeath()
        {
            // TODO alow death signal
        }


        void FindNewDestination(Vector3 destination) // Set destination to click pos
        {
            if (Time.fixedTime > timeBeforeWalk && Input.GetMouseButton(0)) //Delay for waking up animation
            {
                agent.SetDestination(destination);
            }
        }

        void FindNewEnemy(Enemy enemy) // Set destination to enemy
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(2))
            {
                agent.SetDestination(enemy.transform.position);
            }
        }

        private void SetForwardAndTurn(Vector3 move)
        {
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired direction.
            if (move.magnitude > moveThreshold)
            {
                move.Normalize();
            }
            var localMove = transform.InverseTransformDirection(move);
            turnAmount = Mathf.Atan2(localMove.x, localMove.z);
            forwardAmount = localMove.z;
        }

        void UpdateAnimator()
        {
            // update the animator parameters
            myAnimator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
            myAnimator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
            myAnimator.speed = animationSpeedMultiplier;
        }

        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }

    }
}
