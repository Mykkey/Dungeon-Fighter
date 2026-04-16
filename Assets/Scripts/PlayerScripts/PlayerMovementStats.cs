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
}
