using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class ServerHelper : MonoBehaviour
{
    //https://www.youtube.com/watch?v=fdkvm21Y0xE&ab_channel=Tarodev
    private UnityTransport _transport;
    public TMP_InputField JoinCodeInputField;
    private  async void Awake()
    {
        _transport = GetComponent<UnityTransport>();
        await Authenticate();
    }

    private async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void StartHost()
    {
        Allocation a = await RelayService.Instance.CreateAllocationAsync(2/*max players*/);
        string text = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

        Debug.Log(text);
        GUIUtility.systemCopyBuffer = text;

        _transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

        NetworkManager.Singleton.StartHost();
    }

    public async void StartClient()
    {
        string joinCode = JoinCodeInputField.text;
        if (string.IsNullOrWhiteSpace(joinCode))
            return; 

        JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(joinCode);

        _transport.SetClientRelayData(a.RelayServer.IpV4,
                                      (ushort)a.RelayServer.Port,
                                      a.AllocationIdBytes,
                                      a.Key,
                                      a.ConnectionData,
                                      a.HostConnectionData);

        NetworkManager.Singleton.StartClient();
    }
}
