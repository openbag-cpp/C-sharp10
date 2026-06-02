using System;
using System.Collections.Generic;
using System.IO;

namespace Компилятор
{
    struct TextPosition
    {
        private uint _lineNumber;
        private byte _charNumber;

        public uint LineNumber
        {
            get { return _lineNumber; }
            set { _lineNumber = value; }
        }

        public byte CharNumber
        {
            get { return _charNumber; }
            set { _charNumber = value; }
        }

        public TextPosition(uint ln = 0, byte c = 0)
        {
            _lineNumber = ln;
            _charNumber = c;
        }
    }

    struct Err
    {
        private TextPosition _errorPosition;
        private byte _errorCode;

        public TextPosition ErrorPosition
        {
            get { return _errorPosition; }
            set { _errorPosition = value; }
        }

        public byte ErrorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; }
        }

        public Err(TextPosition errorPosition, byte errorCode)
        {
            _errorPosition = errorPosition;
            _errorCode = errorCode;
        }
    }

    class ErrorPrinter
    {
        public static void Print(byte code)
        {
            switch (code)
            {
                case 1:
                    Console.WriteLine("Недопустимый символ");
                    break;
                case 2:
                    Console.WriteLine("Ожидался идентификатор");
                    break;
                case 3:
                    Console.WriteLine("Ожидалась константа");
                    break;
                case 4:
                    Console.WriteLine("Ожидался символ '='");
                    break;
                case 100:
                    Console.WriteLine("Неизвестный токен на верхнем уровне программы");
                    break;
                case 101:
                    Console.WriteLine("Ожидался знак двоеточия ':'");
                    break;
                case 102:
                    Console.WriteLine("Ожидался знак точки с запятой ';'");
                    break;
                case 103:
                    Console.WriteLine("Ожидался идентификатор");
                    break;
                case 104:
                    Console.WriteLine("Ожидался тип данных (integer, real, array, record)");
                    break;
                case 105:
                    Console.WriteLine("Ожидалась открывающая квадратная скобка '['");
                    break;
                case 106:
                    Console.WriteLine("Ожидалась целочисленная константа");
                    break;
                case 107:
                    Console.WriteLine("Ожидалось двоеточие диапазона '..'");
                    break;
                case 108:
                    Console.WriteLine("Ожидалась закрывающая квадратная скобка ']'");
                    break;
                case 109:
                    Console.WriteLine("Ожидалось ключевое слово 'of'");
                    break;
                case 110:
                    Console.WriteLine("Ожидалось ключевое слово 'end'");
                    break;
                case 111:
                    Console.WriteLine("Неверный оператор (конструкция не поддерживается)");
                    break;
                case 112:
                    Console.WriteLine("Ожидалось ключевое слово 'do'");
                    break;
                case 113:
                    Console.WriteLine("Ожидался оператор присваивания ':='");
                    break;
                case 114:
                    Console.WriteLine("Ошибка парных круглых скобок");
                    break;
                case 115:
                    Console.WriteLine("Ошибка в выражении (неверный операнд)");
                    break;
                case 203:
                    Console.WriteLine("Слишком большое целое число (превышен MaxInt)");
                    break;
                case 204:
                    Console.WriteLine("Незакрытая строковая константа (пропущена кавычка)");
                    break;
                case 205:
                    Console.WriteLine("Незакрытый многострочный комментарий");
                    break;
                case 206:
                    Console.WriteLine("Ошибка закрытой скобки (многострочный комментарий)");
                    break;
                default:
                    Console.WriteLine("Неизвестная ошибка");
                    break;
            }
        }
    }

    class InputOutput
    {
        private const byte _errMax = 9;

        private static char _ch;
        public static char Ch
        {
            get { return _ch; }
        }

        private static TextPosition _positionNow;
        public static TextPosition PositionNow
        {
            get { return _positionNow; }
        }

        // Свойство для лексера: помогает вовремя обнаружить конец строки
        public static bool IsEndOfLine
        {
            get { return _positionNow.CharNumber == _lastInLine; }
        }

        private static string _line;
        private static int _lastInLine;
        private static List<Err> _err;
        private static StreamReader _file;

        public static StreamReader File
        {
            get { return _file; }
        }

        private static uint _errCount;

        static InputOutput()
        {
            _positionNow = new TextPosition(1, 0);
            _line = "";
            _lastInLine = 0;
            _err = new List<Err>();
            _errCount = 0;
        }

        public static void Init(string filePath)
        {
            _file = new StreamReader(filePath);
            _errCount = 0;
            _err = new List<Err>();

            if (!_file.EndOfStream)
            {
                _line = _file.ReadLine() + " ";
                _lastInLine = _line.Length - 1;

                _ch = _line[0];
                _positionNow.LineNumber = 1;
                _positionNow.CharNumber = 0;
            }
            else
            {
                _ch = '\0';
                if (_file != null)
                {
                    _file.Close();
                    _file = null;
                }
            }
        }

        public static void NextCh()
        {
            // Если файл уже прочитан и мы вышли за пределы последней строки
            if (_file == null && _positionNow.CharNumber >= _lastInLine)
            {
                _ch = '\0';
                return;
            }

            if (_positionNow.CharNumber == _lastInLine)
            {
                ListThisLine();

                if (_err.Count > 0)
                {
                    ListErrors();
                    _err.Clear();
                }

                ReadNextLine();
                _positionNow.LineNumber = _positionNow.LineNumber + 1;
                _positionNow.CharNumber = 0;

                if (_line != null && _line.Length > 0)
                {
                    _ch = _line[(int)_positionNow.CharNumber];
                }
                else
                {
                    _ch = '\0';
                }
            }
            else
            {
                _positionNow.CharNumber++;

                if (_line != null && _positionNow.CharNumber < _line.Length)
                {
                    _ch = _line[(int)_positionNow.CharNumber];
                }
                else
                {
                    _ch = ' ';
                }
            }
        }

        private static void ListThisLine()
        {
            // Форматирование: под строку выделяется 4 символа, чтобы всё шло ровно
            Console.WriteLine($"{_positionNow.LineNumber,-4} | {_line}");
        }

        private static void ReadNextLine()
        {
            if (_file != null && !_file.EndOfStream)
            {
                _line = _file.ReadLine() + " ";
                _lastInLine = _line.Length - 1;
            }
            else
            {
                // Тихо закрываем файл, когда он кончился. Вызов End() отсюда убран!
                if (_file != null)
                {
                    _file.Close();
                    _file = null;
                }
                _line = null;
                _lastInLine = -1;
            }
        }

        public static void End()
        {
            // КРИТИЧЕСКИЙ КЕЙС: Если парсер завершил работу, но на самой последней строчке 
            // лексер успел зафиксировать ошибку (например, незакрытую строку в конце файла)
            if (_err.Count > 0)
            {
                ListThisLine();
                ListErrors();
                _err.Clear();
            }

            Console.WriteLine($"\nКомпиляция завершена: ошибок — {_errCount}!");

            if (_file != null)
            {
                _file.Close();
                _file = null;
            }
        }

        public static void ListErrors()
        {
            foreach (Err item in _err)
            {
                _errCount++;
                string s = "*";

                if (_errCount < 10)
                {
                    s += "0";
                }

                s += $"{_errCount}*";

                // 7 — это смещение префикса "Строка | " (4 символа на номер + 3 символа на " | ")
                int targetPosition = 7 + item.ErrorPosition.CharNumber;

                while (s.Length < targetPosition)
                {
                    s += " ";
                }

                s += $"^ ошибка код {item.ErrorCode}: ";
                Console.Write(s);

                ErrorPrinter.Print(item.ErrorCode);
            }
        }

        public static void Error(byte errorCode, TextPosition position)
        {
            if (_err.Count <= _errMax)
            {
                bool exists = false;

                foreach (Err e in _err)
                {
                    if (e.ErrorPosition.CharNumber == position.CharNumber)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    Err e = new Err(position, errorCode);
                    _err.Add(e);
                }
            }
        }
    }
}