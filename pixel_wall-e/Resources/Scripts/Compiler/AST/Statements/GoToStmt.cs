using System;
using System.Collections.Generic;

public class GoToStmt : Stmt
{
    public Token Keyword {get;}
    public Token Label { get; }
    public Expression Condition { get; }
    public int maxRepetition;


    public GoToStmt(Token label, Expression Condition, Token keyword)
    {
        this.Label = label;
        this.Condition = Condition;
        Keyword = keyword;
        maxRepetition=10000000;
    }

    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.visitGoToStmt(this);
    }
}