using System;
using System.IO;

namespace Компилятор
{
    class Program
    {
        static void Main()
        {
            string testFileName = "test.pas";

            string testCode =
                "program Test;\n" +
                "var a, b: integer;\n" +
                "begin\n" +
                "  a := 10;\n" +
                "  b := 05;\n" +
                "  writeln(a)\n" +
                "end.";

            File.WriteAllText(testFileName, testCode);

            Console.WriteLine("Тестирование модуля ввода-вывода");
            Console.WriteLine($"Загрузка файла: {testFileName}\n");

            InputOutput.Init(testFileName);

            while (!InputOutput.IsEndOfFile)
            {
                if (InputOutput.positionNow.lineNumber == 1 && InputOutput.positionNow.charNumber == 8)
                {
                    InputOutput.Error(10, InputOutput.positionNow);
                }

                if (InputOutput.positionNow.lineNumber == 6 && InputOutput.positionNow.charNumber == 12)
                {
                    InputOutput.Error(4, InputOutput.positionNow);
                }

                InputOutput.NextCh();
            }

            if (File.Exists(testFileName))
            {
                File.Delete(testFileName);
            }

            Console.ReadKey();
        }
    }
}