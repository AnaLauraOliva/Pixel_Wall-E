public abstract class Stmt
{
    public abstract void Accept<T>(IStatementVisitor<T> visitor);
}