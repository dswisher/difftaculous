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
    /// Annotation indicating that a property is missing.
    /// </summary>
    public class MissingPropertyAnnotation : AbstractDiffAnnotation
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="propertyName">The name of the property that is missing</param>
        public MissingPropertyAnnotation(DiffPath path, string propertyName)
            : base(path)
        {
            PropertyName = propertyName;
        }


        /// <summary>
        /// The name of the missing property.
        /// </summary>
        public string PropertyName { get; private set; }


        /// <summary>
        /// A human-readable explanation of the annotation.
        /// </summary>
        public override string Message
        {
            get { return string.Format("Property '{0}' is missing.", PropertyName); }
        }
    }
}
