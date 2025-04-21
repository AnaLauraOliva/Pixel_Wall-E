using System;
using System.Collections.Generic;
using System.Drawing;

public class Parser
{
    private readonly List<Token> _tokens;
    private List<CompilerException> exceptions;
    private int _current = 0;
    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
        exceptions = new List<CompilerException>();
    }
    public List<CompilerException> GetCompilerExceptions() => exceptions;
    private void synchronize()
    {
        Advance();
        while (!IsAtEnd())
        {
            if (previous().Type == TokenType.EOL) return;

            switch (peek().Type)
            {
                case TokenType.COLOR:
                case TokenType.IDENTIFIER:
                case TokenType.FILL:
                case TokenType.DRAWCIRCLE:
                case TokenType.DRAWLINE:
                case TokenType.DRAWRECTANGLE:
                case TokenType.FOR:
                case TokenType.FUNCTION:
                case TokenType.GETACTUALX:
                case TokenType.GETACTUALY:
                case TokenType.GETCANVASSIZE:
                case TokenType.GETCOLORCOUNT:
                case TokenType.GOTO:
                case TokenType.IF:
                case TokenType.ISBRUSHCOLOR:
                case TokenType.ISBRUSHSIZE:
                case TokenType.ISCANVASCOLOR:
                case TokenType.RETURN:
                case TokenType.SIZE:
                case TokenType.SPAWN:
                case TokenType.WHILE:
                    return;
            }
            Advance();
        }
    }
    //Expression precedence
    //expression -> equality
    //equality   -> comparison ((!=|==)comparison)*
    //comparison -> term((>|>=|<|<=)term)*
    //term       -> factor((+|-)factor)*
    //factor     -> PowRoot((*|/)PowRoot)*
    //PowRoot    -> Unary((**|√)Unary)*
    //Unary      -> (!|-)Unary|Primary
    //Primary(rough verssion) -> Number|String|true|false|(expression)
    //If you want to extend it put a new method on the right precedence method and fix the previus method

    public List<Stmt> parse()
    {
        int counter = 0;
        List<Stmt> statements = new List<Stmt>();
        while (!IsAtEnd())
        {
            while (check(TokenType.EOL))
            {
                Advance();
                if(IsAtEnd())return statements;
                
            }
            counter = _current;
            statements.Add(declaration());
            if (counter == _current)
            {
                _current++;
            }
        }
        return statements;
    }
    private Stmt declaration()
    {
        try
        {
            if (match(TokenType.IDENTIFIER))
            { 
                return VarDeclaration();
            }
            return Statement();
        }
        catch (CompilerException exception)
        {
            exceptions.Add(exception);
            synchronize();
            return null;
        }
    }
    private Stmt VarDeclaration()
    {
        Token name = previous();
        if (IsAtEnd() || check(TokenType.EOL)) return new LabelStmt(name);
        consume(TokenType.ASSIGNMENT, "Expected <- after variable name");
        Expression initializer = expression();
        if(initializer is Literal && ((Literal)initializer).Value is string)
        {
            ParseError(name,"the value of the variable cannot be a string");
            initializer = null;
        }
        if (peek().Type != TokenType.EOF)
            consume(TokenType.EOL, @"Expected line break ('\n') at end of statement");
        return new VariableStmt(name, initializer);
    }
    private Stmt Statement()
    {
        if (match(TokenType.COLOR)) return ColorStatement();
        if (match(TokenType.DRAWCIRCLE)) return DrawCircleStatement();
        if (match(TokenType.DRAWLINE)) return DrawLineStatement();
        if (match(TokenType.DRAWRECTANGLE)) return DrawRectangleStatement();
        if (match(TokenType.FILL)) return FillStatement();
        if (match(TokenType.SIZE)) return SizeStatement();
        if (match(TokenType.SPAWN)) return SpawnStatement();
        if (match(TokenType.LEFT_CURLY_BRACE)) return new BlockStmt(block());
        return ExpressionStatement();
    }
    private List<Stmt> block()
    {
        List<Stmt> statements = new List<Stmt>();
        while (!check(TokenType.RIGHT_CURLY_BRACE) && !IsAtEnd())
        {
            statements.Add(declaration());
        }
        consume(TokenType.RIGHT_CURLY_BRACE, "Expected '}' after block.");
        return statements;
    }
    private Stmt SpawnStatement()
    {
        Token key = previous();
        consume(TokenType.LEFT_PAREN, "Expected ( after Spawn instruction");
        Expression x = expression();
        consume(TokenType.COMMA, "Expected , after expression");
        Expression y = expression();
        consume(TokenType.RIGHT_PAREN, "Expected ) after expression");
        if (peek().Type != TokenType.EOF)
            consume(TokenType.EOL, @"Expected line break ('\n') at end of statement");
        return new SpawnStmt(x, y, key);
    }
    private Stmt ExpressionStatement()
    {
        Expression expr = expression();
        if (peek().Type != TokenType.EOF)
            consume(TokenType.EOL, @"Expected line break ('\n') at end of statement");
        return new ExpressionStmt(expr);

    }
    private Stmt SizeStatement()
    {
        Token key = previous();
        consume(TokenType.LEFT_PAREN, "Expected ( after Size instruction");
        Expression k = expression();
        consume(TokenType.RIGHT_PAREN, "Expected ) after expression");
        if (peek().Type != TokenType.EOF)
            consume(TokenType.EOL, @"Expected line break ('\n') at end of statement");
        return new SizeStmt(k, key);
    }
    private Stmt FillStatement()
    {
        Token key = previous();
        consume(TokenType.LEFT_PAREN, "Expected ( after Fill instruction");
        consume(TokenType.RIGHT_PAREN, "Expected ) after expression");
        if (peek().Type != TokenType.EOF)
            consume(TokenType.EOL, @"Expected line break ('\n') at end of statement");
        return new FillStmt(key);
    }
    private Stmt DrawRectangleStatement()
    {
        Token key = previous();
        consume(TokenType.LEFT_PAREN, "Expected ( after DrawRectangle instruction");
        Expression dirX = expression();
        consume(TokenType.COMMA, "Expected , after expression");
        Expression dirY = expression();
        consume(TokenType.COMMA, "Expected , after expression");
        Expression distance = expression();
        consume(TokenType.COMMA, "Expected , after expression");
        Expression width = expression();
        consume(TokenType.COMMA, "Expected , after expression");
        Expression height = expression();
        consume(TokenType.RIGHT_PAREN, "Expected ) after expression");
        if (peek().Type != TokenType.EOF)
            consume(TokenType.EOL, @"Expected line break ('\n') at end of statement");
        return new DrawRectangleStmt(dirX, dirY, distance, width, height, key);
    }
    private Stmt DrawLineStatement()
    {
        Token key = previous();
        consume(TokenType.LEFT_PAREN, "Expected ( after DrawLine instruction");
        Expression dirX = expression();
        consume(TokenType.COMMA, "Expected , after expression");
        Expression dirY = expression();
        consume(TokenType.COMMA, "Expected , after expression");
        Expression distance = expression();
        consume(TokenType.RIGHT_PAREN, "Expected ) after expression");
        if (peek().Type != TokenType.EOF)
            consume(TokenType.EOL, @"Expected line break ('\n') at end of statement");
        return new DrawLineStmt(dirX, dirY, distance, key);
    }
    private Stmt DrawCircleStatement()
    {
        Token key = previous();
        consume(TokenType.LEFT_PAREN, "Expected ( after DrawCircle instruction");
        Expression dirX = expression();
        consume(TokenType.COMMA, "Expected , after expression");
        Expression dirY = expression();
        consume(TokenType.COMMA, "Expected , after expression");
        Expression radius = expression();
        consume(TokenType.RIGHT_PAREN, "Expected ) after expression");
        if (peek().Type != TokenType.EOF)
            consume(TokenType.EOL, @"Expected line break ('\n') at end of statement");
        return new DrawCircleStmt(dirX, dirY, radius, key);
    }
    private Stmt ColorStatement()
    {
        Token key = previous();
        consume(TokenType.LEFT_PAREN, "( expected after Color instruction");
        Expression expr = expression();
        if (expr is not null && (expr is not Literal || ((Literal)expr).Value is not string)) ParseError(previous(),"The argument of Color instruction must be a string");
        consume(TokenType.RIGHT_PAREN, ") expected after ");
        if (peek().Type != TokenType.EOF)
            consume(TokenType.EOL, @"Expected line break ('\n') at end of statement");
        return new ColorStmt(expr, key);

    }
    private bool IsValidColor(string color)
    {
        if (string.IsNullOrEmpty(color) || color[0].ToString() != "#") return false;
        string hex = color.Substring(1);
        if (hex.Length != 6) return false;
        foreach (char c in hex)
        {
            if (!Uri.IsHexDigit(c)) return false;
        }
        return true;
    }
    private Expression expression()
    {
        return assignment();
    }
    private Expression assignment()
    {
        Expression expr = equality();
        if (match(TokenType.ASSIGNMENT))
        {
            Token equals = previous();
            Expression value = assignment();
            if (expr is Variable)
            {
                Token name = ((Variable)expr).Name;
                return new Assign(name, value);
            }
            ParseError(equals,"Invalid assignment target");
        }
        return expr;
    }
    private Expression equality()
    {
        Expression expression = comparison();
        while (match(new List<TokenType> { TokenType.EQUAL_EQUAL, TokenType.NOT_EQUAL }))
        {
            Token @operator = previous();
            Expression right = comparison();
            expression = new Binary(expression, @operator, right);
        }
        return expression;
    }
    private Expression comparison()
    {
        Expression expression = term();
        while (match(new List<TokenType> { TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL }))
        {
            Token @operator = previous();
            Expression right = term();
            expression = new Binary(expression, @operator, right);
        }
        return expression;
    }
    private Expression term()
    {
        Expression expression = factor();
        while (match(new List<TokenType> { TokenType.PLUS, TokenType.MINUS }))
        {
            Token @operator = previous();
            Expression right = factor();
            expression = new Binary(expression, @operator, right);
        }
        return expression;
    }
    private Expression factor()
    {
        Expression expression = powRoot();
        while (match(new List<TokenType> { TokenType.BY, TokenType.SLASH }))
        {
            Token @operator = previous();
            Expression right = powRoot();
            expression = new Binary(expression, @operator, right);
        }
        return expression;
    }
    private Expression powRoot()
    {
        Expression expression = unary();
        if (match(new List<TokenType> { TokenType.POW, TokenType.ROOT }))
        {
            Token @operator = previous();
            Expression right = powRoot();
            expression = new Binary(expression, @operator, right);
        }
        return expression;
    }
    private Expression unary()
    {
        if (match(new List<TokenType> { TokenType.NOT, TokenType.MINUS }))
        {
            Token @operator = previous();
            Expression right = unary();
            return new Unary(@operator, right);
        }
        return primary();
    }
    private Expression primary()
    {
        if (match(TokenType.FALSE)) return new Literal(false);
        if (match(TokenType.TRUE)) return new Literal(true);
        if (match(TokenType.INT)) return new Literal(previous().Literal);
        if (match(TokenType.STRING))
        {
            Token x = previous();
            if (exceptions.Count == 0)
            {
                switch (x.Literal.ToString())
                {
                    case "Red":
                        return new Literal("#FF0000");

                    case "Blue":
                        return new Literal("#0000FF");

                    case "Green":
                        return new Literal("#00FF00");

                    case "Yellow":
                        return new Literal("#FFFF00");

                    case "Orange":
                        return new Literal("#FFA500");

                    case "Purple":
                        return new Literal("#800080");

                    case "Black":
                        return new Literal("#000000");

                    case "White":
                        return new Literal("#FFFFFF");

                    case "Transparent":
                        return new Literal("Transparent");

                    default:
                        if (IsValidColor(x.Literal.ToString()))
                            return new Literal(x.Literal.ToString());
                        else
                        {
                            ParseError(x,"Invalid Color or hex code");
                            return null;
                        }

                }
            }
        }
        if (match(TokenType.IDENTIFIER)) return new Variable(previous());
        if (match(TokenType.LEFT_PAREN))
        {
            Expression expression = this.expression();
            consume(TokenType.RIGHT_PAREN, "Expected ')' after expression");
            return new Grouping(expression);

        }
        Token aux = peek();
        ParseError(aux,"Expected expression");
        return null;
    }
    private void consume(TokenType type, string message)
    {
        if (check(type)) Advance();
        else
        {
            Token aux = peek();
            exceptions.Add(new CompilerException("Syntactical", message, aux.Line, aux.Column, aux.Type == TokenType.EOF ? "at end" : aux.Lexeme));
        }
    }
    private bool match(List<TokenType> types)
    {
        foreach (TokenType type in types)
        {
            if (check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }
    private bool match(TokenType type)
    {

        if (check(type))
        {
            Advance();
            return true;
        }

        return false;
    }
    private bool check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return peek().Type == type;
    }
    private void Advance()
    {
        if (!IsAtEnd()) _current++;
    }
    //Checks if we still have tokens to parse
    private bool IsAtEnd() => peek().Type == TokenType.EOF;
    private Token peek() => _tokens[_current];
    private Token previous() => _tokens[_current - 1];
    private void ParseError(Token token, string message)
    {
        exceptions.Add(new CompilerException("Syntactical", message,token.Line, token.Column, token.Lexeme));
    }
}