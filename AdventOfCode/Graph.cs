using System.Collections.Generic;

namespace AdventOfCode
{
    public interface IGraph
    {
        Node Get(int id);
        IEnumerable<Node> GetAll();
    }

    public class Graph : IGraph
    {
        private Dictionary<int, Node> nodes = new Dictionary<int, Node>();

        public void AddNode(Node node)
        {
            nodes.Add(node.Id, node);
        }

        public void RemoveNode(int id)
        {
            nodes.Remove(id);
        }

        public Node Get(int id)
        {
            return nodes[id];
        }

        public IEnumerable<Node> GetAll()
        {
            return nodes.Values;
        }
    }


    public class Graph<T> : IGraph
    {
        private Dictionary<int, Node<T>> nodes = new Dictionary<int, Node<T>>();

        public void AddNode(Node<T> node)
        {
            nodes.Add(node.Id, node);
        }

        public void RemoveNode(int id)
        {
            nodes.Remove(id);
        }

        public Node<T> GetNode(int id)
        {
            return nodes[id];
        }

        public IEnumerable<Node<T>> GetAllNodes()
        {
            return nodes.Values;
        }

        public Node Get(int id)
        {
            return nodes[id];
        }

        public IEnumerable<Node> GetAll()
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
        public void RemoveConnection(int id)
        {
            costPerConnection.Remove(id);
        }

        public IEnumerable<int> GetNeighbors()
        {
            return costPerConnection.Keys;
        }

        public bool HasNeighbors()
        {
            return costPerConnection.Count > 0;
        }

        public virtual int GetCost(Node neighbor)
        {
            return costPerConnection[neighbor.Id];
        }
        public virtual int GetCost(int neighborId)
        {
            return costPerConnection[neighborId];
        }
    }

    public class Node<T> : Node
    {
        public T Value;
        public override string ToString()
        {
            return Value.ToString();
        }
    }



}
