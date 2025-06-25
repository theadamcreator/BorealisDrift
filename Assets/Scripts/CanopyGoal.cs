using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CanopyGoal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;          // FP player has tag Player
        GamePhaseManager.Instance.EnterGoalMenu();       // singleton for convenience
    }
}
