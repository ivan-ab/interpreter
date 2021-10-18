namespace Interpreter.Nodes
{
    interface IStatement : INode
    {
        void Accept(IStatementVisitor visitor);
    }
}