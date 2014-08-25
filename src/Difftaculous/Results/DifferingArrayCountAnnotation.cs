﻿#region License
//The MIT License (MIT)

//Copyright (c) 2014 Doug Swisher

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
#endregion

using Difftaculous.Paths;


namespace Difftaculous.Results
{
    /// <summary>
    /// Annotation indicating that two arrays have differing number of elements.
    /// </summary>
    public class DifferingArrayCountAnnotation : AbstractDiffAnnotation
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">The path of the array whose counts differ</param>
        /// <param name="countA">The first count</param>
        /// <param name="countB">The second count</param>
        public DifferingArrayCountAnnotation(DiffPath path, int countA, int countB)
            : base(path)
        {
            CountA = countA;
            CountB = countB;
        }


        /// <summary>
        /// The first count
        /// </summary>
        public int CountA { get; private set; }


        /// <summary>
        /// The second count
        /// </summary>
        public int CountB { get; private set; }


        /// <summary>
        /// A human-readable explanation of the annotation.
        /// </summary>
        public override string Message
        {
            get { return string.Format("array item counts differ: {0} vs. {1}", CountA, CountB); }
        }
    }
}
