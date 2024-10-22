using System.IO;
using System.IO.Compression;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConvertToNDJASON
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
                return;

            string inFileName = args[0];
        
            if (!File.Exists(inFileName))
            {
                Console.WriteLine("File not found: " + inFileName);
                return;
            }

            string outFileName = Path.GetFileNameWithoutExtension(inFileName) + ".ndjson";

            StreamReader inFile = new StreamReader(inFileName);
            //StreamWriter outFile = new StreamWriter(outFileName);

            StreamWriter outFile2 = null;

            Console.WriteLine("Processing..");

            bool inCurlyBrackets = false;
            string txt = "";
            string curList = "";
            while (!inFile.EndOfStream)
            {
                string line = inFile.ReadLine();
                if (line.Contains("["))
                {
                    curList = line.Split(":")[0];

                    FileStream fs = new FileStream(Path.GetFileNameWithoutExtension(inFileName) + "_" + curList.Replace("\"", "") + ".ndjson", FileMode.Create);
                    outFile2 = new StreamWriter(fs, Encoding.UTF8, 512);
                }

                if (line.Contains('{'))
                {
                    inCurlyBrackets = true;
                    txt = "{\"List\":" + curList + ",";
                }
                else if (line.Contains('}'))
                {
                    if (inCurlyBrackets)
                    {
                        inCurlyBrackets = false;
                        txt += line;
                        //outFile.WriteLine(txt);
                        outFile2.WriteLine(txt);
                        txt = "";
                    }
                }
                else if (inCurlyBrackets)
                    txt += line;

                if (line.Contains("]"))
                    outFile2.Close();
            }

            inFile.Close();
            //outFile.Close();

            Console.WriteLine("Done!");
        }
    }
}