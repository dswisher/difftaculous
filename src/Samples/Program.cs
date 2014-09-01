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
using System.Collections.Generic;
using System.Linq;


namespace Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            var samples = LoadSamples();

            if (args.Length == 0)
            {
                ListSamples(samples);
                return;
            }

            foreach (var sample in GetDesiredSamples(samples, args))
            {
                Console.WriteLine("*** {0} ***", sample.Title);
                Console.WriteLine();

                sample.Run();

                Console.WriteLine();
            }
        }



        private static List<ISample> LoadSamples()
        {
            return typeof(ISample).Assembly
                .GetTypes()
                .Where(x => x.GetInterfaces().Contains(typeof(ISample)) && !x.IsAbstract)
                .Select(t => (ISample)Activator.CreateInstance(t))
                .OrderBy(x => x.Title)
                .ToList();
        }



        private static void ListSamples(List<ISample> samples)
        {
            Console.WriteLine("*** SAMPLES ***");
            Console.WriteLine();

            for (int i = 0; i < samples.Count; i++)
            {
                Console.WriteLine("{0,2}. {1}", i+1, samples[i].Title);
            }

            Console.WriteLine();
            Console.WriteLine("As command-line parameters, specify the number(s) of the");
            Console.WriteLine("samples you would like to run, or enter 'all'.");
        }



        private static IEnumerable<ISample> GetDesiredSamples(List<ISample> samples, string[] args)
        {
            HashSet<int> wanted = new HashSet<int>();

            bool all = false;
            if (args.Contains("all"))
            {
                all = true;
            }
            else
            {
                args.Select(int.Parse).ToList().ForEach(x => wanted.Add(x));
            }

            return samples.Where((t, i) => all || wanted.Contains(i + 1));
        }
    }
}
