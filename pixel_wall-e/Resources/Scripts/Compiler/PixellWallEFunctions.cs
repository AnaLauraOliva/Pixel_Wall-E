using System;
using System.Collections.Generic;

public class PixellWallEFunctions : IPixelWallECallable
{
    public PixellWallEFunctions(FunctionStmt declaration)
    {
        Declaration = declaration;
    }

    private FunctionStmt Declaration { get; }
    public int Arity()
    {
        return Declaration.Parameters.Count;
    }

    public object Call(Interpreter interpreter, List<object> args)
    {
        Environment environment = new Environment(interpreter.globals);
        for (int i = 0; i < Declaration.Parameters.Count; i++)
        {
            environment.define(Declaration.Parameters[i].Lexeme, args[i]);
        }
        try
        {
            interpreter.executeBlock(Declaration.Body, environment);
        }
        catch (Return returnValue)
        {
            return returnValue.value;
        }
        return null;
    }
}
public class NativeFunctions : IPixelWallECallable
{
    private int arity;
    private readonly Func<List<object>,object> _function;
    public NativeFunctions(int arity, Func<List<object>,object> func)
    {
        this.arity = arity;
        _function = func;
    }
    public int Arity()
    {
        return arity;
    }

    public object Call(Interpreter interpreter, List<object> args)
    {
        return _function(args);
    }
}