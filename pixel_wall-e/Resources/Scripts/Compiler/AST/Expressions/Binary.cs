public class Binary : Expression
{
    public Expression Left { get;private set; }
    public Token Operator { get; private set; }
    public Expression Right { get; private set; }
    public Binary(Expression left, Token @operator, Expression right)
    {
        Left = left;
        Operator = @operator;
        Right = right;
    }
    public override object Accept(IExpressionVisitor visitor)
    {
        return visitor.visitBinaryExpression(this);
    }
}
