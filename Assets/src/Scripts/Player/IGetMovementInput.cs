using UnityEngine;

namespace Assets
{
    public interface IGetMovementInput
    {
        Vector2 GetMovement();
    }

    public sealed class GetMovementInput : IGetMovementInput
    {
        public Vector2 GetMovement()
        {
            float x = Input.GetAxis(Constants.HORISONTAL_MOVEMENT_KW);
            float y = Input.GetAxis(Constants.VERTICAL_MOVEMENT_KW);

            Vector2 moveInput = new Vector2(x, y);

            if (moveInput.magnitude > 1)
            {
                moveInput.Normalize();
            }

            return moveInput;
        }
    }
}
