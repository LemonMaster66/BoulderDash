using UnityEngine;
using QFSW.QC;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using System.Collections;
using Unity.Netcode.Components;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Mathematics;

public class SpawnPointManager : NetworkBehaviour
{
    public PlayerManager playerManager;
    public RelayManager relayManager;

    public AudioSource HostJoinSFX;
    public AudioSource ClientJoinSFX;

    public ulong ServerId;
    public ulong ClientId;

    public Transform[] SpawnPoints;
    public Transform[] General_SpawnPoints;

    public bool TeleportLoop = true;

    private GameObject PlayerObject;
    private PlayerInfo playerInfo;

    public void DebugTeleport()
    {
        GameObject gameObject = relayManager.GetPlayerGameobject();

        PlayerInfo playerInfo = gameObject.GetComponent<PlayerInfo>();
        playerInfo.ApplyChildPlayer();
        playerInfo.ApplyPlayerId();

        GameObject ChildGameObject = playerInfo.ChildPlayerObject;
        Rigidbody rb = ChildGameObject.GetComponent<Rigidbody>();

        //nrb.(new Vector3(0, 10, 0), Quaternion.identity, new Vector3(100,100,100));
        rb.position += new Vector3(0,10,0);
    }

    public void SendToSpawn() //clients run this code
    {
        // GameObject gameObject = relayManager.GetPlayerGameobject();
        // PlayerInfo playerInfo = gameObject.GetComponent<PlayerInfo>();
        // playerInfo.ApplyPlayerId();

        // Player player = relayManager.GetPlayer();
        // string PlayerName = player.Data["PlayerName"].Value;
        // ServerTransformationServerRpc(playerInfo.PlayerId);
    }

    [ClientRpc]
    public void SendAllToSpawnClientRPC()
    {
        SendToSpawn();
    }

    public void SendAllToSpawn()
    {
        SendToSpawn();
        SendAllToSpawnClientRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerTransformationServerRpc(ulong TargetId)
    {
        // GameObject ServerObject = relayManager.GetPlayerGameobject();
        // NetworkObject ServerNetworkObject = ServerObject.GetComponent<NetworkObject>();
        // ServerId = ServerNetworkObject.OwnerClientId;

        // //Client Stuff    
        // GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        // foreach (GameObject playerObject in players)
        // {
        //     playerInfo = playerObject.GetComponent<PlayerInfo>();
        //     if (playerInfo.PlayerName == ClientPlayerName)
        //     {
        //         PlayerObject = playerObject;
        //         break;
        //     }
        // }
        // NetworkObject networkObject = PlayerObject.GetComponent<NetworkObject>();
        // GameObject ChildObject = playerInfo.ChildPlayerObject;
        // NetworkTransform networkTransform = ChildObject.GetComponent<NetworkTransform>();
        // Rigidbody rigidbody = ChildObject.GetComponent<Rigidbody>();
        // Vector3 RbVelocity = rigidbody.velocity;

        // networkObject.ChangeOwnership(ServerId);

        // rigidbody.velocity = RbVelocity;
        // rigidbody.interpolation = RigidbodyInterpolation.None;
        // networkTransform.Interpolate = false;

        // ClientJoinSFX.Play();
        // int TeleportTransform = playerInfo.PlayerIndex -1;
        // ChildObject.transform.position = SpawnPoints[TeleportTransform].transform.position;
        // ChildObject.transform.rotation = SpawnPoints[TeleportTransform].transform.rotation;

        // StartCoroutine(RestoreOwnershipAfterDelay(rigidbody, networkObject, networkTransform, TargetId));
        // Debug.Log("Moving " + PlayerObject + " to SpawnPoint" + TeleportTransform);


        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        PlayerInfo playerInfo = null;
        GameObject gameObject = null;
        foreach (GameObject playerObject in players)
        {
            playerInfo = playerObject.GetComponent<PlayerInfo>();
            if(playerInfo.PlayerId == TargetId)
            {
                gameObject = playerObject;
            }
        }
        
        GameObject ChildGameObject = playerInfo.ChildPlayerObject;
        NetworkTransform NetworkTransform = ChildGameObject.GetComponent<NetworkTransform>();
        NetworkTransform.Teleport(new Vector3(0,3,0), new quaternion(0,0,0,0), new Vector3(100,100,100));
    }

    private IEnumerator RestoreOwnershipAfterDelay(Rigidbody rigidbody, NetworkObject networkObject, NetworkTransform networkTransform, ulong originalOwnerID)
    {
        yield return new WaitForSeconds(0.08f); //0.08f
        networkObject.ChangeOwnership(originalOwnerID);
        rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        networkTransform.Interpolate = true;
    }

    //***************************************************************

    // [Command]
    // public void ReplacePlayer(GameObject player, GameObject newPlayer)
    // {

    //     GameObject playerObject = GameObject.Find(player.name);
    //     GameObject newPlayerObject = GameObject.Find(newPlayer.name);
    //     int playerIndex = playerObject.GetComponent<PlayerInfo>().PlayerIndex;
    //     int NewPlayerIndex = newPlayerObject.GetComponent<PlayerInfo>().PlayerIndex;

    //     GameObject playerChild = null;
    //     GameObject newPlayerChild = null;
    //     foreach (Transform child in playerObject.transform)
    //     {
    //         if (child.gameObject.activeSelf && child.name.StartsWith("#"))
    //         {
    //             playerChild = child.gameObject;
    //             break;
    //         }
    //     }
    //     foreach (Transform child in newPlayerObject.transform)
    //     {
    //         if (child.gameObject.activeSelf && child.name.StartsWith("#"))
    //         {
    //             newPlayerChild = child.gameObject;
    //             break;
    //         }
    //     } 

    //     NewPlayerIndex = playerIndex;
    //     newPlayerChild.transform.position = playerChild.transform.position;
    //     // newPlayerChild.transform.rotation = playerChild.transform.rotation;
    //     newPlayerObject.name = playerObject.name;
    //     Destroy(playerObject);
    // }

    // [Command]
    // public void JoinSpawnPoint(GameObject player)
    // {
    //     GameObject joinSpawnPoint = GameObject.Find("JoinSpawnPoint");
    //     GameObject newPlayerChild = null;
    //     foreach (Transform child in player.transform)
    //     {
    //         if (child.name.StartsWith("#"))
    //         {
    //             newPlayerChild = child.gameObject;
    //             break;
    //         }
    //     }
    //     newPlayerChild.transform.position = joinSpawnPoint.transform.position;
    //     newPlayerChild.transform.rotation = joinSpawnPoint.transform.rotation;
    // }

    // [Command]
    // public void SendPlayerBackToSpawn(string player)
    // {
    //     GameObject playerObject = GameObject.Find(player);
    //     GameObject playerChild = null;
    //     foreach (Transform child in playerObject.transform)
    //     {
    //         if (child.gameObject.activeSelf && child.name.StartsWith("#"))
    //         {
    //             playerChild = child.gameObject;
    //             break;
    //         }
    //     }

    //     GameObject spawnPoint = GameObject.Find(player + "SpawnPoint");

    //     foreach (Transform child in playerObject.transform)
    //     {
    //         if (child.gameObject.activeSelf && child.name.StartsWith("#"))
    //         {
    //             child.transform.position = spawnPoint.transform.position;
    //             child.transform.rotation = spawnPoint.transform.rotation;
    //         }
    //     }
    // }

    
    // [Command]
    // public void SendAllBackToSpawn()
    // {
    //     GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
    //     for (int i = 0; i < players.Length; i++)
    //     {
    //         GameObject player = players[i];
    //         SendPlayerBackToSpawn(player.name);
    //     }
    // }
 
    // public void SpawnPoint(GameObject playerObject)
    // {
    //     PlayerInfo playerInfo = playerObject.GetComponent<PlayerInfo>();
    //     if(playerInfo.ChildPlayerObject != null)
    //     {
    //         int playerIndex = playerInfo.PlayerIndex;
    //         int TargetSpawnPoint = playerIndex-1;

    //         //Send GameObject to Spawnpoint Position / Rotation
    //         playerInfo.ChildPlayerObject.transform.position = SpawnPoints[TargetSpawnPoint].transform.position;
    //         playerInfo.ChildPlayerObject.transform.rotation = SpawnPoints[TargetSpawnPoint].transform.rotation;

    //         //If it Has been Sent there...
    //         if(playerInfo.ChildPlayerObject.transform.position == SpawnPoints[TargetSpawnPoint].transform.position)
    //         {
    //             Debug.Log("Sent: " + playerInfo.PlayerName + " to: " + SpawnPoints[TargetSpawnPoint].name);
    //         }
    //     }
    // }

    //***************************************************************
    //Multiplayer Spawning
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        // Base General Spawning Code
        Debug.Log("-You Spawned-");

        if(relayManager.IsLobbyHost()) //Host Spawn Code
        {
            HostJoinSFX.Play();
        } 
        else //Client Spawn Code
        {
            ClientSpawnServerRPC();
        }

        //****************************
        //General Spawn Code
        GameObject PlayerObject = relayManager.GetPlayerGameobject();
        PlayerInfo playerInfo = PlayerObject.GetComponent<PlayerInfo>();
        playerInfo.ApplyPlayerIndex();
        playerInfo.ApplyPlayerName();
        

        playerManager.AssignSelfLoop = true;
        playerManager.RenamePlayersLoop = true;
        //SendToSpawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClientSpawnServerRPC(ServerRpcParams serverRpcParams = default) //Run on Host when a Client Spawns
    {
        SomeoneJoinedClientRPC();
        ClientJoinSFX.Play();

        playerManager.RenamePlayersLoop = true;
    }

    [ClientRpc]
    public void SomeoneJoinedClientRPC() //Run on Clients when a Client Spawns
    {
        ClientJoinSFX.Play();
        playerManager.RenamePlayersLoop = true;
    }
}