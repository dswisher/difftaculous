﻿
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Difftaculous.ZModel;


namespace Difftaculous.Paths
{
    internal class ArraySliceFilter : PathFilter
    {
        public int? Start { get; set; }
        public int? End { get; set; }
        public int? Step { get; set; }


        public override IEnumerable<ZToken> ExecuteFilter(IEnumerable<ZToken> current, bool errorWhenNoMatch)
        {
            if (Step == 0)
            {
                throw new JsonPathException("Step cannot be zero.");
            }

            foreach (ZToken t in current)
            {
                ZArray a = t as ZArray;
                if (a != null)
                {
                    // set defaults for null arguments
                    int stepCount = Step ?? 1;
                    int startIndex = Start ?? ((stepCount > 0) ? 0 : a.Count - 1);
                    int stopIndex = End ?? ((stepCount > 0) ? a.Count : -1);

                    // start from the end of the list if start is negitive
                    if (Start < 0) startIndex = a.Count + startIndex;

                    // end from the start of the list if stop is negitive
                    if (End < 0) stopIndex = a.Count + stopIndex;

                    // ensure indexes keep within collection bounds
                    startIndex = Math.Max(startIndex, (stepCount > 0) ? 0 : int.MinValue);
                    startIndex = Math.Min(startIndex, (stepCount > 0) ? a.Count : a.Count - 1);
                    stopIndex = Math.Max(stopIndex, -1);
                    stopIndex = Math.Min(stopIndex, a.Count);

                    bool positiveStep = (stepCount > 0);

                    if (IsValid(startIndex, stopIndex, positiveStep))
                    {
                        for (int i = startIndex; IsValid(i, stopIndex, positiveStep); i += stepCount)
                        {
                            yield return a[i];
                        }
                    }
                    else
                    {
                        if (errorWhenNoMatch)
                        {
                            throw new JsonPathException(string.Format("Array slice of {0} to {1} returned no results.",
                                Start != null ? Start.Value.ToString(CultureInfo.InvariantCulture) : "*",
                                End != null ? End.Value.ToString(CultureInfo.InvariantCulture) : "*"));
                        }
                    }
                }
                else
                {
                    if (errorWhenNoMatch)
                    {
                        throw new JsonPathException(string.Format("Array slice is not valid on {0}.", t.GetType().Name));
                    }
                }

            }
        }



        private bool IsValid(int index, int stopIndex, bool positiveStep)
        {
            if (positiveStep)
                return (index < stopIndex);

            return (index > stopIndex);
        }



        public override void AddJsonPath(StringBuilder sb)
        {
            throw new NotImplementedException();
        }
    }
}
