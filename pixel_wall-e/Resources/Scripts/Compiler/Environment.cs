using System;
using System.Collections.Generic;
using Godot;

public class Environment
{
    public readonly Environment Enclosing;
    private readonly Dictionary<string,object> values = new Dictionary<string, object>();
    private readonly Dictionary<string, NativeFunction> nativeFunctions = new Dictionary<string, NativeFunction>();
    public Environment()
    {
        Enclosing = null;
        }
    public Environment(Environment enclosing)
    {
        Enclosing = enclosing;
    }
    public void define( string name, object value)
    {
        values[name]=value;
    }
    public void DefineNative(string name, NativeFunction nativeFunction)
    {
        nativeFunctions[name] = nativeFunction;
    }
    public void Assign(Token name, object value)
    {
        if(values.ContainsKey(name.Lexeme))
        {
            values[name.Lexeme] = value;
            return;
        }
        if(Enclosing!= null)
        {
            Enclosing.Assign(name,value);
            return;
        }
        throw new RunTimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }
    public object Get(Token name)
    {
        if(values.TryGetValue(name.Lexeme, out object value)) 
        return value;
        if(Enclosing!= null) return Enclosing.Get(name);
        throw new RunTimeError(name, $"Undefined variable '{name.Lexeme}'." );
    }
}
public delegate object NativeFunction(Interpreter interpreter, List<object> arguments);
public class LabelTable
{
    private readonly Dictionary<string, int> labels = new Dictionary<string, int>();
    public void Add(string name, int index)
    {
        labels[name]=index;
    }
    public int GetPosition(Token name)
    {
        if (labels.TryGetValue(name.Lexeme, out int index))
        {
            return index;
        }
        throw new RunTimeError(name, $"Undefined label '{name.Lexeme}'.");
    }
}