namespace Interpreter.Nodes
{
    interface IExpression : INode
    {
        T Accept<T>(IExpressionVisitor<T> visitor);
    }
}