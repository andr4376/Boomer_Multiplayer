using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationManager : MonoBehaviour
{
    public Animator animator;
    public PlayerInputListener playerInputListener;

    const string IDLE = "PlayerIdle";
    const string RUN = "PlayerRun";


    private void Start()
    {
        playerInputListener.OnMoveInput += PlayRunAnimation;
    }

    private void FixedUpdate()
    {
        if (Input.GetAxisRaw(Constants.HORISONTAL_MOVEMENT_KW) != 0)
            return;
        if (Input.GetAxisRaw(Constants.VERTICAL_MOVEMENT_KW) != 0)
            return;

        PlayAnimation(IDLE);
    }
    private void PlayRunAnimation(Vector2 input)
    {
        PlayAnimation(RUN);
    }

    private void OnDestroy()
    {
        playerInputListener.OnMoveInput -= PlayRunAnimation;
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
