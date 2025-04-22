using UnityEngine;
using System.Collections; // Required to use IEnumerator for coroutines

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
        else
        {
            Debug.Log("currentTree found: " + currentTree.name);
        }

        currentTree.Possess();

        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("No Main Camera found in scene!");
            return;
        }

        camFollow = mainCam.GetComponent<CameraFollowTree>();
        if (camFollow == null)
        {
            Debug.LogError("CameraFollowTree script missing on Main Camera!");
            return;
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

        PossessableTree bestCandidate = null;
        float closestAngle = 45f; // Only consider trees roughly within 45 degrees
        float closestDistance = Mathf.Infinity;

        foreach (var tree in allTrees)
        {
            if (tree == currentTree) continue;

            if (tree.possessionPoint == null)
            {
                Debug.LogError("Tree missing possessionPoint: " + tree.name);
                continue;
            }
            if (currentTree.possessionPoint == null)
            {
                Debug.LogError("Current tree is missing possessionPoint: " + currentTree.name);
                return;
            }

            if (camFollow == null)
            {
                Debug.LogWarning("camFollow is null during TryMove! Skipping camera update.");
            }
            else
            {
                camFollow.SetTarget(currentTree.transform);
            }

            camFollow.SetTarget(currentTree.transform);
           
            if (tree == null)
            {
                Debug.LogError("Tree in allTrees was null.");
                continue;
            }

            if (tree.possessionPoint == null)
            {
                Debug.LogError("Tree missing possessionPoint: " + tree.name);
                continue;
            }
            if (currentTree.possessionPoint == null)
            {
                Debug.LogError("currentTree is missing possessionPoint: " + currentTree.name);
                return;
            }

            Vector3 toTree = (tree.possessionPoint.position - currentTree.possessionPoint.position);

            float angle = Vector3.Angle(direction, toTree);

            if (angle < closestAngle)
            {
                float dist = toTree.magnitude;
                if (dist < closestDistance)
                {
                    closestAngle = angle;
                    closestDistance = dist;
                    bestCandidate = tree;
                }
            }
        }

        if (bestCandidate != null)
        {
            currentTree.Unpossess();
            currentTree = bestCandidate;
            currentTree.Possess();
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
