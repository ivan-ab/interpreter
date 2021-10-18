using Interpreter.Nodes.Statements;

namespace Interpreter.Nodes
{
    interface IStatementVisitor
    {
        void VisitIf(IfStatement statement);
        void VisitWhile(WhileStatement statement);
        void VisitExpressionStatement(ExpressionStatement statement);
        void VisitAssignment(Assignment statement);
        void VisitBreak(Break statement);
    }
}