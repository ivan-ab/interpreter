namespace Interpreter.Nodes.Expressions
{
    class Number : IExpression
    {
        public readonly string Lexeme;

        public Number(string lexeme)
        {
            Lexeme = lexeme;
        }

        public string FormattedString => Lexeme;
        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitNumber(this);
    }
}