using System;
using System.Collections.Generic;

public class DrawCircleStmt : Stmt
{
    public readonly Token keyword;

    public Expression DirX { get; }
    public Expression DirY { get; }
    public Expression Radius { get; }


    public DrawCircleStmt(Expression DirX, Expression DirY, Expression Radius, Token keyword)
    {
        this.DirX = DirX;
        this.DirY = DirY;
        this.Radius = Radius;
        this.keyword = keyword;
    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.visitDrawCircleStmt(this);
    }
}