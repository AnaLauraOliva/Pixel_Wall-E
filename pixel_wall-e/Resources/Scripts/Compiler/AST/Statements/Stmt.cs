public abstract class Stmt
{
    public abstract T Accept<T>(IStatementVisitor<T> visitor);
}