using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NavAgent
{
    private static readonly Dictionary<string, float> TileCosts = new Dictionary<string, float>();

    private static readonly List<Vector2Int> Directions = new List<Vector2Int>() {
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1),
    };

    public Tilemap m_tileMap;

    private Vector2 m_target;
    private Vector2 m_start;

    public NavAgent(Tilemap collisionMask)
    {
        m_tileMap = collisionMask;

        TileCosts.Add("CollisionMask_Path", 1.0f);
        TileCosts.Add("CollisionMask_Grass", 100.0f);
        TileCosts.Add("CollisionMask_Wall", 100.0f);
        TileCosts.Add("CollisionMask_WoodFence", 100.0f);
    }

    private class PathNode : IComparable<PathNode>
    {
        public Vector2Int Position { get; set; }
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
            path.Push(new Vector3(currentNode.Position.x, currentNode.Position.y));
            currentNode = currentNode.Parent;
        }

        return path;
    }

    public Stack<Vector3> CalculatePath(Vector2Int position, Vector2Int target)
    {
        var open = new HashSet<PathNode>();
        var closed = new HashSet<PathNode>();

        var startPosition = new PathNode() { Position = position, Parent = null, Weight = 0.0f };
        open.Add(startPosition);

        int count = 0;

        while (open.Count != 0)
        {
            var currentNode = GetNextNode(ref open);
            Debug.Log(currentNode.Position);

            if (currentNode.Position == target)
            {
                return BuildPath(currentNode);
            }

            closed.Add(currentNode);

            foreach (var direction in Directions)
            {
                var nextNodePosition = currentNode.Position + direction;
                var cost = CalculateWeight(nextNodePosition, target, currentNode.Weight);
                Debug.Log(nextNodePosition);

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
                    Debug.Log(cost);
                    open.Add(new PathNode() { Position = nextNodePosition, Parent = currentNode, Weight = cost });
                }
            }

            if (count++ > 100)
                break;
        }

        return null;
    }

    private float CalculateWeight(Vector2Int position, Vector2Int target, float currentWeight)
    {
        return CalculatePathCost(position, currentWeight) + CalculateRemainingWeight(position, target);
    }

    private float CalculateRemainingWeight(Vector2Int position, Vector2Int target)
    {
        return Vector2Int.Distance(position, target);
    }

    private float CalculatePathCost(Vector2Int target, float currentWeight)
    {
        var tile = m_tileMap.GetTile(new Vector3Int(target.x, target.y, 0));
        if (tile == null || !TileCosts.ContainsKey(tile.name))
        {
            return float.PositiveInfinity;
        }
        return currentWeight + TileCosts[tile.name];
    }
}
