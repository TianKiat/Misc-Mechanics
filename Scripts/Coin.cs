//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// Name: Coin
// Description:
// 
// Author:Ng Tian Kiat
// Date: //
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
public class Coin : MonoBehaviour
{
    [HeaderAttribute("Effects settings")]
    private Transform m_StartPosition;//start position of the coin
    public float m_Amplitude;
    public float m_Velocity;
    public float m_rotateSpeed = 4f;
    void Awake()
    {
        m_StartPosition = transform;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = m_StartPosition.position + Vector3.up * (Mathf.Sin(m_Velocity * Time.time) * (m_Amplitude * Time.deltaTime));
        transform.Rotate(Vector3.up * m_rotateSpeed * Time.deltaTime);
    }

}
