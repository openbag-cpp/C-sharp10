using System;
using System.Collections.Generic;

namespace Компилятор
{
    class SyntaxAnalyzer
    {
        private LexicalAnalyzer _lexer;
        private byte _curSym;

        public SyntaxAnalyzer(LexicalAnalyzer lexer)
        {
            _lexer = lexer;
        }

        public void Parse()
        {
            Next();

            if (_curSym == LexicalAnalyzer.programsy)
            {
                Next();

                if (_curSym == LexicalAnalyzer.ident)
                {
                    Next();
                }
                else
                {
                    Error(103);
                }

                if (_curSym == LexicalAnalyzer.semicolon)
                {
                    Next();
                }
                else
                {
                    Error(102);
                }
            }

            while (_curSym != 0)
            {
                if (_curSym == LexicalAnalyzer.varsy)
                {
                    ParseVarBlock();
                }
                else if (_curSym == LexicalAnalyzer.beginsy || _curSym == LexicalAnalyzer.withsy || _curSym == LexicalAnalyzer.ident)
                {
                    ParseStatement();
                }
                else if (_curSym == LexicalAnalyzer.point)
                {
                    Next();
                }
                else
                {
                    Error(100);
                    SkipTo(new HashSet<byte> { LexicalAnalyzer.varsy, LexicalAnalyzer.beginsy, LexicalAnalyzer.withsy, LexicalAnalyzer.ident });
                }
            }
        }

        private void Next()
        {
            _curSym = _lexer.NextSym();
        }

        private void Error(byte errorCode)
        {
            InputOutput.Error(errorCode, InputOutput.PositionNow);
        }

        private void SkipTo(HashSet<byte> syncSet)
        {
            while (_curSym != 0 && !syncSet.Contains(_curSym))
            {
                Next();
            }
        }

        private void ParseVarBlock()
        {
            Next();

            while (_curSym == LexicalAnalyzer.ident)
            {
                ParseVarDeclaration();
            }
        }

        private void ParseVarDeclaration()
        {
            ParseIdentList();

            if (_curSym == LexicalAnalyzer.colon)
            {
                Next();
            }
            else
            {
                Error(101);
                if (_curSym != LexicalAnalyzer.ident && _curSym != LexicalAnalyzer.arraysy && _curSym != LexicalAnalyzer.recordsy)
                {
                    SkipTo(new HashSet<byte> { LexicalAnalyzer.arraysy, LexicalAnalyzer.recordsy, LexicalAnalyzer.semicolon });
                }
            }

            ParseType();

            if (_curSym == LexicalAnalyzer.semicolon)
            {
                Next();
            }
            else
            {
                Error(102);
                SkipTo(new HashSet<byte> { LexicalAnalyzer.ident, LexicalAnalyzer.beginsy, LexicalAnalyzer.varsy, LexicalAnalyzer.endsy });
            }
        }

        private void ParseIdentList()
        {
            if (_curSym == LexicalAnalyzer.ident)
            {
                Next();
            }
            else
            {
                Error(103);
                return;
            }

            while (_curSym == LexicalAnalyzer.comma)
            {
                Next();
                if (_curSym == LexicalAnalyzer.ident)
                {
                    Next();
                }
                else
                {
                    Error(103);
                }
            }
        }

        private void ParseType()
        {
            if (_curSym == LexicalAnalyzer.ident)
            {
                Next();
            }
            else if (_curSym == LexicalAnalyzer.arraysy)
            {
                ParseArrayType();
            }
            else if (_curSym == LexicalAnalyzer.recordsy)
            {
                ParseRecordType();
            }
            else
            {
                Error(104);
                SkipTo(new HashSet<byte> { LexicalAnalyzer.semicolon, LexicalAnalyzer.endsy });
            }
        }

        private void ParseArrayType()
        {
            Next();

            if (_curSym == LexicalAnalyzer.lbracket) Next();
            else
            {
                Error(105);
                if (_curSym != LexicalAnalyzer.intc)
                {
                    SkipTo(new HashSet<byte> { LexicalAnalyzer.ofsy, LexicalAnalyzer.semicolon });
                }
            }

            if (_curSym != LexicalAnalyzer.ofsy && _curSym != LexicalAnalyzer.semicolon)
            {
                if (_curSym == LexicalAnalyzer.intc) Next(); else { Error(106); Next(); }
                if (_curSym == LexicalAnalyzer.twopoints) Next(); else { Error(107); Next(); }
                if (_curSym == LexicalAnalyzer.intc) Next(); else { Error(106); Next(); }

                if (_curSym == LexicalAnalyzer.rbracket) Next();
                else { Error(108); SkipTo(new HashSet<byte> { LexicalAnalyzer.ofsy, LexicalAnalyzer.semicolon }); }
            }

            if (_curSym == LexicalAnalyzer.ofsy)
            {
                Next();
                ParseType();
            }
            else
            {
                if (_curSym != LexicalAnalyzer.semicolon) Error(109);
            }
        }

        private void ParseRecordType()
        {
            Next();

            while (_curSym == LexicalAnalyzer.ident)
            {
                ParseVarDeclaration();
            }

            if (_curSym == LexicalAnalyzer.endsy)
            {
                Next();
            }
            else
            {
                Error(110);
                SkipTo(new HashSet<byte> { LexicalAnalyzer.semicolon, LexicalAnalyzer.endsy });
            }
        }

        private void ParseStatement()
        {
            if (_curSym == LexicalAnalyzer.beginsy)
            {
                ParseCompoundStatement();
            }
            else if (_curSym == LexicalAnalyzer.withsy)
            {
                ParseWithStatement();
            }
            else if (_curSym == LexicalAnalyzer.ident)
            {
                ParseAssignmentStatement();
            }
            else
            {
                Error(111);
                SkipTo(new HashSet<byte> { LexicalAnalyzer.semicolon, LexicalAnalyzer.endsy });
            }
        }

        private void ParseCompoundStatement()
        {
            Next();

            while (_curSym != LexicalAnalyzer.endsy && _curSym != 0)
            {
                ParseStatement();

                if (_curSym == LexicalAnalyzer.semicolon)
                {
                    Next();
                }
                else if (_curSym != LexicalAnalyzer.endsy)
                {
                    Error(102);
                    SkipTo(new HashSet<byte> { LexicalAnalyzer.ident, LexicalAnalyzer.beginsy, LexicalAnalyzer.withsy, LexicalAnalyzer.endsy, LexicalAnalyzer.semicolon });
                    if (_curSym == LexicalAnalyzer.semicolon)
                    {
                        Next();
                    }
                }
            }

            if (_curSym == LexicalAnalyzer.endsy)
            {
                Next();
            }
            else
            {
                Error(110);
            }
        }

        private void ParseWithStatement()
        {
            Next();

            ParseVariable();
            while (_curSym == LexicalAnalyzer.comma)
            {
                Next();
                ParseVariable();
            }

            if (_curSym == LexicalAnalyzer.dosy)
            {
                Next();
            }
            else
            {
                Error(112);
            }

            ParseStatement();
        }

        private void ParseAssignmentStatement()
        {
            ParseVariable();

            if (_curSym == LexicalAnalyzer.assign)
            {
                Next();
            }
            else
            {
                Error(113);
                SkipTo(new HashSet<byte> { LexicalAnalyzer.semicolon, LexicalAnalyzer.endsy });
                return;
            }

            ParseExpression();
        }

        private void ParseVariable()
        {
            if (_curSym == LexicalAnalyzer.ident)
            {
                Next();
            }
            else
            {
                Error(103);
                return;
            }

            while (_curSym == LexicalAnalyzer.lbracket || _curSym == LexicalAnalyzer.point)
            {
                if (_curSym == LexicalAnalyzer.lbracket)
                {
                    Next();
                    ParseExpression();
                    if (_curSym == LexicalAnalyzer.rbracket)
                    {
                        Next();
                    }
                    else
                    {
                        Error(108);
                    }
                }
                else if (_curSym == LexicalAnalyzer.point)
                {
                    Next();
                    if (_curSym == LexicalAnalyzer.ident)
                    {
                        Next();
                    }
                    else
                    {
                        Error(103);
                    }
                }
            }
        }

        private void ParseExpression()
        {
            ParseSimpleExpression();
        }

        private void ParseSimpleExpression()
        {
            ParseTerm();
            while (_curSym == LexicalAnalyzer.plus || _curSym == LexicalAnalyzer.minus)
            {
                Next();
                ParseTerm();
            }
        }

        private void ParseTerm()
        {
            ParseFactor();
            while (_curSym == LexicalAnalyzer.star || _curSym == LexicalAnalyzer.slash)
            {
                Next();
                ParseFactor();
            }
        }

        private void ParseFactor()
        {
            if (_curSym == LexicalAnalyzer.intc || _curSym == LexicalAnalyzer.floatc)
            {
                Next();
            }
            else if (_curSym == LexicalAnalyzer.ident)
            {
                ParseVariable();
            }
            else if (_curSym == LexicalAnalyzer.leftpar)
            {
                Next();
                ParseExpression();
                if (_curSym == LexicalAnalyzer.rightpar)
                {
                    Next();
                }
                else
                {
                    Error(114);
                }
            }
            else
            {
                Error(115);
                Next();
            }
        }
    }
}