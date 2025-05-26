using System;
public enum ExpressionType
{
   STRING, INT, BOOL, VOID
}
public abstract class Expression
{
   public ExpressionType type{get; protected set;}
   public abstract object Accept<T>(IExpressionVisitor<T> visitor);
}