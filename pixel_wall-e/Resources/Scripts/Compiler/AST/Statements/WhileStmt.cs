using System;
using System.Collections.Generic;

public class WhileStmt : Stmt
{
    public Expression Condition { get; }
    public Stmt Body { get; }


    public WhileStmt(Expression Condition, Stmt Body)
    {
        this.Condition = Condition;
        this.Body = Body;

    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.visitWhileStmt(this);
    }
}