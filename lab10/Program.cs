using System;
using System.IO;

namespace Компилятор
{
    class Program
    {
        static void Main(string[] args)
        {
            string testFilePath = "/home/openbag/Desktop/c#-labs/lab10/lab10/Keywords.cs";
            string outputFilePath = "output.txt";

            InputOutput.Init(testFilePath);
            LexicalAnalyzer analyzer = new LexicalAnalyzer();

            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                while (InputOutput.File != null)
                {
                    byte symbolCode = analyzer.NextSym();

                    if (symbolCode == 0)
                    {
                        break;
                    }

                    writer.Write(symbolCode + " ");
                }
            }

            Console.WriteLine($"\nКоды символов успешно сохранены в файл: {outputFilePath}");
            Console.ReadKey();
        }
    }
}