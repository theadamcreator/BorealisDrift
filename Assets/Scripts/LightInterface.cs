using UnityEngine;

/// Any object that attracts enemies because it *glows*.
public interface ILightAttractor
{
    Vector3 Position { get; }
    float Intensity { get; }          // bigger = tastier
    float AttractionRadius { get; }   // where enemies start orbit
    float EngageRadius { get; }   // where they switch to attack
}

/// Anything that can be damaged by projectiles or AoE.
public interface IDamageable
{
    void TakeHit(int damage, Vector3 hitPoint);
}

/* --------------------------- */

/// Simple component to mark lamps, crystals, *or the player*.
/// Adjustable radii so designers can tweak range per prefab.

