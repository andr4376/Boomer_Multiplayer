#if UNITY_EDITOR
using ParrelSync;
#endif
using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;


public class ServerHelper : MonoBehaviour
{
    //https://www.youtube.com/watch?v=fdkvm21Y0xE&ab_channel=Tarodev
    private UnityTransport _transport;
    public TMP_InputField JoinCodeInputField;

    private string _playerId;
    private const string JoinCodeKey = "joinCode";

    private Lobby _connectedLobby;

    private const int maxPlayers = 10;

    private async void Awake()
    {
        _transport = GetComponent<UnityTransport>();

#if UNITY_EDITOR
        //in unity editor, add latency
        _transport.SetDebugSimulatorParameters(
            80, //ms ping
             5,// jitter
              1 //packet drop rate
            );
#endif

        await Authenticate();
    }

    public async void CreateOrJoinLobby()
    {
        await Authenticate();

        _connectedLobby = await QuickJoinLobby() ?? await CreateLobby();
        //disable btn?
    }

    private async Task<Lobby> CreateLobby()
    {
        try
        {
            var a = await RelayService.Instance.CreateAllocationAsync(maxPlayers);

            var joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

            var options = new CreateLobbyOptions
            {
                Data = new System.Collections.Generic.Dictionary<string, DataObject>
                {
                    {
                        JoinCodeKey,
                        new DataObject(DataObject.VisibilityOptions.Public, joinCode)
                    }
                }
            };

            var lobby = await Lobbies.Instance.CreateLobbyAsync("The lobby", maxPlayers, options);

            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));

            _transport.SetHostRelayData(a.RelayServer.IpV4,
                                        (ushort)a.RelayServer.Port,
                                        a.AllocationIdBytes,
                                        a.Key,
                                        a.ConnectionData);

            NetworkManager.Singleton.StartHost();

            return lobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("could not create lobby");
            Debug.LogError(e);
            return null;
        }

    }

    private static IEnumerator HeartbeatLobbyCoroutine(string id, int v)
    {
        var delay = new WaitForSecondsRealtime(v);

        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(id);

            yield return delay;
        }
    }

    private async Task Authenticate()
    {
        var options = new InitializationOptions();
#if UNITY_EDITOR
        options.SetProfile(ClonesManager.IsClone() ? ClonesManager.GetArgument() : "Primary");
#endif
        await UnityServices.InitializeAsync(options);

        if (AuthenticationService.Instance.IsSignedIn == false)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        _playerId = AuthenticationService.Instance.PlayerId;
    }


    private async Task<Lobby> QuickJoinLobby()
    {
        try
        {
            var lobby = await Lobbies.Instance.QuickJoinLobbyAsync();

            Debug.Log("lobby id (not) found: " + lobby?.Id);
            Debug.Log("lobby JoinCodeKey: " + lobby.Data[JoinCodeKey]?.Value);

            var a = await RelayService.Instance.JoinAllocationAsync(lobby.Data[JoinCodeKey].Value);

            _transport.SetClientRelayData(a.RelayServer.IpV4,
                                                  (ushort)a.RelayServer.Port,
                                                  a.AllocationIdBytes,
                                                  a.Key,
                                                  a.ConnectionData,
                                                  a.HostConnectionData);

            NetworkManager.Singleton.StartClient();
            return lobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("could not join lobby");

            Debug.LogException(e);
            return null;
        }
    }

    private void OnDestroy()
    {
        try
        {
            StopAllCoroutines();
            if (_connectedLobby != null)
            {
                if (_connectedLobby.HostId == _playerId)
                    Lobbies.Instance.DeleteLobbyAsync(_connectedLobby.Id);
                else
                    Lobbies.Instance.RemovePlayerAsync(_connectedLobby.Id, _playerId);
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
