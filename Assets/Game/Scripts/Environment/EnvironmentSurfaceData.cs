using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/EnvironmentSurfaceData")]
public class EnvironmentSurfaceData : ScriptableObject
{
    public List<string> SurfaceTypes;
    public List<float> SurfaceMovementModifiers;

    public float GetMovementModifier(string surfaceName)
    {
        var i = SurfaceTypes.IndexOf(surfaceName);
        if (i == -1)
        {
            Debug.LogWarning($"Failed to find movement modifier for the surface '{surfaceName}'");
            return float.PositiveInfinity;
        }
        return SurfaceMovementModifiers[i];
    }
}
