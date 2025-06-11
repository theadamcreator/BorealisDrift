using UnityEngine;
using TMPro;

public class EnergyUI : MonoBehaviour
{
    public TextMeshProUGUI energyText;

    void Update()
    {
        if (EnergyBank.instance != null)
        {
            energyText.text = EnergyBank.instance.totalEnergy.ToString();
        }
    }
}
