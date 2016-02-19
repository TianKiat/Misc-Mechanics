//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// Name: CarLobbyHook
// Description:
// Author: Ng Tian Kiat
// Date: Jan 28 2016
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using UnityStandardAssets.Network;
using UnityEngine.Networking;
public class CarLobbyHook : LobbyHook
{

    //override function to hook lobby player settings
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        //get reference to the car component
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        NetworkCar car = gamePlayer.GetComponent<NetworkCar>();
        //send the player car the lobby settings
        if (car != null)
        {
            car.m_PlayerName = lobby.playerName;
            car.m_PlayerColor = lobby.playerColor;
        }
    }
}
