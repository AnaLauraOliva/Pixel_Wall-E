using System;

public abstract class Expression
{
    //R is a generic parameter to denote the return type in the Visitor pattern of the AST
   public abstract object Accept(IExpressionVisitor visitor);
}