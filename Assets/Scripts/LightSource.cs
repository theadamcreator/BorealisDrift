using UnityEngine;

public class LightSource : MonoBehaviour, ILightAttractor
{
    [Header("Light Attr")]
    public float intensity = 1f;
    public float attractionRadius = 8f;
    public float engageRadius = 4f;

    public Vector3 Position => transform.position;
    public float Intensity => intensity;
    public float AttractionRadius => attractionRadius;
    public float EngageRadius => engageRadius;
}