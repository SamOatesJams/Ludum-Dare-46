﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyMovementController : MonoBehaviour
{
    public CollisionMask CollisionMask;
    public Transform Target;
    public float MovementSpeedMultiplier = 0.01f;
    public float SurfaceSpeedModifier = 0.1f;

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

        if (Target != null)
        {
            RecaluatePath();
        }
    }

    public void RecaluatePath()
    {
        var position = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), 0);
        var target = new Vector3Int(Mathf.FloorToInt(Target.position.x), Mathf.FloorToInt(Target.position.y), 0);
        m_path = m_navAgent.CalculatePath(position, target);

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
        if (CollisionMask == null)
        {
            Debug.LogWarning($"No '{nameof(CollisionMask)}' is set on '{nameof(EnemyMovementController)}:{name}'");
            return;
        }

        if (m_endOfPath)
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
