using UnityEngine;
using TMPro;

public class EnergyUI : MonoBehaviour
{
    public TextMeshProUGUI energyText;

    void Update()
    {
        if (EnergyBank.instance == null) return;

        // If we’re already in the canopy, show the countdown.
        // Otherwise show the totalEnergy we’ve banked so far.
        float seconds = EnergyBank.instance.timerRunning
                        ? EnergyBank.instance.canopyTimeLeft        // counting down
                        : EnergyBank.instance.totalEnergy;          // counting up

        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secondsInt = Mathf.FloorToInt(seconds % 60f);

        energyText.text = $"{minutes:00}:{secondsInt:00}";
    }
}