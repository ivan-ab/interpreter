namespace Interpreter.Nodes.Statements
{
    class Assignment : IStatement
    {
        public readonly string Variable;
        public readonly IExpression Expression;
        public string FormattedString => $"{Variable} = {Expression.FormattedString};\n";

        public Assignment(string variable, IExpression expression)
        {
            Variable = variable;
            Expression = expression;
        }

        public void Accept(IStatementVisitor visitor) => visitor.VisitAssignment(this);
    }
}