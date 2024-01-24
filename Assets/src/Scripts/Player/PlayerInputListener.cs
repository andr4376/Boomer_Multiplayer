using Assets;
using UnityEngine;

public sealed partial class PlayerInputListener : MonoBehaviour
{
    private bool _shootIsDown = false;
    private bool _aimIsDown = false;
    private readonly IGetCameraInput getCameraInput;
    private readonly IGetMovementInput getMovementInput;

    public PlayerInputListener()
    {
        this.getCameraInput = new GetCameraInputPC();
        this.getMovementInput = new GetMovementInput();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
            //camera input
            {
                var cameraInput = this.getCameraInput
                         .GetViewInput();
                if (cameraInput.y != 0 || cameraInput.x != 0)
                    this.OnLookInput?.Invoke(cameraInput);
            }

            //Primary attack / click
            {
                if (Input.GetMouseButtonDown(0))
                    this.onShootInputDown?.Invoke();

                bool shootWasDown = _shootIsDown;

                _shootIsDown = Input.GetMouseButton(0);

                if (_shootIsDown)
                {
                    this.onShootInputHold?.Invoke();
                }
                else if (shootWasDown)
                    this.onShootInputRelease?.Invoke();
            }

            //Secondary attack / click
            {
                if (Input.GetMouseButtonDown(1))
                    this.onAimInputDown?.Invoke();

                bool aimWasDown = _aimIsDown;

                _aimIsDown = Input.GetMouseButton(1);

                if (_aimIsDown)
                {
                    this.onAimInputHold?.Invoke();
                }
                else if (aimWasDown)
                    this.onAimInputRelease?.Invoke();
            }

            //Movement
            {
                //Motion (Direction)
                {
                    Vector2 moveInput = getMovementInput.GetMovement();

                    if (moveInput.x != 0 || moveInput.y != 0)
                    {
                        OnMoveInput?.Invoke(moveInput);
                    }
                }

                //Crouch
                {
                    if (Input.GetKeyDown(KeyCode.LeftControl))
                        OnCrouch?.Invoke();
                }
                //Sprint
                {
                    if (Input.GetKeyDown(KeyCode.LeftShift))
                        OnSprint?.Invoke();
                }
                //Jump
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                        OnJump?.Invoke();
                }
            }

            //reload
            {
                if (Input.GetKeyDown(KeyCode.R))
                    onReloadInput?.Invoke();
            }

        //Interact
        {
            if (Input.GetKeyDown(KeyCode.E))
                OnInteract?.Invoke();
        }
    }
}

public partial class PlayerInputListener
{
    //primary fire
    public PlayerInputEvents.OnShootInputDown onShootInputDown;
    public PlayerInputEvents.OnShootInputHold onShootInputHold;
    public PlayerInputEvents.OnShootInputRelease onShootInputRelease;

    //Secondary fire
    public PlayerInputEvents.OnAimInputDown onAimInputDown;
    public PlayerInputEvents.OnAimInputHold onAimInputHold;
    public PlayerInputEvents.OnAimInputRelease onAimInputRelease;

    //Reload
    public PlayerInputEvents.OnReloadInput onReloadInput;

    //Camera look
    public PlayerInputEvents.OnLookInput OnLookInput;

    //Movement
    public PlayerInputEvents.OnMoveInput OnMoveInput;
    public PlayerInputEvents.OnCrouch OnCrouch;
    public PlayerInputEvents.OnSprint OnSprint;
    public PlayerInputEvents.OnJump OnJump;

    //Interact
    public PlayerInputEvents.OnInteract OnInteract;
}

public static class PlayerInputEvents
{
    public delegate void OnShootInputDown();
    public delegate void OnShootInputHold();
    public delegate void OnShootInputRelease();

    public delegate void OnAimInputDown();
    public delegate void OnAimInputHold();
    public delegate void OnAimInputRelease();

    public delegate void OnReloadInput();

    public delegate void OnLookInput(Vector2 input);

    public delegate void OnMoveInput(Vector2 input);
    public delegate void OnCrouch();
    public delegate void OnSprint();
    public delegate void OnJump();
    public delegate void OnInteract();
}

public static class Constants
{
    public const string MOUSE_X_KW = "Mouse X";
    public const string MOUSE_Y_KW = "Mouse Y";

    public const string HORISONTAL_MOVEMENT_KW = "Horizontal";
    public const string VERTICAL_MOVEMENT_KW = "Vertical";
}
