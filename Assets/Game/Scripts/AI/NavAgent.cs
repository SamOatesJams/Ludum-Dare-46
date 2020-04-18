using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NavAgent
{
    private static readonly List<Vector3Int> Directions = new List<Vector3Int>() {
        new Vector3Int(-1, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(0, 1, 0),
    };

    public CollisionMask m_collisionMask;

    private Vector2 m_target;
    private Vector2 m_start;

    public NavAgent(CollisionMask collisionMask)
    {
        m_collisionMask = collisionMask;
    }

    private class PathNode : IComparable<PathNode>
    {
        public Vector3Int Position { get; set; }
        public float Weight { get; set; }
        public PathNode Parent { get; set; }

        public int CompareTo(PathNode other)
        {
            return Weight.CompareTo(other.Weight);
        }
    }

    private PathNode GetNextNode(ref HashSet<PathNode> open)
    {
        var node = open.Min();
        open.Remove(node);
        return node;
    }

    private Stack<Vector3> BuildPath(PathNode node)
    {
        var path = new Stack<Vector3>();
        var currentNode = node;

        while (currentNode != null)
        {
            // TODO add abit of randomness across the tile
            path.Push(new Vector3(currentNode.Position.x, currentNode.Position.y, currentNode.Position.z));
            currentNode = currentNode.Parent;
        }

        return path;
    }

    public Stack<Vector3> CalculatePath(Vector3Int position, Vector3Int target)
    {
        var open = new HashSet<PathNode>();
        var closed = new HashSet<PathNode>();

        var startPosition = new PathNode() { Position = position, Parent = null, Weight = 0.0f };
        open.Add(startPosition);

        while (open.Count != 0)
        {
            var currentNode = GetNextNode(ref open);

            if (currentNode.Position == target)
            {
                return BuildPath(currentNode);
            }

            closed.Add(currentNode);

            foreach (var direction in Directions)
            {
                var nextNodePosition = currentNode.Position + direction;
                var cost = CalculateWeight(nextNodePosition, target, currentNode.Weight);

                if (float.IsInfinity(cost))
                {
                    continue;
                }

                if (open.SingleOrDefault(node => node.Position == nextNodePosition) != null)
                {

                }
                else if (closed.SingleOrDefault(node => node.Position == nextNodePosition) != null)
                {

                }
                else
                {
                    open.Add(new PathNode() { Position = nextNodePosition, Parent = currentNode, Weight = cost });
                }
            }
        }

        return null;
    }

    private float CalculateWeight(Vector3Int position, Vector3Int target, float currentWeight)
    {
        return CalculatePathCost(position, currentWeight) + CalculateRemainingWeight(position, target);
    }

    private float CalculateRemainingWeight(Vector3Int position, Vector3Int target)
    {
        return Vector3Int.Distance(position, target);
    }

    private float CalculatePathCost(Vector3Int target, float currentWeight)
    {
        return currentWeight + m_collisionMask.GetMovementMultiplier((Vector3) target);
    }
}
