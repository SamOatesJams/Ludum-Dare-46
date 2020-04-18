using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/EnvironmentSurfaceData")]
public class EnvironmentSurfaceData : ScriptableObject
{
    public List<string> SurfaceTypes;
    public List<float> SurfaceMovementModifiers;

    public float GetMovementModifier(string surfaceName)
    {
        return SurfaceMovementModifiers[SurfaceTypes.IndexOf(surfaceName)];
    }
}
