using UnityEngine;

public class TreeEnergy : MonoBehaviour
{
    [Range(1, 3)]
    public int energyAmount = 1;
    public bool hasBeenDrained = false;

    public GameObject energyOrbVisual;
    public Renderer treeRenderer; // Only for proto visual

    void Start()
    {
        energyAmount = Random.Range(1, 4); // 1 to 3 inclusive
        if (energyParticles != null)
        {
            var emission = energyParticles.emission;
            emission.rateOverTime = energyAmount * 5f; // Adjust multiplier to taste
        }
        if (treeRenderer != null)
            treeRenderer.sharedMaterial.color = Color.blue;
    }

    public void DrainEnergy(EnergyBank bank)
    {
        if (hasBeenDrained) return;

        hasBeenDrained = true;
        bank.AddEnergy(energyAmount);

        if (treeRenderer != null)
            treeRenderer.sharedMaterial.color = Color.white;

        if (energyOrbVisual != null)
            energyOrbVisual.SetActive(false);
       
        if (energyParticles != null)
            energyParticles.Stop(); // Fade out, don't destroy for now
    }

    public ParticleSystem energyParticles;
}
