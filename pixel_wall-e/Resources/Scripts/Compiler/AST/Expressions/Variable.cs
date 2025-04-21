public class Variable : Expression
{
    public Variable(Token name)
    {
        Name = name;
    }
    public Token Name { get; }

    public override object Accept(IExpressionVisitor visitor)
    {
        return visitor.visitVariableExpression(this);
    }
}