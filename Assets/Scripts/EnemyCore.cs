using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class EnemyCore : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] int maxHits = 1;          // set to 2 on prefab for shielded variant
    [SerializeField] int energyRefund = 2;     // seconds given back on death (optional)

    public int damage = 1;

    [Header("VFX")]
    [SerializeField] Material mat;             // drag the material used by the mesh
    [SerializeField] float flashDuration = .15f;

    int currentHits;

    void Awake()
    {
        currentHits = maxHits;
        if (!mat) mat = GetComponent<Renderer>().material;
    }

    public void TakeHit(int damage, Vector3 hitPoint)
    {
        currentHits -= damage;
        StartCoroutine(FlashRed());

        if (currentHits <= 0) Die();
    }

    IEnumerator FlashRed()
    {
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", Color.red);
        yield return new WaitForSeconds(flashDuration);
        mat.SetColor("_EmissionColor", Color.black);
    }

    void Die()
    {
        if (energyRefund > 0 && EnergyBank.instance)
            EnergyBank.instance.AddEnergy(energyRefund);

        Destroy(gameObject);
    }
}