using System;
using System.Collections.Generic;

public class GoToStmt : Stmt
{
    public LabelStmt Name { get; }
    public Expression Condition { get; }


    public GoToStmt(LabelStmt Name, Expression Condition)
    {
        this.Name = Name;
        this.Condition = Condition;

    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.visitGoToStmt(this);
    }
}