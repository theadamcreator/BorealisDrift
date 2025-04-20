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
        canopyPhase.SetActive(true); // Activate the canopy
        // Optional: Disable possession system if needed
        gameObject.SetActive(false); // Disable this manager if you only need it once
    }
}
