using AdventOfCode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace AdventOfCode2022
{
    [Day(16)]
    public class Day16 : Day
    {
        Graph valves;
        Valve start;

        private int minutes;


        private Dictionary<string, Action> cache;
        private Dictionary<int, int> bitsetIndices;

        public override string RunPart1()
        {
            ReadInput();


            minutes = 30;
            cache = new Dictionary<string, Action>();
            bitsetIndices = new Dictionary<int, int>();
            int i = 0;
            foreach(var node in valves.GetAllNodes())
            {
                bitsetIndices[node.Id] = i;
                i++;
            }
            var openValves = new BitArray(valves.GetAllNodes().Count());
            openValves.SetAll(false);

            var result = GetBestMove(start.Id, minutes, openValves, 0);
            Console.WriteLine(result.Description);

            return result.Pressure.ToString();
        }


        private Action GetBestMove(int idx, int minutes, BitArray openValves, int indent)
        {
            if (minutes == 0)
                return new Action(0, "nop");

            var cacheKey = GetCacheKey(idx, minutes, openValves);
            if (cache.ContainsKey(cacheKey))
                return cache[cacheKey];

            var currentValve = (Valve)valves.GetNode(idx);


            var pressure = ComputePressure(openValves);
            Action bestAnswer = null;
            var descr = $"{String.Join(" ", new string[indent])} {currentValve.Title} (min {minutes}): ";

            if (!IsOpen(openValves, idx) && currentValve.Rate > 0)
            {
                var newOpenValves = OpenValve(openValves, idx); 
                var a1 = GetBestMove(currentValve.Id, minutes - 1, newOpenValves, indent + 2);
                a1.Description = $"{descr} open\n" + a1.Description;
                bestAnswer = a1;
            }
            foreach(var neighbor in currentValve.GetNeighbors())
            {
                var nNode = valves.GetNode(neighbor);
                var a = GetBestMove(nNode.Id, minutes - 1, openValves, indent + 2);

                if(bestAnswer == null || a.Pressure > bestAnswer.Pressure)
                {
                    a.Description = $"{descr} move {((Valve)nNode).Title}\n" + a.Description;
                    bestAnswer = a;
                }
            }

            var result = new Action(bestAnswer);
            result.Pressure += pressure;
            cache[cacheKey] = result;
            return result;
        }


        public override string RunPart2()
        {
            throw new NotImplementedException();
        }

        private class Action
        {
            public int Pressure { get; set; }
            public string Description { get; set; }
            public Action(int answer, string description)
            {
                Pressure = answer;
                Description = description;
            }
            public Action(Action a)
            {
                Pressure = a.Pressure;
                Description = a.Description;
            }
        }

        private int ComputePressure(BitArray openValves)
        {
            var pressure = 0;
            foreach(var node in valves.GetAllNodes())
            {
                if (openValves[bitsetIndices[node.Id]])
                    pressure += ((Valve)node).Rate;
            }
            return pressure;
        }

        private bool IsOpen(BitArray openValves, int idx)
        {
            return openValves[bitsetIndices[idx]];
        }
        private BitArray OpenValve(BitArray origin, int idx)
        {
            var result = new BitArray(origin);
            result[bitsetIndices[idx]] = true;
            return result;
        }

        private string GetCacheKey(int idx, int minutes, BitArray openValves)
        {
            var title = ((Valve)valves.GetNode(idx)).Title;
            var bits = "";
            for (int i = 0; i < openValves.Length; i++)
                bits += openValves.Get(i) ? "1" : "0";
            return $"{title}_{minutes}_{bits}";
        }


        private void ReadInput()
        {
            var lines = File.ReadAllLines(InputFile);

            valves = new Graph();
            foreach (var line in lines)
            {
                var parts = line.Split(new[] { ' ', '=', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                var node = new Valve(parts[1], int.Parse(parts[5]), parts.Skip(10));
                valves.AddNode(node);
                if (node.Title == "AA")
                    start = node;

            }
        }

        private class Valve : Node
        {
            public string Title { get; private set; }
            public int Rate { get; private set; }
            public IEnumerable<string> LeadIndices { get; private set; }

            public Valve(string index, int rate, IEnumerable<string> leads)
            {
                Title = index;
                Rate = rate;
                LeadIndices = leads.ToList();

                Id = index.GetHashCode();
                foreach(var l in LeadIndices)
                {
                    AddConnection(l.GetHashCode(), 1);
                }
            }

            public override long GetCost(Node neighbor)
            {
                return ((Valve)neighbor).Rate;
            }
        }
    }
}
