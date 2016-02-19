//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// Name: Spring
// Description:
// Author: Ng Tian Kiat
// Date: Jan 28 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public class Spring : NetworkBehaviour
{
    public float m_Score = 10f;//score player gets when successfully hit target
    public float m_SpringForce = 10f;
    private Transform owner;
    private float m_AutoDetonateTime = 5f;
    public GameObject springEffect;
    //initialize
    void Start()
    {
        StartCoroutine(AutoDestroy());
    }

    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(m_AutoDetonateTime);
        SelfDestruct();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.gameObject.CompareTag("Player"))
        {
            GetComponent<AudioSource>().Play();
            GameObject _effect = Instantiate(springEffect, transform.position, transform.rotation) as GameObject;
            NetworkServer.Spawn(_effect);
            Destroy(_effect, 1f);
            other.transform.root.gameObject.GetComponent<Rigidbody>().AddExplosionForce(m_SpringForce, Vector3.zero, 3f, 2f, ForceMode.Impulse);
            SelfDestruct();
            if (other.transform.root.transform != owner)
                owner.gameObject.GetComponent<PlayerStatus>().CmdAddScore(owner.name, m_Score);
        }
    }
    private void SelfDestruct()
    {
        Destroy(gameObject, .5f);
    }
    public void SetupSpring(Transform _Owner)
    {
        owner = _Owner;
    }
}
