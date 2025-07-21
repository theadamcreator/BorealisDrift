using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] GameObject pauseMenu;              // drag your Pause Canvas

    [Header("Input (optional)")]
    [SerializeField] InputActionReference pauseAction;  // drag “Pause” action here if you made one

    [SerializeField] PlayerInput playerInput;

    bool isPaused;

    void OnEnable()
    {
        // enable the custom InputAction (only InputActions need this)
        if (pauseAction != null)
        {
            pauseAction.action.Enable();                // <-- WORKS
            pauseAction.action.performed += OnPausePressed;
        }
    }

    void OnDisable()
    {
        if (pauseAction != null)
            pauseAction.action.performed -= OnPausePressed;
    }

    /* ---------- entry points ---------- */
    void OnPausePressed(InputAction.CallbackContext _) => TogglePause();

    void Update()                                       // fallback Esc check
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame && pauseAction == null)
            TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0f : 1f;
        pauseMenu.SetActive(isPaused);

        // NEW – (de)activate the whole input component
        if (playerInput) playerInput.enabled = !isPaused;

        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    /* --- UI buttons --- */
    public void Resume() => TogglePause();
    public void QuitToDesktop() => Application.Quit();
}