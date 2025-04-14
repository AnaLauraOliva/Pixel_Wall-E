using System;
using System.Collections.Generic;

public class Handler
{
    private readonly string _source;
    private List<CompilerException> _exceptions;
    private List<Token> _tokens;
    public Handler(string source)
    {
        _source = source;
        _exceptions= new List<CompilerException>();
        _tokens = new List<Token>();
        HandleLexer();
        if(_exceptions.Count==0)
        {

        }
    }
    public List<CompilerException> GetExceptions()=>_exceptions;
    private void HandleLexer()
    {
        Lexer lexer = new Lexer(_source);
        _tokens = lexer.ScanTokens();
        _exceptions = lexer.GetCompilerExceptions();
    }
}
