using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerDamageReceiver : MonoBehaviour, IDamageable
{
    [Header("Health & Flash")]
    [Tooltip("Seconds of EnergyBank time lost per hit")]
    public int damage = 1;
    public Material flashMat;          // drag emissive player mat
    public float flashTime = 0.15f;

    EnergyBank bank;

    void Awake()
    {
        bank = EnergyBank.instance;
        if (!flashMat) flashMat = GetComponentInChildren<Renderer>().material;
    }

    public void TakeHit(int damage, Vector3 hitPoint)
    {
        // 1. subtract energy / HP
        if (bank) bank.AddEnergy(-damage);

        // 2. flash red
        StartCoroutine(FlashRed());

        // 3. (optional) check death here, call GamePhaseManager fail, etc.
    }

    System.Collections.IEnumerator FlashRed()
    {
        flashMat.EnableKeyword("_EMISSION");
        flashMat.SetColor("_EmissionColor", Color.red);
        yield return new WaitForSeconds(flashTime);
        flashMat.SetColor("_EmissionColor", Color.black);
    }
}