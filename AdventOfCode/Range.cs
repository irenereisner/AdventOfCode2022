using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode
{
    public class Range
    {
        public int Start { get; set; }
        public int End { get; set; }

        public Range(int start, int end)
        {
            Start = start;
            End = end;
        }

        public bool IsInside(int x)
        {
            return Start <= x && x <= End;
        }


        public IEnumerable<Range> RemoveSubRange(Range range)
        {
            if (range.Start < Start && range.End < Start)
                yield return this;

            if (range.Start > End && range.End > End)
                yield return this;

            if (range.Start <= Start && range.End >= End)
                yield break;

            if (range.Start > Start && range.Start < End)
                yield return new Range(Start, range.Start-1);

            if (range.End < End && range.End > Start)
                yield return new Range(range.End+1, End);
        }

        public int Length
        {
            get { return End - Start + 1; }
        }

        public IEnumerable<int> GetValues()
        {
            for (int x = Start; x <= End; x++)
                yield return x;
        }

        public override string ToString()
        {
            return "{" + Start + "," + End + "}";
        }
    }
}
