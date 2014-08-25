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

using Difftaculous;
using Difftaculous.Adapters;


namespace jdiff
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: jdiff <file1> <file2>");
                return;
            }

            FileInfo infoA = new FileInfo(args[0]);
            FileInfo infoB = new FileInfo(args[1]);

            if (!infoA.Exists)
            {
                Console.WriteLine("File {0} not found.", infoA.FullName);
                return;
            }

            if (!infoB.Exists)
            {
                Console.WriteLine("File {0} not found.", infoB.FullName);
                return;
            }

            try
            {
                var adapterA = new JsonAdapter(ReadFile(infoA));
                var adapterB = new JsonAdapter(ReadFile(infoB));

                var result = DiffEngine.Compare(adapterA, adapterB);

                // TODO - emit better results!  (Use an emitter!)
                if (result.AreSame)
                {
                    Console.WriteLine("No differences found.");
                }
                else
                {
                    foreach (var anno in result.Annotations)
                    {
                        Console.WriteLine("{0}:", anno.Path);
                        Console.WriteLine("    {0}", anno.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled exception in main:");
                Console.WriteLine(ex);
            }
        }



        private static string ReadFile(FileInfo info)
        {
            using (var s = info.OpenText())
            {
                return s.ReadToEnd();
            }
        }
    }
}
