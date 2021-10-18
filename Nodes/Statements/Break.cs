namespace Interpreter.Nodes.Statements
{
    class Break : IStatement
    {
        public string FormattedString => "break;\n";
        public void Accept(IStatementVisitor visitor) => visitor.VisitBreak(this);
    }
}