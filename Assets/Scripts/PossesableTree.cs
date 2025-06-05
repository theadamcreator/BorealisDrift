using UnityEngine;

public class PossessableTree : MonoBehaviour
{
    public Renderer rend; // The renderer of the tree — we’ll change its color to show possession
    public Light treeLight;
    public Transform possessionPoint;
    private void Start()
    {
        rend = GetComponentInChildren<Renderer>(); // Get the visual part of the tree

        if (rend == null)
            Debug.LogWarning("Renderer not found on " + gameObject.name);

    }
    public void Possess()
    {
        if (treeLight != null)
            treeLight.enabled = true;

        TreeEnergy energy = GetComponent<TreeEnergy>();
        if (energy != null && !energy.hasBeenDrained)
        {
            EnergyBank bank = FindObjectOfType<EnergyBank>();
            energy.DrainEnergy(bank);
        }
    }

    public void Unpossess()
    {
        if (treeLight != null)
            treeLight.enabled = false;
    }
}
