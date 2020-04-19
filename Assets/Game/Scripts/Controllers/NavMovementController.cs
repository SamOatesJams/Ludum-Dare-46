using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class NavMovementController : MonoBehaviour
{
    [Header("World References")]
    public CollisionMask CollisionMask;

    [Header("Movement modifiers")]
    public float MovementSpeedMultiplier = 0.01f;
    public float SurfaceSpeedModifier = 0.1f;

    private NavAgent m_navAgent;
    private Stack<Vector3> m_path;

    private Vector3 m_lastPosition;
    private Vector3 m_currentTarget;
    private float m_progress;
    private bool m_followingRoute;

    // Start is called before the first frame update
    void Start()
    {
        m_navAgent = new NavAgent(CollisionMask);
        m_followingRoute = false;
    }

    public void NavigateTo(Vector3 target)
    {
        RecaluatePath(target);

        NextPathNode();
        m_followingRoute = true;
    }

    public void RecaluatePath(Vector3 target)
    {
        var position = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), 0);
        var targetInt = new Vector3Int(Mathf.FloorToInt(target.x), Mathf.FloorToInt(target.y), 0);
        m_path = m_navAgent.CalculatePath(position, targetInt);

        Debug.Assert(m_path != null, "Could not find path to target");
    }

    public void NextPathNode()
    {
        if (m_path.Count == 0)
        {
            m_followingRoute = false;
            return;
        }

        m_currentTarget = m_path.Pop();
        m_lastPosition = transform.position;
        m_progress = 0.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (CollisionMask == null)
        {
            Debug.LogWarning($"No '{nameof(CollisionMask)}' is set on '{nameof(NavMovementController)}:{name}'");
            return;
        }

        if (!m_followingRoute)
        {
            return;
        }

        if (Vector2.Distance(m_currentTarget, transform.position) < 0.1f)
        {
            NextPathNode();
        }

        m_progress += MovementSpeedMultiplier * (1.0f / (CollisionMask.GetMovementMultiplier(transform.position) * SurfaceSpeedModifier));
        transform.position = m_lastPosition + ((m_currentTarget - m_lastPosition) * m_progress);
    }
}
