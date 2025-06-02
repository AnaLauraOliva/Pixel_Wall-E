using System;
using System.Collections.Generic;

public class Handler
{
    private readonly string _source;
    private List<CompilerException> _exceptions;
    private List<Token> _tokens;
    private List<Stmt> _statements;
    Interpreter interpreter;
    public Handler(string source)
    {
        _source = source;
        _exceptions= new List<CompilerException>();
        _tokens = new List<Token>();
        _statements = new List<Stmt>();
        interpreter = new Interpreter();
        
    }
    public void Handle()
    {
        HandleLexer();
        if(_exceptions.Count==0)
        {
            HandleParser();
            if(_exceptions.Count==0)
            HandleSemantic();
        }
    }
    public List<CompilerException> GetExceptions()=>_exceptions;
    private void HandleLexer()
    {
        Lexer lexer = new Lexer(_source);
        _tokens = lexer.ScanTokens();
        _exceptions = lexer.GetCompilerExceptions();
    }
    private void HandleParser()
    {
        Parser parser = new Parser(_tokens);
        _statements= parser.parse();
        _exceptions = parser.GetCompilerExceptions();
    }
    private void HandleSemantic()
    {
        SemanticAnalyzer semantic = new SemanticAnalyzer(_statements);
        semantic.Analyze();
        _exceptions = semantic.GetExceptions();
    }
    public List<Stmt> GetStmts()=>_statements;
}
