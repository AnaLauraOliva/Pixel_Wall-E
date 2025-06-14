using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Text.RegularExpressions;
class Lexer
{
    public string _source;
    private List<Token> _tokens = new List<Token>();
    private List<CompilerException> Exceptions = new List<CompilerException>();
    private int _start = 0;
    private int _current = 0;
    private int _line = 1;
    private int _column = 1;
    //Lenguage keywords
    private Dictionary<string, TokenType> _keywords = new Dictionary<string, TokenType>{
        {"GoTo", TokenType.GOTO},
        {"while", TokenType.WHILE},
        {"for", TokenType.FOR},
        {"if", TokenType.IF},
        {"else", TokenType.ELSE},
        {"func", TokenType.FUNCTION},
        {"Spawn", TokenType.SPAWN},
        {"Color", TokenType.COLOR},
        {"Size", TokenType.SIZE},
        {"DrawLine", TokenType.DRAWLINE},
        {"DrawCircle", TokenType.DRAWCIRCLE},
        {"DrawRectangle", TokenType.DRAWRECTANGLE},
        {"Fill", TokenType.FILL},
        {"NUMBER", TokenType.NUMBER_TYPE},
        {"BOOL", TokenType.BOOLEAN_TYPE},
        {"VOID", TokenType.VOID_TYPE},
        //{"GetActualX", TokenType.GETACTUALX},
        //{"GetActualY", TokenType.GETACTUALY},
        //{"GetCanvasSize", TokenType.GETCANVASSIZE},
        //{"GetColorCount", TokenType.GETCOLORCOUNT},
        //{"IsBrushColor", TokenType.ISBRUSHCOLOR},
        //{"IsBrushSize", TokenType.ISBRUSHSIZE},
        //{"IsCanvasColor", TokenType.ISCANVASCOLOR},
        {"true", TokenType.TRUE},
        {"false", TokenType.FALSE},
        {"return", TokenType.RETURN}
    };
    //Symbols
    private Dictionary<string, TokenType> _symbolTokens = new Dictionary<string, TokenType>{
        {"(", TokenType.LEFT_PAREN},
        {")", TokenType.RIGHT_PAREN},
        {"[", TokenType.LEFT_SQUARE_BRACE},
        {"]", TokenType.RIGHT_SQUARE_BRACE},
        {"{", TokenType.LEFT_CURLY_BRACE},
        {"}", TokenType.RIGHT_CURLY_BRACE},
        {",", TokenType.COMMA},
        {"+", TokenType.PLUS},
        {"-", TokenType.MINUS},
        {"%", TokenType.MODULE},
        {"√", TokenType.ROOT},
        {"/", TokenType.SLASH},
        {"<", TokenType.LESS},
        {">", TokenType.GREATER},
        {"*", TokenType.BY},
        {"!", TokenType.NOT},
        {"<-", TokenType.ASSIGNMENT},
        {"<=", TokenType.LESS_EQUAL},
        {">=", TokenType.GREATER_EQUAL},
        {"**", TokenType.POW},
        {"==", TokenType.EQUAL_EQUAL},
        {"||", TokenType.OR},
        {"&&", TokenType.AND},
        {"//", TokenType.SIMPLE_COMMENT},
        {"/*", TokenType.LEFT_MULTILINE_COMMENT},
        {"!=", TokenType.NOT_EQUAL},
        {":", TokenType.TWO_POINTS}

    };
    //get text source
    public Lexer(string source)
    {
        _source = source;
    }
    //Tokenizing the text
    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            _start = _current;
            ScanToken();
        }
        _tokens.Add(new Token(TokenType.EOF, "", null, _line, _column));
        return _tokens;
    }
    public List<CompilerException> GetCompilerExceptions() => Exceptions;
    //Adding tokens to the token list
    private void ScanToken()
    {
        char x = _source[_current];
        _current++;
        switch (x)
        {
            case ' ':
                _column++;
                return;
            case '\r':
                return;
            case '\t':
                _column += 4;
                return;
            case '\n':
                AddToken(TokenType.EOL);
                _line++;
                _column = 1;
                return;
        }
        if (!IsAtEnd())
        {
            string twoCharSymbol = _source.Substring(_start, 2);
            if (_symbolTokens.TryGetValue(twoCharSymbol, out TokenType type))
            {
                _current++;
                if (type == TokenType.SIMPLE_COMMENT || type == TokenType.LEFT_MULTILINE_COMMENT) Comment(type);
                else AddToken(type);
                return;
            }
        }
        if (_symbolTokens.TryGetValue(x.ToString(), out TokenType _type))
        {
            AddToken(_type);
            return;
        }
        if (x == '"')
        {
            String();
            return;
        }
        if (Regex.IsMatch(x.ToString(), @"[0-9]"))
        {
            Number();
            return;
        }
        if (Regex.IsMatch(x.ToString(),@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ]"))
        {
            Identifier();
            return;
        }
        if(x=='_')
        {
            while(!IsAtEnd()&&Regex.IsMatch(Peek().ToString(), @"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ_]")) _current++;
            Exceptions.Add(new CompilerException("Lexical", "Invalid Identifier. Identifiers cannot start with underscore", _line,_column, _source.Substring(_start, _current-_start)));
            return;
        }
        Exceptions.Add(new CompilerException("Lexical", $"Unexpected character", _line, _column++, x.ToString()));
    }
    //Verifying that the identifier didn't start with numbers
    private bool NoStartsWithNumber()
    {
        if (IsAtEnd()) return true;
        if (!char.IsLetter(_source[_current]))
            return true;
        while (!IsAtEnd() && char.IsLetter(_source[_current])) _current++;
        Exceptions.Add(new CompilerException("Lexical", "Invalid Identifier. Identifiers cannot start with numbers", _line, _column, _source.Substring(_start, _current - _start)));
        return false;
    }
    //Adding only line and multi line to the lenguage
    private void Comment(TokenType type)
    {
        if (type == TokenType.SIMPLE_COMMENT)
        {
            while (Peek() != '\n' && !IsAtEnd()) _current++;
        }
        else
        {
            while (!(Peek() == '*' && Match('/')) && !IsAtEnd())
            {
                _current++;
                if (Peek() == '\n')
                {
                    _column = 1;
                    _line++;
                }
                else _column++;
            }
            if (IsAtEnd()) Exceptions.Add(new CompilerException("Lexical", "Unfinished comment", _line, ++_column, "*/ expected"));
            _current++;
        }
    }

    private bool Match(char expected)
    {
        if (_current + 1 >= _source.Length) return false;
        if (_source[_current + 1] != expected) return false;
        _current++;
        return true;
    }
    private bool IsAtEnd() => _current >= _source.Length;
    private void AddToken(TokenType type, object literal = null)
    {
        string text = _source.Substring(_start, _current - _start);
        _tokens.Add(new Token(type, text, literal, _line, _column));
        _column += _current - _start;
    }
    private char Peek() => IsAtEnd() ? '\0' : _source[_current];
    private void Number()
    {
        while (Regex.IsMatch(Peek().ToString(),"[0-9]")) _current++;
        if (NoStartsWithNumber())
            AddToken(TokenType.INT, double.Parse(_source.Substring(_start, _current - _start)));

    }
    private void Identifier()
    {
        while (Regex.IsMatch(Peek().ToString(), @"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ_]")) _current++;
        //About regex: ^ begging of the string(in this case is not necesary because I am working with a char but I will use it anyway), in this case the char, [] valid characters
        string text = _source.Substring(_start, _current - _start);

        TokenType type = _keywords.TryGetValue(text, out TokenType value) ? value : TokenType.IDENTIFIER;
        AddToken(type);
    }
    private void String()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
            {
                _line++;
                _column = 1;
            }
            _current++;
        }
        if (IsAtEnd())
        {
            Exceptions.Add(new CompilerException("Lexical", "Unclosed string", _line, _column));
            _column++;
            return;
        }
        else
        {
            string value = _current - _start > 0 ? _source.Substring(_start + 1, _current - _start - 1) : " ";
            _current++;
            AddToken(TokenType.STRING, value.ToString());

        }
    }
}