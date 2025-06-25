using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePhaseManager : MonoBehaviour
{
    public static GamePhaseManager Instance { get; private set; }

    [Header("Roots")]
    [SerializeField] GameObject undertreePhase;
    [SerializeField] GameObject canopyPhase;

    [Header("Audio")]
    [SerializeField] AudioListener undertreeListener;
    [SerializeField] AudioListener canopyListener;

    [Header("UI")]
    [SerializeField] GameObject uiGoalMenu;        // canvas refs
    [SerializeField] GameObject uiFailMenu;
    [SerializeField] TMP_Text txtEnergyLeft;
    [SerializeField] TMP_Text txtCycles;

    [SerializeField] PlayerInput canopyInput;
    EnergyBank bank;

    void Awake()
    {
        Instance = this;
        canopyInput = canopyPhase.GetComponentInChildren<PlayerInput>(true);
        bank = EnergyBank.instance;

        canopyPhase.SetActive(false);
        canopyListener.enabled = false;

        uiGoalMenu.SetActive(false);
        uiFailMenu.SetActive(false);
    }
    /* ----------  ENTER CANOPY  ---------- */
    public void EnterCanopyPhase()
    {
        undertreePhase.SetActive(false);
        canopyPhase.SetActive(true);

        undertreeListener.enabled = false;
        canopyListener.enabled = true;

        canopyInput.enabled = true;
        canopyInput.SwitchCurrentActionMap("Player");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        EnergyBank.instance?.BeginCanopyPhase();
        //_listen_ for depletion:
        InvokeRepeating(nameof(CheckForDepletion), 0, .25f);
    }

    /* ----------  GOAL REACHED  ---------- */
    public void EnterGoalMenu()
    {
        CancelInvoke(nameof(CheckForDepletion));

        canopyPhase.SetActive(false);
        canopyInput.enabled = false;
        canopyListener.enabled = false;

        // convert leftover energy to currency
        txtEnergyLeft.text = $"Energy left: {Mathf.CeilToInt(bank.canopyTimeLeft)}";
        bank.CompleteCycle();

        uiGoalMenu.SetActive(true);
        Time.timeScale = 0f;          // pause gameplay
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void ContinueRun()
    {
        uiGoalMenu.SetActive(false);
        Time.timeScale = 1f;

        EnterUndertreePhase();
    }

    /* ----------  FAIL STATE  ---------- */
    void CheckForDepletion()
    {
        if (bank.timerRunning) return;          // only fires once
        CancelInvoke(nameof(CheckForDepletion));
        EnterFailMenu();
    }
    void EnterFailMenu()
    {
        canopyPhase.SetActive(false);
        canopyInput.enabled = false;
        canopyListener.enabled = false;

        txtCycles.text = $"Cycles completed: {bank.cyclesCompleted}";
        uiFailMenu.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void RestartGame()
    {
        bank.ResetForNextCycle();
        uiFailMenu.SetActive(false);
        Time.timeScale = 1f;
        EnterUndertreePhase();
    }

    /* ----------  BACK TO TREES  ---------- */
    void EnterUndertreePhase()
    {
        undertreePhase.SetActive(true);
        undertreeListener.enabled = true;

        // move light-being back to first tree, reset camera etc. if needed
    }
}