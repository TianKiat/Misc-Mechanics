using UnityEngine;

public class Teleport : MonoBehaviour {
    public Vector3 m_TartgetPositionOffset;
    
    //Button to use teleportation
    public KeyCode m_PrimaryActivationKey;
    public KeyCode m_SecondaryActivationKey;
    
    // Layer mask for raycast
    public LayerMask m_RaycastMask;
    
    // Slow motion settings
    public bool UseSlowMotion;
    [TooltipAttribute("How fast the time will slow down")]
    public float m_TimeSlowRate;
    [TooltipAttribute("How much to slow down the time scale")]
    public float m_SlowMotionTimeScale;
    
    //Object that shows the destination of the teleport
    public GameObject GhostPointer;
    private Transform m_GhostPointerPos;
    
    private bool m_IsInSlowMo= false;
    private bool m_IsInTeleportSequence = false;
    
    // The teleport target position
    private Vector3 m_Targetposition;
    public float m_TeleportRange;
    
    private Vector3 m_ScreenCenter;
    private RaycastHit hit;
    private float CharacterHeight;
    void Start()
    {
        m_ScreenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        CharacterHeight = GetComponent<CharacterController>().radius;
        
        m_GhostPointerPos = GhostPointer.GetComponent<Transform>();
    }
    void Update()
    {
        //If user holds activation button start teleport sequence
        if (Input.GetKey(m_PrimaryActivationKey) || Input.GetKey(m_SecondaryActivationKey))
        {
            TeleportSequence();
        }
        else if (Input.GetKeyUp(m_PrimaryActivationKey) || Input.GetKeyUp(m_SecondaryActivationKey))
        {
            transform.position = m_Targetposition;
            Time.timeScale = 1;
            // Time.fixedDeltaTime = 1;
            m_IsInTeleportSequence = false;
            m_IsInSlowMo = false;
        }
    }

    private void TeleportSequence()
    {
        m_IsInTeleportSequence = true;
        Ray ray = Camera.main.ScreenPointToRay(m_ScreenCenter);
        if (Physics.SphereCast(ray.origin, CharacterHeight,ray.direction, out hit, m_TeleportRange, m_RaycastMask))
        {
            m_Targetposition = hit.point - m_TartgetPositionOffset;
            m_GhostPointerPos.position = m_Targetposition;
        }
        //SLow motion stuff
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
    void OnDestroy()
    {
        m_GhostPointerPos = null;
        Destroy(GhostPointer);//destroy if you want
        GhostPointer = null;
    }
}
