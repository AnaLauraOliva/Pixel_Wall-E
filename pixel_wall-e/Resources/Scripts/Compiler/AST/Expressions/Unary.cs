public class Unary : Expression
{
    public Token UnaryOperator { get; private set; }
    public Expression Expression { get; private set;}
    public Unary(Token unaryOperator, Expression expression)
    {
        UnaryOperator = unaryOperator;
        Expression = expression;
    }
    public override object Accept(IExpressionVisitor visitor)
    {
        return visitor.visitUnaryExpression(this);
    }
}