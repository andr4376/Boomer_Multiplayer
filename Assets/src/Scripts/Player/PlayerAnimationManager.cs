using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationManager : MonoBehaviour
{
    public Animator animator;
    public CharacterController characterController;

    const string IDLE = "PlayerIdle";
    const string RUN = "PlayerRun";
    const string JUMP = "PlayerJump";

    private void FixedUpdate()
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
    void PlayAnimation(string animName)
    {

        if (currentAnimation == animName)
            return;
        currentAnimation = animName;
        Debug.Log(currentAnimation);
        animator.CrossFade(animName, 0.1f, 0);
    }
}
