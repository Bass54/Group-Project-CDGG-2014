using Mono.Cecil;
using Unity.VisualScripting;
using UnityEngine;
//using static Unity.VisualScripting.Round<TInput, TOutput>;
using static UnityEngine.SpriteMask;

namespace Controller
{
    [RequireComponent(typeof(PinguinMover))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField]
        private string m_HorizontalAxis = "Horizontal";
        [SerializeField]
        private string m_VerticalAxis = "Vertical";
        [SerializeField]
        private string m_JumpButton = "Jump";
        [SerializeField]
        private KeyCode m_RunKey = KeyCode.LeftShift;

        [Header("Camera")]
        [SerializeField]
        private PlayerCamera m_Camera;
        [SerializeField]
        private string m_MouseX = "Mouse X";
        [SerializeField]
        private string m_MouseY = "Mouse Y";
        [SerializeField]
        private string m_MouseScroll = "Mouse ScrollWheel";

        [Header("Audio")]
        [SerializeField] private AudioSource footstepSource;
        [SerializeField] private AudioClip Walk;
        [SerializeField] private AudioClip Run;

        private PinguinMover m_Mover;
        Animator m_Animator;

        private Vector2 m_Axis;
        private bool m_IsRun;
        private bool m_IsJump;

        private Vector3 m_Target;
        private Vector2 m_MouseDelta;
        private float m_Scroll;

        private void Awake()
        {
            m_Mover = GetComponent<PinguinMover>();

            if (footstepSource == null)
            {
                footstepSource = gameObject.AddComponent<AudioSource>();
                footstepSource.loop = true;
            }
            if (Walk == null)
            {
                Walk = Resources.Load<AudioClip>("Audio/Footsteps/Walking");
            }
            if (Run == null)
            {
                Run = Resources.Load<AudioClip>("Audio/Footsteps/Running");
            }
            if (m_Animator == null)
            {
                m_Animator = GetComponent<Animator>();
            }
        }

        private void Update()
        {
            GatherInput();
            SetInput();

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            bool isMoving = (horizontal != 0f) || (vertical != 0f);
            m_IsRun = Input.GetKey(KeyCode.LeftShift);

            float speed = new Vector3(horizontal, 0f, vertical).magnitude;
            m_Animator.SetFloat("Vert", speed);
            m_Animator.SetFloat("State", m_IsRun ? 1f : 0f);

            AudioClip desired = null;

            if (footstepSource == null || !footstepSource.enabled)
            {
                return;
            }

            if (isMoving) desired = m_IsRun ? Run : Walk;

            if (desired != null)
            {
                if (footstepSource.clip != desired || !footstepSource.isPlaying)
                {
                    footstepSource.clip = desired;
                    footstepSource.loop = true;
                    footstepSource.Play();
                }
            }
            else
            {
                if (footstepSource.isPlaying)
                    footstepSource.Stop();
            }
        }

        public void GatherInput()
        {
            m_Axis = new Vector2(Input.GetAxis(m_HorizontalAxis), Input.GetAxis(m_VerticalAxis));
            m_IsRun = Input.GetKey(m_RunKey);
            m_IsJump = Input.GetButton(m_JumpButton);

            m_Target = (m_Camera == null) ? Vector3.zero : m_Camera.Target;
            m_MouseDelta = new Vector2(Input.GetAxis(m_MouseX), Input.GetAxis(m_MouseY));
            m_Scroll = Input.GetAxis(m_MouseScroll);
           
        }

        public void BindMover(PinguinMover mover)
        {
            m_Mover = mover;
        }

        public void SetInput()
        {
            if (m_Mover != null)
            {
                m_Mover.SetInput(m_Axis, m_Target, m_IsRun, m_IsJump);
            }

            if (m_Camera != null)
            {
                m_Camera.SetInput(m_MouseDelta, m_Scroll);
            }
        }
    }
}