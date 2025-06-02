using System;
using System.Collections.Generic;

public class DrawRectangleStmt : Stmt
{
    public readonly Token keyword;

    public Expression DirX { get; }
    public Expression DirY { get; }
    public Expression Distance { get; }
    public Expression Width { get; }
    public Expression Height { get; }


    public DrawRectangleStmt(Expression DirX, Expression DirY, Expression Distance, Expression Width, Expression Height, Token keyword)
    {
        this.DirX = DirX;
        this.DirY = DirY;
        this.Distance = Distance;
        this.Width = Width;
        this.Height = Height;
        this.keyword = keyword;
    }

    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.visitDrawRectangleStmt(this);
    }
}