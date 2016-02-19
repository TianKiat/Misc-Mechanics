using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Vector3 m_TartgetPositionOffset;

    //Button to use teleportation
    public KeyCode m_PrimaryActivationKey;
    public KeyCode m_SecondaryActivationKey;
    public KeyCode m_CancelKey;
    // Layer mask for raycast
    public LayerMask m_RaycastMask;

    // Slow motion settings
    public bool UseSlowMotion;
    [TooltipAttribute("How fast the time will slow down")]
    public float m_TimeSlowRate;
    [TooltipAttribute("How much to slow down the time scale")]
    public float m_SlowMotionTimeScale;

    //Object that shows the destination of the teleport
    [TooltipAttribute("The object or effect you see to indicate your position")]
    public GameObject GhostPointer;
    public AudioClip m_TeleportSound;
    private AudioSource m_AudioSource;
    private Transform m_GhostPointerPos;

    private bool m_IsInSlowMo = false;
    private bool m_IsInTeleportSequence = false;

    // The teleport target position
    private Vector3 m_Targetposition;

    [TooltipAttribute("How far you can teleport in metres")]
    public float m_TeleportRange;

    private Vector3 m_ScreenCenter;
    private RaycastHit hit;
    private float CharacterHeight;
    void Start()
    {
        m_ScreenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        CharacterHeight = GetComponent<CharacterController>().radius;

        m_GhostPointerPos = GhostPointer.GetComponent<Transform>();

        m_AudioSource = GetComponent<AudioSource>();
        if (m_AudioSource == null)
        {
            Debug.Log("No audio source for teleport effect");
        }
    }
    void Update()
    {
        // toggle the ghost object on or off
        if (m_IsInTeleportSequence)
        {
            GhostPointer.SetActive(true);
        }
        else
        {
            GhostPointer.SetActive(false);
        }
        // If user holds activation button start teleport sequence
        if (Input.GetKey(m_PrimaryActivationKey) || Input.GetKey(m_SecondaryActivationKey))
        {
            m_IsInTeleportSequence = true;
            TeleportSequence();
        }
        else if (Input.GetKeyUp(m_PrimaryActivationKey) || Input.GetKeyUp(m_SecondaryActivationKey))
        {
            if (m_IsInTeleportSequence && Input.GetKey(m_CancelKey))
            {
                m_IsInTeleportSequence = false;
                Time.timeScale = 1;
                m_IsInSlowMo = false;
                return;
            }
            else
            {
                m_IsInTeleportSequence = false;
                // move target to destination
                transform.position = m_Targetposition;
                // play the teleport sound
                m_AudioSource.clip = m_TeleportSound;
                m_AudioSource.Play();
                Time.timeScale = 1;
                m_IsInSlowMo = false;
            }
        }
    }

    private void TeleportSequence()
    {
        if (m_IsInTeleportSequence)
        {

            Ray ray = Camera.main.ScreenPointToRay(m_ScreenCenter);
            if (Physics.SphereCast(ray.origin, CharacterHeight, ray.direction, out hit, m_TeleportRange, m_RaycastMask))
            {
                m_Targetposition = hit.point - m_TartgetPositionOffset;
                m_GhostPointerPos.position = m_Targetposition;
            }
            // Slow motion stuff
            if (UseSlowMotion)
            {
                if (!m_IsInSlowMo)
                {
                    m_IsInSlowMo = true;
                    Time.timeScale = Mathf.Lerp(1, m_SlowMotionTimeScale, m_TimeSlowRate);
                    // Time.fixedDeltaTime = Mathf.Lerp(1, m_SlowMotionTimeScale, m_TimeSlowRate);
                }
            }
        }
    }
    void OnDestroy()
    {
        m_GhostPointerPos = null;
        Destroy(GhostPointer);// destroy if you want
        GhostPointer = null;
        m_AudioSource = null;
    }
}
