using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class KillableScript : NetworkBehaviour
{
    public const int MaxHealth = 100;
    // Define a NetworkVariable to keep track of health
    public NetworkVariable<int> health = new NetworkVariable<int>(MaxHealth, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Set initial health
    public void Awake()
    {
        if (IsServer)
        {
            health.Value = MaxHealth;
        }
        health.OnValueChanged += HealthChanged;
    }
    public override void OnDestroy()
    {
        health.OnValueChanged -= HealthChanged;
        base.OnDestroy();
    }

    private void FixedUpdate()
    {
        if(IsOwner)
        Debug.Log(health.Value);
    }

    private void HealthChanged(int previousValue, int newValue)
    {
        if (newValue <= 0)
            Die();
    }

    // Method to handle character death
    private void Die()
    {
        // Implement your character death logic here
        Debug.Log("Character died id:"+this.NetworkObjectId);

        if (IsOwner)
        {
            RespawnServerRpc();
        }
    }

    [ServerRpc]
    void RespawnServerRpc()
    {
        this.health.Value = MaxHealth;
        RespawnClientRpc();
    }

    [ClientRpc]
    void RespawnClientRpc()
    {
        //respawn use a game manager
        //    https://forum.unity.com/threads/respawning-problem-with-unity-netcode.1479501/
        this.transform.position = Vector3.up * 50;
    }

}