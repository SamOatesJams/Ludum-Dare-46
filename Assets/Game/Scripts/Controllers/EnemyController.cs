using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyController : MonoBehaviour
{
    public Tilemap CollisionMask;
    public Vector2Int Target;
    public float MovementSpeedMultiplier = 0.01f;

    private NavAgent m_navAgent;
    private Stack<Vector3> m_path;

    private Vector3 m_lastPosition;
    private Vector3 m_currentTarget;
    private float m_progress;
    private bool m_endOfPath;

    // Start is called before the first frame update
    void Start()
    {
        m_navAgent = new NavAgent(CollisionMask);
        RecaluatePath();
    }

    public void RecaluatePath()
    {
        var position = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        m_path = m_navAgent.CalculatePath(position, Target);

        Debug.Assert(m_path != null, "Could not find path to target");
        NextPathNode();
        m_endOfPath = false;
    }

    public void NextPathNode()
    {
        if (m_path.Count == 0)
        {
            m_endOfPath = true;
            return;
        }

        m_currentTarget = m_path.Pop();
        m_lastPosition = transform.position;
        m_progress = 0.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_endOfPath)
        {
            return;
        }

        if (Vector2.Distance(m_currentTarget, transform.position) < 0.1f)
        {
            NextPathNode();
        }

        m_progress += MovementSpeedMultiplier;
        transform.position = m_lastPosition + ((m_currentTarget - m_lastPosition) * m_progress);
    }
}
