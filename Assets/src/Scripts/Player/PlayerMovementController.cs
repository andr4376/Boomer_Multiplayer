using System;
using Unity.Burst.CompilerServices;
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

    Vector3 slopeNormal;
    private bool isOnSlopeCache = false;
    public bool IsOnSlope
    {
        get
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1))
            {
                // Get the angle of the slope
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

                // If the slope angle is greater than 45 degrees, apply sliding effect
                if (slopeAngle >= characterController.slopeLimit)
                {
                    slopeNormal = hit.normal;
                    isOnSlopeCache = true;
                    return isOnSlopeCache;
                }
            }
            isOnSlopeCache = false;
            return isOnSlopeCache;
        }
    }
    public float slopeFallingSpeed;
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
        ApplySlidingIfOnSlope();

        Vector3 mov;
        // Apply speed factor to movement only (not jump)
        mov = new Vector3(movement.x, 0, movement.z) * (Speed * currentSpeedFactor);
        mov.y = movement.y;
        characterController.Move(Time.deltaTime * mov);
        movement.x = 0;
        movement.z = 0;
    }

    private void ApplySlidingIfOnSlope()
    {
        //slide down
        if (IsOnSlope)
        {
            Vector3 slideDirection = Vector3.down + slopeNormal;
            characterController.Move(slopeFallingSpeed * Time.deltaTime * slideDirection);
        }
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
    const float gravity = -25f;
    private void OnJump()
    {
        if (characterController.isGrounded && isOnSlopeCache == false)
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
