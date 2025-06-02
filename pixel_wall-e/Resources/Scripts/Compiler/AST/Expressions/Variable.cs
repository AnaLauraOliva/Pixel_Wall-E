public class Variable : Expression
{
    public Variable(Token name, ExpressionType type)
    {
        Name = name;
        this.type = type;
    }
    public void defineType(ExpressionType type)=>this.type = type;
    public Token Name { get; }

    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.visitVariableExpression(this);
    }
}