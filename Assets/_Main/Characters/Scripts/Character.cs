using System;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Characters
{
    [SelectionBase] // Alows us to click on the BASE GAME OBJECT and not the CHILDREN on Scene view
    public class Character : MonoBehaviour
    {
        NavMeshAgent navMeshAgent = null;
        Animator animator = null; 
        Rigidbody myRigidbody = null;
        bool isAlive = true;

        [Header("Animator")]
        [SerializeField] RuntimeAnimatorController animatorController;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] Avatar characterAvatar;

        [Header("Movement")]
        [SerializeField] float moveSpeedMultiplier = 1f;
        [SerializeField] float animationSpeedMultiplier = 1.5f;
        [SerializeField] float movingTurnSpeed = 360;
        [SerializeField] float stationaryTurnSpeed = 180;
        [SerializeField] float moveThreshold = 1f;
        float animatorMoveCap = 1f; // 1f = run & .5f = walk
        float turnAmount;
        float forwardAmount;

        [Header("Capsule Collider")]
        [SerializeField] Vector3 colliderCenter = new Vector3(0,0.8f,0);
        [SerializeField] float colliderRadius = 0.2f;
        [SerializeField] float colliderHeight = 1.6f;

        [Header("Nav Mesh Agent")]
        [SerializeField] float agentSpeed = 5f;
        [SerializeField] float agentAngularSpeed = 12f;
        [SerializeField] float agentAcceleration = 10f;
        [SerializeField] float agentStoppingDistance = 1f;

        [Header("Audio")]
        [SerializeField] float audioSourceSpatialBlend = 0f;
        [SerializeField] float volume = 1f;

        private void Awake()
        {
            AddRequiredComponents();
        }

        void AddRequiredComponents()
        {
            // Add animator, controller, override, avatar
            animator = gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            animator.avatar = characterAvatar;

            // Add capsule collider, center, radius, height
            var capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.center = colliderCenter;
            capsuleCollider.radius = colliderRadius;
            capsuleCollider.height = colliderHeight;

            // Add rigidbody, freeze rotation
            myRigidbody = gameObject.AddComponent<Rigidbody>();
            myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            // Add NavMeshAgent, define rotation&position&autobraking, stopping distance, speed, angular speed, acceleration
            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            navMeshAgent.updatePosition = true;
            navMeshAgent.autoBraking = false;
            navMeshAgent.stoppingDistance = agentStoppingDistance;
            navMeshAgent.speed = agentSpeed;
            navMeshAgent.angularSpeed = agentAngularSpeed;
            navMeshAgent.acceleration = agentAcceleration;

            // Ad AudioSource, false on playonawake&loop, audiospatialblend
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = audioSourceSpatialBlend;
            audioSource.volume = volume;
        }

        private void Update()
        {
            if (!navMeshAgent.isOnNavMesh)
            {
                Debug.LogError(gameObject.name + " this object is outside the navmesh.");
            }
            if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance && isAlive)
            {
                Move(navMeshAgent.desiredVelocity);
            }
            else
            {
                Move(Vector3.zero);
            }
        }

        public void isWalking()
        {
            animatorMoveCap = .5f;
        }

        public void isRunning()
        {
            animatorMoveCap = 1f;
        }

        public float GetAnimationSpeedMultiplier()
        {
            return animator.speed; // Could also use animationSpeedMultiplier
        }


        public void SetDestination(Vector3 worldPos)
        {
            navMeshAgent.destination = worldPos;
        }

        public void CharacterDeath()
        {
            isAlive = false;
        }

        public bool IsCharacterAlive()
        {
            return isAlive;
        }


        public AnimatorOverrideController GetAnimatorOverrideController()
        {
            return animatorOverrideController;
        }

        void Move(Vector3 move) // Move to destination
        {
            SetForwardAndTurn(move);
            ApplyExtraTurnRotation();
            UpdateAnimator();
        }

        //// Halves the move speed from EnemyAI in Patrol state
        //public void OnPatrol()
        //{
        //    agentSpeed = agentSpeed / 2;
        //}

        //// Back to normal speed
        //public void Chasing()
        //{
        //    agentSpeed = agentSpeed * 2;
        //}

        public void OnAnimatorMove()
        {
            //we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (/*m_IsGrounded && */Time.deltaTime > 0)
            {
                Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;
                // animator.deltaPositin is what should be the position last frame, then multiplied by moveSpeedMultipler
                // we preserve the existing y part of the current velocity.
                velocity.y = myRigidbody.velocity.y;
                myRigidbody.velocity = velocity;
            }
        }


        //void FindNewDestination(Vector3 destination) // Set destination to click pos
        //{
        //    if (Time.fixedTime > timeBeforeWalk && Input.GetMouseButton(0)) //Delay for waking up animation
        //    {
        //        navMeshAgent.SetDestination(destination);
        //    }
        //}

        //void FindNewEnemy(Enemy enemy) // Set destination to enemy
        //{
        //    if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(2))
        //    {
        //        navMeshAgent.SetDestination(enemy.transform.position);
        //    }
        //}

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
            animator.SetFloat("Forward", forwardAmount * animatorMoveCap, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
            animator.speed = animationSpeedMultiplier;
        }

        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }

    }
}
