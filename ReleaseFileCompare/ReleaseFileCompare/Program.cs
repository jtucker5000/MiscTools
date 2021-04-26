using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReleaseFileCompare
{
    public class FunctionDetails
    {
       public string Name { get; set; }
       public List<string> Lines { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("Please supply a base folder to use as the working directory");
                return;
            }
            if(!Directory.Exists(args[0]))
            {
                Console.WriteLine("Directory supplied does not exist - please supply a valid working directory");
                return;
            }
            string workingDirectory = args[0].TrimEnd(new [] { '\\'});
            if (!Directory.Exists(workingDirectory + @"\a"))
                Directory.CreateDirectory(workingDirectory + @"\a");
            if (!Directory.Exists(workingDirectory + @"\b"))
                Directory.CreateDirectory(workingDirectory + @"\b");
            using (StreamReader sr = new StreamReader(workingDirectory + @"\fileList.txt"))
            {
                while(!sr.EndOfStream)
                {
                    List<FunctionDetails> fileA = new List<FunctionDetails>();
                    List<FunctionDetails> fileB = new List<FunctionDetails>();
                    string line = sr.ReadLine().Trim().Replace("/", "_");

                    fileA = GetFileDetails(workingDirectory + @"\a\" + line);
                    fileB = GetFileDetails(workingDirectory + @"\b\" + line);

                    CompareFiles(line, fileA, fileB);
                }
                sr.Close();
            }
        }

        static void CompareFiles(string path, List<FunctionDetails> fileA, List<FunctionDetails> fileB)
        {
            //we only care about functions that are in both files for now
            foreach(FunctionDetails fa in fileA)
            {
                FunctionDetails fb = fileB.FirstOrDefault(s => s.Name == fa.Name);
                if (fb == null)
                    continue;

                if(fa.Lines.Count != fb.Lines.Count)
                {
                    Console.WriteLine("Difference " + path + " -- " + fa.Name);
                }
                else
                {
                    for(int c = 0; c < fa.Lines.Count; c++)
                    {
                        if(fa.Lines[c] != fb.Lines[c])
                        {
                            Console.WriteLine("Difference " + path + " -- " + fa.Name);
                            break;
                        }
                    }
                }
            }

        }

        static List<FunctionDetails> GetFileDetails(string path)
        {
            List<FunctionDetails> fileDetails = new List<FunctionDetails>();
            FunctionDetails functionDetails = new FunctionDetails
            {
                Lines = new List<string>()
            };
            int openBracketCount = 0;
            bool insidePublicMethod = false;
            using(StreamReader sr = new StreamReader(path))
            {
                while(!sr.EndOfStream)
                {
                    string line = sr.ReadLine().Trim();
                    if (string.IsNullOrEmpty(line))
                        continue;
                    if(line.Contains("public ") && !line.Contains(" class ") && line.Contains("("))
                    {
                        insidePublicMethod = true;
                    }

                    if (!insidePublicMethod)
                        continue;

                    if(line.Contains("{"))
                    {
                        openBracketCount++;
                    }
                    if(line.Contains("}"))
                    {
                        openBracketCount--;
                    }

                    functionDetails.Lines.Add(line);

                    if(openBracketCount <= 0)
                    {
                        insidePublicMethod = false;
                        functionDetails.Name = functionDetails.Lines[0];
                        functionDetails.Name = functionDetails.Name.Split(new[] { '(' })[0];
                        functionDetails.Name = functionDetails.Name.Split(new[] { ' ' }).Last();
                        fileDetails.Add(functionDetails);
                        functionDetails = new FunctionDetails
                        {
                            Lines = new List<string>()
                        };
                    }
                }
                sr.Close();
            }
            return fileDetails;
        }
    }
}
