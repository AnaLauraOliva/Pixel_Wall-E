using System;
using System.Collections.Generic;

public class FunctionStmt : Stmt
{
    public Token Name { get; }
    public List<Token> Parameters { get; }
    public List<Stmt> Body { get; }
    public List<ExpressionType> ParametersTypes { get; }
    public ExpressionType Type { get; }

    public FunctionStmt(Token Name, List<Token> Parameters, List<Stmt> Body, List<ExpressionType> ParametersTypes, ExpressionType type)
    {
        this.Name = Name;
        this.Parameters = Parameters;
        this.Body = Body;
        this.ParametersTypes = ParametersTypes;
        Type = type;
    }

    public override T Accept<T>(IStatementVisitor<T> visitor)
    {
        return visitor.visitFunctionStmt(this);
    }
}