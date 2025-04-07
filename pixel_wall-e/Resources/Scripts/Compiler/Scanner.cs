using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
class Scanner
{
    private string _source;
    private List<Token> _tokens = new List<Token>();
    private List<Exception> Exceptions = new List<Exception>();
    private int _start = 0;
    private int _current = 0;
    private int _line = 1;
    private int _column = 1;
    private Dictionary<string, TokenType> _keywords = new Dictionary<string, TokenType>{
        {"GoTo", TokenType.GOTO},
        {"while", TokenType.WHILE},
        {"for", TokenType.FOR},
        {"if", TokenType.IF},
        {"else", TokenType.ELSE},
        {"function", TokenType.FUNCTION}
    };
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
        {"/", TokenType.SLASH},
        {"<", TokenType.LESS},
        {">", TokenType.GREATER},
        {"*", TokenType.BY},
        {"!", TokenType.NOT},
        {"<-", TokenType.ASSIGNMENT},
        {"<=", TokenType.LESS_EQUAL},
        {">=", TokenType.GREATER_EQUAL},
        {"**", TokenType.POW},
        {"+=", TokenType.PLUS_ASSIGN},
        {"-=", TokenType.MINUS_ASSIGN},
        {"*=", TokenType.BY_ASSIGN},
        {"/=", TokenType.DIVIDE_ASSIGN},
        {"%=", TokenType.MODULE_ASSIGN},
        {"==", TokenType.EQUAL_EQUAL},
        {"||", TokenType.OR},
        {"&&", TokenType.AND},
        {"//", TokenType.SIMPLE_COMMENT},
        {"/*", TokenType.LEFT_MULTILINE_COMMENT},
        {"!=", TokenType.NOT_EQUAL}

    };
    public Scanner(string source)
    {
        _source = source;
    }
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
                _line++;
                _column = 1;
                return;
        }
        if (!IsAtEnd())
        {
            string twoCharSymbol = _source.Substring(_start, 2);
            if (_symbolTokens.TryGetValue(twoCharSymbol, out TokenType type))
            {
                if (type == TokenType.SIMPLE_COMMENT || type == TokenType.LEFT_MULTILINE_COMMENT) Comment(type);
                else AddToken(type, null);
                return;
            }
        }
        if(_symbolTokens.TryGetValue(x.ToString(),out TokenType _type))
        {
            AddToken(_type, null);
            return;
        }
        if(x=='"')
        {
            String();
            return;
        }
        if(char.IsDigit(x))
        {
            Number();
            return;
        }
        if(char.IsLetter(x))
        {
            Identifier();
            return;
        }
        WallE.AddError("Unexpected character", _line, _column++);
    }
    private void Comment(TokenType type)
    {
        if (type == TokenType.SIMPLE_COMMENT)
        {
            while (Peek() != '\n' && !IsAtEnd()) _current++;
        }
        else
        {
            while ((Peek() != '*' || !Match('/')) && !IsAtEnd()) 
            {
                _current++;
                if(Peek()=='\n')
                {
                    _column = 1;
                    _line++;
                }
                else _column++;
            }
            if(IsAtEnd()) WallE.AddError("Unclosed comment", _line, _column);
        }
    }
    private bool Match(char expected)
    {
        if (!IsAtEnd()) return false;
        if (Peek() != expected) return false;
        _current++;
        return true;
    }
    private bool IsAtEnd() => _current >= _source.Length;
    private void AddToken(TokenType type, object literal)
    {
        string text = _source.Substring(_start, _current - _start);
        _tokens.Add(new Token(type, text, literal, _line, _column));
        _column += _current - _start;
    }
    private bool GetToken(char expected)
    {
        if (IsAtEnd() || _source[_current] != expected)
        {
            _current++;
            return false;
        }

        _current++;
        return true;
    }
    private char Peek() => IsAtEnd() ? '\0' : _source[_current];
    private void Number()
    {
        while (char.IsDigit(Peek())) _current++;
        AddToken(TokenType.INT, int.Parse(_source.Substring(_start, _current - _start)));
    }
    private void Identifier()
    {
        while (char.IsLetterOrDigit(Peek()) || Peek() == '-') _current++;
        string text = _source.Substring(_start, _current - _start);
        TokenType type = _keywords.TryGetValue(text, out TokenType value) ? value : TokenType.IDENTIFIER;
        AddToken(type, null);
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
            WallE.AddError("\" expected", _line, _column);
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