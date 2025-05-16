using UnityEngine;

public class GamePhaseManager : MonoBehaviour
{
    public GameObject canopyPhase;

    void Start()
    {
        canopyPhase.SetActive(false); // Hide canopy player + camera
    }

    public void EnterCanopyPhase()
    {
        canopyPhase.SetActive(true);
        EnergyBank.instance?.BeginCanopyPhase(); // Start the countdown timer
        gameObject.SetActive(false);
    }
}
