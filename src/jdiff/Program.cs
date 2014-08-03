
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
                var contentA = new JsonAdapter(ReadFile(infoA));
                var contentB = new JsonAdapter(ReadFile(infoB));

                var result = Diff.Compare(contentA, contentB);

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
