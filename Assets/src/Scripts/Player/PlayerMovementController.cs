using System;
using UnityEngine;
using UnityEngine.Windows;

[RequireComponent(typeof(PlayerInputListener))]
[RequireComponent(typeof(CharacterController))]
public sealed partial class PlayerMovementController : MonoBehaviour
{
    private const int DEFAULT_SPEED_FACTOR = 1;
    private PlayerInputListener playerInputListeneer;
    private CharacterController characterController;

    [SerializeField]
    Vector3 movement;

    [SerializeField]
    private PLAYER_STANCE _playerStance;

    public PLAYER_STANCE PlayerStance
    {
        get => _playerStance;
        set
        {
            if (_playerStance == value)
                return;

            _playerStance = value;
            OnStanceChanged(value);
        }
    }

    public float Speed = 12f;
    public float CrouchSpeedFactor = 0.5f;
    public float SprintSpeedFactor = 2;

    private float currentSpeedFactor = DEFAULT_SPEED_FACTOR;

    private void Awake()
    {
        this.playerInputListeneer = GetComponent<PlayerInputListener>();
        this.characterController = GetComponent<CharacterController>();

        this.playerInputListeneer.OnMoveInput += UpdateMovement;
        this.playerInputListeneer.OnSprint += OnSprint;
        this.playerInputListeneer.OnCrouch += OnCrouch;
        this.playerInputListeneer.OnJump += OnJump;
    }

    void Update()
    {
        Applygravity();
        //Apply speedfactor to movement only (not jump)
        Vector3 mov = new Vector3(movement.x, 0, movement.z) * (Speed * currentSpeedFactor);
        mov.y = movement.y;
        characterController.Move(mov * Time.deltaTime);

        movement.x = 0;
        movement.z = 0;
    }

    void UpdateMovement(Vector2 input)
    {
        movement +=
            transform.right * input.x //left right
            + transform.forward * input.y  /*forward back*/;
    }

    void OnSprint()
    {
        if (PlayerStance == PLAYER_STANCE.SPRINT)
        {
            PlayerStance = PLAYER_STANCE.DEFAULT;
            return;
        }
        PlayerStance = PLAYER_STANCE.SPRINT;
    }

    void OnCrouch()
    {
        if (PlayerStance == PLAYER_STANCE.CROUCH)
        {
            PlayerStance = PLAYER_STANCE.DEFAULT;
            return;
        }
        PlayerStance = PLAYER_STANCE.CROUCH;
    }


    void OnStanceChanged(PLAYER_STANCE newStance)
    {
        switch (newStance)
        {
            case PLAYER_STANCE.SPRINT:
                currentSpeedFactor = SprintSpeedFactor;
                break;
            case PLAYER_STANCE.CROUCH:
                currentSpeedFactor = CrouchSpeedFactor;
                break;
            default:
                currentSpeedFactor = DEFAULT_SPEED_FACTOR;
                break;
        }
    }

    private void OnDestroy()
    {
        this.playerInputListeneer.OnMoveInput -= UpdateMovement;
        this.playerInputListeneer.OnSprint -= OnSprint;
        this.playerInputListeneer.OnCrouch -= OnCrouch;
        this.playerInputListeneer.OnJump -= OnJump;
    }
}

//Jumping
public partial class PlayerMovementController
{
    public float JumpForce = 50;
    const float gravity = -18f;
    private void OnJump()
    {
        if (characterController.isGrounded)
        {
            movement.y = MathF.Sqrt(JumpForce * -2 * gravity);
        }
    }

    private void Applygravity()
    {
        if (characterController.isGrounded && movement.y < 0)
        {
            movement.y = -1.25f;
        }

        movement.y += gravity * Time.deltaTime;
    }
}

public enum PLAYER_STANCE
{
    DEFAULT,
    SPRINT,
    CROUCH
}
