using UnityEngine;
using System.Collections; // Required to use IEnumerator for coroutines

public class TreePossession : MonoBehaviour
{
    public PossessableTree currentTree; // This holds the tree you are currently possessing
    public float moveDelay = 0.2f;      // Cooldown between moves so input doesn’t feel too twitchy

    private bool canMove = true;        // Prevents spamming directional input

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
        // Send a ray in the chosen direction from the current tree
        Ray ray = new Ray(currentTree.transform.position, direction);

        // If it hits something within 5 units...
        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
            // Check if the thing hit has a PossessableTree script
            PossessableTree newTree = hit.collider.GetComponent<PossessableTree>();
            if (newTree != null)
            {
                // "Leave" the current tree (change color, remove glow, etc.)
                currentTree.Unpossess();

                // "Enter" the new tree
                currentTree = newTree;
                currentTree.Possess();

                // Optional: start the cooldown so player can’t spam movement
                StartCoroutine(MoveCooldown());
            }
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
