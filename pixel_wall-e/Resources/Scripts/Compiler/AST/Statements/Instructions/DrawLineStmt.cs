using System;
using System.Collections.Generic;

public class DrawLineStmt : Stmt
{
    public readonly Token keyword;

    public Expression DirX { get; }
    public Expression DirY { get; }
    public Expression Distance { get; }


    public DrawLineStmt(Expression DirX, Expression DirY, Expression Distance, Token keyword)
    {
        this.DirX = DirX;
        this.DirY = DirY;
        this.Distance = Distance;
        this.keyword = keyword;
    }

    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.visitDrawLineStmt(this);
    }
}