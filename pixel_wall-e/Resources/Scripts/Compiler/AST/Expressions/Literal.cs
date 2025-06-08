public class Literal : Expression
{
    public object Value { get; private set;}
    public Token Keyword { get; }

    public Literal(object value, ExpressionType type, Token keyword = null)
    {
        Value = value;
        Keyword = keyword;
        this.type = type;
    }
    public void ChangeValue(string nVal)=> Value=nVal;
    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.visitLiteralExpression(this);
    }

}