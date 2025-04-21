using Godot;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class CompilerVisual : Node
{
    private CodeEdit _codeEdit;
    private TextEdit _problems;
    private Handler handler;
    private Popup popup;
    private Button export;
    private Button import;
    private FileDialog dialog;
    CanvasPanelscript script;
    List<Stmt> statements = new List<Stmt>();
    private bool canCompile = true;
    private string _currentPath = string.Empty;
    public override void _Ready()
    {
        _codeEdit = GetNode<HBoxContainer>("TextContainer").GetNode<CodeEdit>("CodeEdit");
        _codeEdit.TextChanged += Compile;

        _problems = GetNode<Container>("ProblemsContainer").GetNode<TextEdit>("ProblemsDisplayer");

        popup = GetNode<Popup>("ExportOrImport");
        popup.GetNode<Button>("OK").Pressed += OkPressed;

        dialog = GetNode<FileDialog>("SaveImport");
        dialog.Access = FileDialog.AccessEnum.Filesystem;
        dialog.FileSelected += ImportOrExport;
        dialog.AddFilter("*.pw", "Pixel Wall-E File");

        export = GetNode<HBoxContainer>("ButtonsContainer").GetNode<Button>("Save");
        import = GetNode<HBoxContainer>("ButtonsContainer").GetNode<Button>("Import");
        export.Pressed += ExportButtonPressed;
        import.Pressed += ImportButtonPressed;

        script= GetNode<CanvasPanelscript>("CanvasPanel");
    }
    private void Compile()
    {
        _problems.Text = "";
        if (_codeEdit.Text != "")
        {
            //handler=new Handler(_codeEdit.Text);
            Lexer lexer = new Lexer(_codeEdit.Text);
            List<Token> _tokens = lexer.ScanTokens();
            List<CompilerException> exceptions = lexer.GetCompilerExceptions();
            if (exceptions.Count != 0)
            {
                PrintExceptions(exceptions);
                canCompile = false;
            }
            else
            {
                canCompile = true;
                Parser parser = new Parser(_tokens);
                statements = parser.parse();
                exceptions = parser.GetCompilerExceptions();
                if (exceptions.Count != 0)
                {
                    PrintExceptions(exceptions);
                    canCompile = false;
                }
                else
                {
                    canCompile = true;
                }
            }
        }
    }

    private void PrintExceptions(List<CompilerException> exceptions)
    {
        foreach (CompilerException exception in exceptions)
        {
            _problems.Text += exception.ToString() + "\n";
        }
    }
    private void _on_back_button_down()
    {
        GetTree().ChangeSceneToPacked((PackedScene)ResourceLoader.Load("res://Resources/Scenes/MainMenu.tscn"));
    }
    //It's made this way to use the same fileDialog por both: import and export
    private void ImportOrExport(string path)
    {
        if (dialog.FileMode == FileDialog.FileModeEnum.OpenFile)
        {
            Import(path);
        }
        else if (dialog.FileMode == FileDialog.FileModeEnum.SaveFile)
        {
            Export(path);
        }
    }
    private void _on_execute_button_down()
    {
        if (canCompile && statements.Count != 0)
        {
            Interpreter interpreter = new Interpreter();
            interpreter.Interpret(statements);
            if (interpreter.exceptions.Count != 0)
            {
                List<RunTimeError> errors = interpreter.exceptions;
                _problems.Text = errors[0].ToString();
            }
            else
            {
                _problems.Text = "Compilado correctamente";
                script.ExecuteCommandQueue(Canvas.getQueue());
                Canvas.QuitInstructionsInTheQueue();
                
            }
        }
    }
    private string PrintAST(Expression expression)
    {
        if (expression is Binary binary)
        {
            return $"({PrintAST(binary.Left)} {binary.Operator.Lexeme} {PrintAST(binary.Right)})";
        }
        if (expression is Literal literal)
        {
            return literal.Value.ToString();
        }
        return "?";
    }
    private void Export(string filePath)
    {
        if (!filePath.EndsWith(".pw")) filePath += ".pw";
        try
        {
            File.WriteAllText(filePath, _codeEdit.Text);
            popup.GetNode<Label>("Text").Text = "Código exportado correctamente a " + filePath;
            popup.Visible = true;

        }
        catch (IOException e)
        {
            popup.GetNode<Label>("Text").Text = "Error al exportar el código: " + e.Message;
            popup.Visible = true;
        }
    }
    private void Import(string filePath)
    {
        //Checking that the file exists
        if (!File.Exists(filePath))
        {
            popup.GetNode<Label>("Text").Text = "El archivo no existe: " + filePath;
            return;
        }
        try
        {
            string codeContent = File.ReadAllText(filePath);
            popup.GetNode<Label>("Text").Text = "Archivo importado correctamente desde " + filePath;
            _codeEdit.Text = codeContent;
            Compile();
        }
        catch (System.Exception e)
        {
            popup.GetNode<Label>("Text").Text = "Error al importar código: " + e.Message;

        }
    }
    private void ExportButtonPressed()
    {
        dialog.FileMode = FileDialog.FileModeEnum.SaveFile;
        dialog.Title = "Guargar archivo";
        dialog.Show();
    }
    private void ImportButtonPressed()
    {
        dialog.FileMode = FileDialog.FileModeEnum.OpenFile;
        dialog.Title = "Importar archivo";
        dialog.Show();
    }
    private void OkPressed() => popup.Visible = false;
    public void _on_exit_button_down()
    {
        GetTree().Quit();
    }
}