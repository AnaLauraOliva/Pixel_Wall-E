public interface IStatementVisitor
{
    void visitBlockStmt(BlockStmt stmt);
    void visitExpressionStmt(ExpressionStmt stmt);
    void visitFunctionStmt(FunctionStmt stmt);
    void visitIfStmt(IfStmt stmt);
    void visitReturnStmt(ReturnStmt stmt);
    void visitGoToStmt(GoToStmt stmt);
    void visitWhileStmt(WhileStmt stmt);
    void visitForStmt(ForStmt stmt);
    void visitSpawnStmt(SpawnStmt stmt);
    void visitColorStmt(ColorStmt stmt);
    void visitSizeStmt(SizeStmt stmt);
    void visitDrawLineStmt(DrawLineStmt stmt);
    void visitDrawCircleStmt(DrawCircleStmt stmt);
    void visitDrawRectangleStmt(DrawRectangleStmt stmt);
    void visitFillStmt(FillStmt stmt);
    void visitLabelStmt(LabelStmt stmt);
    void visitVariableStmt(VariableStmt stmt);
}