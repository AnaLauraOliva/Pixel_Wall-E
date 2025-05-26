public class Binary : Expression
{
    public Expression Left { get;private set; }
    public Token Operator { get; private set; }
    public Expression Right { get; private set; }
    public Binary(Expression left, Token @operator, Expression right, ExpressionType type)
    {
        Left = left;
        Operator = @operator;
        Right = right;
        this.type = type;
    }
    public override object Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.visitBinaryExpression(this);
    }
}
