namespace Interpreter.Nodes.Statements
{
    class ExpressionStatement : IStatement
    {
        public readonly IExpression Expression;

        public ExpressionStatement(IExpression expression)
        {
            Expression = expression;
        }

        public string FormattedString => $"{Expression.FormattedString};\n";
        public void Accept(IStatementVisitor visitor) => visitor.VisitExpressionStatement(this);
    }
}