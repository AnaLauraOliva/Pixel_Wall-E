using System;
using System.Collections.Generic;

public class FunctionStmt : Stmt
{
    public Token Name { get; }
    public List<Token> Parameters { get; }
    public List<Stmt> Body { get; }


    public FunctionStmt(Token Name, List<Token> Parameters, List<Stmt> Body)
    {
        this.Name = Name;
        this.Parameters = Parameters;
        this.Body = Body;

    }

    public override void Accept(IStatementVisitor visitor)
    {
        visitor.visitFunctionStmt(this);
    }
}