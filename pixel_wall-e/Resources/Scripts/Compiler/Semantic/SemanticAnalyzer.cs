using System;
using System.Collections.Generic;
public class SemanticAnalyzer : IExpressionVisitor<object>, IStatementVisitor<object>
{
    List<Stmt> statements;
    public List<CompilerException> exceptions;
    SemanticEnvironment env = new SemanticEnvironment();
    public SemanticAnalyzer(List<Stmt> statements)
    {
        env.defineFunc("GetActualX", ExpressionType.INT, 0);
        env.defineFunc("GetActualY", ExpressionType.INT, 0);
        env.defineFunc("GetCanvasSize", ExpressionType.INT, 0);
        env.defineFunc("GetColorCount", ExpressionType.INT, 5);
        env.defineFunc("IsBrushColor", ExpressionType.INT, 1);
        env.defineFunc("IsBrushSize", ExpressionType.INT, 1);
        env.defineFunc("IsCanvasColor", ExpressionType.INT, 3);
        this.statements = statements;
        this.exceptions = new List<CompilerException>();

    }
    public void Analyze()
    {
        foreach (Stmt stmt in statements)
        {
            if (stmt is LabelStmt && ((LabelStmt)stmt).Name.Lexeme is not null)
            {
                env.defineLabel(((LabelStmt)stmt).Name.Lexeme.ToString());
            }
            else if (stmt is FunctionStmt)
            {
                if (env.HasFunc(((FunctionStmt)stmt).Name.Lexeme)) exceptions.Add(error($"This function already exists. Name{((FunctionStmt)stmt).Name.Lexeme}", ((FunctionStmt)stmt).Name));
                //else env.defineFunc(((FunctionStmt)stmt).Name.Lexeme,((FunctionStmt)stmt).Parameters)
            }
        }
        foreach (Stmt stmt in statements) AnalyzeStmt(stmt);
    }

    public void AnalyzeStmt(Stmt stmt)
    {
        stmt.Accept(this);
    }
    public object visitAssignExpression(Assign expression)
    {
        string name = expression.Name.Lexeme;
        bool? isTrue = env.Assign(name, expression.type);
        Evaluate(expression.Value);
        if (isTrue == null) exceptions.Add(error($"Undefined variable {name} at current scope", expression.Name));
        else if (isTrue == false) exceptions.Add(error($"You cannot assign {expression.type} to {env.HasVar(name)}", expression.Name));
        return null;
    }

    public object visitBinaryExpression(Binary expression)
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
                    exceptions.Add(error("Operands must be numbers", expression.Operator));
                break;
            case TokenType.EQUAL_EQUAL:
            case TokenType.NOT_EQUAL:
                if (expression.Left.type != expression.Right.type) exceptions.Add(error("Operands must be of the same type", expression.Operator));
                break;
            case TokenType.AND:
            case TokenType.OR:
                if (expression.Left.type != ExpressionType.BOOL || expression.Right.type != ExpressionType.BOOL)
                    exceptions.Add(error("Operands must be booleans", expression.Operator));
                break;
        }
        return null;
    }
    public object visitBlockStmt(BlockStmt stmt)
    {
        ExecuteBlock(stmt.Statements, new SemanticEnvironment(env));
        return null;
    }


    public object visitCallExpression(Call expression)
    {
        // expression.Callee.Accept(this);
        //List<ExpressionType> arguments;
        if (expression.Callee is Variable callee)
        {
            string funcName = callee.Name.Lexeme;
            if (!env.HasFunc(funcName))
            {
                exceptions.Add(error($"Undefined function: {funcName}", callee.Name));
            }
            else
            {
                int count = env.ReturnArgumentCount(funcName);
                expression.SetReturnType(env.getReturnType(funcName));
                if (expression.Arguments.Count != count)
                    exceptions.Add(error($"Missing arguments. Expected:{count}, Obtained: {expression.Arguments.Count}", callee.Name));
                else
                {
                    /*List<ExpressionType> types = env.getArguments(funcName);
                    for (int i = 0; i < types.Count; i++)
                    {
                        if (types[i] != expression.Arguments[i].type) exceptions.Add(error($"Incorrect type. Expected:{types[i]}, Obtained: {expression.Arguments[i].type}", callee.Name));
                    }*/
                }
            }
        }
        else
        {
            exceptions.Add(error($"You can only call functions. Invalid type:{expression.Callee.type}", expression.Paren));
        }
        foreach (var arg in expression.Arguments)
        {
            arg.Accept(this);

        }
        return null;
    }

    public object visitColorStmt(ColorStmt stmt)
    {
        stmt.ColorName.Accept(this);
        if (stmt.ColorName is null || stmt.ColorName is not Literal || ((Literal)stmt.ColorName).Value is not string) exceptions.Add(error("The argument of Color instruction must be a string", stmt.Keyword));
        return null;
    }

    public object visitDrawCircleStmt(DrawCircleStmt stmt)
    {
        stmt.DirX.Accept(this);
        if (stmt.DirX.type != ExpressionType.INT) exceptions.Add(error("DirX argument must be int", stmt.keyword));
        stmt.DirY.Accept(this);
        if (stmt.DirY.type != ExpressionType.INT) exceptions.Add(error("DirY argument must be int", stmt.keyword));
        stmt.Radius.Accept(this);
        if (stmt.Radius.type != ExpressionType.INT) exceptions.Add(error("Radius argument must be int", stmt.keyword));
        return null;
    }

    public object visitDrawLineStmt(DrawLineStmt stmt)
    {
        stmt.DirX.Accept(this);
        if (stmt.DirX.type != ExpressionType.INT) exceptions.Add(error("DirX argument must be int", stmt.keyword));
        stmt.DirY.Accept(this);
        if (stmt.DirY.type != ExpressionType.INT) exceptions.Add(error("DirY argument must be int", stmt.keyword));
        stmt.Distance.Accept(this);
        if (stmt.Distance.type != ExpressionType.INT) exceptions.Add(error("Distance argument must be int", stmt.keyword));
        return null;
    }

    public object visitDrawRectangleStmt(DrawRectangleStmt stmt)
    {
        stmt.DirX.Accept(this);
        if (stmt.DirX.type != ExpressionType.INT) exceptions.Add(error("DirX argument must be int", stmt.keyword));
        stmt.DirY.Accept(this);
        if (stmt.DirY.type != ExpressionType.INT) exceptions.Add(error("DirY argument must be int", stmt.keyword));
        stmt.Distance.Accept(this);
        if (stmt.Distance.type != ExpressionType.INT) exceptions.Add(error("Distance argument must be int", stmt.keyword));
        stmt.Height.Accept(this);
        if (stmt.Height.type != ExpressionType.INT) exceptions.Add(error("Height argument must be int", stmt.keyword));
        stmt.Width.Accept(this);
        if (stmt.Width.type != ExpressionType.INT) exceptions.Add(error("Width argument must be int", stmt.keyword));
        return null;
    }

    public object visitExpressionStmt(ExpressionStmt stmt)
    {
        stmt.Expr.Accept(this);
        return null;
    }

    public object visitFillStmt(FillStmt stmt)
    {
        return null;
    }

    public object visitForStmt(ForStmt stmt)
    {
        throw new NotImplementedException();
    }

    public object visitFunctionStmt(FunctionStmt stmt)
    {
        throw new NotImplementedException();
    }

    public object visitGoToStmt(GoToStmt stmt)
    {
        if (!env.HasLabel(stmt.Label.Lexeme)) exceptions.Add(error("This label doesn't exists", stmt.Label));
        stmt.Condition.Accept(this);
        if (stmt.Condition.type is not ExpressionType.BOOL)
        {
            exceptions.Add(error("GoTo condition must be boolean", stmt.Keyword));
        }
        return null;
    }

    public object visitGroupingExpression(Grouping expression)
    {
        expression.Expression.Accept(this);
        return null;
    }

    public object visitIfStmt(IfStmt stmt)
    {
        stmt.condition.Accept(this);
        if (stmt.condition.type is not ExpressionType.BOOL) exceptions.Add(error("If condition must be boolean", stmt.keyword));
        stmt.thenBranch.Accept(this);
        if (stmt.elseBranch is not null) stmt.elseBranch.Accept(this);
        return null;
    }

    public object visitLabelStmt(LabelStmt stmt)
    {
        return null;
    }

    public object visitLiteralExpression(Literal expression)
    {
        return null;
    }

    public object visitLogicalExpression(Logical expression)
    {
        Evaluate(expression.Left);
        Evaluate(expression.Right);
        if (expression.Left.type is not ExpressionType.BOOL || expression.Right.type is not ExpressionType.BOOL) exceptions.Add(error("Operands must be booleans", expression.Operator));
        return null;
    }

    public object visitReturnStmt(ReturnStmt stmt)
    {
        return null;
    }

    public object visitSizeStmt(SizeStmt stmt)
    {
        Evaluate(stmt.K);
        if (stmt.K.type is not ExpressionType.INT) exceptions.Add(error("argument k must be an int", stmt.keyword));
        return null;
    }

    public object visitSpawnStmt(SpawnStmt stmt)
    {
        Evaluate(stmt.X);
        if (stmt.X.type is not ExpressionType.INT) exceptions.Add(error("Argument X must bu numeric", stmt.keyword));
        Evaluate(stmt.Y);
        if (stmt.Y.type is not ExpressionType.INT) exceptions.Add(error("Argument Y must bu numeric", stmt.keyword));
        return null;
    }


    public object visitUnaryExpression(Unary expression)
    {
        Evaluate(expression.Expression);
        if (expression.UnaryOperator.Type is TokenType.MINUS && expression.Expression.type is not ExpressionType.INT)
            exceptions.Add(error($"You cannot use unary operator - with {expression.Expression.type}", expression.UnaryOperator));
        else if (expression.UnaryOperator.Type is TokenType.NOT && expression.Expression.type is not ExpressionType.BOOL)
            exceptions.Add(error($"You cannot use unary operator ! with {expression.Expression.type}", expression.UnaryOperator));
        return null;
    }

    public object visitVariableExpression(Variable expression)
    {
        if (!env.HasVar(expression.Name.Lexeme)) exceptions.Add(error($"Undefined variable {expression.Name.Lexeme}", expression.Name));
        else expression.defineType(env.GetVarType(expression.Name.Lexeme));
        return null;
    }

    public object visitVariableStmt(VariableStmt stmt)
    {
        Evaluate(stmt.initializer);
        env.defineVar(stmt.Name.Lexeme, stmt.initializer.type);
        return null;
    }

    public object visitWhileStmt(WhileStmt stmt)
    {
        Evaluate(stmt.Condition);
        if (stmt.Condition.type is not ExpressionType.BOOL) exceptions.Add(error("While condition must be bool", stmt.keyword));
        stmt.Body.Accept(this);
        return null;
    }
    private void Evaluate(Expression expression) => expression.Accept(this);
    private CompilerException error(string message, Token token)
    {
        return new CompilerException("Semantic", message, token);
    }
    private void ExecuteBlock(List<Stmt> statements, SemanticEnvironment environment)
    {
        SemanticEnvironment prev = env;
        try
        {
            env = environment;
            foreach (Stmt stmt in statements)
            {
                stmt.Accept(this);
            }
        }
        finally
        {

            this.env = prev;
        }
    }
}