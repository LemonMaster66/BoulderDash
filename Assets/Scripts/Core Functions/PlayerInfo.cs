using UnityEngine;
using QFSW.QC;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;

public class PlayerInfo : NetworkBehaviour
{
    public int PlayerIndex;
    public string PlayerName;
    public ulong PlayerId;
    public bool isObjectRenamed;

    public RelayManager relayManager;
    //private PlayerManager playerManager;

    public GameObject ChildPlayerObject;

    [Header("Lobby Variables")]
    public bool Spawned = false;
    public bool Ready;

    void Awake()
    {
        ApplyPlayerIndex();
        ApplyPlayerId();
    }
    
    public void ApplyPlayerIndex()
    {
        relayManager = FindObjectOfType<RelayManager>();
        int players = 0;
        foreach(Player player in relayManager.JoinedLobby.Players)
        {
            players++;
        }
        PlayerIndex = players;
    }

    public void ApplyPlayerName()
    {
        relayManager = FindObjectOfType<RelayManager>();
        Player TargetPlayer = relayManager.GetPlayer();
        PlayerName = TargetPlayer.Data["PlayerName"].Value;
    }

    [Command]
    public void ApplyChildPlayer()
    {
        relayManager = FindObjectOfType<RelayManager>();
        GameObject PlayerObject = relayManager.GetPlayerGameobject();
        PlayerInfo playerInfo = PlayerObject.GetComponent<PlayerInfo>();
        playerInfo.ChildPlayerObject = PlayerObject.gameObject.transform.GetChild(0).gameObject;
        Debug.Log("Found ChildObject: " + ChildPlayerObject);
    }

    [Command]
    public void ApplyPlayerId()
    {
        GameObject PlayerObject = relayManager.GetPlayerGameobject();
        PlayerInfo playerInfo = PlayerObject.GetComponent<PlayerInfo>();
        NetworkManager networkManager = FindObjectOfType<NetworkManager>();
        playerInfo.PlayerId = networkManager.LocalClientId;
    }
}