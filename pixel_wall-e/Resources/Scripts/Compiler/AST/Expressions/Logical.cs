public class Logical : Expression
{
    public Logical(Expression left, Token @operator, Expression right)
    {
        Left = left;
        Operator = @operator;
        Right = right;
        type = ExpressionType.BOOL;
    }

    public Expression Left { get; }
    public Token Operator { get; }
    public Expression Right { get; }

    public override object Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.visitLogicalExpression(this);
    }
}