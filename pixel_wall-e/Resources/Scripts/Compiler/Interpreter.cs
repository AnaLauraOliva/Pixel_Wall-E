using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
public class Interpreter : IExpressionVisitor, IStatementVisitor
{
    private LabelTable labels = new LabelTable();
    private Environment environment = new Environment();
    public List<RunTimeError> exceptions = new List<RunTimeError>();
    public void Interpret(List<Stmt> Statements)
    {
        try
        {
            for (int i = 0; i < Statements.Count; i++)
            {
                if(Statements[i] is LabelStmt)
                {
                    labels.Add(((LabelStmt)Statements[i]).Name.Lexeme, i);
                }
            }
            foreach (Stmt stmt in Statements)
            {
                execute(stmt);
            }
        }
        catch (RunTimeError error)
        {

            exceptions.Add(error);
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
    public object visitLiteralExpression(Literal expression) => expression.Value is IConvertible ? Convert.ToInt32(expression.Value) : expression.Value;
    public object visitGroupingExpression(Grouping expression) => Evaluate(expression.Expression);
    private object Evaluate(Expression expression) => expression.Accept(this);
    public object visitUnaryExpression(Unary expression)
    {
        object value = Evaluate(expression.Expression);
        switch (expression.UnaryOperator.Type)
        {
            case TokenType.MINUS:
                checkNumberOperand(expression.UnaryOperator,value);
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
                return !IsEqual(leftBranch, rightBranch);
            case TokenType.EQUAL_EQUAL:
                return IsEqual(leftBranch, rightBranch);
            case TokenType.MINUS:
                checkNumberOperands(expression.Operator, leftBranch, rightBranch);
                return (int)leftBranch - (int)rightBranch;
            case TokenType.PLUS:
                checkNumberOperands(expression.Operator, leftBranch, rightBranch);
                return (int)leftBranch + (int)rightBranch;
            case TokenType.SLASH:
                checkNumberOperands(expression.Operator, leftBranch, rightBranch);
                return (int)leftBranch / (int)rightBranch;
            case TokenType.BY:
                checkNumberOperands(expression.Operator, leftBranch, rightBranch);
                return (int)leftBranch * (int)rightBranch;
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
        throw new NotImplementedException();
    }

    public object visitLogicalExpression(Logical expression)
    {
        throw new NotImplementedException();
    }

    public object visitVariableExpression(Variable expression)
    {
        return environment.Get(expression.Name);
    }
    #endregion
#region stmts
    public void visitBlockStmt(BlockStmt stmt)
    {
        executeBlock(stmt.Statements, new Environment(environment));
    }
    private void executeBlock(List<Stmt> statements, Environment environment)
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

    public void visitExpressionStmt(ExpressionStmt stmt)
    {
        Evaluate(stmt.Expr);
    }

    public void visitFunctionStmt(FunctionStmt stmt)
    {
        throw new NotImplementedException();
    }

    public void visitIfStmt(IfStmt stmt)
    {
        throw new NotImplementedException();
    }

    public void visitReturnStmt(ReturnStmt stmt)
    {
        throw new NotImplementedException();
    }

    public void visitGoToStmt(GoToStmt stmt)
    {
        throw new NotImplementedException();
    }

    public void visitWhileStmt(WhileStmt stmt)
    {
        throw new NotImplementedException();
    }

    public void visitForStmt(ForStmt stmt)
    {
        throw new NotImplementedException();
    }

    public void visitSpawnStmt(SpawnStmt stmt)
    {
        object x = Evaluate(stmt.X);
        object y = Evaluate(stmt.Y);
        if(x is not int || y is not int) throw new RunTimeError(stmt.keyword, "Coordenates must be ints");
        if((int)x<0||(int)y<0||(int)x>=Canvas.GetCanvasSize()||(int)y>=Canvas.GetCanvasSize())throw new RunTimeError(stmt.keyword, "The Spawn position is out of the bounds of the canvas");
        Canvas.Spawn((int)x,(int)y);
    }

    public void visitColorStmt(ColorStmt stmt)
    {
        Canvas.ChangeColor(((Literal)stmt.ColorName).Value.ToString());
    }

    public void visitSizeStmt(SizeStmt stmt)
    {
        object k = Evaluate(stmt.K);
        if(k is not int) throw new RunTimeError(stmt.keyword, "Size argument k must be an int");
        if((int)k<=0) throw new RunTimeError(stmt.keyword, "Size argument k must be greater than or equal to zero");
        Canvas.ChangeSize((int)k);
    }

    public void visitDrawLineStmt(DrawLineStmt stmt)
    {
        object DirX = Evaluate(stmt.DirX);
        object DirY = Evaluate(stmt.DirY);
        object Distance = Evaluate(stmt.Distance);
        if(DirX is not int) throw new RunTimeError(stmt.keyword, "DrawLine argument DirX must be an int");
        if(DirY is not int) throw new RunTimeError(stmt.keyword, "DrawLine argument DirY must be an int");
        if(Distance is not int) throw new RunTimeError(stmt.keyword, "DrawLine argument Distance must be an int");
        if((int)DirX<-1||(int)DirX>1) throw new RunTimeError(stmt.keyword, "DrawLine argument DirX must be 1, 0 or -1");
        if((int)DirY<-1||(int)DirY>1) throw new RunTimeError(stmt.keyword, "DrawLine argument DirY must be 1, 0 or -1");
        if((int)Distance<=0)  throw new RunTimeError(stmt.keyword, "DrawLine argument Distance must be greater than zero");
        int endPointX = Canvas.robot.X + (int)DirX*(int)Distance;
        int endPointY = Canvas.robot.Y + (int)DirY*(int)Distance;
        if(endPointX<0||endPointX>=Canvas.GetCanvasSize()||endPointY<0||endPointY>=Canvas.GetCanvasSize()) throw new RunTimeError(stmt.keyword, "If you paint that line Wall-E will be out of the bounds of the canvas");
        Canvas.DrawLine((int)DirX, (int)DirY, (int)Distance);
    }

    public void visitDrawCircleStmt(DrawCircleStmt stmt)
    {
        object DirX = Evaluate(stmt.DirX);
        object DirY = Evaluate(stmt.DirY);
        object Radius = Evaluate(stmt.Radius);
        if(DirX is not int) throw new RunTimeError(stmt.keyword, "DrawCircle argument DirX must be an int");
        if(DirY is not int) throw new RunTimeError(stmt.keyword, "DrawCircle argument DirY must be an int");
        if(Radius is not int) throw new RunTimeError(stmt.keyword, "DrawCircle argument Radius must be an int");
        if((int)DirX<-1||(int)DirX>1) throw new RunTimeError(stmt.keyword, "DrawCircle argument DirX must be 1, 0 or -1");
        if((int)DirY<-1||(int)DirY>1) throw new RunTimeError(stmt.keyword, "DrawCircle argument DirY must be 1, 0 or -1");
        if((int)Radius<=0)  throw new RunTimeError(stmt.keyword, "DrawCircle argument Radius must be greater than zero");
        int endPointX = Canvas.robot.X + (int)DirX*(int)Radius;
        int endPointY = Canvas.robot.Y + (int)DirY*(int)Radius;
        if(endPointX<0||endPointX>=Canvas.GetCanvasSize()||endPointY<0||endPointY>=Canvas.GetCanvasSize()) throw new RunTimeError(stmt.keyword, "If you paint that circle Wall-E will be out of the bounds of the canvas");
        Canvas.DrawCircle((int)DirX, (int)DirY, (int)Radius);
    }

    public void visitDrawRectangleStmt(DrawRectangleStmt stmt)
    {
        object DirX = Evaluate(stmt.DirX);
        object DirY = Evaluate(stmt.DirY);
        object Distance = Evaluate(stmt.Distance);
        object Width = Evaluate(stmt.Width);
        object Height = Evaluate(stmt.Height);
        if(DirX is not int) throw new RunTimeError(stmt.keyword, "DrawRectangle argument DirX must be an int");
        if(DirY is not int) throw new RunTimeError(stmt.keyword, "DrawRectangle argument DirY must be an int");
        if(Distance is not int) throw new RunTimeError(stmt.keyword, "DrawRectangle argument Distance must be an int");
        if(Width is not int) throw new RunTimeError(stmt.keyword, "DrawRectangle argument Width must be an int");
        if(Height is not int) throw new RunTimeError(stmt.keyword, "DrawRectangle argument Height must be an int");
        if((int)DirX<-1||(int)DirX>1) throw new RunTimeError(stmt.keyword, "DrawRectangle argument DirX must be 1, 0 or -1");
        if((int)DirY<-1||(int)DirY>1) throw new RunTimeError(stmt.keyword, "DrawRectangle argument DirY must be 1, 0 or -1");
        if((int)Distance<=0)  throw new RunTimeError(stmt.keyword, "DrawRectangle argument Distance must be greater than zero");
        if((int)Width<=0)  throw new RunTimeError(stmt.keyword, "DrawRectangle argument Width must be greater than zero");
        if((int)Height<=0)  throw new RunTimeError(stmt.keyword, "DrawRectangle argument Height must be greater than zero");
        int endPointX = Canvas.robot.X + (int)DirX*(int)Distance;
        int endPointY = Canvas.robot.Y + (int)DirY*(int)Distance;
        if(endPointX<0||endPointX>=Canvas.GetCanvasSize()||endPointY<0||endPointY>=Canvas.GetCanvasSize()) throw new RunTimeError(stmt.keyword, "If you paint that rectangle Wall-E will be out of the bounds of the canvas");
        Canvas.DrawRectangle((int)DirX, (int)DirY, (int)Distance, (int)Width, (int)Height);
    }

    public void visitFillStmt(FillStmt stmt)
    {
        Canvas.Fill();
    }

    public void visitLabelStmt(LabelStmt stmt)
    {
        
    }

    public void visitVariableStmt(VariableStmt stmt)
    {
        object value = Evaluate(stmt.initializer);
        environment.define(stmt.Name.Lexeme, value);
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
    public override string ToString()
    {
        return $"RunTime exception: {message} at Line: {token.Line} Column: {token.Column}. (Token {token.Lexeme})";
    }
}