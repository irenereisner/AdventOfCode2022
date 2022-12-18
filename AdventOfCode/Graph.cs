using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    public class Graph
    {
        private Dictionary<int, Node> nodes = new Dictionary<int, Node>();

        public void AddNode(Node node)
        {
            nodes.Add(node.Id, node);
        }

        public Node GetNode(int id)
        {
            return nodes[id];
        }

        public IEnumerable<Node> GetAllNodes()
        {
            return nodes.Values;
        }
    }


    public class Node
    {
        public int Id { get; set; }
        private Dictionary<int, int> costPerConnection = new Dictionary<int, int>();

        public void AddConnection(int id, int cost)
        {
            costPerConnection[id] = cost;
        }

        public IEnumerable<int> GetNeighbors()
        {
            return costPerConnection.Keys;
        }

        public bool HasNeighbors()
        {
            return costPerConnection.Count > 0;
        }

        public virtual long GetCost(Node neighbor)
        {
            return costPerConnection[neighbor.Id];
        }
    }



}
