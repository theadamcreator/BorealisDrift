using UnityEngine;

public class PossessableTree : MonoBehaviour
{
    public Renderer rend; // The renderer of the tree — we’ll change its color to show possession

    private void Start()
    {
        rend = GetComponentInChildren<Renderer>(); // Get the visual part of the tree

        if (rend == null)
            Debug.LogWarning("Renderer not found on " + gameObject.name);
    }

    public void Possess()
    {
        // Change color to show this tree is currently possessed
        if (rend != null)
        {
            var block = new MaterialPropertyBlock();
            rend.GetPropertyBlock(block);
            block.SetColor("_BaseColor", Color.cyan);
            rend.SetPropertyBlock(block);
        }
    }


    public void Unpossess()
    {
        // Reset color to default (not possessed)
        if (rend != null)
            rend.material.SetColor("_BaseColor", Color.white);
    }
}
