//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// Name: GameManager
// Description:
// Author: Ng Tian Kiat
// Date: Jan 28 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Network;
public class GameManager : NetworkBehaviour
{
    public static GameManager s_Singleton = null;//instance of GM allowing this to be access by othe scripts
    [Header("Game Settings")]
    public int m_RoundsToWin = 3;//number of rounds the player has to win
    public float m_RoundDuration = 120f;//duration in seconds
    public float m_StartDelay = 5f;//delay before the start of the game
    public float m_EndDelay = 5f;//delay before the end of the game
    public GameObject m_CameraRig;//main scene camera
    [Header("UI Reference")]
    public Text m_MessageText;//text for displaying between rounds
    public Text[] m_PlayerList;//text boxes for the player list
    public GameObject PlayerGlobalUI;
    public Text m_ScoreBoardText;
    public Text m_TimerText;
    [SyncVarAttribute(hook = "OnScoreChanged")]
    string m_ScoreString;
    [SyncVarAttribute(hook = "OnTimeChange")]
    string m_TimeString;
    //Dictionary<string, NetworkCar> playerRegistry = new Dictionary<string, NetworkCar>();//the player Registry of players
    //List<PlayerStatus> playerStatuses = new List<PlayerStatus>();//list of player statuses in game
    GameObject[] players;
    private int m_CurrentRoundNumber;
    public NetworkStartPosition[] m_StartPositions;//start positions in the map
    public GameObject m_Flag;//flag gameobject
    public Transform[] m_FlagSpawnPoints;//spawn points for flag
    private WaitForSeconds m_StartWait;//use to store start delay as waitforseconds
    private WaitForSeconds m_EndWait;//use to store start delay as waitforseconds
    private NetworkCar m_RoundWinner;//player that won the round
    private NetworkCar m_GameWinner;//winner of the game
    private bool m_GameIsFinished = false;
    private bool m_IsStart = true;//whether its the first spawn
    //awake method
    void Awake()
    {
        s_Singleton = this;
        DontDestroyOnLoad(gameObject);//do not destroy when loading scene
    }
    // [ServerCallbackAttribute]
    //start method
    void Start()
    {
        //convert start and end delay to waitforseconds
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);
        //set up camera
        if (m_CameraRig != null)
            m_CameraRig.SetActive(false);//turn the camera off
        else
            Debug.Log("There is no camera rig for the game");
        //set up message text
        m_MessageText.enabled = true;
        //start the gameloop
        StartCoroutine(GameLoop());
    }
    private string TimeStringBuilder(float _Time)
    {
        string minutes = Mathf.Floor(_Time / 60).ToString("00");
        string seconds = Mathf.Floor(_Time % 60).ToString("00");
        return minutes + ":" + seconds;
    }
    [ClientRpcAttribute]
    void RpcUpdateTime(string _time)
    {
        m_TimerText.text = _time;
    }
    private string ScoreStringBuilder()
    {
        string ScoreString = string.Empty;
        // foreach (PlayerStatus item in playerStatuses)
        // {
        //     ScoreString += item.name + ": " + Mathf.RoundToInt(item.score) + " pts\n";
        // }
        for (int i = 0; i < players.Length; i++)
        {
            ScoreString += players[i].name + ": " + Mathf.RoundToInt(players[i].GetComponent<PlayerStatus>().score) + "Pts\n";
        }
        return ScoreString;
    }
    [ClientRpcAttribute]
    void RpcUpdateScore(string _score)
    {
        m_ScoreBoardText.text = _score;
    }
    void OnScoreChanged(string _NewScore)
    {
        m_ScoreString = _NewScore;
        m_ScoreBoardText.text = m_ScoreString;
    }
    void OnTimeChange(string _NewTime)
    {
        m_TimeString = _NewTime;
        m_TimerText.text = m_TimeString;
    }
    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());//wait for round starting to finish
        yield return StartCoroutine(RoundPlaying());//wait for round to stop playing
        yield return StartCoroutine(roundEnding());//wait for round to stop ending
        if (m_GameWinner != null)
        {
            // If there is a game winner, wait for certain amount or all player confirmed to start a game again
            m_GameIsFinished = true;
            float leftWaitTime = 15.0f;
            int flooredWaitTime = 15;

            while (leftWaitTime > 0.0f)
            {
                yield return null;
                leftWaitTime -= Time.deltaTime;

                int newFlooredWaitTime = Mathf.FloorToInt(leftWaitTime);

                if (newFlooredWaitTime != flooredWaitTime)
                {
                    flooredWaitTime = newFlooredWaitTime;
                    string message = GetEndMessage();
                    RpcUpdateMessage(message);
                }
            }
            LobbyManager.s_Singleton.ServerReturnToLobby();//go to lobby
        }
        else
            StartCoroutine(GameLoop());
    }
    //reset cars 
    private IEnumerator RoundStarting()
    {
        yield return new WaitForSeconds(5f);
        RpcRoundStarting();
        //wait
        yield return m_StartWait;
    }
    [ClientRpcAttribute]
    void RpcRoundStarting()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        DisableControls();
        resetPlayers();
        m_CurrentRoundNumber++;
        m_MessageText.text = "ROUND " + m_CurrentRoundNumber;
        Debug.Log(players.Length);
    }
    private IEnumerator RoundPlaying()
    {
        WaitForSeconds _CountDown = new WaitForSeconds(1f);
        RpcUpdateMessage("3");
        yield return _CountDown;
        RpcUpdateMessage("2");
        yield return _CountDown;
        RpcUpdateMessage("1");
        yield return _CountDown;
        RpcUpdateMessage("Start!");
        yield return _CountDown;
        float currentTime = m_RoundDuration;
        RpcRoundPlaying();
        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1);
            currentTime--;
            //Debug.Log(playerRegistry.Count);

            // playerRegistry[key].CmdUpdateScores(_ScoreString, _TimeString);
            UpdateGlobalPlayerUI(currentTime);
        }
        yield return null;
    }
    [ClientRpcAttribute]
    void RpcRoundPlaying()
    {
        m_MessageText.text = string.Empty;
        //while the time is not over we can control the cars
        EnableControls();
        //spawn the flag
        if (m_CurrentRoundNumber == 1)
            CmdSpawnFlag(true);
        else
            CmdSpawnFlag(false);
    }
    private IEnumerator roundEnding()
    {
        m_MessageText.text = "ROUND OVER\n\nGOOD JOB :D";
        m_RoundWinner = null;//clear round winner of the previous round
        m_RoundWinner = GetRoundWinner();//get the winner of this round
        if (m_RoundWinner != null)
            m_RoundWinner.m_NumberOfRoundsWon++;//increment the number of rounds won for that player
        m_GameWinner = GetGameWinner();//get the game winner if there is one

        string _Message = GetEndMessage();
        RpcUpdateMessage(_Message);
        RpcRoundEnding();
        yield return m_EndWait;
    }
    [ClientRpcAttribute]
    private void RpcRoundEnding()
    {
        DisableControls();
    }

    [ClientRpcAttribute]
    void RpcUpdateMessage(string _Msg)
    {
        m_MessageText.text = _Msg;
    }
    [ServerCallbackAttribute]
    void UpdateGlobalPlayerUI(float _currentTime)
    {
        //building strings to update players GUI
        RpcUpdateScore(ScoreStringBuilder());
        RpcUpdateTime(TimeStringBuilder(_currentTime));
        m_TimerText.text = m_TimeString;
        // m_ScoreBoardText.text = m_ScoreString;
        // Debug.Log("Time: " + m_TimeString + " , " + "Scores: " + m_ScoreString);
    }
    private NetworkCar GetRoundWinner()
    {
        NetworkCar _RoundWinner = null;
        float _Score = 0;
        for (int i = 0; i < players.Length; i++)
        {
            //pick the playerstatus with the highest score
            if (players[i].GetComponent<PlayerStatus>().score > _Score)
            {
                _Score = players[i].GetComponent<PlayerStatus>().score;
                //pick the highest scoring network cars
                _RoundWinner = players[i].gameObject.GetComponent<NetworkCar>();
                return _RoundWinner;
            }
        }
        return null;
    }
    private NetworkCar GetGameWinner()
    {
        NetworkCar _GameWinner = null;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<NetworkCar>().m_NumberOfRoundsWon == m_RoundsToWin)
            {

                _GameWinner = players[i].GetComponent<NetworkCar>();
                return _GameWinner;
            }
        }
        return null;
    }
    private string GetEndMessage()
    {
        string _Message = "DRAW";
        //if there is a round winner
        if (m_RoundWinner != null)
            _Message = m_RoundWinner.m_PlayerName + " WINS THE ROUND";
        _Message += "\n\n\n\n";//leave some space
        //display the number of rounds won by the players
        for (int i = 0; i < players.Length; i++)
        {
            _Message += players[i].GetComponent<NetworkCar>().m_PlayerName + " : " + players[i].GetComponent<NetworkCar>().m_NumberOfRoundsWon + " WINS\n";
        }
        //if there is a game winner override the current message and display the game winner
        if (m_GameWinner != null)
            _Message = m_GameWinner.m_PlayerName + " WINS THE GAME!!!";
        return _Message;//return the winner
    }
    //method to reset all tanks
    private void resetPlayers()
    {
        int startPositionsIndex = Mathf.RoundToInt(UnityEngine.Random.Range(0, 1));//go between 0 and 1
        //loop thru all players in the registry and reset their position
        for (int i = 0; i < players.Length; i++)
        {
            //reset the player position
            players[i].GetComponent<NetworkCar>().Reset(m_StartPositions[startPositionsIndex].transform);
            Debug.Log(startPositionsIndex);
            //toggle start position between the 2
            if (startPositionsIndex == 1)
                startPositionsIndex = 0;
            else if (startPositionsIndex == 0)
                startPositionsIndex = 1;
            Debug.Log(startPositionsIndex);
            players[i].GetComponent<NetworkCar>().CmdDisableControls();//disable some stuff    
        }
    }
    //method to disable player controls
    private void DisableControls()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<NetworkCar>().CmdDisableControls();//disable some stuff    

        }
    }
    //method to disable player controls
    private void EnableControls()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<NetworkCar>().CmdEnableControls();//disable some stuff    

        }
    }
    // public void masterRegisterPlayer(GameObject _player)
    // {
    //     CmdMasterRegisterPlayer(_player);
    // }
    //     //method for GM to register player
    //     [CommandAttribute]
    //     void CmdMasterRegisterPlayer(GameObject _player)
    //     {
    //         playerRegistry.Add(_player.name, _player.GetComponent<NetworkCar>());
    //         playerStatuses.Add(_player.GetComponent<PlayerStatus>());
    //         Debug.Log(playerRegistry.Count + " " + playerStatuses.Count);
    //     }
    //     private void RegisterPlayers()
    //     {
    //         GameObject[] _players = GameObject.FindGameObjectsWithTag("Player");
    //         for (int i = 0; i < _players.Length; i++)
    //         {
    //             if (playerRegistry.ContainsKey(_players[i].name))
    //             {
    // 
    //                 playerRegistry.Add(_players[i].name, _players[i].GetComponent<NetworkCar>());
    //                 playerStatuses.Add(_players[i].gameObject.GetComponent<PlayerStatus>());
    //                 Debug.Log(playerRegistry.Count + " " + playerStatuses.Count);
    //             }
    //         }
    //     }
    //method to get the player with flag
    public Transform GetPlayerWithFlag()
    {
        Transform _playerWithFlag = null;
        foreach (var item in players)
        {
            if (item.GetComponent<PlayerStatus>().flagGet)
            {
                _playerWithFlag = item.gameObject.transform;
                Debug.Log(_playerWithFlag.name + " has the flag!");
                return _playerWithFlag;
            }
        }
        return null;
    }
    //method to spawn flag at a random position
    [Command]
    public void CmdSpawnFlag(bool _IsStart)
    {
        if (!_IsStart)
        {
            //random index in flag spawn point array
            int _index = Mathf.RoundToInt(UnityEngine.Random.Range(0, m_FlagSpawnPoints.Length - 1));
            Mathf.Clamp(_index, 0f, m_FlagSpawnPoints.Length - 1);
            Vector3 _SpawnPosition = new Vector3(m_FlagSpawnPoints[_index].transform.position.x, m_FlagSpawnPoints[_index].transform.position.y, m_FlagSpawnPoints[_index].transform.position.z);
            //spawn flag
            GameObject _flag = Instantiate(m_Flag, _SpawnPosition, Quaternion.identity) as GameObject;
            NetworkServer.Spawn(_flag);
            Debug.Log("Spawned a flag");
        }
        else
        {
            Vector3 _SpawnPosition = new Vector3(m_FlagSpawnPoints[0].transform.position.x, m_FlagSpawnPoints[0].transform.position.y, m_FlagSpawnPoints[0].transform.position.z);
            //spawn flag
            GameObject _flag = Instantiate(m_Flag, _SpawnPosition, Quaternion.identity) as GameObject;
            NetworkServer.Spawn(_flag);
            Debug.Log("Spawned a flag");
        }
    }
}
