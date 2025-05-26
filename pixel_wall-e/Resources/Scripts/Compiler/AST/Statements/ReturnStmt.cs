using System;
using System.Collections.Generic;

public class ReturnStmt : Stmt
{
    public Token keyword { get; }
    public Expression value { get; }


    public ReturnStmt(Token keyword, Expression value)
    {
        this.keyword = keyword;
        this.value = value;

    }

    public override void Accept<T>(IStatementVisitor<T> visitor)
    {
        visitor.visitReturnStmt(this);
    }
}