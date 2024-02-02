using System.Threading.Tasks;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationManager : NetworkBehaviour
{
    public Animator animator;
    public CharacterController characterController;
    public PlayerInputListener playerInputListener;
    const string IDLE = "PlayerIdle";
    const string RUN = "knightWalk";
    const string JUMP = "knightJump";
    const string ATTACK = "knightAttack";

    private void Awake()
    {
        playerInputListener.onShootInputDown += PlayAttack;
    }
    private void OnDestroy()
    {

    }
    void PlayAttack()
    {
        PlayAnimation(ATTACK, 1);

        DealDamageAfterDelay();
    }

    async Task DealDamageAfterDelay()
    {
        await Task.Delay(340);
        ApplyDamageServerRpc();
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
                killable.TakeDamage(25);
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the ray in the Scene view
        Debug.DrawRay(transform.position + Vector3.up, transform.forward * 3, Color.red);
    }

    private void Update()
    {
        if (characterController.isGrounded == false)
        {
            //jump
            PlayAnimation(JUMP);
            return;
        }
        if (characterController.velocity.magnitude > 0.5)
        {
            PlayAnimation(RUN);
            return;
        }

        PlayAnimation(IDLE);
    }

    string currentAnimation = IDLE;
    void PlayAnimation(string animName, int layer = 0)
    {
        if (currentAnimation == animName)
            return;
        currentAnimation = animName;
        animator.CrossFade(animName, 0.05f, layer);



    }
}
