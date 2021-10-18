namespace Interpreter.Nodes.Expressions
{
    class Paren : IExpression
    {
        public readonly IExpression Value;

        public Paren(IExpression value)
        {
            Value = value;
        }

        public string FormattedString => $"({Value.FormattedString})";
        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitParen(this);
    }
}