using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdventOfCode2022
{

    public class Day6 : IDay
    {
        private string filename;
        private int markerLength = 4;

        public Day6(string filename)
        {
            this.filename = filename;
        }

        public string RunPart1()
        {
            markerLength = 4;
            var input = File.ReadAllText(filename);
            return GetMarkerPosition(input).ToString();
        }


        public string RunPart2()
        {
            markerLength = 14;
            var input = File.ReadAllText(filename);
            return GetMarkerPosition(input).ToString();
        }

        private int GetMarkerPosition(string input)
        {
            for(int i = markerLength; i < input.Length; i++)
            {
                var possibleMarker = input.Substring(i - markerLength, markerLength);
                if (IsMarker(possibleMarker))
                    return i;
            }
            throw new NotSupportedException("No marker found");
        }

        private bool IsMarker(string text)
        {
            return text.Distinct().Count() == text.Length;
        }
    }
}