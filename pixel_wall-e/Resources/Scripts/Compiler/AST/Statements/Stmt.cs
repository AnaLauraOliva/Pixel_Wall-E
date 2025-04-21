public abstract class Stmt
{
    public abstract void Accept(IStatementVisitor visitor);
}