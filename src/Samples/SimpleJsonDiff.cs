#region License
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

using System;
using System.IO;
using System.Reflection;
using Difftaculous;
using Difftaculous.Adapters;
using Difftaculous.Results;


namespace Samples
{
    internal class SimpleJsonDiff : ISample
    {
        public string Title { get { return "Simple JSON Diff"; } }


        public void Run()
        {
            string a = LoadFile("SimpleJsonA.json");
            string b = LoadFile("SimpleJsonB.json");

            IDiffResult result = DiffEngine.Compare(new JsonAdapter(a), new JsonAdapter(b));

            Console.WriteLine("AreSame: {0}", result.AreSame);

            foreach (var anno in result.Annotations)
            {
                Console.WriteLine("{0}: {1}", anno.Path, anno.Message);
            }
        }



        private static string LoadFile(string name)
        {
            string path = "Samples.DataFiles." + name;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("Could not load embedded resource: " + path);
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
