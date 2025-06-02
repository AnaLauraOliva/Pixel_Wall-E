using System;
using System.Collections.Generic;
public class SemanticAnalyzer : IExpressionVisitor<ReturnStatus>, IStatementVisitor<ReturnStatus>
{
    private List<Stmt> statements;
    private List<CompilerException> exceptions = new List<CompilerException>();
    private List<string> Vars;
    private List<string> Funcs;
    private bool canDeclareFunction = true;
    SemanticEnvironment env = new SemanticEnvironment();
    SemanticEnvironment globals = new SemanticEnvironment();
    public SemanticAnalyzer(List<Stmt> statements)
    {
        globals.defineFunc("GetActualX", ExpressionType.INT, new List<ExpressionType>());
        globals.defineFunc("GetActualY", ExpressionType.INT, new List<ExpressionType>());
        globals.defineFunc("GetCanvasSize", ExpressionType.INT, new List<ExpressionType>());
        globals.defineFunc("GetColorCount", ExpressionType.INT, new List<ExpressionType>() { ExpressionType.STRING, ExpressionType.INT, ExpressionType.INT, ExpressionType.INT, ExpressionType.INT });
        globals.defineFunc("IsBrushColor", ExpressionType.INT, new List<ExpressionType>() { ExpressionType.STRING });
        globals.defineFunc("IsBrushSize", ExpressionType.INT, new List<ExpressionType>() { ExpressionType.INT });
        globals.defineFunc("IsCanvasColor", ExpressionType.INT, new List<ExpressionType>() { ExpressionType.STRING, ExpressionType.INT, ExpressionType.INT });
        env = globals;
        this.statements = statements;
    }
    public List<CompilerException> GetExceptions() => exceptions;
    public void Analyze()
    {
        foreach (Stmt stmt in statements)
        {
            if (stmt is LabelStmt)
            {
                if (env.HasLabel(((LabelStmt)stmt).Name.Lexeme.ToString())) exceptions.Add(error($"This label already exists. Name{((LabelStmt)stmt).Name.Lexeme.ToString()}", ((LabelStmt)stmt).Name));
                else env.defineLabel(((LabelStmt)stmt).Name.Lexeme.ToString());
            }
            else if (stmt is FunctionStmt)
            {
                if (env.HasFunc(((FunctionStmt)stmt).Name.Lexeme)) exceptions.Add(error($"This function already exists. Name{((FunctionStmt)stmt).Name.Lexeme}", ((FunctionStmt)stmt).Name));
                else AnalyzeStmt(stmt);
            }
        }
        foreach (Stmt stmt in statements)
        {
            canDeclareFunction = true;
            if (stmt is not LabelStmt && stmt is not FunctionStmt)
                AnalyzeStmt(stmt);
        }
    }

    private void AnalyzeStmt(Stmt stmt)
    {
        try
        {
            stmt.Accept(this);
        }
        catch (CompilerException e)
        {
            exceptions.Add(e);
        }

    }
    public ReturnStatus visitAssignExpression(Assign expression)
    {
        string name = expression.Name.Lexeme;
        Evaluate(expression.Value);
        bool? isTrue = env.Assign(name, expression.Value.type);
        if (isTrue == null) throw error($"Undefined variable {name} at current scope", expression.Name);
        else if (isTrue == false) throw error($"You cannot assign {expression.type} to {env.GetVarType(name)}", expression.Name);
        return ReturnStatus.Never;
    }

    public ReturnStatus visitBinaryExpression(Binary expression)
    {
        Evaluate(expression.Left);
        Evaluate(expression.Right);
        switch (expression.Operator.Type)
        {
            case TokenType.PLUS:
            case TokenType.MINUS:
            case TokenType.SLASH:
            case TokenType.MODULE:
            case TokenType.ROOT:
            case TokenType.GREATER:
            case TokenType.GREATER_EQUAL:
            case TokenType.LESS:
            case TokenType.LESS_EQUAL:
            case TokenType.BY:
            case TokenType.POW:
                if (expression.Left.type != ExpressionType.INT || expression.Right.type != ExpressionType.INT)
                    throw error("Operands must be numbers", expression.Operator);
                break;
            case TokenType.EQUAL_EQUAL:
            case TokenType.NOT_EQUAL:
                if (expression.Left.type != expression.Right.type) throw error("Operands must be of the same type", expression.Operator);
                break;
            case TokenType.AND:
            case TokenType.OR:
                if (expression.Left.type != ExpressionType.BOOL || expression.Right.type != ExpressionType.BOOL)
                    throw error("Operands must be booleans", expression.Operator);
                break;
        }
        return ReturnStatus.Never;
    }
    public ReturnStatus visitBlockStmt(BlockStmt stmt)
    {
        ReturnStatus status = ReturnStatus.Never;
        ExecuteBlock(stmt.Statements, new SemanticEnvironment(env), ref status);
        return status;
    }
    public ReturnStatus visitCallExpression(Call expression)
    {
        // expression.Callee.Accept(this);
        //List<ExpressionType> arguments;
        if (expression.Callee is Variable callee)
        {
            string funcName = callee.Name.Lexeme;
            if (!env.HasFunc(funcName))
            {
                throw error($"Undefined function: {funcName}", callee.Name);
            }
            else
            {
                int count = env.ReturnArgumentCount(funcName);
                if (count == -1) throw error($"Undefined function: {funcName}", callee.Name);
                expression.SetReturnType(env.getReturnType(funcName));
                if (expression.Arguments.Count != count)
                    throw error($"Missing arguments. Expected:{count}, Obtained: {expression.Arguments.Count}", callee.Name);
                else
                {
                    List<ExpressionType> types = env.getArguments(funcName);
                    for (int i = 0; i < types.Count; i++)
                    {
                        if (types[i] != expression.Arguments[i].type) exceptions.Add(error($"Incorrect type. Expected:{types[i]}, Obtained: {expression.Arguments[i].type}", callee.Name));
                    }
                }
            }
        }
        else
        {
            throw error($"You can only call functions. Invalid type:{expression.Callee.type}", expression.Paren);
        }
        foreach (var arg in expression.Arguments)
        {
            arg.Accept(this);

        }
        return ReturnStatus.Never;
    }

    public ReturnStatus visitColorStmt(ColorStmt stmt)
    {
        stmt.ColorName.Accept(this);
        if (stmt.ColorName is null || stmt.ColorName is not Literal || ((Literal)stmt.ColorName).Value is not string) throw error("The argument of Color instruction must be a string", stmt.Keyword);
        return ReturnStatus.Never;
    }

    public ReturnStatus visitDrawCircleStmt(DrawCircleStmt stmt)
    {
        stmt.DirX.Accept(this);
        if (stmt.DirX.type != ExpressionType.INT) throw error("DirX argument must be int", stmt.keyword);
        stmt.DirY.Accept(this);
        if (stmt.DirY.type != ExpressionType.INT) throw error("DirY argument must be int", stmt.keyword);
        stmt.Radius.Accept(this);
        if (stmt.Radius.type != ExpressionType.INT) throw error("Radius argument must be int", stmt.keyword);
        return ReturnStatus.Never;
    }

    public ReturnStatus visitDrawLineStmt(DrawLineStmt stmt)
    {
        stmt.DirX.Accept(this);
        if (stmt.DirX.type != ExpressionType.INT) throw error("DirX argument must be int", stmt.keyword);
        stmt.DirY.Accept(this);
        if (stmt.DirY.type != ExpressionType.INT) throw error("DirY argument must be int", stmt.keyword);
        stmt.Distance.Accept(this);
        if (stmt.Distance.type != ExpressionType.INT) throw error("Distance argument must be int", stmt.keyword);
        return ReturnStatus.Never;
    }

    public ReturnStatus visitDrawRectangleStmt(DrawRectangleStmt stmt)
    {
        stmt.DirX.Accept(this);
        if (stmt.DirX.type != ExpressionType.INT) throw error("DirX argument must be int", stmt.keyword);
        stmt.DirY.Accept(this);
        if (stmt.DirY.type != ExpressionType.INT) throw error("DirY argument must be int", stmt.keyword);
        stmt.Distance.Accept(this);
        if (stmt.Distance.type != ExpressionType.INT) throw error("Distance argument must be int", stmt.keyword);
        stmt.Height.Accept(this);
        if (stmt.Height.type != ExpressionType.INT) throw error("Height argument must be int", stmt.keyword);
        stmt.Width.Accept(this);
        if (stmt.Width.type != ExpressionType.INT) throw error("Width argument must be int", stmt.keyword);
        return ReturnStatus.Never;
    }

    public ReturnStatus visitExpressionStmt(ExpressionStmt stmt)
    {
        stmt.Expr.Accept(this);
        return ReturnStatus.Always;
    }

    public ReturnStatus visitFillStmt(FillStmt stmt)
    {
        return ReturnStatus.Never;
    }

    public ReturnStatus visitFunctionStmt(FunctionStmt stmt)
    {
        if(canDeclareFunction==false) throw error("You cannot declare a function here",stmt.Name);
        canDeclareFunction = false;
        SemanticEnvironment nEnv = new SemanticEnvironment(globals);
        ReturnStatus status = ReturnStatus.Never;
        for (int i = 0; i < stmt.Parameters.Count; i++)
        {
            nEnv.defineVar(stmt.Parameters[i].Lexeme, stmt.ParametersTypes[i]);
        }
        env.defineFunc(stmt.Name.Lexeme, stmt.Type, stmt.ParametersTypes);
        ExecuteBlock(stmt.Body, nEnv, ref status);
        if (stmt.Type != ExpressionType.VOID && status != ReturnStatus.Always) exceptions.Add(error("Not all code paths return a value", stmt.Name));
        CheckReturnTypes(stmt.Body, nEnv, stmt.Type);
        return ReturnStatus.Never;
    }
    private void CheckReturnTypes(List<Stmt> statements, SemanticEnvironment NEnv, ExpressionType expected)
    {
        SemanticEnvironment prev = env;
        try
        {
            env = NEnv;
            CheckStatements(statements, expected);
        }
        finally
        {
            env = prev;
        }
    }
    private void CheckStatements(List<Stmt> statements, ExpressionType expected)
    {
        foreach (var stmt in statements)
        {
            CheckStatement(stmt, expected);
        }
    }
    private void CheckStatement(Stmt stmt, ExpressionType expected)
    {
        SemanticEnvironment temp;
        SemanticEnvironment prev = env;
        switch (stmt)
        {
            case ReturnStmt returnStmt:
                ExpressionType actualType = returnStmt.value is null ? ExpressionType.VOID : returnStmt.value.type;
                if (expected != actualType)
                    throw error($"You cannot return {actualType} on {expected} function.", returnStmt.keyword);
                break;
            case IfStmt ifStmt:
                if (ifStmt.thenBranch is BlockStmt b)
                {
                    temp = new SemanticEnvironment(env);
                    env = temp;
                    foreach (Stmt item in b.Statements)
                    {
                        item.Accept(this);
                    }
                }
                CheckStatement(ifStmt.thenBranch, expected);
                env = prev;
                if (ifStmt.elseBranch != null)
                {
                    if (ifStmt.elseBranch is BlockStmt block)
                    {
                        temp = new SemanticEnvironment(env);
                        foreach (Stmt item in block.Statements)
                        {
                            item.Accept(this);
                        }
                        env = temp;
                    }
                    CheckStatement(ifStmt.elseBranch, expected);
                    env = prev;
                }
                break;
            case BlockStmt blockStmt:
                temp = new SemanticEnvironment(env);
                foreach (Stmt item in blockStmt.Statements)
                {
                    item.Accept(this);
                }
                env = temp;
                CheckStatements(blockStmt.Statements, expected);
                env = prev;
                break;
            case WhileStmt whileStmt:
                if (whileStmt.Body is BlockStmt block2)
                {
                    temp = new SemanticEnvironment(env);
                    foreach (Stmt item in block2.Statements)
                    {
                        item.Accept(this);
                    }
                    env = temp;
                }
                CheckStatement(whileStmt, expected);
                env = prev;
                break;
            default:
                break;
        }
    }
    public ReturnStatus visitGoToStmt(GoToStmt stmt)
    {
        if (!env.HasLabel(stmt.Label.Lexeme)) throw error("This label doesn't exists", stmt.Label);
        stmt.Condition.Accept(this);
        if (stmt.Condition.type is not ExpressionType.BOOL)
        {
            throw error("GoTo condition must be boolean", stmt.Keyword);
        }
        return ReturnStatus.Never;
    }

    public ReturnStatus visitGroupingExpression(Grouping expression)
    {
        expression.Expression.Accept(this);
        return ReturnStatus.Never;
    }

    public ReturnStatus visitIfStmt(IfStmt stmt)
    {
        canDeclareFunction = false;
        stmt.condition.Accept(this);
        if (stmt.condition.type is not ExpressionType.BOOL) throw error("If condition must be boolean", stmt.keyword);
        ReturnStatus then = stmt.thenBranch.Accept(this);
        ReturnStatus elseBranch = ReturnStatus.Never;
        if (stmt.elseBranch is not null) elseBranch = stmt.elseBranch.Accept(this);
        if (then == ReturnStatus.Always && elseBranch == ReturnStatus.Always) return ReturnStatus.Always;
        else if (then == ReturnStatus.Never && elseBranch == ReturnStatus.Never) return ReturnStatus.Never;
        return ReturnStatus.Maybe;
    }

    public ReturnStatus visitLabelStmt(LabelStmt stmt)
    {
        return ReturnStatus.Never;
    }

    public ReturnStatus visitLiteralExpression(Literal expression)
    {
        return ReturnStatus.Never;
    }

    public ReturnStatus visitLogicalExpression(Logical expression)
    {
        Evaluate(expression.Left);
        Evaluate(expression.Right);
        if (expression.Left.type is not ExpressionType.BOOL || expression.Right.type is not ExpressionType.BOOL) throw error("Operands must be booleans", expression.Operator);
        return ReturnStatus.Never;
    }

    public ReturnStatus visitReturnStmt(ReturnStmt stmt)
    {
        if (stmt.value is not null)
            Evaluate(stmt.value);
        return ReturnStatus.Always;
    }

    public ReturnStatus visitSizeStmt(SizeStmt stmt)
    {
        Evaluate(stmt.K);
        if (stmt.K.type is not ExpressionType.INT) throw error("argument k must be an int", stmt.keyword);
        return ReturnStatus.Never;
    }

    public ReturnStatus visitSpawnStmt(SpawnStmt stmt)
    {
        Evaluate(stmt.X);
        if (stmt.X.type is not ExpressionType.INT) throw error("Argument X must bu numeric", stmt.keyword);
        Evaluate(stmt.Y);
        if (stmt.Y.type is not ExpressionType.INT) throw error("Argument Y must bu numeric", stmt.keyword);
        return ReturnStatus.Never;
    }


    public ReturnStatus visitUnaryExpression(Unary expression)
    {
        Evaluate(expression.Expression);
        if (expression.UnaryOperator.Type is TokenType.MINUS && expression.Expression.type is not ExpressionType.INT)
            throw error($"You cannot use unary operator - with {expression.Expression.type}", expression.UnaryOperator);
        else if (expression.UnaryOperator.Type is TokenType.NOT && expression.Expression.type is not ExpressionType.BOOL)
            throw error($"You cannot use unary operator ! with {expression.Expression.type}", expression.UnaryOperator);
        return ReturnStatus.Never;
    }

    public ReturnStatus visitVariableExpression(Variable expression)
    {
        if (!env.HasVar(expression.Name.Lexeme)) throw error($"Undefined variable {expression.Name.Lexeme}", expression.Name);
        else expression.defineType(env.GetVarType(expression.Name.Lexeme));
        return ReturnStatus.Never;
    }

    public ReturnStatus visitVariableStmt(VariableStmt stmt)
    {
        Evaluate(stmt.initializer);
        env.defineVar(stmt.Name.Lexeme, stmt.initializer.type);
        return ReturnStatus.Never;
    }

    public ReturnStatus visitWhileStmt(WhileStmt stmt)
    {
        canDeclareFunction = false;
        Evaluate(stmt.Condition);
        if (stmt.Condition.type is not ExpressionType.BOOL) throw error("While condition must be bool", stmt.keyword);
        ReturnStatus status = stmt.Body.Accept(this);
        return stmt.Condition is Literal lit && lit.Value is true ? status : ReturnStatus.Never;
    }
    private void Evaluate(Expression expression) => expression.Accept(this);
    private CompilerException error(string message, Token token)
    {
        return new CompilerException("Semantic", message, token);
    }
    private void ExecuteBlock(List<Stmt> statements, SemanticEnvironment environment, ref ReturnStatus status)
    {
        SemanticEnvironment prev = env;
        try
        {
            env = environment;
            foreach (Stmt stmt in statements)
            {
                if(status!=ReturnStatus.Always)
                status = stmt.Accept(this);
                else stmt.Accept(this);
            }
        }
        finally
        {

            this.env = prev;
        }
    }
}
public enum ReturnStatus
{
    Always, Never, Maybe
}