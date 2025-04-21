using System.Collections.Generic;

public class Call : Expression
{
    public Call(Expression callee, Token paren, List<Expression>arguments)
    {
        Callee = callee;
        Paren = paren;
        Arguments = arguments;
    }

    public Expression Callee { get; }
    public Token Paren { get; }
    public List<Expression> Arguments { get; }

    public override object Accept(IExpressionVisitor visitor)
    {
        return visitor.visitCallExpression(this);
    }
}