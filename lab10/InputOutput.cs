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
                    {
                        Console.WriteLine("Недопустимый символ");
                        break;
                    }
                case 2:
                    {
                        Console.WriteLine("Ожидался идентификатор");
                        break;
                    }
                case 3:
                    {
                        Console.WriteLine("Ожидалась константа");
                        break;
                    }
                case 4:
                    {
                        Console.WriteLine("Ожидался символ '='");
                        break;
                    }
                case 203:
                    {
                        Console.WriteLine("Слишком большое целое число (превышен MaxInt)");
                        break;
                    }
                case 204:
                    {
                        Console.WriteLine("Незакрытая строковая константа (пропущена кавычка)");
                        break;
                    }
                case 205:
                    {
                        Console.WriteLine("Незакрытый многострочный комментарий");
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Неизвестная ошибка");
                        break;
                    }
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

        private static string _line;
        private static int _lastInLine;
        private static List<Err> _err;

        private static StreamReader _file;
        public static StreamReader File
        {
            get { return _file; }
        }

        private static uint _errCount;
        private static Random _rnd;

        static InputOutput()
        {
            _positionNow = new TextPosition(1, 0);
            _line = "";
            _lastInLine = 0;
            _err = new List<Err>();
            _errCount = 0;
            _rnd = new Random();
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
                End();
            }
        }

        public static void NextCh()
        {
            if (_file == null)
            {
                _ch = '\0';
                return;
            }

            if (_ch != ' ' && _rnd.Next(0, 100) < 5)
            {
                byte randomCode = (byte)_rnd.Next(1, 5);
                Error(randomCode, _positionNow);
            }

            if (_positionNow.CharNumber == _lastInLine)
            {
                ListThisLine();

                if (_err.Count > 0)
                {
                    ListErrors();
                }

                ReadNextLine();
                _positionNow.LineNumber = _positionNow.LineNumber + 1;
                _positionNow.CharNumber = 0;

                if (_file != null)
                {
                    _ch = _line[_positionNow.CharNumber];
                }
                else
                {
                    _ch = '\0';
                }
            }
            else
            {
                _positionNow.CharNumber++;
                _ch = _line[_positionNow.CharNumber];
            }
        }

        private static void ListThisLine()
        {
            Console.WriteLine($"{_positionNow.LineNumber,-4} | {_line}");
        }

        private static void ReadNextLine()
        {
            if (_file != null && !_file.EndOfStream)
            {
                _line = _file.ReadLine() + " ";
                _lastInLine = _line.Length - 1;
                _err = new List<Err>();
            }
            else
            {
                End();
            }
        }

        public static void End()
        {
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