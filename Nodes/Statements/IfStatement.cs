namespace Interpreter.Nodes.Statements
{
    class IfStatement : IStatement
    {
        public readonly IExpression Condition;
        public readonly Block Block;
        public string FormattedString => $"if ({Condition.FormattedString}) {Block.FormattedString}";

        public IfStatement(IExpression condition, Block block)
        {
            Condition = condition;
            Block = block;
        }

        public void Accept(IStatementVisitor visitor) => visitor.VisitIf(this);
    }
}