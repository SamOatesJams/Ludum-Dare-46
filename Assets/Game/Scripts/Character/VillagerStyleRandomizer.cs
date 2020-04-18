using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class VillagerStyleRandomizer : MonoBehaviour
{
    public VillagerStylePalette Palette;

    private static readonly int HairColor = Shader.PropertyToID("_HairColor");
    private static readonly int TopColor = Shader.PropertyToID("_TopColor");
    private static readonly int LegColor = Shader.PropertyToID("_LegColor");

    public void Start()
    {
        if (Palette.HairColors.Length == 0 || Palette.TopColors.Length == 0 || Palette.LegColors.Length == 0)
        {
            Debug.LogWarning($"The village '{name}' is missing one or more color override properties");
            return;
        }

        var spriteRender = GetComponent<SpriteRenderer>();
        var material = spriteRender.sharedMaterial;
        material = new Material(material);
        spriteRender.sharedMaterial = material;

        material.SetColor(HairColor, Palette.HairColors[Random.Range(0, Palette.HairColors.Length)]);
        material.SetColor(TopColor, Palette.TopColors[Random.Range(0, Palette.TopColors.Length)]);
        material.SetColor(LegColor, Palette.LegColors[Random.Range(0, Palette.LegColors.Length)]);
    }
}
