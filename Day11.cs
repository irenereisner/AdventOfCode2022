using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace AdventOfCode2022
{

    public class Day11 : IDay
    {
        private string filename;

        private Dictionary<int, Monkey> monkeys = new Dictionary<int, Monkey>();

        public Day11(string filename)
        {
            this.filename = filename;
        }

        public string RunPart1()
        {
            var lines = File.ReadAllLines(filename);

            ReadInput(lines);
            RunOperations(20, true);

            var inspections = monkeys.Values.Select(m => m.Inspections).OrderByDescending(m => m).Take(2).ToList();

            return (inspections[0] * inspections[1]).ToString();
        }


        public string RunPart2()
        {
            var lines = File.ReadAllLines(filename);
            ReadInput(lines);
            RunOperations(10000, false);

            var inspections = monkeys.Values.Select(m => m.Inspections).OrderByDescending(m => m).Take(2).ToList();

            return ((long)inspections[0] * inspections[1]).ToString();
        }


        private int GetMonkeyIndex(string line)
        {
            var part2 = line.Substring(line.IndexOf(' ')+1);
            part2 = part2.Substring(0, part2.Length - 1);
            return int.Parse(part2);
        }

        private void ReadInput(string[] lines)
        {
            for (int i = 0; i < lines.Length; i += 7)
            {
                var monkeyIndex = GetMonkeyIndex(lines[i]);
                var monkey = new Monkey(monkeyIndex);
                monkey.ReadThrowRoutine(lines.Skip(i + 3).Take(3).ToList());
                monkey.ReadStartingItems(lines[i + 1]);
                monkey.ReadOperation(lines[i + 2]);
                monkeys[monkeyIndex] = monkey;
            }


            var allDivisors = monkeys.Select(m => m.Value.Divisor).ToList();
            foreach(var monkey in monkeys)
            {
                foreach (var item in monkey.Value.Items)
                    item.AddDivisors(allDivisors);
            }
        }

        private void RunOperations(int rounds, bool reduceWorryLevel)
        {


            for (int round = 0; round < rounds; round++)
            {
                for (int monkeyIndex = 0; monkeyIndex < monkeys.Count; monkeyIndex++)
                {
                    var monkey = monkeys[monkeyIndex];

                    monkey.DoOperation();
                    if(reduceWorryLevel) 
                        monkey.GetBored();
                    monkey.TestAndThrow(monkeys);

                }

                if(round == 0 || round == 19 || (round+1) % 1000 == 0)
                {
                    Console.WriteLine($"== After round {round+1} ==");
                    foreach (var monkey in monkeys.Values)
                    {
                        Console.WriteLine(monkey);
                    }
                    Console.WriteLine();
                }
            }
        }


        private class Monkey
        {
            public int Index;
            public List<MultiNumber> Items = new List<MultiNumber>();
            public int Inspections = 0;
            public string Operand1;
            public string Operand2;
            public string Operation;
            public int Divisor = 1;
            public int ThrowIndexTrue;
            public int ThrowIndexFalse;

            public Monkey(int index)
            {
                Index = index;
            }

            public void ReadStartingItems(string input)
            {
                Items = input.Split(':').Last().Split(',').Select(str => 
                    new MultiNumber(int.Parse(str.Trim()), Divisor)).ToList();
            }

            public void ReadOperation(string input)
            {
                var op = input.Split(':').Last().Trim();
                var calc = op.Split('=').Last().Trim();

                var calcItems = calc.Split(' ').Select(str => str.Trim()).ToList();
                Operand1 = calcItems[0];
                Operation = calcItems[1];
                Operand2 = calcItems[2];
            }

            public void DoOperation()
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    var oldValue = Items[i];
                    var newValue = oldValue;

                    if(Operation == "+")
                    {
                        if (Operand1 == "old" && Operand2 == "old")
                        {
                            Items[i].Multiply(2);
                        }
                        else if (Operand1 == "old")
                        {
                            Items[i].Add(int.Parse(Operand2));
                        }
                        else
                            throw new NotImplementedException();
                    }
                    else if(Operation == "*")
                    {
                        if (Operand1 == "old" && Operand2 == "old")
                        {
                            Items[i].Square();
                        }
                        else if (Operand1 == "old")
                        {
                            Items[i].Multiply(int.Parse(Operand2));
                        }
                        else
                            throw new NotImplementedException();
                    }

                    Inspections++;
                }
            }

            public void DoOperationOld()
            {
                /*for (int i = 0; i < Items.Count; i++)
                {
                    var oldValue = Items[i];
                    var newValue = oldValue;
                    var operand1 = Operand1 == "old" ? oldValue : long.Parse(Operand1);
                    var operation = Operation;
                    var operand2 = Operand2 == "old" ? oldValue : long.Parse(Operand2);
                    if (operation == "+")
                        newValue = operand1 + operand2;
                    else if (operation == "-")
                        newValue = operand1 - operand2;
                    else if (operation == "*")
                        newValue = operand1 * operand2;
                    Items[i] = newValue;

                    Inspections++;
                }*/
            }

            public void GetBored()
            {
                for(int i = 0; i < Items.Count; i++)
                {
                    Items[i].Divide(3);
                    //Items[i] = Items[i] / 3;
                }
            }

            public void ReadThrowRoutine(List<string> inputs)
            {
                Divisor = int.Parse(inputs[0].Split(' ').Last());
                ThrowIndexTrue = int.Parse(inputs[1].Split(' ').Last());
                ThrowIndexFalse = int.Parse(inputs[2].Split(' ').Last());
            }
            
            public void TestAndThrow(Dictionary<int, Monkey> allMonkeys)
            {
                for(int i = 0; i < Items.Count; i++)
                {
                    var isDivisible = Items[i].IsDivisible(Divisor); // Items[i] % (long)Divisor == 0;
                    var monkeyIndex = isDivisible ? ThrowIndexTrue : ThrowIndexFalse;
                    allMonkeys[monkeyIndex].Items.Add(Items[i]);
                }
                // all items are thrown away
                Items.Clear();
            }

            public override string ToString()
            {
                //return $"Monkey {Index}: {Inspections} inspections";
                return $"Monkey {Index}: [{string.Join(",", Items)}] ({Inspections} inspections)";
            }
        }



        private class MultiNumber
        {
            Dictionary<int, Number> All = new Dictionary<int, Number>();

            public MultiNumber(int number, IEnumerable<int> divisors)
            {
                foreach(var divisor in divisors)
                {
                    All.Add(divisor, new Number(number, divisor));
                }
            }
            public MultiNumber(int number, int divisor)
            {
                All.Add(divisor, new Number(number, divisor));
            }

            public bool IsDivisible(int divisor)
            {
                return All[divisor].Remainder == 0;
            }

            public void AddDivisors(IEnumerable<int> divisors)
            {
                var number = (int)GetNumber();
                foreach(var divisor in divisors)
                {
                    if(!All.ContainsKey(divisor))
                        All.Add(divisor, new Number(number, divisor));
                }
            }

            public long GetNumber()
            {
                return All.First().Value.GetNumber();
            }

            public void Add(int number)
            {
                foreach(var n in All.Values)
                {
                    n.Add(number);
                }
            }
            public void Multiply(int number)
            {
                foreach (var n in All.Values)
                {
                    n.Multiply(number);
                }
            }
            public void Divide(int number)
            {
                foreach (var n in All.Values)
                {
                    n.Divide(number);
                }
            }


            public void Square()
            {

                foreach (var n in All.Values)
                {
                    n.Square();
                }
            }

            public override string ToString()
            {
                return All.Values.First().ToString();
            }


        }

        private class Number
        {
            public readonly int Divisor;
            public int Count;
            public int Remainder;

            public Number(int number, int divisor)
            {
                Divisor = divisor;
                Count = 0;
                Remainder = number;
                FitRemainder();
            }

            public long GetNumber()
            {
                return (long)Count * Divisor + Remainder;
            }

            public void Add(int number)
            {
                Remainder += number;
                FitRemainder();
            }

            public void Multiply(int number)
            {
                Count *= number;
                Remainder *= number;
                FitRemainder();

            }

            public void Square()
            {
                Count = Count * Count * Divisor + 2 * Count * Remainder;
                Remainder = Remainder * Remainder;
                FitRemainder(); 
            }

            public void Divide(int d)
            {
                var val = (Count * Divisor + Remainder) / d;
                Count = 0;
                Remainder = val;
                FitRemainder();
            }

            private void FitRemainder()
            {
                Count += Remainder / Divisor;
                Remainder = Remainder % Divisor;
            }

            public override string ToString()
            {
                return ((long)Divisor * Count + Remainder).ToString();
            }
        }
    }
}