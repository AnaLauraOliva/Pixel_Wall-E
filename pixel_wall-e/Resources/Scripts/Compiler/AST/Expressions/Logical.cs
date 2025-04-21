public class Logical : Expression
{
    public Logical(Expression left, Token @operator, Expression right)
    {
        Left = left;
        Operator = @operator;
        Right = right;
    }

    public Expression Left { get; }
    public Token Operator { get; }
    public Expression Right { get; }

    public override object Accept(IExpressionVisitor visitor)
    {
        return visitor.visitLogicalExpression(this);
    }
}