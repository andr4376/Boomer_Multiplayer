using Unity.Netcode;
using UnityEngine;

public class FireBallShooter : NetworkBehaviour
{
    public PlayerInputListener PlayerInputListener;
    public GameObject FireballProjectilePrefab;

    public AudioSource AudioSource;
    public AudioClip AudioClip;

    private float _lastshootTs = 0;
    public float firerate = 2;

    private void Awake()
    {
        PlayerInputListener.onShootInputDown += TellServerToShootOnOtherClientsAndShootLocally;
    }

    private void TellServerToShootOnOtherClientsAndShootLocally()
    {
        if (Time.time >= _lastshootTs + firerate)
        {
            ShootFireBallServerRpc(transform.position, transform.rotation);
            SpawnFireBall(transform.position, transform.rotation);
            _lastshootTs = Time.time;
        }
    }
    [ServerRpc]
    private void ShootFireBallServerRpc(Vector3 position, Quaternion rotation)
    {
        ShootFireBallClientRpc(position, rotation);
    }

    [ClientRpc]
    private void ShootFireBallClientRpc(Vector3 position, Quaternion rotation)
    {
        if (!IsOwner)
        {
            SpawnFireBall(position, rotation);
        }
    }

    private void SpawnFireBall(Vector3 position, Quaternion rotation)
    {
        var fireball = Instantiate(FireballProjectilePrefab, position, rotation);
        var fireBallScript = fireball.GetComponent<FireBallScript>();
        fireBallScript.AudioSource = this.AudioSource;
        fireBallScript.Shooter = this;
        AudioSource.PlayOneShot(AudioClip);
    }
}
