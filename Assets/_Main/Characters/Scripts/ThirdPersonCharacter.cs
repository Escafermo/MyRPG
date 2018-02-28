//using UnityEngine;

//namespace RPG.Characters
//{
//	[RequireComponent(typeof(Rigidbody))]
//	[RequireComponent(typeof(CapsuleCollider))]
//	[RequireComponent(typeof(Animator))]
//	public class ThirdPersonCharacter : MonoBehaviour
//	{
//		[SerializeField] float movingTurnSpeed = 360;
//		[SerializeField] float stationaryTurnSpeed = 180;
//        [SerializeField] float moveThreshold = 1f;
		

//		Rigidbody myRigidbody;
//		Animator myAnimator;
//		float turnAmount;
//		float forwardAmount;
		
//		void Start()
//		{
//			myAnimator = GetComponent<Animator>();
//			myRigidbody = GetComponent<Rigidbody>();

//			myRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

//            myAnimator.applyRootMotion = true;
//        }


//        public void Move(Vector3 move)
//        {
//            SetForwardAndTurn(move);
//            ApplyExtraTurnRotation();
//            UpdateAnimator();
//        }

//        private void SetForwardAndTurn(Vector3 move)
//        {
//            // convert the world relative moveInput vector into a local-relative
//            // turn amount and forward amount required to head in the desired direction.
//            if (move.magnitude > moveThreshold)
//            {
//                move.Normalize();
//            }
//            var localMove = transform.InverseTransformDirection(move);
//            turnAmount = Mathf.Atan2(localMove.x, localMove.z);
//            forwardAmount = localMove.z;
//        }

//        void UpdateAnimator()
//		{
//			// update the animator parameters
//			myAnimator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
//			myAnimator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
//		}

//		void ApplyExtraTurnRotation()
//		{
//			// help the character turn faster (this is in addition to root rotation in the animation)
//			float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
//			transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
//		}
//	}
//}
