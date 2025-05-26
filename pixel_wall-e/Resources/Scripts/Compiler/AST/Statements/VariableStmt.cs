using System;
using System.Collections.Generic;

public class VariableStmt : Stmt
{
    public Token Name { get; }
    public Expression initializer { get; }


    public VariableStmt(Token Name, Expression initializer)
    {
        this.Name = Name;
        this.initializer = initializer;

    }

    public override void Accept<T>(IStatementVisitor<T> visitor)
    {
        visitor.visitVariableStmt(this);
    }
}