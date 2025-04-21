public class Grouping : Expression
{
    public Expression Expression { get; private set; }
    public Grouping(Expression expression)
    {

        Expression = expression;
    }
    public override object Accept(IExpressionVisitor visitor)
    {
        return visitor.visitGroupingExpression(this);
    }
}