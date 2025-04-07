public enum TokenType
{
    //Operadores y símbolos de un caracter
    PLUS, MINUS, SLASH, MODULE, COMMA, LEFT_PAREN, RIGHT_PAREN, LEFT_SQUARE_BRACE, RIGHT_SQUARE_BRACE, LEFT_CURLY_BRACE, RIGHT_CURLY_BRACE, LESS, GREATER, BY, NOT,
    //+   -      /      %       ,      (           )            [                  ]                   {                 }                  <     >        *   !

    //Tokens de dos caracteres
    ASSIGNMENT, LESS_EQUAL, GREATER_EQUAL, POW, MINUS_ASSIGN, PLUS_ASSIGN, BY_ASSIGN, DIVIDE_ASSIGN, MODULE_ASSIGN, EQUAL_EQUAL, OR, AND, SIMPLE_COMMENT, LEFT_MULTILINE_COMMENT, NOT_EQUAL,
    //<-        <=          >=             **   -=            +=           *=         /=             %=             ==           ||  &&   //              /*                      !=

    //Literales
    IDENTIFIER, STRING, INT,

    //keywords
    GOTO, WHILE, FOR, IF, ELSE, FUNCTION,

    EOF//fin del archivo
}
public class Token
{
    public TokenType Type { get; private set; }
    public string Lexeme { get; private set; }
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