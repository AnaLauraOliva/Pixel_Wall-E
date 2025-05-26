public class Assign:Expression
{
    public Assign(Token name, Expression value)
    {
        Name = name;
        Value = value;
        type = Value.type;
    }

    public Token Name { get; }
    public Expression Value { get; }

    public override object Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.visitAssignExpression(this);
    }
}