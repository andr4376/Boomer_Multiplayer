using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PlayerInputListener))]
public class PlayerAttackController : NetworkBehaviour
{
    public PlayerAnimationManager animationManager;
    PlayerInputListener playerInputListener;

    public AudioSource playerAudioSource;

    public float AttackCooldown = 1;
    float attackTs = 0;
    private void Awake()
    {
        this.playerInputListener = GetComponent<PlayerInputListener>();

        playerInputListener.onShootInputDown += Attack; 
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        playerInputListener.onShootInputDown -= Attack;
    }

    private void Attack()
    {
        if (Time.time < attackTs + AttackCooldown)
            return; 

        attackTs = Time.time;

        animationManager.PlayAttack();
        Invoke(nameof(ApplyDamageServerRpc), 0.35f);

        playerAudioSource.Play();
    }

    private void OnDrawGizmos()
    {
        // Draw the ray in the Scene view
        Debug.DrawRay(transform.position + Vector3.up, transform.forward * 3, Color.red);
    }

    [ServerRpc]
    public void ApplyDamageServerRpc()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 3))
        {
            var killable = hit.transform.GetComponent<KillableScript>();

            if (killable is not null)
                killable.health.Value -= 25;
        }
    }
}
