using System;
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

            // Pass the input to the CreatureMover component (this will handle movement)
            SetInput(axis, target, isRun, false);  // false means not jumping here


            m_Movement.Move(Time.deltaTime, in m_Axis, m_IsRun, out var animAxis, out var isAir);
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
                // Convert movement input to world direction
                //Vector3 movementDirection = new Vector3(m_Axis.x, 0f, m_Axis.y).normalized;
                m_IsMoving = true;
            }
            else
            {
                m_IsMoving = false;
                m_Target = Vector3.zero;

                //transform.rotation = Quaternion.LookRotation(transform.forward);
                //transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }

            //m_Axis = axis;
            //m_Target = target;
            //m_IsRun = isRun;

            //if (m_Axis.sqrMagnitude < Mathf.Epsilon)
            //{
            //    m_Axis = Vector2.zero;
            //    m_IsMoving = false;
            //}
            //else
            //{
            //    m_Axis = Vector3.ClampMagnitude(m_Axis, 1f);
            //    m_IsMoving = true;
            //}
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.normal.y > m_Controller.stepOffset)
            {
                m_Movement.SetSurface(hit.normal);
            }
        }

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

            public void Move(float deltaTime, in Vector2 axis, bool isRun, out Vector2 animAxis, out bool isAir)
            {

                Vector3 inputDirection = new Vector3(axis.x, 0f, axis.y).normalized;

                if (inputDirection.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
                    m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, targetRotation, deltaTime * m_RotateSpeed);
                }

                float speed = isRun ? m_RunSpeed : m_WalkSpeed;
                Vector3 velocity = inputDirection * speed;

                CaculateGravity(deltaTime, out isAir);
                //Displace(deltaTime, in movement, isRun);

                m_Controller.Move(velocity * deltaTime);

                //GenAnimationAxis(in movement, out animAxis);
                animAxis = new Vector2(inputDirection.x, inputDirection.z);

            }

            private void ConvertMovement(in Vector2 axis, in Vector3 targetForward, out Vector3 movement)
            {
                Vector3 forward;
                Vector3 right;

                if (m_Space == Space.Self)
                {
                    forward = new Vector3(-targetForward.x, 0f, -targetForward.z).normalized;
                    right = Vector3.Cross(Vector3.up, forward).normalized;
                }
                else
                {
                    forward = Vector3.forward;
                    right = Vector3.right;
                }

                movement = axis.x * right + axis.y * forward;
                movement = Vector3.ProjectOnPlane(movement, m_Normal);
            }

            private void Displace(float deltaTime, in Vector3 movement, bool isRun)
            {
                Vector3 displacement = (isRun ? m_RunSpeed : m_WalkSpeed) * movement;
                displacement += m_GravityAcelleration;
                displacement *= deltaTime;

                m_Controller.Move(displacement);
            }

            private void CaculateGravity(float deltaTime, out bool isAir)
            {
                if (m_Controller.isGrounded)
                {
                    m_GravityAcelleration = Physics.gravity; // Reset gravity
                    isAir = false;
                }
                else
                {
                    isAir = true;
                    m_GravityAcelleration += Physics.gravity * deltaTime; // Apply gravity over time
                }
                //m_jumpTimer = Mathf.Max(m_jumpTimer - deltaTime, 0f);

                //if (m_Controller.isGrounded)
                //{
                //    m_GravityAcelleration = Physics.gravity;
                //    isAir = false;

                //    return;
                //}

                //isAir = true;

                //m_GravityAcelleration += Physics.gravity * deltaTime;
                //return;
            }

            private void GenAnimationAxis(in Vector3 movement, out Vector2 animAxis)
            {
                if (m_Space == Space.Self)
                {
                    animAxis = new Vector2(Vector3.Dot(movement, m_Transform.right), Vector3.Dot(movement, m_Transform.forward));
                }
                else
                {
                    animAxis = new Vector2(Vector3.Dot(movement, Vector3.right), Vector3.Dot(movement, Vector3.forward));
                }
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

                //m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, targetRotation, deltaTime * 10f);

                //if (angle > 90f) // Adjust threshold if needed
                //{
                //    m_Transform.rotation = targetRotation;
                //}
                //else
                //{
                //    m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, targetRotation, deltaTime * 10f);
                //}

                if (isRunning && angle > 120f)
                {
                    // Snap instantly — avoids spinning lag
                    m_Transform.rotation = targetRotation;
                }
                else
                {
                    // Smooth rotate — looks natural
                    float rotationSpeed = isRunning ? 15f : 10f;
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