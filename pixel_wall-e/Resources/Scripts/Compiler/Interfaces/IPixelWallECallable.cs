using System.Collections.Generic;

public interface IPixelWallECallable
{
    public int Arity();
    public object Call(Interpreter interpreter, List<object> args);
}