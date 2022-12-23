#define DEBUG_OUTPUT
using AdventOfCode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2022
{
    [Day(16)]
    public class Day16 : Day
    {
        List<int> validValves;
        BitArray initialOpenValves;

        Graph<Valve> graph;
        Node<Valve> start;

        Dictionary<int, Dictionary<int, List<int>>> shortestPaths;

        Dictionary<int, Dictionary<int, Result>> cache;


        private int highestFinished = 0;
        private bool isPart2 = false;
        public override string RunPart1()
        {
            return Run(30);
        }


        public override string RunPart2()
        {

            isPart2 = true;
            return Run(26);

        }

        private string Run(int minutes)
        {
            ReadInput();
            InitializeValidValves();
            ComputeShortestPaths();
            InitCache();

            Result result;
            if (isPart2)
                result = GetBestPath(new State(start.Id, start.Id, minutes, 0, 0, initialOpenValves));
            else
                result = GetBestPath(new State(start.Id, minutes, 0, initialOpenValves));
#if DEBUG_OUTPUT
            Console.WriteLine(result.Path);
#endif
            return result.Pressure.ToString();
        }


        private Result GetBestPath(State state)
        {
            if (state.RemainingMinutes <= 0)
                return new Result()
                {
                    Pressure = 0,
#if DEBUG_OUTPUT
                    Path = "\ntimeout" 
#endif
                };


            var currentPressurePerMinute = ComputePressureForOneMinute(state.OpenValves);

            if (AllOpen(state.OpenValves))
            {
                highestFinished = Math.Max(highestFinished, state.RemainingMinutes);
                return new Result()
                {
                    Pressure = state.RemainingMinutes * currentPressurePerMinute,
#if DEBUG_OUTPUT
                    Path = "\nfinished " + currentPressurePerMinute
#endif
                };
            }


            if (state.RemainingMinutes < highestFinished - 1)
                return new Result()
                {
                    Pressure = 0,
#if DEBUG_OUTPUT
                    Path = "\nAbort remaining" + state.RemainingMinutes
#endif
                };


            if (TryGetCache(state, out Result cacheResult))
                return cacheResult;

            var handled = new Dictionary<int, State>();
            for (int i = 0; i < state.NodeId.Length; i++)
            {
                if (state.RemainingPathMinutes[i] > 0)
                {
                    var handledState = state.Clone();
                    handledState.RemainingPathMinutes[i]--;
                    if (handledState.RemainingPathMinutes[i] == 0)
                    {
                        handledState.OpenValves[validValves.IndexOf(state.NodeId[i])] = true;
#if DEBUG_OUTPUT
                        handledState.Descriptions[i] = "Open" + graph.GetNode(handledState.NodeId[i]).Value.Title;
#endif
                    }
                    else
                    {
#if DEBUG_OUTPUT
                        handledState.Descriptions[i] = "Cont" + graph.GetNode(handledState.NodeId[i]).Value.Title;
#endif
                    }
                    handled.Add(i, handledState);
                }
            }
            if (handled.Count == 2)
            {
                var handledState = handled[0];
                handledState.NodeId[1] = handled[1].NodeId[1];
                handledState.RemainingPathMinutes[1] = handled[1].RemainingPathMinutes[1];
                handledState.RemainingMinutes--;
                handledState.OpenValves.Or(handled[1].OpenValves);
#if DEBUG_OUTPUT
                handledState.Descriptions[1] = handled[1].Descriptions[1];
#endif

                var result = GetBestPath(handledState);
                result.Pressure += currentPressurePerMinute;
#if DEBUG_OUTPUT
                result.Path = $"\n[{(isPart2 ? 27 : 31) - state.RemainingMinutes}]: {string.Join(" ", handledState.Descriptions)} ({currentPressurePerMinute})" + result.Path;
#endif
                SetCache(state, result);
                return result;
            }


            Result bestPressure = new Result()
            {
                Pressure = 0,
#if DEBUG_OUTPUT
                Path = "" 
#endif
            };

#if DEBUG_OUTPUT
            var currentTitle = "";
            for(int i =0; i <state.NodeId.Length; i++)
            {
                currentTitle += graph.GetNode(state.NodeId[i]).Value.Title + "(" + state.RemainingPathMinutes[i] + ") / ";
            }
#endif

            State nextState = handled.ContainsKey(1) ? handled[1] : state.Clone();
            nextState.RemainingMinutes--;
            foreach (var h in handled)
                nextState.OpenValves.Or(h.Value.OpenValves);

            List<State> actions = new List<State>();
            List<State> actions0 = new List<State>();
            if (handled.ContainsKey(0))
            {
                handled[0].RemainingMinutes--;
                actions0.Add(handled[0]);
            }
            else
            {
                actions0 = GetMoveActions(nextState, 0).ToList();
            }

            if (state.NodeId.Length > 1 && !handled.ContainsKey(1))
            {
                bool isAtStart = state.NodeId.All(n => n == start.Id);
                foreach (var a in actions0)
                {
                    var minIdx = 0;
                    if (isAtStart)
                    {
                        // at start pairs of moves can be swapped, i.e. we don't have to compute mirrored pairs
                        minIdx = validValves.IndexOf(a.NodeId[0]) + 1;
                    }
                    actions.AddRange(GetMoveActions(a, 1, minIdx));
                }
            }
            else
            {
                actions = actions0;
#if DEBUG_OUTPUT
                if(handled.ContainsKey(1))
                {
                    foreach (var a in actions)
                        a.Descriptions[1] = handled[1].Descriptions[1];
                }
#endif
            }


            foreach (var action in actions)
            {
                var nextResult = GetBestPath(action);
                if (nextResult.Pressure > bestPressure.Pressure)
                {
                    bestPressure = new Result()
                    {
                        Pressure = nextResult.Pressure,
#if DEBUG_OUTPUT
                        Path = $"\n[{(isPart2 ? 27 : 31) - state.RemainingMinutes}] {currentTitle}: {string.Join(" ", action.Descriptions)} {currentPressurePerMinute} {nextResult.Path}"
#endif
                    };
                }
            }


            bestPressure.Pressure += currentPressurePerMinute;

            SetCache(state, bestPressure);
            return bestPressure;
        }


        private IEnumerable<State> GetMoveActions(State currentState, int stateIdx, int minI = 0)
        {
            int count = 0;
            var nodeId = currentState.NodeId[stateIdx];
            for (int i = minI; i < validValves.Count; i++)
            {
                var nextValveId = validValves[i];
                if (/*!currentState.NodeId.Contains(nextValveId) &&*/ !currentState.OpenValves[i])
                {
                    var path = shortestPaths[nodeId][nextValveId];
                    var pathMinutes = path.Count - 1;
                    var state = currentState.Clone();
                    state.RemainingPathMinutes[stateIdx] = pathMinutes;
                    state.NodeId[stateIdx] = nextValveId;
#if DEBUG_OUTPUT
                    state.Descriptions[stateIdx] = " Move" + graph.GetNode(nextValveId).Value.Title;
#endif
                    yield return state;
                    count++;
                }
            }
            if (count == 0)
                yield return currentState;
        }

        private bool AllOpen(BitArray openValves)
        {
            for (int i = 0; i < openValves.Length; i++)
                if (!openValves[i])
                    return false;
            return true;
        }

        static int cacheMultiplier = 100;
        private void InitCache()
        {
            cache = new Dictionary<int, Dictionary<int, Result>>();
            var m = validValves.Count;
            cacheMultiplier = m + 1;
            var count = m * cacheMultiplier + m;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    var key = i * cacheMultiplier + j;
                    cache[key] = new Dictionary<int, Result>();
                }
            }
        }

        private void SetCache(State state, Result result)
        {
            var cacheKey1 = state.GetHashCode1();
            var cacheKey2 = state.GetHashCode2();
            cache[cacheKey1][cacheKey2] = result;
        }
        private bool TryGetCache(State state, out Result result)
        {
            var cacheKey1 = state.GetHashCode1();
            var cacheKey2 = state.GetHashCode2();
            var cc = cache[cacheKey1];
            if (cc.ContainsKey(cacheKey2))
            {
                result = cc[cacheKey2];
                return true;

            }
            result = new Result();
            return false;
        }


        private int ComputePressureForOneMinute(BitArray openValves)
        {
            var sum = 0;
            for (int i = 0; i < validValves.Count; i++)
            {
                if (openValves[i])
                {
                    var nextValveId = validValves[i];
                    sum += graph.GetNode(nextValveId).Value.Rate;
                }
            }
            return sum;
        }


        private void InitializeValidValves()
        {
            validValves = new List<int>();
            foreach (var node in graph.GetAllNodes().OrderByDescending(n => n.Value.Rate))
            {
                if (node.Value.Rate > 0)
                    validValves.Add(node.Id);
            }

            initialOpenValves = new BitArray(validValves.Count);
            for (int i = 0; i < initialOpenValves.Length; i++)
                initialOpenValves[i] = false;

        }
        private void ComputeShortestPaths()
        {
            shortestPaths = new Dictionary<int, Dictionary<int, List<int>>>();

            var srcValves = new List<int>(validValves);
            if (!srcValves.Contains(start.Id))
                srcValves.Add(start.Id);
            foreach (var valveId in srcValves)
            {
                shortestPaths.Add(valveId, new Dictionary<int, List<int>>());
                var dijkstra = new Dijkstra();
                dijkstra.TraverseWholeGraph(graph, graph.GetNode(valveId));
                foreach (var otherId in validValves)
                {
                    if (otherId == valveId) continue;
                    var path = dijkstra.GetPath(graph.GetNode(valveId), graph.GetNode(otherId)).ToList();
                    shortestPaths[valveId].Add(otherId, path);
                }
            }
        }

        private void ReadInput()
        {
            var lines = File.ReadAllLines(InputFile);

            graph = new Graph<Valve>();
            var connections = new Dictionary<int, List<string>>();
            int idx = 0;
            foreach (var line in lines)
            {
                var parts = line.Split(new[] { ' ', '=', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                var node = new Node<Valve>() { Id = idx, Value = new Valve(parts[1], int.Parse(parts[5])) };
                connections.Add(idx, parts.Skip(10).ToList());
                graph.AddNode(node);
                if (node.Value.Title == "AA")
                    start = node;
                idx++;
            }
            foreach (var id in connections.Keys)
            {
                var node = graph.GetNode(id);
                foreach (var c in connections[id])
                {
                    node.AddConnection(graph.GetAllNodes().FirstOrDefault(n => n.Value.Title == c).Id, 1);
                }
            }
        }

        private class State
        {
            public int[] NodeId;
            public int RemainingMinutes;
            public int[] RemainingPathMinutes;
            public BitArray OpenValves;
#if DEBUG_OUTPUT
            public string[] Descriptions;
#endif
            public State(int nodeId, int elephantId, int remainingMinutes, int remainingPathMinutes, int elephantPathMinutes, BitArray openValves)
            {
                NodeId = new int[] { nodeId, elephantId };
                RemainingMinutes = remainingMinutes;
                RemainingPathMinutes = new int[] { remainingPathMinutes, elephantPathMinutes };
                OpenValves = openValves;
#if DEBUG_OUTPUT
                Descriptions = new string[] { nodeId.ToString(), elephantId.ToString() };
#endif
            }
            public State(int nodeId, int remainingMinutes, int remainingPathMinutes, BitArray openValves)
            {
                NodeId = new int[] { nodeId };
                RemainingMinutes = remainingMinutes;
                RemainingPathMinutes = new int[] { remainingPathMinutes };
                OpenValves = openValves;
#if DEBUG_OUTPUT
                Descriptions = new string[] { nodeId.ToString() };
#endif
            }

            private State()
            { }

            public State Clone()
            {
                return new State()
                {
                    NodeId = (int[])this.NodeId.Clone(),
                    RemainingMinutes = this.RemainingMinutes,
                    RemainingPathMinutes = (int[])this.RemainingPathMinutes.Clone(),
                    OpenValves = new BitArray(this.OpenValves),
#if DEBUG_OUTPUT
                    Descriptions = (string[])this.Descriptions.Clone(),
#endif
                };
            }

            public override bool Equals(object obj)
            {
                var other = (State)obj;
                /*return NodeId.SequenceEqual(other.NodeId) &&
                    RemainingMinutes == other.RemainingMinutes &&
                    RemainingPathMinutes.SequenceEqual(other.RemainingPathMinutes) &&
                    new BitArray(OpenValves).Xor(other.OpenValves).OfType<bool>().All(e => !e);*/

                if (RemainingMinutes != other.RemainingMinutes)
                    return false;
                for (int i = 0; i < NodeId.Length; i++)
                {
                    if (NodeId[i] != other.NodeId[i] || RemainingPathMinutes[i] != other.RemainingPathMinutes[i])
                        return false;
                }
                for (int i = 0; i < OpenValves.Length; i++)
                {
                    if (OpenValves[i] != other.OpenValves[i])
                        return false;
                }
                return true;

            }

            public int GetHashCode1()
            {
                int hash = 0;
                foreach (var id in this.NodeId)
                    hash = hash * cacheMultiplier + id.GetHashCode();
                return hash;
            }
            public int GetHashCode2()
            {
                int[] bitArray = new int[1];
                this.OpenValves.CopyTo(bitArray, 0);

                int hash = 1430287;
                hash = hash * 7302013 ^ this.RemainingMinutes.GetHashCode();
                foreach (var v in this.RemainingPathMinutes)
                    hash = hash * 7302013 ^ v.GetHashCode();
                hash = hash * 7302013 ^ bitArray[0].GetHashCode();
                return hash;
            }

            public override int GetHashCode()
            {
                int[] bitArray = new int[1];
                this.OpenValves.CopyTo(bitArray, 0);

                int hash = 1430287;
                foreach (var id in this.NodeId)
                    hash = hash * 7302013 ^ id.GetHashCode();
                hash = hash * 7302013 ^ this.RemainingMinutes.GetHashCode();
                foreach (var v in this.RemainingPathMinutes)
                    hash = hash * 7302013 ^ v.GetHashCode();
                hash = hash * 7302013 ^ bitArray[0].GetHashCode();
                return hash;
            }
        }

        private struct Result
        {
            public int Pressure;
#if DEBUG_OUTPUT
            public string Path;
#endif
        }

        private class Valve
        {
            public string Title { get; private set; }
            public int Rate { get; private set; }

            public Valve(string index, int rate)
            {
                Title = index;
                Rate = rate;
            }

            public override string ToString()
            {
                return Title;
            }
        }
    }
}
