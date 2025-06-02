using System;
using System.Collections.Generic;

public class IfStmt : Stmt
{
    public Token keyword{get;}
    public Expression condition { get; }
    public Stmt thenBranch { get; }
    public Stmt elseBranch { get; }


    public IfStmt(Expression condition, Stmt thenBranch, Stmt elseBranch, Token keyword)
    {
        this.keyword = keyword;
        this.condition = condition;
        this.thenBranch = thenBranch;
        this.elseBranch = elseBranch;

    }

    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.visitIfStmt(this);
    }
}