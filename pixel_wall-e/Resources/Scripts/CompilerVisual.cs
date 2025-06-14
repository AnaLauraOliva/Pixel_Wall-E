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
        //_codeEdit.TextChanged += Compile;

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

        script = GetNode<CanvasPanelscript>("CanvasPanel");
    }
    public void FillStatements(List<Stmt>list)=> statements =list;

    public void PrintExceptions(List<CompilerException> exceptions)
    {
        _problems.Text="";
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
        else if (dialog.FileMode == FileDialog.FileModeEnum.SaveFile&&dialog.Title=="Guardar archivo")
        {
            Export(path);
        }
        else if(dialog.FileMode == FileDialog.FileModeEnum.SaveFile&&dialog.Title=="Guardar foto")
        {
            SavePhoto(path);
        }
    }
    private void SavePhoto(string filePath)
    {
        if(!filePath.EndsWith(".jpg")&&!filePath.EndsWith(".jpeg")&&!filePath.EndsWith(".png"))filePath+=".png";
        try
        {
            TextureRect textureRect = GetNode<TextureRect>("BigCanvas");
            ImageTexture imageTexture = (ImageTexture)textureRect.Texture;
            Image image = imageTexture.GetImage();
            if(filePath.EndsWith(".jpg")||!filePath.EndsWith(".jpeg")) image.SaveJpg(filePath);
            else image.SavePng(filePath);
            popup.GetNode<Label>("Text").Text = "Imagen exportada correctamente a " + filePath;
            popup.Visible = true;
        }
        catch (Exception e)
        {
            popup.GetNode<Label>("Text").Text = "Error al exportar la imagen: " + e.Message;
            popup.Visible = true;
        }
        dialog.ClearFilters();
        dialog.AddFilter("*.pw", "Pixel Wall-E File");
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
            }
            script.ExecuteCommandQueue(Canvas.getQueue());
        }
        Canvas.QuitInstructionsInTheQueue();
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
            CodeEditVisual ce = GetNode<HBoxContainer>("TextContainer").GetNode<CodeEditVisual>("CodeEdit");
            
            ce.Compile();
            //Compile();
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
    private void _on_save_canvas_pressed()
    {
        dialog.FileMode = FileDialog.FileModeEnum.SaveFile;
        dialog.ClearFilters();
        dialog.AddFilter("*.png");
        dialog.AddFilter("*.jpg");
        dialog.Title = "Guardar foto";
        dialog.Show();
    }
    private void OkPressed() => popup.Visible = false;
    public void _on_exit_button_down()
    {
        GetTree().Quit();
    }
}