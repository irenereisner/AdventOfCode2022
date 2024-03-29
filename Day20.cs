﻿using AdventOfCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2022
{
    [Day(20)]
    public class Day20 : Day
    {

        public override string RunPart1()
        {
            var inputList = File.ReadAllLines(InputFile).Select(l => long.Parse(l)).ToList();
            Decrypt(inputList, 1);
            var val1000 = GetValue(inputList, 1000);
            var val2000 = GetValue(inputList, 2000);
            var val3000 = GetValue(inputList, 3000);
            var sum = val1000 + val2000 + val3000;

            Console.WriteLine($"{val1000} {val2000} {val3000}");

            return sum.ToString();
        }




        public override string RunPart2()
        {

            var inputList = File.ReadAllLines(InputFile).Select(l => long.Parse(l) * 811589153).ToList();

            Decrypt(inputList, 10);
            var val1000 = GetValue(inputList, 1000);
            var val2000 = GetValue(inputList, 2000);
            var val3000 = GetValue(inputList, 3000);
            var sum = val1000 + val2000 + val3000;

            Console.WriteLine($"{val1000} {val2000} {val3000}");

            return sum.ToString();
        }

        private void Decrypt(List<long> list, int rounds)
        {
            //Console.WriteLine("list: " + string.Join(", ", list));

            var indexList = Enumerable.Range(0, list.Count).ToList();

            for (int round = 0; round < rounds; round++)
            {

                for (int i = 0; i < list.Count; i++)
                {
                    var srcIndex = indexList[i];
                    var value = list[srcIndex];

                    if (value == 0) continue;
                    var dstIndex = (int)(srcIndex + (value % (list.Count - 1)));

                    list.RemoveAt(srcIndex);

                    if (dstIndex > list.Count)
                        dstIndex -= list.Count;
                    else if (dstIndex == 0)
                        dstIndex = list.Count;
                    else if (dstIndex < 0)
                        dstIndex = list.Count + dstIndex;

                    if (dstIndex == list.Count)
                        list.Add(value);
                    else if (dstIndex < 0)
                        list.Insert(list.Count + dstIndex, value);
                    else
                        list.Insert(dstIndex, value);


                    if (dstIndex < srcIndex)
                    {
                        srcIndex++;
                    }

                    var shift = dstIndex > srcIndex ? -1 : 1;
                    var minIndex = srcIndex < dstIndex ? srcIndex : dstIndex;
                    var maxIndex = srcIndex > dstIndex ? srcIndex : dstIndex;
                    if (srcIndex < dstIndex)
                    {
                        maxIndex++;
                    }
                    else
                    {
                        minIndex--;
                    }
                    for (int j = 0; j < indexList.Count; j++)
                    {
                        if (indexList[j] > minIndex && indexList[j] < maxIndex)
                        {
                            indexList[j] += shift;
                        }
                    }
                    indexList[i] = dstIndex;



                    //Console.WriteLine($"-----------{i}({value})-----------");
                    //Console.WriteLine("list: " + string.Join(", ", list));

                    //var testList = indexList.Select(ii => list[ii]).ToList();
                    //if (!originalList.SequenceEqual(testList))
                    //   Console.WriteLine($"error at index {i}: {srcIndex} => {dstIndex}");

                }

                //Console.WriteLine($"-----------{round}-----------");
                //Console.WriteLine("list: " + string.Join(", ", list));

            }
        }

        private long GetValue(List<long> list, int index)
        {
            var zeroIndex = list.IndexOf(0);

            var i = (zeroIndex + index) % list.Count;
            return list[i];
        }

        public void Test()
        {
            var inputLists = new[]
            {
                new List<long>() {1, 2, -3, 3, -2, 0, 4 },
                new List<long>() { 0, 2, -4, 3, 6, 3, 17, -1, -19 },
                new List<long>() { 1, 2, 3, 4, 5, 1, 2, 3, 0 },
            };
            var outputLists = new[]
            {
                new List<long>() { 1, 2, -3, 4, 0, 3, -2 },
                new List<long>(){ 0, 2, 3, -4, -19, 3, -1, 17, 6 },
                new List<long>() { 1, 3, 2, 3, 1, 4, 2, 5, 0 },
            };

            for (int i = 0; i < inputLists.Length; i++)
            {
                Decrypt(inputLists[i], 1);
                Console.WriteLine("Test passed: " + inputLists[i].SequenceEqual(outputLists[i]));
                Console.WriteLine("--------------------");

            }

        }
    }
}