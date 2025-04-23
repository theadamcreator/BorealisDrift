using UnityEngine;
using System.Collections; // Required to use IEnumerator for coroutines
using System.Collections.Generic;

public class TreePossession : MonoBehaviour
{
    public PossessableTree currentTree; // This holds the tree you are currently possessing
    public float moveDelay = 0.2f;      // Cooldown between moves so input doesn’t feel too twitchy
    private CameraFollowTree camFollow;
    private bool canMove = true;        // Prevents spamming directional input

    void Start()
    {
        if (currentTree == null)
        {
            Debug.LogError("currentTree is null on start!");
            return;
        }

        Debug.Log("currentTree found: " + currentTree.name);
        currentTree.Possess();

        StartCoroutine(AssignCameraAfterDelay()); //New: delay camera assignment
    }

    IEnumerator AssignCameraAfterDelay()
    {
        yield return null; // Wait a frame for Unity to properly register the MainCamera

        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("No Main Camera found in scene after delay!");
            yield break;
        }

        camFollow = mainCam.GetComponent<CameraFollowTree>();
        if (camFollow == null)
        {
            Debug.LogError("CameraFollowTree script missing on Main Camera after delay!");
            yield break;
        }

        camFollow.SetTarget(currentTree.transform);
    }
    void Update()
    {
        if (!canMove) return; // If still on cooldown, ignore input

        // Check arrow key input and try to move in that direction
        if (Input.GetKeyDown(KeyCode.RightArrow))
            TryMove(Vector3.right); // Move right in world space
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            TryMove(Vector3.left);  // Move left
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            TryMove(Vector3.forward); // Move forward (Z+)
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            TryMove(Vector3.back);    // Move backward (Z-)

        // Pressing Space will trigger the jump to the Canopy phase
        if (Input.GetKeyDown(KeyCode.Space)) AscendToCanopy();
    }

    // This method tries to move your light-being from one tree to another
    void TryMove(Vector3 direction)
    {
        PossessableTree[] allTrees = FindObjectsOfType<PossessableTree>();
        List<PossessableTree> potentialTargets = new List<PossessableTree>();

        foreach (var tree in allTrees)
        {
            if (tree == currentTree) continue;
            if (tree.possessionPoint == null) continue;
            if (currentTree.possessionPoint == null) continue;

            potentialTargets.Add(tree);
        }

        // Sort by distance
        potentialTargets.Sort((a, b) =>
        {
            float distA = Vector3.Distance(currentTree.possessionPoint.position, a.possessionPoint.position);
            float distB = Vector3.Distance(currentTree.possessionPoint.position, b.possessionPoint.position);
            return distA.CompareTo(distB);
        });

        // Grab up to 2 closest trees
        int count = Mathf.Min(2, potentialTargets.Count);
        PossessableTree bestMatch = null;
        float bestDot = -1f;

        for (int i = 0; i < count; i++)
        {
            Vector3 toTree = (potentialTargets[i].possessionPoint.position - currentTree.possessionPoint.position).normalized;
            float dot = Vector3.Dot(direction.normalized, toTree);

            if (dot > bestDot) // Best alignment with player input
            {
                bestDot = dot;
                bestMatch = potentialTargets[i];
            }
        }

        if (bestMatch != null)
        {
            currentTree.Unpossess();
            currentTree = bestMatch;
            currentTree.Possess();

            if (camFollow != null)
                camFollow.SetTarget(currentTree.transform);

            StartCoroutine(MoveCooldown());
        }
    }

    // This starts the jump to the canopy
    void AscendToCanopy()
    {
        Debug.Log("Launching to Canopy from: " + currentTree.name);
        gameManager.EnterCanopyPhase(); // Let GameManager handle the transition
    }

    // Cooldown between moves
    IEnumerator MoveCooldown()
    {
        canMove = false;
        yield return new WaitForSeconds(moveDelay); // wait a bit
        canMove = true;
    }

    public GamePhaseManager gameManager; // Reference to the thing that controls which phase is active
}
