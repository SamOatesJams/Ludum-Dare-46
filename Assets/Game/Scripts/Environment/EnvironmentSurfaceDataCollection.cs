using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Environment/Surface Data Collection")]
public class EnvironmentSurfaceDataCollection : ScriptableObject
{
    public EnvironmentSurfaceData[] Data;

    public EnvironmentSurfaceData GetSurfaceData(string surfaceName)
    {
        var data = Data.FirstOrDefault(x => x.SurfaceName == surfaceName);

        if (data == null)
        {
            Debug.LogWarning($"Failed to find movement modifier for the surface '{surfaceName}'");
            return null;
        }

        return data;
    }

    public float GetMovementModifier(string surfaceName)
    {
        var data = GetSurfaceData(surfaceName);
        return data == null ? data.MovementModifier : float.PositiveInfinity;
    }
}
