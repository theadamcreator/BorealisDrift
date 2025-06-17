using UnityEngine.InputSystem;
using UnityEngine;

public class GamePhaseManager : MonoBehaviour
{
    [Header("Roots")]
    [SerializeField] private GameObject undertreePhase;   // drag the ForestPhase GO
    [SerializeField] private GameObject canopyPhase;   // drag CanopyPhase GO

    [Header("AudioListeners")]
    [SerializeField] private AudioListener undertreeListener;  // inside tree camera
    [SerializeField] private AudioListener canopyListener;  // inside FP camera

    [Header("Input")]
    [SerializeField] private PlayerInput canopyInput;                     // cached at Start()

    private void Awake()
    {
        canopyInput = canopyPhase.GetComponentInChildren<PlayerInput>(true);
        canopyPhase.SetActive(false);           // canopy starts hidden
        canopyListener.enabled = false;         // only one listener on
    }

    public void EnterCanopyPhase()
    {
        // 1 swap roots
        undertreePhase.SetActive(false);
        canopyPhase.SetActive(true);

        // 2 audio
        undertreeListener.enabled = false;
        canopyListener.enabled = true;

        // 3 enable & switch action-map
        canopyInput.enabled = true;
        canopyInput.SwitchCurrentActionMap("Player");   // your action-map name

        // 4 lock cursor for mouse-look
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 5 start the timer
        EnergyBank.instance?.BeginCanopyPhase();
    }
}
