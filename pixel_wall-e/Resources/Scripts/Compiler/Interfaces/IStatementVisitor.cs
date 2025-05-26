public interface IStatementVisitor<T>
{
    T visitBlockStmt(BlockStmt stmt);
    T visitExpressionStmt(ExpressionStmt stmt);
    T visitFunctionStmt(FunctionStmt stmt);
    T visitIfStmt(IfStmt stmt);
    T visitReturnStmt(ReturnStmt stmt);
    T visitGoToStmt(GoToStmt stmt);
    T visitWhileStmt(WhileStmt stmt);
    T visitForStmt(ForStmt stmt);
    T visitSpawnStmt(SpawnStmt stmt);
    T visitColorStmt(ColorStmt stmt);
    T visitSizeStmt(SizeStmt stmt);
    T visitDrawLineStmt(DrawLineStmt stmt);
    T visitDrawCircleStmt(DrawCircleStmt stmt);
    T visitDrawRectangleStmt(DrawRectangleStmt stmt);
    T visitFillStmt(FillStmt stmt);
    T visitLabelStmt(LabelStmt stmt);
    T visitVariableStmt(VariableStmt stmt);
}