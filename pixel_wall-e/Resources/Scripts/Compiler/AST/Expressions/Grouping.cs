public class Grouping : Expression
{
    public Expression Expression { get; private set; }
    public Grouping(Expression expression, ExpressionType type)
    {
        this.type=type;
        Expression = expression;
    }
    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.visitGroupingExpression(this);
    }
}