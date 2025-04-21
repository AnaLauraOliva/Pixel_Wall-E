public interface IExpressionVisitor
{
    object visitBinaryExpression(Binary expression);
    object visitGroupingExpression(Grouping expression);
    object visitLiteralExpression(Literal expression);
    object visitUnaryExpression(Unary expression);
    object visitAssignExpression(Assign expression);
    object visitCallExpression(Call expression);
    object visitLogicalExpression(Logical expression);
    object visitVariableExpression(Variable expression);
}