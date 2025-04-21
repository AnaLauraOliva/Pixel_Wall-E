public class Literal : Expression
{

    public object Value { get; private set;}
    public Literal(object value)
    {
        Value = value;
    }
    public override object Accept(IExpressionVisitor visitor)
    {
        return visitor.visitLiteralExpression(this);
    }

}