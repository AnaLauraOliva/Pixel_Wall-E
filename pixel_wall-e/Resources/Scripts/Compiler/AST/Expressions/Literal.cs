public class Literal : Expression
{
    public object Value { get; private set;}
    public Literal(object value, ExpressionType type)
    {
        Value = value;
        this.type = type;
    }
    public override object Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.visitLiteralExpression(this);
    }

}