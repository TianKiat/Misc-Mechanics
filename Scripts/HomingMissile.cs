//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// Name: HomingMissile
// Description:
// Author: Ng Tian Kiat
// Date: Jan 28 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class HomingMissile : NetworkBehaviour
{
    //properties
    public float m_Score = 6f;//score player gets when successfully hit target
    private Transform target;
    private Transform owner;
    [SerializeField]
    private float speed = 5;//speed
    [SerializeField]
    private float autoDetonateTime = 15f;
    [SerializeField]
    private float homingSensitivity = .2f;//sensitivity
    [SerializeField]
    private GameObject explosion;//explosion prefab
    void Start()
    {
        StartCoroutine(AutoDetonate());//start the auto detonate sequence
    }
    void Update()
    {
        if (target != null)
        {
            Vector3 relativePos = target.position - transform.position;// calculate the relative position to target
            Quaternion rotation = Quaternion.LookRotation(relativePos);// rotate to target
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, homingSensitivity);//slerp to target
        }
        transform.Translate(0, 0, speed * Time.deltaTime, Space.Self);//move toward target
    }
    [CommandAttribute]
    private void CmdExplodeSelf()
    {
        GetComponent<AudioSource>().Play();
        GameObject _explosion = Instantiate(explosion,transform.position,transform.rotation) as GameObject;
        NetworkServer.Spawn(_explosion);
        Destroy(gameObject, .5f);
    }
    private IEnumerator AutoDetonate()
    {
        yield return new WaitForSeconds(autoDetonateTime);
        CmdExplodeSelf();
    }
    void OnTriggerEnter(Collider other)
    {
        CmdExplodeSelf();
        //if it hits the other player
        if (other.transform.root.gameObject == target.gameObject)
        {
            CmdExplodeSelf();
            Debug.Log(other.transform.root.gameObject.name + " got hit by " + owner.name);
            other.transform.root.gameObject.GetComponent<NetworkCar>().RpcStun();
            owner.gameObject.GetComponent<PlayerStatus>().CmdAddScore(owner.name, m_Score);
            owner.gameObject.GetComponent<PlayerStatus>().flagGet = false;
        }
    }
    public void SetupRocket(Transform _Target, Transform _Owner)
    {
        target = _Target;
        Debug.Log(_Target.name + " Is the target");
        owner = _Owner;
    }
}