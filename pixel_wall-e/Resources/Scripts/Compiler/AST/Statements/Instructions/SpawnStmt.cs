using System;
using System.Collections.Generic;

public class SpawnStmt : Stmt
{
    public readonly Token keyword;

    public Expression X { get; }
    public Expression Y { get; }


    public SpawnStmt(Expression X, Expression Y, Token keyword)
    {
        this.X = X;
        this.Y = Y;
        this.keyword = keyword;
    }

    public override void Accept<T>(IStatementVisitor<T> visitor)
    {
        visitor.visitSpawnStmt(this);
    }
}