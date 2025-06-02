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

    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.visitAssignExpression(this);
    }
}