using UnityEngine;

public class CameraLookController : MonoBehaviour
{
    public Transform PlayerBody;
    public float LookSensitivity = 500;

    public PlayerInputListener PlayerInputListener;
    float xRotation = 0;
    // Start is called before the first frame update
    void Start()
    {
        this.PlayerInputListener.OnLookInput += UpdateCamera;

        Cursor.lockState= CursorLockMode.Locked;
    }

    void UpdateCamera(Vector2 input)
    {
        input.x *= LookSensitivity * Time.deltaTime;
        input.y *= LookSensitivity * Time.deltaTime;

        xRotation -= input.y;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        PlayerBody.Rotate(Vector3.up * input.x);
    }

    private void OnDestroy()
    {
        this.PlayerInputListener.OnLookInput-= UpdateCamera;
    }
}
