using System;
using System.Collections.Generic;
public class SemanticEnvironment
{
    Dictionary<string, ExpressionType> vars = new Dictionary<string, ExpressionType>();
    Dictionary<string, FunctionsInfo> functions = new Dictionary<string, FunctionsInfo>();
    SemanticEnvironment Clousure;
    List<string> Labels= new List<string>();
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
    public void defineFunc(string name, ExpressionType type, int args) => functions[name]= new FunctionsInfo(args,type);
    public bool? Assign(string name, ExpressionType type)
    {
        if (vars.ContainsKey(name)) return vars[name] == type;
        if (Clousure != null) return Clousure.Assign(name, type);
        return null;
    }
    public int ReturnArgumentCount(string name) => functions[name].arguments;
    public bool HasVar(string name) => vars.ContainsKey(name);
    public bool HasFunc(string name) => functions.ContainsKey(name);
    public bool HasLabel (string name)=> Labels.Contains(name);
    public ExpressionType GetVarType(string name)=> vars[name];
    public ExpressionType getReturnType(string name)=> functions[name].ReturnType;

}
public class FunctionsInfo
{
    public int arguments;
    public ExpressionType ReturnType = ExpressionType.VOID;
    public FunctionsInfo(int args, ExpressionType returnType=ExpressionType.VOID)
    {
        arguments = args;
        ReturnType = returnType;
    }
}