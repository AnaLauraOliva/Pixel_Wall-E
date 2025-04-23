using System;
using System.Collections.Generic;

public class GoToStmt : Stmt
{
    public Token Label { get; }
    public Expression Condition { get; }


    public GoToStmt(Token label, Expression Condition)
    {
        this.Label = label;
        this.Condition = Condition;

    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.visitGoToStmt(this);
    }
}