public class Assign:Expression
{
    public Assign(Token name, Expression value)
    {
        Name = name;
        Value = value;
    }

    public Token Name { get; }
    public Expression Value { get; }

    public override object Accept(IExpressionVisitor visitor)
    {
        return visitor.visitAssignExpression(this);
    }
}