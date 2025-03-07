using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace PdfLibCore.PdfPrint
{
    [SupportedOSPlatform("windows")]
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide the path to the PDF file(s) as command line arguments.");
                return;
            }

            Printer(args);
        }

        public static void Printer(string[] filesToPrint)
        {
            foreach (var fileName in filesToPrint)
            {
                try
                {
                    if (File.Exists(fileName))
                    {
                        Console.WriteLine($"Printing file: {fileName}");
                        FilePrinter.PrintPdf(fileName);
                        Console.WriteLine("Print Task completed.");
                    }
                    else
                    {
                        Console.WriteLine($"File does not exist: {fileName}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while printing the file {fileName}: {ex.Message}");
                }
            }
        }
    }
}