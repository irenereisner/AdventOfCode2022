using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2022
{
    [Day(13)]
    public class Day13 : Day
    {
        public override string RunPart1()
        {
            var lines = File.ReadAllLines(InputFile);

            var comparer = new ItemComparer();


            var sumOfCorrectPairs = 0;
            for(int i = 0, index=1; i < lines.Length; i+=3, index++)
            {
                var leftPacket = ReadItem(lines[i]);
                var rightPacket = ReadItem(lines[i+1]);

                Console.WriteLine("left:  " + leftPacket);
                Console.WriteLine("right: " + rightPacket);


                var result = comparer.Compare(leftPacket, rightPacket);
                Console.WriteLine($"{index}: {result}");
                Console.WriteLine();

                if (result == 1)
                    sumOfCorrectPairs += index;
            }
            return sumOfCorrectPairs.ToString();
        }


        public override string RunPart2()
        {
            var lines = File.ReadAllLines(InputFile);

            var allItems = new List<Item>();

            for (int i = 0, index = 1; i < lines.Length; i += 3, index++)
            {
                var leftPacket = ReadItem(lines[i]);
                var rightPacket = ReadItem(lines[i + 1]);
                allItems.Add(leftPacket);
                allItems.Add(rightPacket);
            }

            var divider0 = ReadItem("[[2]]");
            var divider1 = ReadItem("[[6]]");
            allItems.Add(divider0);
            allItems.Add(divider1);

            var comparer = new ItemComparer();
            var sorted = allItems.OrderByDescending(x => x, comparer).ToList();
            foreach (var item in sorted)
                Console.WriteLine(item);


            var index0 = sorted.IndexOf(divider0) + 1;
            var index1 = sorted.IndexOf(divider1) + 1;
            return (index0 * index1).ToString();
        }

        private Item ReadItem(string input)
        {
            if(input.StartsWith("["))
            {
                var trimmed = input.Substring(1, input.Length - 2);
                if (trimmed.Length == 0)
                    return new Item(new List<Item>());

                var substrings = new List<string>();

                var openCount = 0;
                var lastIndex = 0;
                for(int i = 0;i < trimmed.Length; i++)
                {
                    if (trimmed[i] == '[')
                        openCount++;
                    else if (trimmed[i] == ']')
                        openCount--;
                    else if (trimmed[i] == ',' && openCount == 0)
                    {
                        substrings.Add(trimmed.Substring(lastIndex, i - lastIndex));
                        lastIndex = i + 1;
                    }
                }
                substrings.Add(trimmed.Substring(lastIndex, trimmed.Length - lastIndex));

               
                var items = substrings.Select(subItem => ReadItem(subItem));
                return new Item(items);
            }
            return new Item(int.Parse(input));
        }

        private class ItemComparer : Comparer<Item>
        {
            public override int Compare(Item left, Item right)
            {
                if (left.IsInteger && right.IsInteger)
                {
                    return right.SingleValue.CompareTo(left.SingleValue);
                }

                if (left.IsList && right.IsList)
                {
                    var minCount = Math.Min(left.Values.Count, right.Values.Count);
                    for (int i = 0; i < minCount; i++)
                    {
                        var result = Compare(left.Values[i], right.Values[i]);
                        if (result != 0)
                            return result;
                    }
                    return right.Values.Count.CompareTo(left.Values.Count);
                }

                if (!left.IsList)
                {
                    var tempList = new Item(new List<Item>() { left });
                    return Compare(tempList, right);
                }
                else
                {
                    var tempList = new Item(new List<Item>() { right });
                    return Compare(left, tempList);
                }
            }

        }


        private class Item
        {
            public int SingleValue;
            public List<Item> Values;

            public Item(int singleValue)
            {
                SingleValue = singleValue;
            }
            public Item(IEnumerable<Item> items)
            {
                Values = items.ToList();
            }
            public bool IsList { get { return Values != null; } }
            public bool IsInteger { get { return Values == null; } }

            public override string ToString()
            {
                if (IsInteger)
                    return SingleValue.ToString();

                return "[" + string.Join(",", Values) + "]";
            }
        }
    }
}