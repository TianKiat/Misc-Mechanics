//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// Name: PlayerShoot
// Description:
// Author: Liao Keyi and Ng Tian Kiat
// Date: //
// Last Modified: by TK added networking functionality
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

using System;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerPowerUps : NetworkBehaviour
{
    [SyncVar]private int m_RocketCount = 0;
    public int m_MaxRocketCount = 3;
    [SyncVar]private int m_SpringCount = 0;
    public int m_MaxSpringCount = 3;
    public GameObject m_Missile;
    public GameObject m_Spring;
    public Transform m_Muzzle;
    private NetworkCar myNetworkCar;
    //special effects
    private AudioSource myAudioSource;
    public AudioClip springSound;
    public AudioClip rocketSound;
    void Awake()
    {
        myNetworkCar = GetComponent<NetworkCar>();
        myAudioSource = GetComponent<AudioSource>();
    }
    [ClientCallback]
    void Update()
    {
        if (!isLocalPlayer)
            return;
            
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.JoystickButton17))
        {
            FireRocket();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Joystick1Button19))
        {
            LaySpring();
        }
    }
    private void LaySpring()
    {
        if (m_SpringCount > 0)
        {
            CmdLaySpring();
        }
    }
    private void FireRocket()
    {
        if (m_RocketCount > 0)
        {
            CmdPlayerShot(gameObject.name);
        }
    }
    [Command]
    private void CmdLaySpring()
    {
        //lay the spring
        GameObject spring = Instantiate(m_Spring, Vector3.forward * -2, Quaternion.identity) as GameObject;
        spring.GetComponent<Spring>().SetupSpring(transform);
        m_SpringCount--;
        NetworkServer.Spawn(spring);
        myAudioSource.clip = springSound;
        myAudioSource.Play();
        //myNetworkCar.CmdUpdatePowerUps(m_RocketCount + "/" + m_MaxRocketCount, m_SpringCount + "/" + m_MaxSpringCount);
    }
    [Command]
    private void CmdPlayerShot(string _playerID)
    {
        //Shoot the rocket
        Transform _Target = GameManager.s_Singleton.GetPlayerWithFlag();
        if (_Target != null && m_RocketCount > 0 && _Target != gameObject.transform)
        {
            GameObject rocket = Instantiate(m_Missile, new Vector3(m_Muzzle.position.x, m_Muzzle.position.y, m_Muzzle.position.z), m_Muzzle.localRotation) as GameObject;
            rocket.GetComponent<HomingMissile>().SetupRocket(_Target, transform);
            NetworkServer.Spawn(rocket);
            myAudioSource.clip = rocketSound;
            myAudioSource.Play();
            m_RocketCount--;//use rocket
            //myNetworkCar.CmdUpdatePowerUps(m_RocketCount + "/" + m_MaxRocketCount, m_SpringCount + "/" + m_MaxSpringCount);
        }
    }
    //method to add rockets
    [Command]
    public void CmdAddRocket()
    {
        if (m_RocketCount < m_MaxRocketCount)
            m_RocketCount++;
        //myNetworkCar.CmdUpdatePowerUps(m_RocketCount + "/" + m_MaxRocketCount, m_SpringCount + "/" + m_MaxSpringCount);
    }
    //method to add springs
    [Command]
    public void CmdAddSpring()
    {
        if (m_SpringCount < m_MaxSpringCount)
            m_SpringCount++;
        //myNetworkCar.CmdUpdatePowerUps(m_RocketCount + "/" + m_MaxRocketCount, m_SpringCount + "/" + m_MaxSpringCount);
    }
}
