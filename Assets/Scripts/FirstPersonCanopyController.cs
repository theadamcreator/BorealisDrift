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
    private Vector3 airVelocity;     // stores horizontal momentum
    [Range(0, 5)] public float airFriction = 1.5f; // 0 = no slowing

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
            if (cc.isGrounded)
            {
                verticalVelocity = jumpForce;
                airVelocity = (transform.forward * moveInput.y + transform.right * moveInput.x)
                              * moveSpeed;             // capture current ground speed
            }
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
        if (cc.isGrounded)
        {
            airVelocity = (transform.forward * moveInput.y + transform.right * moveInput.x)
                          * moveSpeed;
        }
        else //  IN AIR 
        {
            // keep last horizontal velocity but let it decay a bit
            airVelocity = Vector3.Lerp(airVelocity, Vector3.zero,
                                       airFriction * Time.deltaTime);

            // allow light aircontrol: add a bit of fresh input each frame
            Vector3 airControl = (transform.forward * moveInput.y + transform.right * moveInput.x)
                                 * moveSpeed * 0.3f;          // 0.3 = 30 percent strength
            airVelocity += airControl * Time.deltaTime;
        }

        // vertical
        if (!cc.isGrounded) verticalVelocity += gravity * Time.deltaTime;
        else if (verticalVelocity < 0) verticalVelocity = 0;

        Vector3 frameMove = airVelocity;
        frameMove.y = verticalVelocity;

        cc.Move(frameMove * Time.deltaTime);
    }
}