using System;

public abstract class Expression
{
   public abstract object Accept(IExpressionVisitor visitor);
}