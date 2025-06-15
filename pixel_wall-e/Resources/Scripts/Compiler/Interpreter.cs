using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
public class Interpreter : IExpressionVisitor<object>, IStatementVisitor<object>
{
    private LabelTable labels = new LabelTable();
    private Environment environment;
    private int callDepth = 0;
    public Environment globals = new Environment();
    public List<RunTimeError> exceptions = new List<RunTimeError>();
    int current = 0;
    public Interpreter()
    {
        environment = new Environment();
        globals.defineNative("GetActualX", new NativeFunctions(0, _ => Canvas.GetActualX()));//_=> ignores args and returns Canvas.GetActualX
        globals.defineNative("GetActualY", new NativeFunctions(0, _ => Canvas.GetActualY()));
        globals.defineNative("GetCanvasSize", new NativeFunctions(0, _ => Canvas.GetCanvasSize()));
        globals.defineNative("GetColorCount", new NativeFunctions(5, args =>
        {
            var color = args[0] as string;
            var x1 = Convert.ToInt32(args[1]);
            var y1 = Convert.ToInt32(args[2]);
            var x2 = Convert.ToInt32(args[3]);
            var y2 = Convert.ToInt32(args[4]);
            return Canvas.GetColorCount(color, x1, y1, x2, y2);
        }
        ));
        globals.defineNative("ConvertToHex", new NativeFunctions(3, args =>
        {
            Dictionary<int, string> HexEquivalence = new Dictionary<int, string>()
            {
                [0] = "0",
                [1] = "1",
                [2] = "2",
                [3] = "3",
                [4] = "4",
                [5] = "5",
                [6] = "6",
                [7] = "7",
                [8] = "8",
                [9] = "9",
                [10] = "A",
                [11] = "B",
                [12] = "C",
                [13] = "D",
                [14] = "E",
                [15] = "F"
            };
            string hex = "#";
            foreach (int X in args)
            {
                int actual =Math.Clamp(X,0,255);
                int div = actual/16;
                int rest = actual%16;
                hex+=HexEquivalence[div];
                hex+=HexEquivalence[rest];
            }
            return hex;
        }));
        globals.defineNative("IsBrushColor", new NativeFunctions(1, args =>
        {
            return Canvas.IsBrushColor(args[0] as string);
        }));
        globals.defineNative("IsBrushSize", new NativeFunctions(1, args =>
        {
            return Canvas.IsBrushSize(Convert.ToInt32(args[0]));
        }));
        globals.defineNative("IsCanvasColor", new NativeFunctions(3, args => Canvas.IsCanvasColor(args[0] as string, Convert.ToInt32(args[1]), Convert.ToInt32(args[2]))));
        environment = globals;

    }
    public void Interpret(List<Stmt> Statements)
    {
        try
        {
            for (int i = 0; i < Statements.Count; i++)
            {
                if (Statements[i] is LabelStmt)
                {
                    labels.Add(((LabelStmt)Statements[i]).Name.Lexeme, i);
                }
                if (Statements[i] is FunctionStmt)
                    execute(Statements[i]);
            }
            while (current < Statements.Count)
            {
                if (Statements[current] is not LabelStmt && Statements[current] is not FunctionStmt)
                    execute(Statements[current]);
                current++;
                callDepth = 0;
            }
            current = 0;
        }
        catch (RunTimeError error)
        {
            exceptions.Add(error);
        }
        catch (Return)
        {
            exceptions.Add(new RunTimeError("You have returned sucessfully"));
        }
    }
    private void execute(Stmt stmt)
    {
        stmt.Accept(this);
    }
    private string stringify(object ob)
    {
        if (ob == null) return "null";
        if (ob is int)
        {
            string text = ob.ToString();
            return text;
        }
        return ob.ToString();
    }
    #region Expressions
    public object visitLiteralExpression(Literal expression) => int.TryParse(expression.Value.ToString(), out int value) ? value : expression.Value;
    public object visitGroupingExpression(Grouping expression) => Evaluate(expression.Expression);
    private object Evaluate(Expression expression) => expression.Accept(this);
    public object visitUnaryExpression(Unary expression)
    {
        object value = Evaluate(expression.Expression);
        switch (expression.UnaryOperator.Type)
        {
            case TokenType.MINUS:
                checkNumberOperand(expression.UnaryOperator, value);
                return -(int)value;
            case TokenType.NOT:
                return !isTrue(value);
        }
        return null;
    }
    private bool isTrue(object value)
    {
        if (value == null) return false;
        if (value is bool) return (bool)value;
        return true;
    }
    public object visitBinaryExpression(Binary expression)
    {
        object leftBranch = Evaluate(expression.Left);
        object rightBranch = Evaluate(expression.Right);
        switch (expression.Operator.Type)
        {
            case TokenType.GREATER:
                checkNumberOperands(expression.Operator, leftBranch, rightBranch);
                return (int)leftBranch > (int)rightBranch;
            case TokenType.GREATER_EQUAL:
                checkNumberOperands(expression.Operator, leftBranch, rightBranch);
                return (int)leftBranch >= (int)rightBranch;
            case TokenType.LESS:
                checkNumberOperands(expression.Operator, leftBranch, rightBranch);
                return (int)leftBranch < (int)rightBranch;
            case TokenType.LESS_EQUAL:
                checkNumberOperands(expression.Operator, leftBranch, rightBranch);
                return (int)leftBranch <= (int)rightBranch;
            case TokenType.NOT_EQUAL:
                checkNumberOperands(expression.Operator, leftBranch, rightBranch);
                return !IsEqual(leftBranch, rightBranch);
            case TokenType.EQUAL_EQUAL:
                checkNumberOperands(expression.Operator, leftBranch, rightBranch);
                return IsEqual(leftBranch, rightBranch);
            case TokenType.MINUS:
                checkNumberOperands(expression.Operator, leftBranch, rightBranch);
                return (int)leftBranch - (int)rightBranch;
            case TokenType.PLUS:
                checkNumberOperands(expression.Operator, leftBranch, rightBranch);
                return (int)leftBranch + (int)rightBranch;
            case TokenType.SLASH:
                checkNumberOperands(expression.Operator, leftBranch, rightBranch);
                if ((int)rightBranch == 0) throw new RunTimeError(expression.Operator, "You cannot divide by zero");
                return (int)leftBranch / (int)rightBranch;
            case TokenType.BY:
                checkNumberOperands(expression.Operator, leftBranch, rightBranch);
                return (int)leftBranch * (int)rightBranch;
            case TokenType.MODULE:
                checkNumberOperands(expression.Operator, leftBranch, rightBranch);
                return (int)leftBranch % (int)rightBranch;
            case TokenType.POW:
                checkNumberOperands(expression.Operator, leftBranch, rightBranch);
                return (int)Math.Pow((int)leftBranch, (int)rightBranch);
            case TokenType.ROOT:
                checkNumberOperands(expression.Operator, leftBranch, rightBranch);
                return (int)Math.Pow((int)rightBranch, (double)(1 / Convert.ToDouble(leftBranch)));
        }
        return null;
    }
    private bool IsEqual(object a, object b)
    {
        if (a == null && b == null) return true;
        if (a == null) return false;
        return a.Equals(b);
    }
    private void checkNumberOperand(Token @operator, object operand)
    {
        if (operand is Int32) return;
        throw new RunTimeError(@operator, "Operand must be a number");
    }
    private void checkNumberOperands(Token @operator, object left, object right)
    {
        if (left is Int32 && right is Int32) return;
        throw new RunTimeError(@operator, "Operands must be numbers");
    }

    public object visitAssignExpression(Assign expression)
    {
        object value = Evaluate(expression.Value);
        environment.Assign(expression.Name, value);
        return value;
    }

    public object visitCallExpression(Call expression)
    {
        callDepth++;
        if (callDepth > 100000) throw new RunTimeError($"StackOverflow: Deepth recurtion. Calls:{callDepth}");
        object callee = expression.Callee;
        if (callee is not Variable) throw new RunTimeError(expression.Paren, "Callee must be an identifier");
        callee = environment.GetFunc(((Variable)callee).Name);
        List<object> arguments = new List<object>();
        foreach (Expression arg in expression.Arguments)
        {
            arguments.Add(Evaluate(arg));
        }
        if (!(callee is IPixelWallECallable))
        {
            throw new RunTimeError(expression.Paren, "You can only call functions");
        }

        IPixelWallECallable function = (IPixelWallECallable)callee;
        if (arguments.Count != function.Arity())
        {
            throw new RunTimeError(expression.Paren, $"Expected {function.Arity()} arguments but got {arguments.Count}.");
        }
        return function.Call(this, arguments);
    }

    public object visitLogicalExpression(Logical expression)
    {
        object left = Evaluate(expression.Left);
        if (expression.Operator.Type == TokenType.OR)
        {
            if (isTrue(left)) return left;
        }
        else
        {
            if (!isTrue(left)) return left;
        }
        return Evaluate(expression.Right);
    }

    public object visitVariableExpression(Variable expression)
    {
        return environment.Get(expression.Name);
    }
    #endregion
    #region stmts
    public object visitBlockStmt(BlockStmt stmt)
    {
        executeBlock(stmt.Statements, new Environment(environment));
        return null;
    }
    public void executeBlock(List<Stmt> statements, Environment environment)
    {
        Environment previous = this.environment;
        try
        {
            this.environment = environment;
            foreach (Stmt statement in statements)
            {
                execute(statement);
            }
        }
        finally
        {

            this.environment = previous;
        }
    }

    public object visitExpressionStmt(ExpressionStmt stmt)
    {
        Evaluate(stmt.Expr);
        return null;
    }

    public object visitFunctionStmt(FunctionStmt stmt)
    {
        PixellWallEFunctions function = new PixellWallEFunctions(stmt);
        environment.defineFunc(stmt.Name, function);
        return null;
    }

    public object visitIfStmt(IfStmt stmt)
    {
        if (isTrue(Evaluate(stmt.condition)))
        {/*
            if (stmt.thenBranch is BlockStmt)
            {
                foreach (Stmt statement in ((BlockStmt)stmt.thenBranch).Statements)
                {
                    execute(statement);
                }
            }
            else*/
            execute(stmt.thenBranch);
        }
        else if (stmt.elseBranch != null)
        {/*
            if (stmt.elseBranch is BlockStmt)
            {
                foreach (Stmt statement in ((BlockStmt)stmt.elseBranch).Statements)
                {
                    execute(statement);
                }
            }
            else */
            execute(stmt.elseBranch);
        }
        return null;
    }

    public object visitReturnStmt(ReturnStmt stmt)
    {
        object value = null;
        if (stmt.value != null) value = Evaluate(stmt.value);
        throw new Return(value);
    }

    public object visitGoToStmt(GoToStmt stmt)
    {
        int index = labels.GetPosition(stmt.Label);
        stmt.maxRepetition--;
        if (stmt.maxRepetition <= 0) throw new RunTimeError(stmt.Keyword, "Infinit Loop");
        object condition = Evaluate(stmt.Condition);
        if (isTrue(condition))
        {
            current = index;
        }
        return null;
    }

    public object visitWhileStmt(WhileStmt stmt)
    {
        int maxRepetition = 10000000;
        while (isTrue(Evaluate(stmt.Condition)))
        {
            /*if (stmt.Body is BlockStmt)
            {
                foreach (Stmt statement in ((BlockStmt)stmt.Body).Statements)
                {
                    execute(statement);
                }
            }
            else*/
            maxRepetition--;
            if (maxRepetition <= 0) throw new RunTimeError(stmt.keyword, "Endles cycle");
            execute(stmt.Body);
        }
        return null;
    }
    public object visitSpawnStmt(SpawnStmt stmt)
    {
        object x = Evaluate(stmt.X);
        object y = Evaluate(stmt.Y);
        if (x is not int || y is not int) throw new RunTimeError(stmt.keyword, "Coordenates must be ints");
        if ((int)x < 0 || (int)y < 0 || (int)x >= Canvas.GetCanvasSize() || (int)y >= Canvas.GetCanvasSize()) throw new RunTimeError(stmt.keyword, "The Spawn position is out of the bounds of the canvas");
        Canvas.Spawn((int)x, (int)y);
        return null;
    }

    public object visitColorStmt(ColorStmt stmt)
    {
        string color = Evaluate(stmt.ColorName).ToString();
        Canvas.ChangeColor(color);
        return null;
    }

    public object visitSizeStmt(SizeStmt stmt)
    {
        object k = Evaluate(stmt.K);
        if (k is not int) throw new RunTimeError(stmt.keyword, "Size argument k must be an int");
        if ((int)k <= 0) throw new RunTimeError(stmt.keyword, "Size argument k must be greater than or equal to zero");
        Canvas.ChangeSize((int)k);
        return null;
    }

    public object visitDrawLineStmt(DrawLineStmt stmt)
    {
        object DirX = Evaluate(stmt.DirX);
        object DirY = Evaluate(stmt.DirY);
        object Distance = Evaluate(stmt.Distance);
        if (DirX is not int) throw new RunTimeError(stmt.keyword, "DrawLine argument DirX must be an int");
        if (DirY is not int) throw new RunTimeError(stmt.keyword, "DrawLine argument DirY must be an int");
        if (Distance is not int) throw new RunTimeError(stmt.keyword, "DrawLine argument Distance must be an int");
        if ((int)DirX < -1 || (int)DirX > 1) throw new RunTimeError(stmt.keyword, "DrawLine argument DirX must be 1, 0 or -1");
        if ((int)DirY < -1 || (int)DirY > 1) throw new RunTimeError(stmt.keyword, "DrawLine argument DirY must be 1, 0 or -1");
        if (!ValidDir((int)DirX, (int)DirY)) throw new RunTimeError(stmt.keyword, "Not valid direction");
        if ((int)Distance < 0) throw new RunTimeError(stmt.keyword, "DrawLine argument Distance must be greater than or equal to zero");
        int endPointX = Canvas.robot.X + (int)DirX * (int)Distance;
        int endPointY = Canvas.robot.Y + (int)DirY * (int)Distance;
        if (endPointX < 0 || endPointX >= Canvas.GetCanvasSize() || endPointY < 0 || endPointY >= Canvas.GetCanvasSize()) throw new RunTimeError(stmt.keyword, "If you paint that line Wall-E will be out of the bounds of the canvas");
        Canvas.DrawLine((int)DirX, (int)DirY, (int)Distance);
        return null;
    }
    private bool ValidDir(int DirX, int DirY)
    {
        switch ((DirX, DirY))
        {
            case (-1, -1):
            case (-1, 0):
            case (-1, 1):
            case (0, 1):
            case (1, 1):
            case (1, 0):
            case (1, -1):
            case (0, -1):
                return true;
        }
        return false;
    }

    public object visitDrawCircleStmt(DrawCircleStmt stmt)
    {
        object DirX = Evaluate(stmt.DirX);
        object DirY = Evaluate(stmt.DirY);
        object Radius = Evaluate(stmt.Radius);
        if (DirX is not int) throw new RunTimeError(stmt.keyword, "DrawCircle argument DirX must be an int");
        if (DirY is not int) throw new RunTimeError(stmt.keyword, "DrawCircle argument DirY must be an int");
        if (Radius is not int) throw new RunTimeError(stmt.keyword, "DrawCircle argument Radius must be an int");
        if ((int)DirX < -1 || (int)DirX > 1) throw new RunTimeError(stmt.keyword, "DrawCircle argument DirX must be 1, 0 or -1");
        if ((int)DirY < -1 || (int)DirY > 1) throw new RunTimeError(stmt.keyword, "DrawCircle argument DirY must be 1, 0 or -1");
        if (!ValidDir((int)DirX, (int)DirY)) throw new RunTimeError(stmt.keyword, "Not valid direction");
        if ((int)Radius <= 0) throw new RunTimeError(stmt.keyword, "DrawCircle argument Radius must be greater than zero");
        int endPointX = Canvas.robot.X + (int)DirX * (int)Radius;
        int endPointY = Canvas.robot.Y + (int)DirY * (int)Radius;
        if (endPointX < 0 || endPointX >= Canvas.GetCanvasSize() || endPointY < 0 || endPointY >= Canvas.GetCanvasSize()) throw new RunTimeError(stmt.keyword, "If you paint that circle Wall-E will be out of the bounds of the canvas");
        Canvas.DrawCircle((int)DirX, (int)DirY, (int)Radius);
        return null;
    }

    public object visitDrawRectangleStmt(DrawRectangleStmt stmt)
    {
        object DirX = Evaluate(stmt.DirX);
        object DirY = Evaluate(stmt.DirY);
        object Distance = Evaluate(stmt.Distance);
        object Width = Evaluate(stmt.Width);
        object Height = Evaluate(stmt.Height);
        //Error control
        if (DirX is not int) throw new RunTimeError(stmt.keyword, "DrawRectangle argument DirX must be an int");
        if (DirY is not int) throw new RunTimeError(stmt.keyword, "DrawRectangle argument DirY must be an int");
        if (Distance is not int) throw new RunTimeError(stmt.keyword, "DrawRectangle argument Distance must be an int");
        if (Width is not int) throw new RunTimeError(stmt.keyword, "DrawRectangle argument Width must be an int");
        if (Height is not int) throw new RunTimeError(stmt.keyword, "DrawRectangle argument Height must be an int");
        if ((int)DirX < -1 || (int)DirX > 1) throw new RunTimeError(stmt.keyword, "DrawRectangle argument DirX must be 1, 0 or -1");
        if ((int)DirY < -1 || (int)DirY > 1) throw new RunTimeError(stmt.keyword, "DrawRectangle argument DirY must be 1, 0 or -1");
        if (!ValidDir((int)DirX, (int)DirY)) throw new RunTimeError(stmt.keyword, "Not valid direction");
        if ((int)Distance < 0) throw new RunTimeError(stmt.keyword, "DrawRectangle argument Distance must be greater than or equal to zero");
        if ((int)Width <= 0) throw new RunTimeError(stmt.keyword, "DrawRectangle argument Width must be greater than zero");
        if ((int)Height <= 0) throw new RunTimeError(stmt.keyword, "DrawRectangle argument Height must be greater than zero");
        int endPointX = Canvas.robot.X + (int)DirX * (int)Distance;
        int endPointY = Canvas.robot.Y + (int)DirY * (int)Distance;
        if (endPointX < 0 || endPointX >= Canvas.GetCanvasSize() || endPointY < 0 || endPointY >= Canvas.GetCanvasSize()) throw new RunTimeError(stmt.keyword, "If you paint that rectangle Wall-E will be out of the bounds of the canvas");
        Canvas.DrawRectangle((int)DirX, (int)DirY, (int)Distance, (int)Width, (int)Height);
        return null;
    }

    public object visitFillStmt(FillStmt stmt)
    {
        Canvas.Fill();
        return null;
    }

    public object visitLabelStmt(LabelStmt stmt)
    {
        return null;
    }

    public object visitVariableStmt(VariableStmt stmt)
    {
        object value = Evaluate(stmt.initializer);
        environment.define(stmt.Name.Lexeme, value);
        return null;
    }
    #endregion
}

public class RunTimeError : Exception
{
    public readonly Token token;
    private readonly string message;

    public RunTimeError(Token token, string message)
    {
        this.token = token;
        this.message = message;
    }
    public RunTimeError(string message)
    {
        this.message = message;
    }
    public override string ToString()
    {
        if (token is null) return $"Runtime exception: {message}";
        return $"RunTime exception: {message} at Line: {token.Line} Column: {token.Column}. (Token {token.Lexeme})";
    }
}
public class Return : Exception
{
    public readonly object value;
    public Return(object value) : base(null, null)
    {
        this.value = value;
    }
}