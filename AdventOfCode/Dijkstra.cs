using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public class Dijkstra
    {
        Dictionary<int, long> distances;
        Dictionary<int, Node> predecessors;
        List<int> queue;
        public IEnumerable<int> FindShortestPath(Graph graph, Node start, Node end)
        {
            Initialize(graph, start);
            while (queue.Count > 0)
            {
                var u = graph.GetNode(GetNext());
                queue.Remove(u.Id);
                if (u == end)
                {
                    return GetPath(start, end);
                }
                foreach (var neighborId in u.GetNeighbors())
                {
                    if (queue.Contains(neighborId))
                    {
                        UpdateCost(u, graph.GetNode(neighborId));
                    }
                }
            }
            throw new Exception($"No path from {start} to {end}");
        }

        public void TraverseWholeGraph(Graph graph, Node start)
        {
            Initialize(graph, start);
            while (queue.Count > 0)
            {
                var u = graph.GetNode(GetNext());
                queue.Remove(u.Id);
                foreach (var neighborId in u.GetNeighbors())
                {
                    if (queue.Contains(neighborId))
                    {
                        UpdateCost(u, graph.GetNode(neighborId));
                    }
                }
            }
        }

        public IEnumerable<int> GetPath(Node start, Node end)
        {
            var list = new List<int>();
            var node = end;
            list.Add(node.Id);
            while (node != start)
            {
                node = predecessors[node.Id];
                list.Add(node.Id);
            }
            list.Reverse();
            return list;
        }

        private void UpdateCost(Node current, Node neighbor)
        {
            var newDist = distances[current.Id] + current.GetCost(neighbor);
            if (newDist < distances[neighbor.Id])
            {
                distances[neighbor.Id] = newDist;
                predecessors[neighbor.Id] = current;
            }
        }

        private void Initialize(Graph graph, Node start)
        {
            distances = new Dictionary<int, long>();
            predecessors = new Dictionary<int, Node>();
            foreach (var node in graph.GetAllNodes())
            {
                distances.Add(node.Id, int.MaxValue);
                predecessors.Add(node.Id, null);
            }
            distances[start.Id] = 0;
            queue = graph.GetAllNodes().Select(n => n.Id).ToList();
        }

        private int GetNext()
        {
            var minDist = long.MaxValue;
            var minDistIndex = -1;
            foreach (var node in queue)
            {
                if (distances[node] < minDist)
                {
                    minDist = distances[node];
                    minDistIndex = node;
                }
            }
            return minDistIndex;
        }
    }

}
