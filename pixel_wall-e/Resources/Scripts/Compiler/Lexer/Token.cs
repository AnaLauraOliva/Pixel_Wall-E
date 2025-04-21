public enum TokenType
{
    //Operator and symbols
    PLUS, MINUS, SLASH, MODULE, ROOT, COMMA, LEFT_PAREN, RIGHT_PAREN, LEFT_SQUARE_BRACE, RIGHT_SQUARE_BRACE, LEFT_CURLY_BRACE, RIGHT_CURLY_BRACE, LESS, GREATER, BY, NOT,
    //+   -      /      %       √     ,      (           )            [                  ]                   {                 }                  <     >        *   !

    //Two Characters tokens
    ASSIGNMENT, LESS_EQUAL, GREATER_EQUAL, POW, EQUAL_EQUAL, OR, AND, SIMPLE_COMMENT, LEFT_MULTILINE_COMMENT, NOT_EQUAL,
    //<-        <=          >=             **    ==           ||  &&   //              /*                      !=      

    //Literals
    IDENTIFIER, STRING, INT,

    //keywords
    GOTO, WHILE, FOR, IF, ELSE, ELIF, FUNCTION, RETURN, TRUE, FALSE,
    //wall-e functions
    SPAWN, COLOR, SIZE, DRAWLINE, DRAWCIRCLE, DRAWRECTANGLE, FILL, GETACTUALX, GETACTUALY, GETCANVASSIZE, GETCOLORCOUNT, ISBRUSHCOLOR, ISBRUSHSIZE, ISCANVASCOLOR,
    EOL,//end of line
    UNKNOWN,//The token isn't recognized
    EOF//end of file
}
public class Token
{
    public TokenType Type { get; private set; }
    public string Lexeme { get; private set; }
    //El literal hay que borrarlo
    public object Literal { get; private set; }
    public int Line { get; private set; }
    public int Column { get; private set; }

    public Token(TokenType type, string lexeme, object literal, int line, int column)
    {
        Type = type;
        Lexeme = lexeme;
        Literal = literal;
        Line = line;
        Column = column;

    }

    public override string ToString()
    {
        return $"{Type} {Lexeme} {Literal} at line: {Line}, column: {Column}";
    }
}