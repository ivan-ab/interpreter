namespace Interpreter.Nodes.Expressions
{
    class Identifier : IExpression
    {
        public readonly string Name;

        public Identifier(string name)
        {
            Name = name;
        }

        public string FormattedString => Name;
        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitIdentifier(this);
    }
}