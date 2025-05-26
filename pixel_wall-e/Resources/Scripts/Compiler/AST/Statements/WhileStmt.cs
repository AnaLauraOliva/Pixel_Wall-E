using System;
using System.Collections.Generic;

public class WhileStmt : Stmt
{
    public Token keyword{get;}
    public Expression Condition { get; }
    public Stmt Body { get; }


    public WhileStmt(Token keyword,Expression Condition, Stmt Body)
    {
        this.keyword = keyword;
        this.Condition = Condition;
        this.Body = Body;

    }

    public override void Accept<T>(IStatementVisitor<T> visitor)
    {
        visitor.visitWhileStmt(this);
    }
}