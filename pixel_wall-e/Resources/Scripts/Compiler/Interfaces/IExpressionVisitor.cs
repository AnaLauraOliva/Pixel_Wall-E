public interface IExpressionVisitor<T>
{
    T visitBinaryExpression(Binary expression);
    T visitGroupingExpression(Grouping expression);
    T visitLiteralExpression(Literal expression);
    T visitUnaryExpression(Unary expression);
    T visitAssignExpression(Assign expression);
    T visitCallExpression(Call expression);
    T visitLogicalExpression(Logical expression);
    T visitVariableExpression(Variable expression);
}