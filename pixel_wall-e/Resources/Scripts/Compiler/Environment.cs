using System;
using System.Collections.Generic;
using Godot;

public class Environment
{
    public readonly Environment Enclosing;
    private readonly Dictionary<string,object> values = new Dictionary<string, object>();
    private readonly Dictionary<string,object> functions = new Dictionary<string, object>();
    public Environment()
    {
        Enclosing = null;
        }
    public Environment(Environment enclosing)
    {
        Enclosing = enclosing;
    }
    public void defineNative(string name, object value)=>functions[name]= value;
    public void defineFunc(Token name, object value)
    {
        if(functions.ContainsKey(name.Lexeme)) throw new RunTimeError(name,$"The function{name.Lexeme} already exists");
        functions.Add(name.Lexeme,value);

    }
    public void define( string name, object value)
    {
        values[name]=value;
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
        throw new RunTimeError(name, "Undefined variable");
        //define(name.Lexeme, value);
    }
    public object Get(Token name)
    {
        if(values.TryGetValue(name.Lexeme, out object value)) 
        return value;
        if(Enclosing!= null) return Enclosing.Get(name);
        throw new RunTimeError(name, $"Undefined variable '{name.Lexeme}'." );
    }
    public object GetFunc(Token name)
    {
        if(functions.TryGetValue(name.Lexeme, out object value)) return value;
        if(Enclosing!= null) return Enclosing.GetFunc(name);
        throw new RunTimeError(name, $"Undefined function '{name.Lexeme}'." );
    }
}
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