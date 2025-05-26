using System;

public class CompilerException:Exception
{
    public int Line { get;}
    public int Column { get;}
    public string InvalidToken { get;}
    public string KindOfException{get;}

    public CompilerException(string kindOfException,string message,int line, int column, string invalidToken=""):base(message)
    {
        KindOfException = kindOfException;
        Line = line;
        Column = column;
        InvalidToken = invalidToken;
    }
    public CompilerException(string kindOfException,string message,Token token):base(message)
    {
        KindOfException = kindOfException;
        Line = token.Line;
        Column = token.Column;
        InvalidToken = token.Lexeme;
    }
    public override string ToString()
    {
        return $"{KindOfException} exception: {Message} (Token: {InvalidToken}) at Line: {Line}, Column: {Column}";
    }
}