using System;
using System.Collections.Generic;
using System.Linq;
using Interpreter.Lexer;
using Interpreter.Nodes;
using Interpreter.Nodes.Expressions;
using Interpreter.Nodes.Statements;

namespace Interpreter
{
    class Parser
    {
        readonly IReadOnlyList<Token> tokens;
        int position = 0;
        Token CurrentToken => tokens[position];

        public Parser(IEnumerable<Token> tokens)
        {
            var t = tokens.Where(IsNotWhitespace).ToList();
            t.Add(new Token(TokenType.EOF, ""));
            this.tokens = t;
        }

        bool IsNotWhitespace(Token t)
        {
            switch (t.Type)
            {
                case TokenType.Whitespaces: return false;
                case TokenType.SingleLineComment: return false;
                case TokenType.MultiLineComment: return false;
            }

            return true;
        }

        void ExpectEof()
        {
            if (!IsType(TokenType.EOF))
            {
                throw ParserError($"Не допарсили до конца, остался {CurrentToken}");
            }
        }

        void ReadNextToken()
        {
            position += 1;
        }

        void Reset()
        {
            position = 0;
        }

        Exception ParserError(string message)
        {
            var result = string.Join(" ", tokens.Select(x => x.Lexeme));
            var length = tokens.Take(position).Select(x => x.Lexeme.Length + 1).Sum();
            var pointer = new string(' ', length) + '^';
            return new Exception(string.Join("\n", message, result, pointer));
        }

        bool SkipIf(string s)
        {
            if (CurrentIs(s))
            {
                ReadNextToken();
                return true;
            }

            return false;
        }

        bool CurrentIs(string s) => string.Equals(CurrentToken.Lexeme, s, StringComparison.Ordinal);
        bool IsType(TokenType type) => CurrentToken.Type == type;

        void Expect(string s)
        {
            if (!SkipIf(s))
            {
                throw ParserError($"Ожидали \"{s}\", получили {CurrentToken}");
            }
        }

        IExpression ParsePrimaryExpression()
        {
            if (SkipIf("("))
            {
                var expression = new Paren(ParseExpression());
                Expect(")");
                return expression;
            }

            if (IsType(TokenType.NumberLiteral))
            {
                var lexeme = CurrentToken.Lexeme;
                ReadNextToken();
                return new Number(lexeme);
            }
            else if (IsType(TokenType.Identifier))
            {
                var lexeme = CurrentToken.Lexeme;
                ReadNextToken();
                return new Identifier(lexeme);
            }

            throw ParserError($"Ожидали идентификатор, число или скобку, получили {CurrentToken}");
        }

        IExpression ParseCallExpression()
        {
            var expression = ParsePrimaryExpression();
            while (true)
            {
                if (SkipIf("("))
                {
                    var arguments = new List<IExpression>();
                    if (!CurrentIs(")"))
                    {
                        arguments.Add(ParseExpression());
                        while (SkipIf(","))
                        {
                            arguments.Add(ParseExpression());
                        }
                    }

                    Expect(")");
                    expression = new CallExpression(expression, arguments);
                }
                else
                {
                    break;
                }
            }

            return expression;
        }

        IExpression ParseUnaryExpression()
        {
            if (SkipIf("-"))
            {
                var expression = ParseUnaryExpression();
                return new UnaryOperation(UnaryOpType.UnaryMinus, expression);
            }
            else if (SkipIf("!"))
            {
                var expression = ParseUnaryExpression();
                return new UnaryOperation(UnaryOpType.LogicalNegation, expression);
            }

            return ParseCallExpression();
        }

        IExpression ParseMultiplicativeExpression()
        {
            var left = ParseUnaryExpression();
            while (true)
            {
                if (SkipIf("*"))
                {
                    var right = ParseUnaryExpression();
                    left = new BinaryOperation(left, BinaryOpType.Multiplication, right);
                }
                else if (SkipIf("/"))
                {
                    var right = ParseUnaryExpression();
                    left = new BinaryOperation(left, BinaryOpType.Division, right);
                }
                else if (SkipIf("%"))
                {
                    var right = ParseUnaryExpression();
                    left = new BinaryOperation(left, BinaryOpType.Remainder, right);
                }
                else break;
            }

            return left;
        }

        IExpression ParseAdditiveExpression()
        {
            var left = ParseMultiplicativeExpression();
            while (true)
            {
                if (SkipIf("+"))
                {
                    var right = ParseMultiplicativeExpression();
                    left = new BinaryOperation(left, BinaryOpType.Addition, right);
                }
                else if (SkipIf("-"))
                {
                    var right = ParseMultiplicativeExpression();
                    left = new BinaryOperation(left, BinaryOpType.Subtraction, right);
                }
                else break;
            }

            return left;
        }

        IExpression ParseRelationalExpression()
        {
            var left = ParseAdditiveExpression();
            while (true)
            {
                if (SkipIf("<"))
                {
                    var right = ParseAdditiveExpression();
                    left = new BinaryOperation(left, BinaryOpType.Less, right);
                }

                if (SkipIf("<="))
                {
                    var right = ParseAdditiveExpression();
                    left = new BinaryOperation(left, BinaryOpType.LessEqual, right);
                }

                if (SkipIf(">"))
                {
                    var right = ParseAdditiveExpression();
                    left = new BinaryOperation(left, BinaryOpType.Greater, right);
                }

                if (SkipIf(">="))
                {
                    var right = ParseAdditiveExpression();
                    left = new BinaryOperation(left, BinaryOpType.GreaterEqual, right);
                }
                else break;
            }

            return left;
        }

        IExpression ParseEqualityExpression()
        {
            var left = ParseRelationalExpression();
            while (true)
            {
                if (SkipIf("=="))
                {
                    var right = ParseRelationalExpression();
                    left = new BinaryOperation(left, BinaryOpType.Equal, right);
                }

                if (SkipIf("!="))
                {
                    var right = ParseRelationalExpression();
                    left = new BinaryOperation(left, BinaryOpType.NotEqual, right);
                }
                else break;
            }

            return left;
        }

        IExpression ParseLogicalAndExpression()
        {
            var left = ParseEqualityExpression();
            while (true)
            {
                if (SkipIf("&&"))
                {
                    var right = ParseEqualityExpression();
                    left = new BinaryOperation(left, BinaryOpType.LogicalAnd, right);
                }
                else break;
            }

            return left;
        }

        IExpression ParseLogicalOrExpression()
        {
            var left = ParseLogicalAndExpression();
            while (true)
            {
                if (SkipIf("||"))
                {
                    var right = ParseLogicalAndExpression();
                    left = new BinaryOperation(left, BinaryOpType.LogicalOr, right);
                }
                else break;
            }

            return left;
        }

        IExpression ParseConditionalExpression()
        {
            var condition = ParseLogicalOrExpression();
            if (SkipIf("?"))
            {
                var trueExpr = ParseExpression();
                Expect(":");
                var falseExpr = ParseExpression();
                return new ConditionalExpression(condition, trueExpr, falseExpr);
            }

            return condition;
        }

        IExpression ParseExpression() => ParseConditionalExpression();

        Block ParseBlock()
        {
            Expect("{");
            var statements = new List<IStatement>();
            while (!SkipIf("}"))
            {
                statements.Add(ParseStatement());
            }

            return new Block(statements);
        }

        WhileStatement ParseWhileStatement()
        {
            Expect("while");
            Expect("(");
            var condition = ParseExpression();
            Expect(")");
            var block = ParseBlock();
            return new WhileStatement(condition, block);
        }

        IfStatement ParseIfStatement()
        {
            Expect("if");
            Expect("(");
            var condition = ParseExpression();
            Expect(")");
            var block = ParseBlock();
            return new IfStatement(condition, block);
        }

        IStatement ParseStatement()
        {
            if (CurrentIs("if"))
            {
                return ParseIfStatement();
            }

            if (CurrentIs("while"))
            {
                return ParseWhileStatement();
            }

            if (SkipIf("break"))
            {
                Expect(";");
                return new Break();
            }

            var expression = ParseExpression();
            if (SkipIf("="))
            {
                if (!(expression is Identifier))
                {
                    throw ParserError("Присваивание не в переменную");
                }

                var restAssigmentExpression = ParseExpression();
                Expect(";");
                return new Assignment((expression as Identifier).Name, restAssigmentExpression);
            }
            else
            {
                Expect(";");
                return new ExpressionStatement(expression);
            }
        }

        public ProgramNode ParseProgram()
        {
            Reset();
            var statements = new List<IStatement>();
            while (!IsType(TokenType.EOF))
            {
                statements.Add(ParseStatement());
            }

            var result = new ProgramNode(statements);
            ExpectEof();
            return result;
        }
    }
}