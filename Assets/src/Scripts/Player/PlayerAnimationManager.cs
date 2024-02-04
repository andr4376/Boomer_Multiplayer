
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationManager : NetworkBehaviour
{
    private Animator animator;
    public CharacterController characterController;
    const string IDLE = "PlayerIdle";
    const string RUN = "knightWalk";
    const string JUMP = "knightJump";
    const string ATTACK = "knightAttack";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
  
    public void PlayAttack()
    {
        PlayAnimation(ATTACK, 1);
    }
   

    private void Update()
    {
        if (characterController.isGrounded == false)
        {
            //jump
            Debug.Log("jumping");
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
