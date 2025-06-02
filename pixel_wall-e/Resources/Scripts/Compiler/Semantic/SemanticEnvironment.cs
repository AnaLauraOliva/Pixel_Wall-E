using System;
using System.Collections.Generic;
public class SemanticEnvironment
{
    Dictionary<string, ExpressionType> vars = new Dictionary<string, ExpressionType>();
    Dictionary<string, FunctionsInfo> functions = new Dictionary<string, FunctionsInfo>();
    SemanticEnvironment Clousure;
    List<string> Labels = new List<string>();
    public SemanticEnvironment()
    {
        Clousure = null;
    }
    public SemanticEnvironment(SemanticEnvironment clousure)
    {
        Clousure = clousure;
    }
    public void defineLabel(string label) => Labels.Add(label);
    public void defineVar(string name, ExpressionType type) => vars[name] = type;
    public void defineFunc(string name, ExpressionType type, List<ExpressionType> args) => functions[name] = new FunctionsInfo(args, type);
    public void defineReturnType(string name, ExpressionType type)
    {
        if (functions.ContainsKey(name))
        {
            functions[name].ReturnType=type;
            return;
        }
        if (Clousure != null) Clousure.defineReturnType(name, type);
    }
    public bool? Assign(string name, ExpressionType type)
    {
        if (vars.ContainsKey(name)) return vars[name] == type;
        if (Clousure != null) return Clousure.Assign(name, type);
        return null;
    }
    public int ReturnArgumentCount(string name)
    {
        if (Clousure != null) return Clousure.ReturnArgumentCount(name);
        return functions.ContainsKey(name) ? functions[name].arguments.Count : -1;
    }
    public bool HasVar(string name)
    {
        if (vars.ContainsKey(name)) return true;
        if (Clousure != null) return Clousure.HasVar(name);
        return false;
    }
    public bool HasFunc(string name)
    {
        if (functions.ContainsKey(name)) return true;
        if (Clousure != null) return Clousure.HasFunc(name);
        return false;
    }
    public bool HasLabel(string name)
    {
        if (Labels.Contains(name)) return true;
        if (Clousure != null) return Clousure.HasLabel(name);
        return false;
    }
    public ExpressionType GetVarType(string name)
    {
        if (vars.ContainsKey(name)) return vars[name];
        if (Clousure != null) return Clousure.GetVarType(name);
        return ExpressionType.VOID;
    }
    public ExpressionType getReturnType(string name)
    {
        if (functions.ContainsKey(name)) return functions[name].ReturnType;
        if (Clousure != null) return Clousure.getReturnType(name);
        return ExpressionType.VOID;
    }
    public List<string> GetLabels() => Labels;
    public List<ExpressionType> getArguments(string name)
    {
        if (functions.ContainsKey(name)) return functions[name].arguments;
        if (Clousure != null) return Clousure.getArguments(name);
        return new List<ExpressionType>();
    }
}
public class FunctionsInfo
{
    public List<ExpressionType> arguments;
    public ExpressionType ReturnType = ExpressionType.VOID;
    public FunctionsInfo(List<ExpressionType> args, ExpressionType returnType = ExpressionType.VOID)
    {
        arguments = args;
        ReturnType = returnType;
    }
}