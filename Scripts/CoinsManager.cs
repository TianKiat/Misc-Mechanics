//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// Name: CoinsManager
// Description:
// 
// Author:Ng Tian Kiat
// Date: //
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class CoinsManager : NetworkBehaviour
{
    [HeaderAttribute("References")]
    public Transform[] m_CoinSpawnPoints;//spawn points of the coins
    public GameObject m_CoinPrefab;//coin prefab
    [HeaderAttribute("Settings")]
    public float m_RefreshDelay = 5f;
    private WaitForSeconds m_RefreshWait;
    public float m_SpawnDelay = 1f;
    private WaitForSeconds m_SpawnWait;
    
    void Start()
    {
        m_RefreshWait = new WaitForSeconds(m_RefreshDelay);
        m_SpawnWait = new WaitForSeconds(m_SpawnDelay);
        StartCoroutine(RefreshCoins());
    }

    private IEnumerator RefreshCoins()
    {
        yield return new WaitForSeconds(10f);
         while (true)
        {
            for (int i = 0; i < m_CoinSpawnPoints.Length; i++)
            {
                //if one of the spawn points is empty
                if (m_CoinSpawnPoints[i].childCount < 1)
                {
                    CmdSpawnPowerUp(m_CoinSpawnPoints[i].gameObject);//spawn a coin and set it as a child of this transform
                }
                yield return m_SpawnWait;
            }
            yield return m_RefreshWait;
        }
    }
    [CommandAttribute]
    private void CmdSpawnPowerUp(GameObject _parent)
    {
        
        GameObject coin = Instantiate(m_CoinPrefab, _parent.transform.position, Quaternion.identity) as GameObject;
        coin.transform.parent = _parent.transform;//make the coin a child of _parent
        NetworkServer.Spawn(coin);
    }
}
