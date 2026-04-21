using UnityEngine;

[CreateAssetMenu(menuName ="Player Movement")]
public class PlayerMovementStats : ScriptableObject
{
    [Header("Walk")]
    [Range(1, 100)] public float maxWalkSpeed = 12.5f;
    [Range(0.25f, 50)] public float groundAcceleration;
    [Range(0.25f, 50)] public float groundDeceleration;
    [Range(0.25f, 50)] public float airAcceleration;
    [Range(0.25f, 50)] public float airDeceleration;

    [Header("Run")]
    [Range(1, 100)] public float maxRunSpeed = 20;

    [Header("Grounded/Collision Checks")]
    public LayerMask groundLayer;
    public float groundDetectionRayLength = 0.02f;
    public float headDetectionRayLength = 0.02f;
    [Range(0, 1)] public float headWidth = 0.75f;

    [Header("Jump")]
    public float jumpHeight = 6.5f;
    [Range(1, 1.1f)] public float jumpHeightCompensationFactor = 1.054f;
    public float timeTillJumpApex = 0.35f;
    [Range(0.01f, 5)] public float gravityOnReleaseMultiplier = 2;
    public float maxFallSpeed = 26;
    [Range(1, 5)] public int numberOfJumpsAllowed = 2;

    [Header("Jump Cut")]
    [Range(0.02f, 0.3f)] public float timeForUpwardsCancel = 0.027f;

    [Header("Jump Apex")]
    [Range(0.5f, 1)] public float apexThreshold = 0.97f;
    [Range(0.01f, 1)] public float apexHangTime = 0.075f;

    [Header("Jump Buffer")]
    [Range(0, 1)] public float JumpBufferTime = 0.125f;

    [Header("Jump Coyote Time")]
    [Range(0, 1)] public float jumpCoyoteTime = 0.1f;

    [Header("Debug")]
    public bool debugShowIsGroundedBox;
    public bool debugShowHeadBumpBox;

    [Header("JumpVisualisation Tool")]
    public bool showWalkJumpArc = false;
    public bool showRunJumpArc = false;
    public bool stopOnCollision = true;
    public bool drawRight = true;
    [Range(5, 100)] public int arcResolution = 20;
    [Range(0, 500)] public int visualisationSteps = 90;

    public float Gravity { get; private set; }
    public float InitialJumpVelocity { get; private set; }
    public float AdjustedJumpHeight { get; private set; }

    private void OnValidate()
    {
        CalculateValues();
    }

    private void OnEnable()
    {
        CalculateValues();
    }

    private void CalculateValues()
    {
        AdjustedJumpHeight = jumpHeight * jumpHeightCompensationFactor;
        Gravity = -(2f * AdjustedJumpHeight) / Mathf.Pow(timeTillJumpApex, 2);
        InitialJumpVelocity = Mathf.Abs(Gravity) * timeTillJumpApex;
    }
}
