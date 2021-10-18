using Interpreter.Nodes.Expressions;

namespace Interpreter.Nodes
{
    interface IExpressionVisitor<T>
    {
        T VisitBinary(BinaryOperation expression);
        T VisitConditional(ConditionalExpression expression);
        T VisitUnary(UnaryOperation expression);
        T VisitCall(CallExpression expression);
        T VisitParen(Paren expression);
        T VisitNumber(Number expression);
        T VisitIdentifier(Identifier expression);
    }
}