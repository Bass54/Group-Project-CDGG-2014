using System;
using System.Threading;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class PinguinMover : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField]
        private float m_WalkSpeed = 1f;
        [SerializeField]
        private float m_RunSpeed = 4f;
        [SerializeField, Range(0f, 360f)]
        private float m_RotateSpeed = 110f;
        [SerializeField]
        private Space m_Space = Space.Self;
        [SerializeField]
        private float m_JumpHeight = 5f;
        [SerializeField]
        private bool m_IsJumping = false;

        [Header("Animator")]
        [SerializeField]
        private string m_VerticalID = "Vert";
        [SerializeField]
        private string m_StateID = "State";
        [SerializeField]
        private LookWeight m_LookWeight = new(1f, 0.3f, 0.7f, 1f);

        private Transform m_Transform;
        private CharacterController m_Controller;
        private Animator m_Animator;

        private MovementHandler m_Movement;
        private AnimationHandler m_Animation;

        private Vector2 m_Axis;
        private Vector3 m_Target;
        private bool m_IsRun;

        private bool m_IsMoving;

        public TextMeshProUGUI ammoText;
        private int count;

        public Vector2 Axis => m_Axis;
        public Vector3 Target => m_Target;
        public bool IsRun => m_IsRun;

        private void OnValidate()
        {
            m_WalkSpeed = Mathf.Max(m_WalkSpeed, 0f);
            m_RunSpeed = Mathf.Max(m_RunSpeed, m_WalkSpeed);

            m_Movement?.SetStats(m_WalkSpeed / 3.6f, m_RunSpeed / 3.6f, m_RotateSpeed, m_JumpHeight, m_Space);
        }

        private void Awake()
        {
            m_Transform = transform;
            m_Controller = GetComponent<CharacterController>();
            m_Animator = GetComponent<Animator>();

            m_Movement = new MovementHandler(m_Controller, m_Transform, m_WalkSpeed, m_RunSpeed, m_RotateSpeed, m_JumpHeight, m_Space);
            m_Animation = new AnimationHandler(m_Animator, m_VerticalID, m_StateID);
        }

        //private void Start()
        //{
        //    SetAmmoText();
        //}

        private void Update()
        {
            // Capture horizontal and vertical input from WASD or Arrow keys
            float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow keys
            float vertical = Input.GetAxis("Vertical"); // W/S or Up/Down Arrow keys

            // Combine the input into a Vector2 (this is used for movement direction)
            Vector2 axis = new Vector2(horizontal, vertical);

            // Use the camera's position as the target (or another target for movement)
            Vector3 target = Camera.main.transform.position;  // Or any other target

            // Set whether the player is running (e.g., holding down Shift key)
            bool isRun = Input.GetKey(KeyCode.LeftShift);  // Run with Left Shift key

            bool isJump = Input.GetKeyDown(KeyCode.Space);  // Jump with Space key

            // Pass the input to the CreatureMover component (this will handle movement)
            SetInput(axis, target, isRun, isJump);  // false means not jumping here


            m_Movement.Move(Time.deltaTime, in m_Axis, m_IsRun, m_IsJumping, out var animAxis, out var isAir);
            m_Animation.Animate(in animAxis, m_IsRun ? 1f : 0f, Time.deltaTime);

            
        }

        private void OnAnimatorIK()
        {
            m_Animation.AnimateIK(in m_Target, m_LookWeight);
        }

        public void SetInput(Vector2 axis, Vector3 target, bool isRun, bool isJump)
        {
            m_Axis = axis;
            m_Target = target;
            m_IsRun = isRun;

            if (m_Axis.sqrMagnitude > Mathf.Epsilon) // Check if there's movement input
            {
                m_IsMoving = true;
            }
            else
            {
                m_IsMoving = false;
                m_Target = Vector3.zero;
            }

            m_IsJumping = isJump;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.normal.y > m_Controller.stepOffset)
            {
                m_Movement.SetSurface(hit.normal);
            }
        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    if (other.CompareTag("Ammo"))
        //    {
        //        count++;
        //        SetAmmoText();
        //        Destroy(other.gameObject);
        //    }
        //}

        //void SetAmmoText()//to be continued...
        //{
        //    ammoText.text = $"Ammo: {count.ToString()}";
        //}

        [Serializable]
        private struct LookWeight
        {
            public float weight;
            public float body;
            public float head;
            public float eyes;

            public LookWeight(float weight, float body, float head, float eyes)
            {
                this.weight = weight;
                this.body = body;
                this.head = head;
                this.eyes = eyes;
            }
        }

        #region Handlers
        private class MovementHandler
        {
            private readonly CharacterController m_Controller;
            private readonly Transform m_Transform;

            private float m_WalkSpeed;
            private float m_RunSpeed;
            private float m_RotateSpeed;

            private Space m_Space;

            private readonly float m_Luft = 75f;

            private float m_TargetAngle;
            private bool m_IsRotating = false;

            private Vector3 m_Normal;
            private Vector3 m_GravityAcelleration = Physics.gravity;
            private float m_JumpHeight = 5f;

            private float m_jumpTimer;
            private Vector3 m_LastForward;

            public MovementHandler(CharacterController controller, Transform transform, float walkSpeed, float runSpeed, float rotateSpeed, float jumpHeight, Space space)
            {
                m_Controller = controller;
                m_Transform = transform;

                m_WalkSpeed = walkSpeed;
                m_RunSpeed = runSpeed;
                m_RotateSpeed = rotateSpeed;


                m_Space = space;
            }

            public void SetStats(float walkSpeed, float runSpeed, float rotateSpeed, float jumpHeight, Space space)
            {
                m_WalkSpeed = walkSpeed;
                m_RunSpeed = runSpeed;
                m_RotateSpeed = rotateSpeed;

                m_Space = space;
            }

            public void SetSurface(Vector3 normal)
            {
                m_Normal = normal;
            }

            public void Move(float deltaTime, in Vector2 axis, bool isRun, bool isJumping, out Vector2 animAxis, out bool isAir)
            {

                Vector3 inputDirection = new Vector3(axis.x, 0f, axis.y).normalized;

                if (inputDirection.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
                    m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, targetRotation, deltaTime * m_RotateSpeed);
                }

                float speed = isRun ? m_RunSpeed : m_WalkSpeed;
                Vector3 velocity = inputDirection * speed;

                CaculateGravity(deltaTime, isJumping, out isAir);

                velocity.y = m_GravityAcelleration.y;

                m_Controller.Move(velocity * deltaTime);

                animAxis = new Vector2(inputDirection.x, inputDirection.z);

            }

            private void CaculateGravity(float deltaTime, bool isJumping, out bool isAir)
            {
                if (m_Controller.isGrounded)
                {
                    m_GravityAcelleration.y = -0.5f;
                    isAir = false;

                    if (isJumping)
                    {
                        m_GravityAcelleration.y = m_JumpHeight; // Apply the jump force
                        isJumping = false;
                    }
                }
                else
                {
                    isAir = true;
                    m_GravityAcelleration.y += Physics.gravity.y * deltaTime; // Apply gravity over time
                }
                m_jumpTimer = Mathf.Max(m_jumpTimer - deltaTime, 0f);
            }

            public void Turn(Vector3 targetForward, float deltaTime, bool isRunning)
            {
                if (targetForward.sqrMagnitude < 0.01f)
                {
                    return;
                }

                // Get the target rotation
                Quaternion targetRotation = Quaternion.LookRotation(targetForward, Vector3.up);
                float angle = Quaternion.Angle(m_Transform.rotation, targetRotation);

                // Smooth rotate — looks natural
                float rotationSpeed = isRunning ? 10f : 5f;

                if (angle > 5f)
                {
                    // Snap instantly — avoids spinning lag
                    m_Transform.rotation = targetRotation;
                }
                else
                {
                    m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, targetRotation, deltaTime * rotationSpeed);
                }
            }
        }
         private class AnimationHandler
         {
             private readonly Animator m_Animator;
             private readonly string m_VerticalID;
             private readonly string m_StateID;

             private readonly float k_InputFlow = 4.5f;

             private float m_FlowState;
             private Vector2 m_FlowAxis;

            public AnimationHandler(Animator animator, string verticalID, string stateID)
            {
                m_Animator = animator;
                m_VerticalID = verticalID;
                m_StateID = stateID;
            }

            public void Animate(in Vector2 axis, float state, float deltaTime)
            {
                float movementMagnitude = axis.magnitude;

                m_Animator.SetFloat("Vert", movementMagnitude, 0.1f, deltaTime);

                m_FlowAxis = Vector2.ClampMagnitude(m_FlowAxis + k_InputFlow * deltaTime * (axis - m_FlowAxis).normalized, 1f);
                m_FlowState = Mathf.Clamp01(m_FlowState + k_InputFlow * deltaTime * Mathf.Sign(state - m_FlowState));
            }

            public void AnimateIK(in Vector3 target, LookWeight lookWeight)
            {
                m_Animator.SetLookAtPosition(target);
                m_Animator.SetLookAtWeight(lookWeight.weight, lookWeight.body, lookWeight.head, lookWeight.eyes);
            }
        }
         #endregion
    }
}