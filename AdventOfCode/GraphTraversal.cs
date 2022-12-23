using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace AdventOfCode
{
    public class GraphTraversal
    {


        public static void BFS<T>(Graph<T> graph, Node<T> start, Action<Node<T>> action)
        {
            var queue = new Queue<int>();
            queue.Enqueue(start.Id);
            var visited = new HashSet<int>();
            visited.Add(start.Id);

            while (queue.Count > 0)
            {
                var node = graph.GetNode(queue.Dequeue());
                
                action(node);

                foreach (var neighborId in node.GetNeighbors())
                {
                    if (!visited.Contains(neighborId))
                    {
                        queue.Enqueue(neighborId); 
                        visited.Add(neighborId);
                    }
                }
            }
        }
        
        public static int DFS<T>(Graph<T> graph, Node<T> start, Func<Node<T>, int> func)
        {
            var stack = new Stack<IEnumerator<int>>();
            var discovered = new HashSet<int>();
            discovered.Add(start.Id);
            var result = func(start);

            stack.Push(start.GetNeighbors().GetEnumerator());

            while(stack.Count > 0)
            {
                if(stack.Peek().MoveNext())
                {
                    var w = stack.Peek().Current;
                    if(!discovered.Contains(w))
                    {
                        discovered.Add(w);
                        var node = graph.GetNode(w);
                        result += func(node);
                        stack.Push(node.GetNeighbors().GetEnumerator());
                    }
                }
                else
                {
                    stack.Pop();
                }
            }
            return result;
        }
    }
}
