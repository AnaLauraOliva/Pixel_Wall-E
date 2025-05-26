using System;
using System.Collections.Generic;

public class PixellWallEFunctions : IPixelWallECallable
{
    public int Arity()
    {
        throw new System.NotImplementedException();
    }

    public object Call(Interpreter interpreter, List<object> args)
    {
        throw new System.NotImplementedException();
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