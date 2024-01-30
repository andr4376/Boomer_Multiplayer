using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationManager : MonoBehaviour
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
        animator.CrossFade(animName, 0.1f, layer);
    }
}
