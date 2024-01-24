using UnityEngine;

public interface IGetCameraInput
{
    Vector2 GetViewInput();

}
public sealed class GetCameraInputPC : IGetCameraInput
{
    public Vector2 GetViewInput()
    {
        float mouseX = Input.GetAxis(Constants.MOUSE_X_KW);
        float mouseY = Input.GetAxis(Constants.MOUSE_Y_KW);

        return new Vector2(mouseX, mouseY);
    }
}
