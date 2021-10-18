namespace Interpreter.Nodes.Expressions
{
    class ConditionalExpression : IExpression
    {
        public readonly IExpression Condition;
        public readonly IExpression TrueExpr;
        public readonly IExpression FalseExpr;

        public string FormattedString =>
            $"{Condition.FormattedString} ? {TrueExpr.FormattedString} : {FalseExpr.FormattedString}";

        public ConditionalExpression(IExpression condition, IExpression trueExpr, IExpression falseExpr)
        {
            Condition = condition;
            TrueExpr = trueExpr;
            FalseExpr = falseExpr;
        }

        public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitConditional(this);
    }
}