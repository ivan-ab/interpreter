using System;
using System.Collections.Generic;
using System.Linq;
using Interpreter.Nodes;
using Interpreter.Nodes.Expressions;
using Interpreter.Nodes.Statements;
using Interpreter.Values;

namespace Interpreter
{
    class Interpreter : IStatementVisitor, IExpressionVisitor<object>
    {
        Dictionary<string, object> variables = new Dictionary<string, object>();
        bool hadBreak;

        public Interpreter(IReadOnlyDictionary<string, object> customVariables)
        {
            variables = new Dictionary<string, object>
            {
                { "true", true },
                { "false", false },
                { "null", null },
                { "dump", new DumpFunction() }
            };
            foreach (var kv in customVariables)
            {
                variables[kv.Key] = kv.Value;
            }
        }

        public void RunProgram(ProgramNode program)
        {
            foreach (var statement in program.Statements)
            {
                Run(statement);
                if (hadBreak)
                {
                    throw Error("break снаружи цикла");
                }
            }
        }

        void Run(IStatement statement) => statement.Accept(this);

        void RunBlock(Block block)
        {
            foreach (var statement in block.Statements)
            {
                Run(statement);
                if (hadBreak)
                {
                    break;
                }
            }
        }

        public void VisitIf(IfStatement statement)
        {
            if (Calc<bool>(statement.Condition))
            {
                RunBlock(statement.Block);
            }
        }

        public void VisitWhile(WhileStatement statement)
        {
            while (Calc<bool>(statement.Condition))
            {
                RunBlock(statement.Block);
                if (hadBreak)
                {
                    hadBreak = false;
                    break;
                }
            }
        }

        public void VisitExpressionStatement(ExpressionStatement statement)
        {
            Calc(statement.Expression);
        }

        public void VisitAssignment(Assignment statement)
        {
            variables[statement.Variable] = Calc(statement.Expression);
        }

        public void VisitBreak(Break statement)
        {
            if (hadBreak)
            {
                throw new Exception("wtf");
            }

            hadBreak = true;
        }

        object Calc(IExpression expression)
        {
            return expression.Accept(this);
        }

        T Cast<T>(object value)
        {
            if (!(value is T))
            {
                throw Error($"Ожидали {typeof(T)}, получили {value}");
            }

            return (T)value;
        }

        T Calc<T>(IExpression expression)
        {
            return Cast<T>(Calc(expression));
        }

        public object VisitConditional(ConditionalExpression expression)
        {
            return Calc<bool>(expression.Condition)
                ? Calc(expression.TrueExpr)
                : Calc(expression.FalseExpr);
        }

        public object VisitBinary(BinaryOperation expression)
        {
            var left = expression.Left;
            var right = expression.Right;
            switch (expression.Operation)
            {
                case BinaryOpType.Addition:
                    return Calc<int>(left) + Calc<int>(right);
                case BinaryOpType.Subtraction:
                    return Calc<int>(left) - Calc<int>(right);
                case BinaryOpType.Multiplication:
                    return Calc<int>(left) * Calc<int>(right);
                case BinaryOpType.Division:
                    return Calc<int>(left) / Calc<int>(right);
                case BinaryOpType.Remainder:
                    return Calc<int>(left) % Calc<int>(right);
                case BinaryOpType.Equal:
                    return CalcEqual(Calc(left), Calc(right));
                case BinaryOpType.NotEqual:
                    return !CalcEqual(Calc(left), Calc(right));
                case BinaryOpType.Less:
                    return CalcLess(Calc(left), Calc(right));
                case BinaryOpType.Greater:
                    return CalcLess(b: Calc(left), a: Calc(right));
                case BinaryOpType.LessEqual:
                    return !CalcLess(b: Calc(left), a: Calc(right));
                case BinaryOpType.GreaterEqual:
                    return !CalcLess(Calc(left), Calc(right));
                case BinaryOpType.LogicalAnd:
                    return Calc<bool>(left) && Calc<bool>(right);
                case BinaryOpType.LogicalOr:
                    return Calc<bool>(left) || Calc<bool>(right);
                default:
                    throw Error($"Неизвестная операция {expression.Operation}");
            }
        }

        bool CalcEqual(object a, object b)
        {
            if (a == null || b == null)
            {
                return (a == null) == (b == null);
            }

            if (a.GetType() != b.GetType())
            {
                return false;
            }

            if (a is int)
            {
                return (int)a == (int)b;
            }

            if (a is bool)
            {
                return (bool)a == (bool)b;
            }

            throw Error($"Неверный тип операндов {a} {b}");
        }

        bool CalcLess(object a, object b)
        {
            if (a is bool && b is bool)
            {
                return !(bool)a && (bool)b;
            }

            if (a is int && b is int)
            {
                return (int)a < (int)b;
            }

            throw Error($"Неверный тип операндов {a} {b}");
        }

        public object VisitUnary(UnaryOperation expression)
        {
            switch (expression.Operation)
            {
                case UnaryOpType.UnaryMinus:
                    return -Calc<int>(expression.Expression);
                case UnaryOpType.LogicalNegation:
                    return !Calc<bool>(expression.Expression);
            }

            throw Error($"Неизвестная операция {expression.Operation}");
        }

        public object VisitParen(Paren expression) => Calc(expression.Value);

        public object VisitCall(CallExpression expression)
        {
            var value = Calc(expression.Function);
            var function = value as ICallable;
            if (function == null)
            {
                throw Error($"Вызвали не функцию, а {value}");
            }

            var args = expression.Arguments.Select(Calc).ToArray();
            return function.Call(args);
        }

        public object VisitNumber(Number expression)
        {
            return int.Parse(expression.Lexeme);
        }

        public object VisitIdentifier(Identifier expression)
        {
            object value;
            if (variables.TryGetValue(expression.Name, out value))
            {
                return value;
            }

            throw Error($"Неизвестная переменная {expression.Name}");
        }

        Exception Error(string message)
        {
            return new Exception(message);
        }
    }
}