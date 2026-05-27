using System;

namespace Компилятор
{
    class LexicalAnalyzer
    {
        public const byte
            star = 21,
            slash = 60,
            equal = 16,
            comma = 20,
            semicolon = 14,
            colon = 5,
            point = 61,
            arrow = 62,
            leftpar = 9,
            rightpar = 4,
            lbracket = 11,
            rbracket = 12,
            flpar = 63,
            frpar = 64,
            later = 65,
            greater = 66,
            laterequal = 67,
            greaterequal = 68,
            latergreater = 69,
            plus = 70,
            minus = 71,
            lcomment = 72,
            rcomment = 73,
            assign = 51,
            twopoints = 74,
            ident = 2,
            floatc = 82,
            intc = 15,
            casesy = 31,
            elsesy = 32,
            filesy = 57,
            gotosy = 33,
            thensy = 52,
            typesy = 34,
            untilsy = 53,
            dosy = 54,
            withsy = 37,
            ifsy = 56,
            insy = 100,
            ofsy = 101,
            orsy = 102,
            tosy = 103,
            endsy = 104,
            varsy = 105,
            divsy = 106,
            andsy = 107,
            notsy = 108,
            forsy = 109,
            modsy = 110,
            nilsy = 111,
            setsy = 112,
            beginsy = 113,
            whilesy = 114,
            arraysy = 115,
            constsy = 116,
            labelsy = 117,
            downtosy = 118,
            packedsy = 119,
            recordsy = 120,
            repeatsy = 121,
            programsy = 122,
            functionsy = 123,
            procedurensy = 124;

        private static bool _буква
        {
            get
            {
                return (InputOutput.Ch >= 'a' && InputOutput.Ch <= 'z') ||
                       (InputOutput.Ch >= 'A' && InputOutput.Ch <= 'Z');
            }
        }

        private static bool _цифра
        {
            get
            {
                return InputOutput.Ch >= '0' && InputOutput.Ch <= '9';
            }
        }

        private byte _symbol;
        private TextPosition _token;
        private string _addrName;
        private int _nmbInt;
        private float _nmbFloat;
        private char _oneSymbol;

        private Keywords _keywordsTable = new Keywords();

        public byte NextSym()
        {
            while (InputOutput.Ch == ' ')
            {
                InputOutput.NextCh();
            }

            if (InputOutput.Ch == '\0')
            {
                return 0;
            }

            _token = new TextPosition(InputOutput.PositionNow.LineNumber, InputOutput.PositionNow.CharNumber);

            if (_цифра)
            {
                byte digit;
                Int16 maxint = Int16.MaxValue;
                _nmbInt = 0;

                while (InputOutput.Ch >= '0' && InputOutput.Ch <= '9')
                {
                    digit = (byte)(InputOutput.Ch - '0');

                    if (_nmbInt < maxint / 10 || (_nmbInt == maxint / 10 && digit <= maxint % 10))
                    {
                        _nmbInt = 10 * _nmbInt + digit;
                    }
                    else
                    {
                        InputOutput.Error(203, InputOutput.PositionNow);
                        _nmbInt = 0;

                        while (InputOutput.Ch >= '0' && InputOutput.Ch <= '9')
                        {
                            InputOutput.NextCh();
                        }
                    }

                    InputOutput.NextCh();
                }

                _symbol = intc;
            }
            else if (_буква)
            {
                string name = "";

                while ((InputOutput.Ch >= 'a' && InputOutput.Ch <= 'z') ||
                       (InputOutput.Ch >= 'A' && InputOutput.Ch <= 'Z') ||
                       (InputOutput.Ch >= '0' && InputOutput.Ch <= '9'))
                {
                    name += InputOutput.Ch;
                    InputOutput.NextCh();
                }

                string lowerName = name.ToLower();
                byte length = (byte)lowerName.Length;

                if (_keywordsTable.Kw.ContainsKey(length)
                    && _keywordsTable.Kw[length].ContainsKey(lowerName))
                {
                    _symbol = _keywordsTable.Kw[length][lowerName];
                }
                else
                {
                    _symbol = ident;
                }
            }
            else
            {
                switch (InputOutput.Ch)
                {
                    case '<':
                        {
                            InputOutput.NextCh();

                            if (InputOutput.Ch == '=')
                            {
                                _symbol = laterequal;
                                InputOutput.NextCh();
                            }
                            else if (InputOutput.Ch == '>')
                            {
                                _symbol = latergreater;
                                InputOutput.NextCh();
                            }
                            else
                            {
                                _symbol = later;
                            }
                            break;
                        }
                    case ':':
                        {
                            InputOutput.NextCh();

                            if (InputOutput.Ch == '=')
                            {
                                _symbol = assign;
                                InputOutput.NextCh();
                            }
                            else
                            {
                                _symbol = colon;
                            }
                            break;
                        }
                    case ';':
                        {
                            _symbol = semicolon;
                            InputOutput.NextCh();
                            break;
                        }
                    case '.':
                        {
                            InputOutput.NextCh();

                            if (InputOutput.Ch == '.')
                            {
                                _symbol = twopoints;
                                InputOutput.NextCh();
                            }
                            else
                            {
                                _symbol = point;
                            }
                            break;
                        }
                    default:
                        {
                            InputOutput.Error(1, InputOutput.PositionNow);
                            InputOutput.NextCh();
                            _symbol = 255;
                            break;
                        }
                }
            }
            return _symbol;
        }
    }
}