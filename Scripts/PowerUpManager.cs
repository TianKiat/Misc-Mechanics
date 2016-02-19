//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// Name: PowerUpManager
// Description:
// Author: Ng Tian Kiat
// Date: Jan 28 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PowerUpManager : NetworkBehaviour
{
    [HeaderAttribute("References")]
    public Transform[] m_SpawnPoints;//spawnpoints for the power ups
    public GameObject[] m_Powerups;//the type of powerups
    [HeaderAttribute("Settings")]
    public float m_RespawnDelay;//how long it takes to respawn the powerups
    private WaitForSeconds m_RespawnWait;
    void Start()
    {
        m_RespawnWait = new WaitForSeconds(m_RespawnDelay);
        StartCoroutine(RefreshPowerups());
    }
    //coroutine to refresh the powerups
    private IEnumerator RefreshPowerups()
    {
        yield return new WaitForSeconds(10f);
        while (true)
        {
            for (int i = 0; i < m_SpawnPoints.Length; i++)
            {
                //if one of the spawn points is empty
                if (m_SpawnPoints[i].childCount < 1)
                {
                    CmdSpawnPowerUp(m_SpawnPoints[i].gameObject);//spawn a powerup and set it as a child of this transform
                }
                yield return new WaitForSeconds(.5f);
            }
            yield return m_RespawnWait;
        }
    }
    [Command]
    //method to spawn a random powerup
    private void CmdSpawnPowerUp(GameObject _Parent)
    {
        int _index = Mathf.RoundToInt(Random.Range(0,m_Powerups.Length));
        GameObject powerup = Instantiate(m_Powerups[_index], _Parent.transform.position, Quaternion.identity) as GameObject;
        powerup.transform.SetParent(_Parent.transform);
        // disable for debugging
        NetworkServer.Spawn(powerup);//spawn it in the server
    }
}
