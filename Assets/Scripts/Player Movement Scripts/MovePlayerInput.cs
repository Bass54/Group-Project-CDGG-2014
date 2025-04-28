using Mono.Cecil;
using UnityEngine;
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
        [SerializeField] private AudioSource Walk;
        [SerializeField] private AudioSource Run;

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
            m_Animator = GetComponent<Animator>();
            Walk = GetComponent<AudioSource>();
            Run = GetComponent<AudioSource>();
            Walk.loop = true;
            Run.loop = true;

        }

        private void Update()
        {
            GatherInput();
            SetInput();

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            m_Target.Set(horizontal, 0f, vertical);
            m_Target.Normalize();


            bool hasHorizaontalINput = !Mathf.Approximately(horizontal, 0f);
            bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
            bool isMoving = hasHorizaontalINput || hasVerticalInput;
            m_IsRun = Input.GetKey(KeyCode.LeftShift);

            float speed = new Vector3(horizontal, 0f, vertical).magnitude;
            m_Animator.SetFloat("Vert", speed);
            m_Animator.SetFloat("State", m_IsRun ? 1f : 0f);

<<<<<<< HEAD
            AudioClip desired = null;

            if (footstepSource == null || !footstepSource.enabled)
            {
                return;
            }

            if (isMoving) desired = m_IsRun ? Run : Walk;

            if (desired != null)
=======
            if (isMoving)
>>>>>>> parent of ef19dcf (Fixed the sound)
            {
                if (m_IsRun)
                {
                    if (!Run.isPlaying) Run.Play();
                    if (Walk.isPlaying) Walk.Stop();
                }
                else
                {
                    if (!Walk.isPlaying) Walk.Play();
                    if (Run.isPlaying) Run.Stop();
                }
            }
            else
            {
                if (Walk.isPlaying) Walk.Stop();
                if (Run.isPlaying) Run.Stop();
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