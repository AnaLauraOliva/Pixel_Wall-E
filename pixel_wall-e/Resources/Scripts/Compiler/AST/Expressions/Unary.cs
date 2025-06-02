public class Unary : Expression
{
    public Token UnaryOperator { get; private set; }
    public Expression Expression { get; private set;}
    public Unary(Token unaryOperator, Expression expression, ExpressionType type)
    {
        UnaryOperator = unaryOperator;
        Expression = expression;
        this.type=type;
    }
    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.visitUnaryExpression(this);
    }
}