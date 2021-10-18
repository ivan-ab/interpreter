using System.Collections.Generic;

namespace Interpreter.Nodes.Expressions
{
    class BinaryOperation : IExpression
    {
        public readonly IExpression Left;
        public readonly BinaryOpType Operation;
        public readonly IExpression Right;

        public BinaryOperation(IExpression left, BinaryOpType operation, IExpression right)
        {
            Left = left;
            Operation = operation;
            Right = right;
        }

        static readonly IReadOnlyDictionary<BinaryOpType, string> operators = new Dictionary<BinaryOpType, string>
        {
            { BinaryOpType.Addition, "+" },
            { BinaryOpType.Subtraction, "-" },
            { BinaryOpType.Multiplication, "*" },
            { BinaryOpType.Division, "/" },
            { BinaryOpType.Remainder, "%" },
            { BinaryOpType.Equal, "==" },
            { BinaryOpType.LogicalAnd, "&&" },
            { BinaryOpType.LogicalOr, "||" },
            { BinaryOpType.NotEqual, "!=" },
            { BinaryOpType.Less, "<" },
            { BinaryOpType.Greater, ">" },
            { BinaryOpType.LessEqual, "<=" },
            { BinaryOpType.GreaterEqual, ">=" },
        };

        public string FormattedString => $"{Left.FormattedString} {operators[Operation]} {Right.FormattedString}";
        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitBinary(this);
    }
}