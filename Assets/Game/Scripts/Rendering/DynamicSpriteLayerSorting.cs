using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DynamicSpriteLayerSorting : MonoBehaviour
{
    private const int DefaultOrder = 1000;
    public float Offset;

    private SpriteRenderer m_renderer;
    private Camera m_camera;

    public void Start()
    {
        m_renderer = GetComponent<SpriteRenderer>();
        m_camera = Camera.main;
    }

    public void Update()
    {
        if (m_camera == null)
        {
            m_camera = Camera.main;
            return;
        }

        var screenSpace = m_camera.WorldToScreenPoint(transform.position);
        m_renderer.sortingOrder = (int)(DefaultOrder - (screenSpace.y + Offset));        
    }
}
