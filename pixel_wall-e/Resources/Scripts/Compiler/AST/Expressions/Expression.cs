using System;
public enum ExpressionType
{
   STRING, INT, BOOL, VOID, NONE
}
public abstract class Expression
{
   public ExpressionType type{get; protected set;}
   public abstract T Accept<T>(IExpressionVisitor<T> visitor);
}