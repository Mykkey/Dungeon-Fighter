using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput playerInput;

    public static Vector2 movement;
    public static bool jumpWasPressed;
    public static bool jumpIsHeld;
    public static bool jumpWasReleased;
    public static bool runIsHeld;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction runAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["move"];
        jumpAction = playerInput.actions["jump"];
        runAction = playerInput.actions["run"];
    }

    private void Update()
    {
        handleMovementInputs();
    }

    private void handleMovementInputs()
    {
        movement = moveAction.ReadValue<Vector2>();

        jumpWasPressed = jumpAction.WasPressedThisFrame();
        jumpIsHeld = jumpAction.IsPressed();
        jumpWasReleased = jumpAction.WasReleasedThisFrame();

        runIsHeld = runAction.IsPressed();
    }
}
