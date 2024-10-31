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


      if (inFileName.Contains("BBR"))
      {
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
      else if (inFileName.Contains("EJF"))
      {
        int nFeatureCollection = 0;
        int nCurlyBrackets = 0;
        string txt = "";

        bool data_ejerskab_med_stamoplysninger = false;
        if (inFileName.Contains("stamopl"))
          data_ejerskab_med_stamoplysninger = true;

        FileStream fs = new FileStream(Path.GetFileNameWithoutExtension(inFileName) + ".ndjson", FileMode.Create);
        outFile2 = new StreamWriter(fs, Encoding.UTF8, 512);

        while (!inFile.EndOfStream)
        {
          string line = inFile.ReadLine();
          //if (line.Contains("FeatureCollection"))
          if (line == "\"features\": [")
          {
            nFeatureCollection += 1;

            if (nFeatureCollection > 1)
            {
              Console.WriteLine("Mere end én FeatureCollection i fil...");
              return;
            }
          }

          if (nFeatureCollection > 0 && !line.Contains("administratoroplysninger") && !line.Contains("ejendeVirksomhed") && !line.Contains("ejendePerson") && !line.Contains("ejeroplysninger"))
          {
            if (line.Contains('{'))
            {
              nCurlyBrackets += line.Count(t => t == '{');

              txt += line;
            }
            else if (line.Contains('}'))
            {
              txt += line;

              nCurlyBrackets -= line.Count(t => t == '}');

              if (nCurlyBrackets == 0)
              {
                if (txt.EndsWith(",") || txt.EndsWith("]"))
                  txt = txt.Remove(txt.Length - 1);

                outFile2.WriteLine(txt);
                txt = "";
              }
            }
            else if (nCurlyBrackets > 0)
              txt += line;
          }
        }

        inFile.Close();
        outFile2.Close();

        Console.WriteLine("Done!");
      }
      else
      {
        Console.WriteLine("Inputfil skal være BBR eller EJF data...");
        return;
      }
    }
  }
}