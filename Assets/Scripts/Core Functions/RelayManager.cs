using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using QFSW.QC;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using QFSW.QC.Suggestors.Tags;
using UnityEngine.SceneManagement;

public class RelayManager : MonoBehaviour
{
    public int MaxPlayers = 3 + 1;
    public string PlayerID = "";
    public string JoinCode = "";
    public TextMeshPro JoinCodeText;
    public TextMeshPro LobbyInfoText;
    public TextMeshPro PlayerListText;
    public TextMeshPro LobbiesText;

    public int LobbyCount;
    private bool Authenticated = false;

    public PlayerManager playerManager;
    public SpawnPointManager spawnPointManager;
    // public PlayerInfo playerinfo;


    private Lobby HostLobby;
    public Lobby JoinedLobby;
    private float HeartbeatTimer;
    private float LobbyUpdateTimer;
    private string playerName;


    [Command]
    public async void Authenticate()
    {
        PlayerID = "";
        JoinCode = "";

        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            PlayerID = AuthenticationService.Instance.PlayerId;
            Debug.Log("Signed In: " + PlayerID);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        //CreateRelay();
        playerName = "LemonMaster" + Random.Range(10,99);
        Debug.Log(playerName);

        Authenticated = true;
    }

    [Command]
    private async Task<string> CreateRelay()
    {
        try 
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayers);
            JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            return JoinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    [Command]
    private async void JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
    

    //*******************************************************************************************************************************************
    // Lobby Loop Stuff

    private void Update()
    {
        LobbyHeartbeat();
        HandleLobbyPollForUpdates();
    }

    private async void LobbyHeartbeat()
    {
        if(Authenticated == true)
        {
            HeartbeatTimer -= Time.deltaTime;
            if(HeartbeatTimer < 0f)
            {
                float HeartbeatTimerMax = 15;
                HeartbeatTimer = HeartbeatTimerMax;

                if(HostLobby != null)
                {
                    await LobbyService.Instance.SendHeartbeatPingAsync(HostLobby.Id);
                }

                ListLobbies();
            }
        }
    }

    private async void HandleLobbyPollForUpdates()
    {
        if(Authenticated == true)
        {
            LobbyUpdateTimer -= Time.deltaTime;
            if(LobbyUpdateTimer < 0f)
            {
                float LobbyUpdateTimerMax = 1.5f;
                LobbyUpdateTimer = LobbyUpdateTimerMax;

                if(JoinedLobby != null)
                {
                    Lobby lobby = await LobbyService.Instance.GetLobbyAsync(JoinedLobby.Id);
                    JoinedLobby = lobby;

                    JoinCode = lobby.Data["LobbyCode"].Value;;
                    LobbyInfoText.text = lobby.Data["Name"].Value + " " + lobby.Data["GameMode"].Value + " " + lobby.Data["Map"].Value;
                    DisplayPlayers(JoinedLobby);
                }
            }
        }
    }

    //*******************************************************************************************************************************************
    // Lobby Core Functions

    public bool IsLobbyHost()
    {
        return JoinedLobby != null && JoinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) },
            }
        };
    }

    [Command]
    public GameObject GetPlayerGameobject()
    {
        Player player = GetPlayer();
        string playername = player.Data["PlayerName"].Value;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerObject in players)
        {
            PlayerInfo playerInfo = playerObject.GetComponent<PlayerInfo>();
            playerInfo.ApplyPlayerName();

            if(playerInfo.PlayerName == playername)
            {
                return playerObject;
            }
            else
            {
                continue;
            }
        }
        Debug.LogWarning("Couldnt Find Player Object");
        return null;
    }


    public GameObject GetPlayerGameobjectById(ulong targetClientId)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerObject in players)
        {
            PlayerInfo playerInfo = playerObject.GetComponent<PlayerInfo>();
            if (playerInfo.PlayerId == targetClientId)
            {
                return playerObject;
            }
        }
        Debug.Log("No player with matching OwnerClientId found for targetClientId: " + targetClientId);
        return null;
    }
    

    public string GetScene()
    {
        string SceneName;
        SceneName = JoinedLobby.Data["Map"].Value;
        Debug.Log(SceneName);

        return SceneName;
    }

    private void DisplayPlayers(Lobby lobby)
    {
        PlayerListText.text = "";
        foreach(Player player in lobby.Players)
        {
            if(lobby.HostId == player.Id)
            {
                PlayerListText.text = PlayerListText.text + player.Id + " " + player.Data["PlayerName"].Value + " " + "[Host]" + "\n";
            }
            else
            {
                PlayerListText.text = PlayerListText.text + player.Id + " " + player.Data["PlayerName"].Value + "\n";
            }

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject playerObject in players)
            {
                PlayerInfo playerInfo = playerObject.GetComponent<PlayerInfo>();
                playerInfo.PlayerName = player.Data["PlayerName"].Value;
            }
        }
    }

    public async void LobbyStartCode()
    {
        string RelayCode = await CreateRelay();
        
        Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(JoinedLobby.Id, new UpdateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
            {
                { "StartCode", new DataObject(DataObject.VisibilityOptions.Member, RelayCode)}
            }
        });

        JoinedLobby = lobby;
    }

    [Command] [ClientRpc]
    public void StartMultiplayerGameClientRPC()
    {
        string Map = JoinedLobby.Data["Map"].Value;
        SceneManager.LoadScene(Map);
        spawnPointManager.SendAllToSpawn();
    }    

    //*******************************************************************************************************************************************
    // Lobby Create Join Leave Functions

    [Command]
    public async void CreateLobby(string lobbyName)
    {
        try 
        {
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, "DefaultMode")},
                    { "Map", new DataObject(DataObject.VisibilityOptions.Public, "DebugLand")},
                    { "Name" , new DataObject(DataObject.VisibilityOptions.Public, lobbyName)},
                    { "LobbyCode", new DataObject(DataObject.VisibilityOptions.Public, "0")},
                    { "StartCode", new DataObject(DataObject.VisibilityOptions.Member, "0")}
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MaxPlayers, createLobbyOptions);
            HostLobby = lobby;
            JoinedLobby = HostLobby;

            lobby = await Lobbies.Instance.UpdateLobbyAsync(JoinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "LobbyCode", new DataObject(DataObject.VisibilityOptions.Public, lobby.LobbyCode)}
                }
            });

            Debug.Log("Created Lobby: " + lobby.Data["Name"].Value + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);
            

            JoinCodeText.text = lobby.Data["LobbyCode"].Value;
            LobbyInfoText.text = lobby.Data["Name"].Value + " " + lobby.Data["GameMode"].Value + " " + lobby.Data["Map"].Value;

            LobbyStartCode();
            ListLobbies();

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    public async void QuickJoinLobby()
    {
        try 
        {
            QuickJoinLobbyOptions quickJoinLobbyOptions = new QuickJoinLobbyOptions
            {
                Player = GetPlayer()
            };
            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(quickJoinLobbyOptions);
            JoinedLobby = lobby;

            DisplayPlayers(JoinedLobby);
            JoinRelay(JoinedLobby.Data["StartCode"].Value);

            JoinCodeText.text = JoinedLobby.Data["LobbyCode"].Value;
            LobbyInfoText.text = lobby.Data["Name"].Value + " " + lobby.Data["GameMode"].Value + " " + lobby.Data["Map"].Value;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void JoinPrivateLobby(string lobbyCode)
    {
        try 
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            JoinedLobby = lobby;

            DisplayPlayers(lobby);
            JoinRelay(JoinedLobby.Data["StartCode"].Value);

            JoinCodeText.text = JoinedLobby.Data["LobbyCode"].Value;
            LobbyInfoText.text = lobby.Data["Name"].Value + " " + lobby.Data["GameMode"].Value + " " + lobby.Data["Map"].Value;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void LeaveLobby()
    {
        try 
        {
            await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, AuthenticationService.Instance.PlayerId);
            LobbyInfoText.text = "Nothing";
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void DeleteLobby()
    {
        try 
        {
            await LobbyService.Instance.DeleteLobbyAsync(JoinedLobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void ListLobbies()
    {
        try 
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 5,
                Filters = new List<QueryFilter> 
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(true, QueryOrder.FieldOptions.Created)
                } 
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            LobbiesText.text = "";

            foreach(Lobby lobby in queryResponse.Results)
            {
                int ActivePlayers = lobby.AvailableSlots - MaxPlayers;
                ActivePlayers = ActivePlayers *-1;

                LobbiesText.text = LobbiesText.text + lobby.Data["Name"].Value + " | Players: " + ActivePlayers + "/" + MaxPlayers + " | JoinCode: " + lobby.Data["LobbyCode"].Value + "\n";
            }

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    //*******************************************************************************************************************************************
    // Lobby Managing Functions

    [Command]
    private async void UpdateLobbyGameMode(string gameMode)
    {
        try 
        {
            JoinedLobby = await Lobbies.Instance.UpdateLobbyAsync(JoinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode)}
                }
            });
            //JoinedLobby = HostLobby;
            LobbyInfoText.text = JoinedLobby.Data["Name"].Value + " " + JoinedLobby.Data["GameMode"].Value + " " + JoinedLobby.Data["Map"].Value;

            DisplayPlayers(HostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void UpdateLobbyMap([SceneName] string Map)
    {
        try 
        {
            JoinedLobby = await Lobbies.Instance.UpdateLobbyAsync(JoinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {"Map", new DataObject(DataObject.VisibilityOptions.Public, Map)}
                }
            });
            //JoinedLobby = HostLobby;
            LobbyInfoText.text = JoinedLobby.Data["Name"].Value + " " + JoinedLobby.Data["GameMode"].Value + " " + JoinedLobby.Data["Map"].Value;

            DisplayPlayers(HostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void UpdateLobbyName(string Name)
    {
        try 
        {
            JoinedLobby = await Lobbies.Instance.UpdateLobbyAsync(JoinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {"Name", new DataObject(DataObject.VisibilityOptions.Public, Name)}
                }
            });
            //JoinedLobby = HostLobby;
            LobbyInfoText.text = JoinedLobby.Data["Name"].Value + " " + JoinedLobby.Data["GameMode"].Value + " " + JoinedLobby.Data["Map"].Value;

            DisplayPlayers(HostLobby);
            ListLobbies();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void UpdatePlayername(string newPlayerName)
    {
        try 
        {
            playerName = newPlayerName;
            await LobbyService.Instance.UpdatePlayerAsync(JoinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                }
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    public async void MigrateLobbyHost(string PlayerName)
    {
        try 
        {
            foreach(Player player in JoinedLobby.Players)
            {
                if(player.Data["PlayerName"].Value == PlayerName)
                {
                    HostLobby = await Lobbies.Instance.UpdateLobbyAsync(HostLobby.Id, new UpdateLobbyOptions
                    {
                        HostId = player.Id
                    });
                    JoinedLobby = HostLobby;

                    DisplayPlayers(HostLobby);
                }
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    public async void KickPlayer(string PlayerName)
    {
        try 
        {
            foreach(Player player in JoinedLobby.Players)
            {
                if(player.Data["PlayerName"].Value == PlayerName)
                {
                    await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, player.Id);
                    DisplayPlayers(HostLobby);
                }
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

}


//Extra Functions
// ListLobbies