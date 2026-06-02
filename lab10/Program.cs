using System;
using System.IO;

namespace Компилятор
{
    class Program
    {
        static void Main(string[] args)
        {
            string testFilePath = "/home/openbag/Desktop/c#-labs/lab10/lab10/test.pas";
            string outputFilePath = "output.txt";

            if (!File.Exists(testFilePath))
            {
                Console.WriteLine($"Ошибка: Файл {testFilePath} не найден!");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("---Лексический анализатор---");

            InputOutput.Init(testFilePath);
            LexicalAnalyzer lexerForLog = new LexicalAnalyzer();

            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                while (true)
                {
                    byte symbolCode = lexerForLog.NextSym();

                    if (symbolCode == 0)
                    {
                        break;
                    }

                    writer.Write(symbolCode + " ");
                }
            }
            InputOutput.End();
            Console.WriteLine($"Коды символов успешно сохранены в файл: {outputFilePath}\n");


            Console.WriteLine("---Синтаксический анализатор---");

            InputOutput.Init(testFilePath);

            LexicalAnalyzer syntaxLexer = new LexicalAnalyzer();
            SyntaxAnalyzer syntaxAnalyzer = new SyntaxAnalyzer(syntaxLexer);

            syntaxAnalyzer.Parse();

            InputOutput.End();

            Console.WriteLine("\nРабота компилятора завершена");
        }
    }
}