using System.Collections.Generic;

namespace Interpreter.Nodes.Expressions
{
    class UnaryOperation : IExpression
    {
        public readonly UnaryOpType Operation;
        public readonly IExpression Expression;

        static readonly IReadOnlyDictionary<UnaryOpType, string> operators = new Dictionary<UnaryOpType, string>
        {
            { UnaryOpType.UnaryMinus, "-" },
            { UnaryOpType.LogicalNegation, "!" },
        };

        public string FormattedString => $"{operators[Operation]} {Expression.FormattedString}";

        public UnaryOperation(UnaryOpType operation, IExpression expression)
        {
            Operation = operation;
            Expression = expression;
        }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitUnary(this);
    }
}