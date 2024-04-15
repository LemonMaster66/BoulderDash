using UnityEngine;
using Cinemachine;
using QFSW.QC;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerManager : MonoBehaviour
{
    public CinemachineFreeLook freeLook;
    public Transform OriginalTarget;
    public Transform target;

    private RelayManager relayManager;

    [Header("Spawn Loops")]
    public bool AssignSelfLoop = false;
    public bool RenamePlayersLoop = false;

    void Update()
    {
        TryAssignSelf();
        TryRenameAllPlayers();
    }

    //**************************************************************
    //Camera stuff

    [Command]
    public void AssignCAC(string player)
    {
        target = null;
        GameObject playerObject = GameObject.Find(player);
        foreach (Transform child in playerObject.transform)
        {
            if (child.gameObject.activeSelf && child.name.StartsWith("#"))
            {
                target = child;
                break;
            }
        }

        if (target != null)
        {
            freeLook.Follow = target.transform;
            freeLook.LookAt = target.transform;
        }
    }

    [Command]
    public void AssignInput(string player)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerObject in players)
        {
            if (playerObject.name == player)
            {
                foreach (Transform child in playerObject.transform)
                {
                    if (child.gameObject.activeSelf && child.name.StartsWith("#"))
                    {
                        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
                        foreach (PlayerInput input in playerInputs)
                        {
                            input.enabled = false;
                        }

                        PlayerInput playerInput = child.GetComponent<PlayerInput>();
                        if (playerInput != null)
                        {
                            playerInput.enabled = true;
                        }

                        break;
                    }
                }

                break;
            }
        }
    }

    [Command]
    public void AssignPlayer(string player)
    {
        AssignCAC(player);
        AssignInput(player);
    }

    [Command]
    public void AssignSelf()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); //Find all players
        foreach (GameObject playerObject in players) //For every Player
        {
            NetworkObject networkComponent = playerObject.GetComponent<NetworkObject>(); //Get the NetworkObject Component
            if(networkComponent.IsOwner) //If you are the owner of your game
            {
                foreach (Transform child in playerObject.transform) //for every child in the Owner Player
                {
                    if (child.gameObject.activeSelf && child.name.StartsWith("#"))
                    {
                        target = child;

                        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>(); //finds every PlayerInput Component
                        foreach (PlayerInput input in playerInputs)
                        {
                            input.enabled = false; //disables every PlayerInput Component
                        }

                        PlayerInput playerInput = child.GetComponent<PlayerInput>(); //finds the Player's PlayerInput Component
                        if (playerInput != null)
                        {
                            playerInput.enabled = true; //enables the Player's PlayerInput Component
                        }

                        break;
                    }
                }

                //Assigns the Cinemachine Follow and LookAt to the Target
                if (target != null) 
                {
                    freeLook.Follow = target.transform;
                    freeLook.LookAt = target.transform;
                }
            }
        }
    }



    public void TryAssignSelf()
    {
        if(AssignSelfLoop == true)
        {
            if(target == null)
            {
                //Debug.Log("Trying to AssignSelf()...");
                AssignSelf();
            }
            else
            {
                //Debug.Log("Assigned Self Completed!");
                AssignSelfLoop = false;
            }
        }
    }



    //**************************************************************
    // Not Camera stuff


    [Command]
    public void TryRenameAllPlayers()
    {
        if(RenamePlayersLoop == true)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); //Finds all the Player Objects
            foreach(GameObject player in players) //for every player object
            {
                PlayerInfo playerInfo = player.GetComponentInChildren<PlayerInfo>(); //get the PlayerInfo component

                playerInfo.isObjectRenamed = false;
                if(playerInfo.isObjectRenamed == false)
                {
                    if(player.name != "Player" + playerInfo.PlayerIndex) //if its not named correctly
                    {
                        //Debug.Log("Trying to Rename: " + player.name + " to: Player" + playerInfo.PlayerIndex); //try to rename it
                        player.name = "Player" + playerInfo.PlayerIndex; //example: Player1
                        playerInfo.isObjectRenamed = true;
                    }
                    else //if it is named correctly
                    {
                        //Debug.Log("Renamed to: Player" + playerInfo.PlayerIndex); // end the loop
                        playerInfo.isObjectRenamed = true;
                        RenamePlayersLoop = false;
                    }
                }
            }
        }
    }
}