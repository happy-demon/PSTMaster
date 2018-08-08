using ArgumentsParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTMaster
{
    class Program
    {
        static StringBuilder sb = new StringBuilder();
        static void Main(string[] args)
        {
            Arguments CommandLine = new Arguments(args);
                      
            // Look for specific arguments values and display 
            // them if they exist (return null if they don't)
            if (args.Length == 0)
            {
                PrintInfo("Run PSTMaster.exe /help to see usage.");
            }

            else if (CommandLine["mode"].ToLower().Equals("find") && CommandLine["location"] != null && CommandLine["collectpath"] != null)
            {
                var collectPath = CommandLine["collectpath"];

                if (!collectPath.EndsWith("\\"))
                {
                    collectPath += "\\";
                }

                if (CommandLine["location"].ToLower().Equals("alllocal"))
                {
                    DriveInfo[] TotalDrives = DriveInfo.GetDrives();

                    foreach (DriveInfo drvinfo in TotalDrives)
                    {
                        try
                        {
                            string driveLetter = drvinfo.Name.ToString();
                            ApplyAllFiles(driveLetter, ProcessFile);
                        }
                        catch (Exception e)
                        {
                            PrintError(e.Message);
                        }
                    }
                    File.WriteAllText(collectPath + "Result.csv", sb.ToString());
                }
                else
                {
                    var driveLetter = CommandLine["location"];
                    if (!driveLetter.EndsWith("\\"))
                    {
                        driveLetter += "\\";
                    }

                    try
                    {
                        ApplyAllFiles(driveLetter, ProcessFile);
                    }
                    catch (Exception e)
                    {
                        PrintError(e.Message);
                        PrintInfo("Run PSTMaster.exe /help to see usage.");
                    }
                    File.WriteAllText(collectPath + "Result.csv", sb.ToString());
                }
            }

            else
            {
                PrintInfo("Run PSTMaster.exe /help to see usage.");
            }
        }

        static void ProcessFile(string path)
        {
            Console.WriteLine(path);
        }

        static StringBuilder ApplyAllFiles(string folder, Action<string> fileAction)
        {
            foreach (string file in Directory.GetFiles(folder, "*.pst"))
            {
                fileAction(file);
                sb.AppendLine(file);
            }
            foreach (string subDir in Directory.GetDirectories(folder))
            {
                try
                {
                    ApplyAllFiles(subDir, fileAction);
                }
                catch
                {
                    // swallow, log, whatever
                }
            }
            return sb;
        }

        static void PrintInfo(string txt)
        {
            ConsoleColor currentForegroud = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(txt);
            Console.ForegroundColor = currentForegroud;
        }

        static void PrintError(string txt)
        {
            ConsoleColor currentForegroud = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(txt);
            Console.ForegroundColor = currentForegroud;
        }
    }
}
