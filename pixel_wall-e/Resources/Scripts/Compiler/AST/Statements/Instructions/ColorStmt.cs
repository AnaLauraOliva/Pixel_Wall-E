using System;
using System.Collections.Generic;
using Godot;

public class ColorStmt : Stmt
{
    public Expression ColorName { get; }
    public readonly Token Keyword;


    public ColorStmt(Expression ColorName, Token keyword)
    {
        this.ColorName = ColorName;
        Keyword = keyword;
    }

    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.visitColorStmt(this);
    }
}