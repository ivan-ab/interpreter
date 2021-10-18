namespace Interpreter.Nodes.Statements
{
    class WhileStatement : IStatement
    {
        public readonly IExpression Condition;
        public readonly Block Block;
        public string FormattedString => $"while ({Condition.FormattedString}) {Block.FormattedString}";

        public WhileStatement(IExpression condition, Block block)
        {
            Condition = condition;
            Block = block;
        }

        public void Accept(IStatementVisitor visitor) => visitor.VisitWhile(this);
    }
}