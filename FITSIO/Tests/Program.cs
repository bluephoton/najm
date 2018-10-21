using System;
using System.Collections.Generic;
using System.Text;
using Najm.FITSIO;
using System.IO;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath;
            StreamReader sr = new StreamReader(@".\..\..\testfiles.txt");
            List<KeyValuePair<int, string>> times = new List<KeyValuePair<int, string>>(100);
            while (!sr.EndOfStream)
            {
                IFITSFile ff = Factory.CreateFITSFile();
                filePath = sr.ReadLine();
                if (filePath.StartsWith("#"))
                {
                    continue;
                }
                if (filePath.StartsWith("@end"))
                {
                    break;
                }
                Console.Write(string.Format("Processing: [{0}]", filePath));
                try
                {
                    int before = Environment.TickCount;
                    ff.Load(filePath);
                    int time = Environment.TickCount - before;
                    // clear line
                    Console.CursorLeft = 0;
                    Console.Write(new StringBuilder(Console.WindowWidth).Append(' ', Console.WindowWidth - 1).ToString());
                    Console.CursorLeft = 0;
                    // write time
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(time.ToString() + "\t");
                    Console.ResetColor();
                    // write file name
                    Console.Write(string.Format("@ [{0}]", filePath));
                    times.Add(new KeyValuePair<int,string>(time, filePath));
                }
                //*
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("FAILED");
                    Console.ResetColor();
                }
                //*/
                // next line
                Console.WriteLine();
            }
            Console.WriteLine("Done!");
            Console.ReadKey();

            StreamWriter w = new StreamWriter(@"c:\temp\ztimes.txt");
            foreach (KeyValuePair<int,string> p in times)
            {
                w.WriteLine(string.Format("{0}\t{1}", p.Key, p.Value));
            }
            w.Close();
        }
    }
}
