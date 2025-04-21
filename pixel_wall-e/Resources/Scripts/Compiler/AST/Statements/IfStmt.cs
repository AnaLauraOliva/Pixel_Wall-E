using System;
using System.Collections.Generic;

public class IfStmt : Stmt
{
    public Expression condition { get; }
    public Stmt thenBranch { get; }
    public Stmt elseBranch { get; }


    public IfStmt(Expression condition, Stmt thenBranch, Stmt elseBranch)
    {
        this.condition = condition;
        this.thenBranch = thenBranch;
        this.elseBranch = elseBranch;

    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.visitIfStmt(this);
    }
}