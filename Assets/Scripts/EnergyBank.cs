using UnityEngine;

public class EnergyBank : MonoBehaviour
{
    public static EnergyBank instance; // Singleton to persist across scenes

    public int totalEnergy = 0;
    public float canopyTimeLeft = 0f;
    public bool timerRunning = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject); // Keeps the energy alive through scenes
    }

    public void AddEnergy(int amount)
    {
        totalEnergy += amount;
        Debug.Log(" Energy collected: " + amount + " | Total: " + totalEnergy);
    }

    public void BeginCanopyPhase()
    {
        canopyTimeLeft = totalEnergy;
        timerRunning = true;
    }

    void Update()
    {
        if (timerRunning)
        {
            canopyTimeLeft -= Time.deltaTime;
            if (canopyTimeLeft <= 0f)
            {
                canopyTimeLeft = 0f;
                timerRunning = false;
                Debug.Log(" TIME'S UP! You're out of canopy energy.");
                // TODO: Trigger fall back / fail state
            }
        }
    }
}

