using System;
using System.Collections.Generic;

public class Call : Expression
{
    public Call(Expression callee, Token paren, List<Expression>arguments)
    {
        Callee = callee;
        Paren = paren;
        Arguments = arguments;
        type = ExpressionType.VOID;
    }
    public void SetReturnType(ExpressionType type)
    {
        this.type = type;
    }

    public Expression Callee { get; }
    public Token Paren { get; }
    public List<Expression> Arguments { get; }

    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        return visitor.visitCallExpression(this);
    }
}