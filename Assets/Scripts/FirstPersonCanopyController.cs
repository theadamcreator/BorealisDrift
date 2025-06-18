using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Basic floating first-person controller using the new Input System.
/// Requires a CharacterController and a Camera as child.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class FirstPersonCanopyController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 4f;
    public float gravity = 0f;          // keep at 0 to “float”

    [Header("Look")]
    public float lookSensitivity = 3f;
    public float maxPitch = 80f;

    [Header("References")]
    public Transform camRoot;           // assign your child camera here

    // input actions (auto-generated when you import CanopyControls input action asset)
    private CanopyControls controls;
    private CharacterController cc;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalVelocity;
    private float pitch;                // camera X-rotation

    private void Awake()
    {

        cc = GetComponent<CharacterController>();
        controls = new CanopyControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += _ => moveInput = Vector2.zero;

        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += _ => lookInput = Vector2.zero;

        controls.Player.Jump.performed += _ =>
        {
            if (cc.isGrounded) verticalVelocity = jumpForce;
        };

        controls.Player.Fire.performed += _ => GetComponent<Shooter>().TryShoot();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        // LOOK
        pitch = Mathf.Clamp(pitch - lookInput.y * lookSensitivity, -maxPitch, maxPitch);
        camRoot.localEulerAngles = new Vector3(pitch, 0, 0);
        transform.Rotate(Vector3.up, lookInput.x * lookSensitivity);

        // MOVE (relative to facing)
        Vector3 dir = transform.forward * moveInput.y + transform.right * moveInput.x;
        dir = dir.normalized * moveSpeed;

        // simple vertical
        if (!cc.isGrounded) verticalVelocity += gravity * Time.deltaTime;
        else if (verticalVelocity < 0) verticalVelocity = 0;

        dir.y = verticalVelocity;

        cc.Move(dir * Time.deltaTime);
    }
}