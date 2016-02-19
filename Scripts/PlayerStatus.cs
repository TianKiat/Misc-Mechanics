//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// Name: PlayerStatus
// Description:
// 
// Author: Liao Keyi and Tian Kiat
// Date: //
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

using UnityEngine;
using UnityEngine.Networking;

public class PlayerStatus : NetworkBehaviour
{

    [SerializeField]
    [SyncVarAttribute(hook = "OnScoreChange")]
    public float score = 0;

    [SerializeField]
    [SyncVarAttribute(hook = "OnFlagGetChange")]
    public bool flagGet = false;
    public GameObject flagEffect;
    private GameObject instanceOfEffect;
    private void Update()
    {
        if (flagGet == true)
        {
            CmdScoreCount();
        }
    }
    public void CaptureFlag()
    {
        CmdLoseflag();
    }
     public void LoseFlag()
    {
        CmdCaptureFlag();
    }
    [CommandAttribute]
    public void CmdLoseflag()
    {
        Debug.Log("Flag captured by " + gameObject.name);
        if (!instanceOfEffect)
        {
            instanceOfEffect = Instantiate(flagEffect, transform.position, Quaternion.identity) as GameObject;
            instanceOfEffect.transform.parent = transform;
            NetworkServer.Spawn(instanceOfEffect);
        }
        flagGet = true;
    }
    [CommandAttribute]
    public void CmdCaptureFlag()
    {
        Debug.Log(gameObject.name + " lost the flag");
        Destroy(instanceOfEffect);
        flagGet = false;
    }
    [Command]
    private void CmdScoreCount()
    {
        score += 1 * Time.deltaTime;
    }
    [Command]
    public void CmdAddScore(string _PlayerID, float _ScoreToAdd)
    {
        score += _ScoreToAdd;
        // Debug.Log(_PlayerID + " +" + _ScoreToAdd + " and now has" + score);
    }
    void OnScoreChange(float _newScore)
    {
        score = _newScore;
    }
    void OnFlagGetChange(bool _newFlagGet)
    {
        flagGet = _newFlagGet;
    }

}
