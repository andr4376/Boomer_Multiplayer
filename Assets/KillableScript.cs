using System;
using Unity.Netcode;
using UnityEngine;

public class KillableScript : NetworkBehaviour
{
    public const int MaxHealth = 100;
    // Define a NetworkVariable to keep track of health
    private NetworkVariable<int> health = new NetworkVariable<int>(MaxHealth, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Set initial health
    public void Awake()
    {
        if (IsServer)
        {
            health.Value = MaxHealth;
        }
        health.OnValueChanged += HealthChanged;
    }

    private void HealthChanged(int previousValue, int newValue)
    {
        if (newValue <= 0)
            Die();
    }

    private void FixedUpdate()
    {
        if(IsOwner)
        Debug.Log(health.Value);
    }

    public void TakeDamage(int damage)
    {
        ApplyDamage(damage);
    }

    // Method to handle damage
    private void ApplyDamage(int damage)
    {
        health.Value -= damage;
    }

    // Method to handle character death
    private void Die()
    {
        // Implement your character death logic here
        Debug.Log("Character died id:"+this.NetworkObjectId);
    }
}