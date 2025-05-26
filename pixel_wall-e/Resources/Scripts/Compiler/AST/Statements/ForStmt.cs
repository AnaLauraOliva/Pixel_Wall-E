using System;
using System.Collections.Generic;

public class ForStmt : Stmt
{
    public Stmt Initializer { get; }
    public Expression Condition { get; }
    public Expression Increment { get; }
    public Stmt Body { get; }


    public ForStmt(Stmt Initializer, Expression Condition, Expression Increment, Stmt Body)
    {
        this.Initializer = Initializer;
        this.Condition = Condition;
        this.Increment = Increment;
        this.Body = Body;

    }

    public override void Accept<T>(IStatementVisitor<T> visitor)
    {
        visitor.visitForStmt(this);
    }
}