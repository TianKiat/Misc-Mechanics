//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// Name: PowerUp
// Description:
// Author: Ng Tian Kiat
// Date: Jan 28 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using UnityEngine.Networking;
public class PowerUp : NetworkBehaviour
{
    public enum PowerupType
    {
        rocket, spring, none
    }
    public PowerupType m_Type;
    public GameObject effect;
    void OnTriggerEnter(Collider collider)
    {
        if (m_Type == PowerupType.rocket && collider.gameObject.transform.root.GetComponent<PlayerPowerUps>())
            collider.gameObject.transform.root.GetComponent<PlayerPowerUps>().CmdAddRocket();
        if (m_Type == PowerupType.spring && collider.gameObject.transform.root.GetComponent<PlayerPowerUps>())
            collider.gameObject.transform.root.GetComponent<PlayerPowerUps>().CmdAddSpring();  
        GameObject _effect = Instantiate(effect, transform.position, transform.rotation) as GameObject;
        NetworkServer.Spawn(_effect);
        GetComponent<AudioSource>().Play(); 
        Destroy(_effect, .5f);
        //destroy this powerup
        Destroy(gameObject, .7f);
    }
 

}
