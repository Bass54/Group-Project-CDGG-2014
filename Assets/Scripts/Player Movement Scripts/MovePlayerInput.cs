using UnityEngine;

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

        private PinguinMover m_Mover;
        AudioSource m_AudioSource;
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
            m_AudioSource = GetComponent<AudioSource>();
            m_Animator = GetComponent<Animator>();
        }

        private void Update()
        {
            GatherInput();
            SetInput();
        }

        //private void FixedUpdate()
        //{
        //    float horizontal = Input.GetAxis("Horizontal");
        //    float vertical = Input.GetAxis("Vertical");

        //    m_Target.Set(horizontal, 0f, vertical);
        //    m_Target.Normalize();


        //    bool hasHorizaontalINput = !Mathf.Approximately(horizontal, 0f);
        //    bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        //    bool isWalking = hasHorizaontalINput || hasVerticalInput;
        //    m_Animator.SetBool("IsWalking", isWalking);
        //    m_Animator.SetBool("IsRunning", m_IsRun);
        //    m_Animator.SetBool("IsJumping", m_IsJump);

        //    if (isWalking)
        //    {
        //        if (!m_AudioSource.isPlaying)
        //        {
        //            m_AudioSource.Play();
        //        }
        //    }
        //    else if (m_IsRun)
        //    {
        //        if (!m_AudioSource.isPlaying)
        //        {
        //            m_AudioSource.Play();
        //        }
        //    }
        //    else if(m_IsJump)
        //    {
        //        if (!m_AudioSource.isPlaying)
        //        {
        //            m_AudioSource.Play();
        //        }
        //    }
        //    else
        //    {
        //        m_AudioSource.Stop();
        //    }

        //    if (m_IsJump)
        //    {
        //        m_Animator.SetTrigger("Jump");
        //    }
        //    if (m_IsRun)
        //    {
        //        m_Animator.SetTrigger("Run");
        //    }
        //}

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