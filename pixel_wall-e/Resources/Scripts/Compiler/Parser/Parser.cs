using System;
using System.Collections.Generic;
//using System.Drawing;

public class Parser
{
    private readonly List<Token> _tokens;
    private List<CompilerException> exceptions;
    public Dictionary<string,ExpressionType> vars;
    private int _current = 0;
    private int spawnCount = 0;
    public Parser(List<Token> tokens)
    {
        vars=new Dictionary<string, ExpressionType>();
        _tokens = tokens;
        exceptions = new List<CompilerException>();
    }
    public List<CompilerException> GetCompilerExceptions() => exceptions;
    private void synchronize()
    {
        Advance();
        while (!IsAtEnd())
        {
            if (previous().Type == TokenType.EOL)
            {
                //exceptions.Remove(exceptions[exceptions.Count-1]);
                return;
            }

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
        List<Stmt> statements = new List<Stmt>();
        while (!IsAtEnd())
        {
            statements.Add(declaration());
        }
        return statements;
    }
    private Stmt declaration()
    {
        try
        {
            ConsumeEOL();
            if (spawnCount == 0 && match(TokenType.SPAWN))
            {
                spawnCount = 1;
                return SpawnStatement();
            }
            else if (spawnCount == 0 && !match(TokenType.SPAWN))
            {
                throw new CompilerException("Syntactical", "You have to beggin your code spawning Wall-E", peek());
            }
            if (match(TokenType.IDENTIFIER))
            {
                if((check(TokenType.EOL)||check(TokenType.EOF)||!vars.ContainsKey(previous().Lexeme))&&!check(TokenType.LEFT_PAREN))
                return VarDeclaration();
                else GoPrevious();
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
    private Stmt VarDeclaration(bool isFor = false)
    {
        Token name = previous();
        if (IsAtEnd() || check(TokenType.EOL))
        {
            return new LabelStmt(name);
        }
        consume(TokenType.ASSIGNMENT, "Expected <- after variable name");
        Expression initializer = expression();
        if (initializer is Literal && ((Literal)initializer).Value is string)
        {
            throw error(name, "the value of the variable cannot be a string");
        }
        if (peek().Type != TokenType.EOF && !isFor)
            consume(TokenType.EOL, @"Expected line break ('\n') at end of variable declaration");
        vars.Add(name.Lexeme, initializer.type);
        return new VariableStmt(name, initializer);
    }
    private Stmt Statement()
    {
        if (IsAtEnd()) return null;
        if (match(TokenType.COLOR)) return ColorStatement();
        if (match(TokenType.DRAWCIRCLE)) return DrawCircleStatement();
        if (match(TokenType.DRAWLINE)) return DrawLineStatement();
        if (match(TokenType.DRAWRECTANGLE)) return DrawRectangleStatement();
        if (match(TokenType.FILL)) return FillStatement();
        if (match(TokenType.SIZE)) return SizeStatement();
        if (spawnCount != 0 && match(TokenType.SPAWN)) throw new CompilerException("Syntactical", "You can only spawn Wall-E one time", previous());
        if (match(TokenType.LEFT_CURLY_BRACE)) return new BlockStmt(block());
        if (match(TokenType.IF)) return ifStatement();
        if (match(TokenType.WHILE)) return whileStatement();
        if (match(TokenType.GOTO)) return GoToStatement();
        if (match(TokenType.FOR)) return ForStatement();
        if (match(TokenType.ELSE)) throw new CompilerException("Syntactical", "Else cannot start a statement", previous());
        return ExpressionStatement();
    }
    private Stmt ExpressionStatement()
    {
        Expression expr = expression();
        if (peek().Type != TokenType.EOF)
            consume(TokenType.EOL, @"Expected line break ('\n') at end of statement");
        if(expr is Assign) return new ExpressionStmt(expr);
        if(expr is Call) return new ExpressionStmt(expr);
        throw new CompilerException("Syntactical", "Unknown statement", peek());
    }
    private Stmt ForStatement()
    {
        Token keyword = previous();
        consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'");
        Stmt initializer;
        if (match(TokenType.COMMA)) initializer = null;
        else
        {
            consume(TokenType.IDENTIFIER, "Initializer expected");
            initializer = VarDeclaration(true);
        }
        consume(TokenType.COMMA, "Expect ',' after initializer (for)");
        Expression condition = null;
        if (!check(TokenType.COMMA)) condition = expression();
        consume(TokenType.COMMA, "Expect ',' after loop condition (for)");
        Expression increment = null;
        if (!check(TokenType.RIGHT_PAREN))
            increment = expression();
        consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses (for)");
        ConsumeEOL();
        Stmt body = Statement();
        if (increment != null)
        {
            body = new BlockStmt(
                new List<Stmt>() { body, new ExpressionStmt(increment) }
            );
        }
        body = new WhileStmt(keyword, condition, body);
        if (initializer != null)
            body = new BlockStmt(new List<Stmt>() { initializer, body });
        return body;

    }
    private Stmt GoToStatement()
    {
        Token keyword = previous();
        consume(TokenType.LEFT_SQUARE_BRACE, "Expect '[' after 'GoTo'");
        consume(TokenType.IDENTIFIER, "Expect Label after '[' in GoTo");
        Token label = null;
        if (previous().Type == TokenType.IDENTIFIER) label = previous();
        consume(TokenType.RIGHT_SQUARE_BRACE, "Expect ']' after Label in GoTo");
        consume(TokenType.LEFT_PAREN, "Expect '(' after ']' in GoTo");
        Expression expr = expression();
        consume(TokenType.RIGHT_PAREN, "Expect ')' after GoTo condition");
        if (peek().Type != TokenType.EOF)
            consume(TokenType.EOL, @"Expected line break ('\n') at end of statement");
        return new GoToStmt(label, expr, keyword);
    }
    private Stmt whileStatement()
    {
        Token keyword = previous();
        consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'");
        Expression condition = expression();
        consume(TokenType.RIGHT_PAREN, "Expected ')' after while condition");
        while (match(TokenType.EOL)) { }
        Stmt body = Statement();
        return new WhileStmt(keyword, condition, body);
    }
    private Stmt ifStatement()
    {
        Token keyword = previous();
        consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'");
        Expression condition = expression();
        consume(TokenType.RIGHT_PAREN, "Expected ')' after if condition");
        ConsumeEOL();
        Stmt thenBranch = Statement();
        Stmt elseBanch = null;
        ConsumeEOL();
        if (match(TokenType.ELSE))
        {
            ConsumeEOL();
            elseBanch = Statement();
        }
        return new IfStmt(condition, thenBranch, elseBanch, keyword);
    }
    private List<Stmt> block()
    {
        List<Stmt> statements = new List<Stmt>();
        while (!check(TokenType.RIGHT_CURLY_BRACE) && !IsAtEnd())
        {
            while (match(TokenType.EOL)) { }
            if (check(TokenType.RIGHT_CURLY_BRACE)) break;
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
    //This is for put a variable as a parameter or as a expression
    private Expression assignment()
    {
        Expression expr = and();
        if (match(TokenType.ASSIGNMENT))
        {
            Token equals = previous();
            Expression value = assignment();
            if (expr is Variable)
            {
                Token name = ((Variable)expr).Name;
                return new Assign(name, value);
            }
            throw error(equals, "Invalid assignment target");
        }
        return expr;
    }
    private Expression and()
    {
        Expression expression = or();
        while (match(TokenType.AND))
        {
            Token @operator = previous();
            Expression right = or();
            expression = new Logical(expression, @operator, right);
        }
        return expression;
    }
    private Expression or()
    {
        Expression expression = equality();
        while (match(TokenType.OR))
        {
            Token @operator = previous();
            Expression right = equality();
            expression = new Logical(expression, @operator, right);
        }
        return expression;
    }
    private Expression equality()
    {
        Expression expression = comparison();
        while (match(new List<TokenType> { TokenType.EQUAL_EQUAL, TokenType.NOT_EQUAL }))
        {
            Token @operator = previous();
            Expression right = comparison();
            expression = new Binary(expression, @operator, right, ExpressionType.BOOL);
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
            expression = new Binary(expression, @operator, right, ExpressionType.BOOL);
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
            expression = new Binary(expression, @operator, right, ExpressionType.INT);
        }
        return expression;
    }
    private Expression factor()
    {
        Expression expression = powRoot();
        while (match(new List<TokenType> { TokenType.BY, TokenType.SLASH, TokenType.MODULE }))
        {
            Token @operator = previous();
            Expression right = powRoot();
            expression = new Binary(expression, @operator, right, ExpressionType.INT);
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
            expression = new Binary(expression, @operator, right,ExpressionType.INT);
        }
        return expression;
    }
    private Expression unary()
    {
        if (match(new List<TokenType> { TokenType.NOT, TokenType.MINUS }))
        {
            Token @operator = previous();
            Expression right = unary();
            return new Unary(@operator, right, @operator.Type==TokenType.NOT?ExpressionType.BOOL:ExpressionType.INT);
        }
        return call();
    }
    private Expression call()
    {
        Expression expr = primary();
        if (match(TokenType.LEFT_PAREN))
        {
            expr = finishCall(expr);
        }
        return expr;
    }
    private Expression finishCall(Expression callee)
    {
        List<Expression> arguments = new List<Expression>();
        if (!check(TokenType.RIGHT_PAREN))
        {
            do
            {
                //I will use Java max arguments support because C# max support is insane
                if (arguments.Count >= 255) exceptions.Add( error(peek(), "Can't have more than 255 arguments."));
                arguments.Add(expression());
            } while (match(TokenType.COMMA));
        }
        consume(TokenType.RIGHT_PAREN, "Expected ')' after arguments");
        Token paren = previous();
        return new Call(callee, paren, arguments);
    }
    private Expression primary()
    {
        if (match(TokenType.FALSE)) return new Literal(false, ExpressionType.BOOL);
        if (match(TokenType.TRUE)) return new Literal(true, ExpressionType.BOOL);
        if (match(TokenType.INT))
        { 
            if((double)previous().Literal<int.MinValue||(double)previous().Literal>int.MaxValue)throw error(previous(),"This number is out of range");
            return new Literal(Convert.ToInt32((double)previous().Literal), ExpressionType.INT);
        }
        if (match(TokenType.STRING))
        {
            Token x = previous();
            if (exceptions.Count == 0)
            {
                switch (x.Literal.ToString())
                {
                    case "Red":
                        return new Literal("#FF0000",ExpressionType.STRING);

                    case "Blue":
                        return new Literal("#0000FF",ExpressionType.STRING);

                    case "Green":
                        return new Literal("#00FF00",ExpressionType.STRING);

                    case "Yellow":
                        return new Literal("#FFFF00",ExpressionType.STRING);

                    case "Orange":
                        return new Literal("#FFA500",ExpressionType.STRING);

                    case "Purple":
                        return new Literal("#800080",ExpressionType.STRING);

                    case "Black":
                        return new Literal("#000000",ExpressionType.STRING);

                    case "White":
                        return new Literal("#FFFFFF",ExpressionType.STRING);

                    case "Transparent":
                        return new Literal("Transparent",ExpressionType.STRING);

                    default:
                        if (IsValidColor(x.Literal.ToString()))
                            return new Literal(x.Literal.ToString(),ExpressionType.STRING);
                        else
                        {
                            throw error(x, "Invalid Color or hex code");
                        }

                }
            }
        }
        if (match(TokenType.IDENTIFIER))
        {
            Token name = previous();
            ExpressionType type = vars.ContainsKey(name.Lexeme)?vars[name.Lexeme]:ExpressionType.VOID;
            return new Variable(name,type);
        }
        if (match(TokenType.LEFT_PAREN))
        {
            Expression expression = this.expression();
            consume(TokenType.RIGHT_PAREN, "Expected ')' after expression");
            return new Grouping(expression, expression.type);

        }
        Token aux = peek();
        throw error(aux, "Expected expression");
    }
    private void consume(TokenType type, string message)
    {
        if (check(type)) Advance();
        else
        {
            Token aux = peek();
            throw error(aux, message);
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
    private void ConsumeEOL()
    {
        while (check(TokenType.EOL))
        {
            Advance();
        }
    }
    //Checks if we still have tokens to parse
    private bool IsAtEnd() => peek().Type == TokenType.EOF;
    private Token peek() => _tokens[_current];
    private Token previous() => _tokens[_current - 1];
    private void GoPrevious()=>_current--;
    private CompilerException error(Token token, string message)
    {
        return new CompilerException("Syntactical", message, token.Line, token.Column, token.Lexeme);
    }
}