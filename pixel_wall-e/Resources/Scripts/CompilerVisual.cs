using Godot;
using System;
using System.IO;
using System.Collections.Generic;

public partial class CompilerVisual : Node
{
    private CodeEdit _codeEdit;
    private TextEdit _problems;
    private Scanner scanner;
    private Popup popup;
    private Button export;
    private Button import;
    private FileDialog dialog;
    private string _currentPath = string.Empty;
    public override void _Ready()
    {
        _codeEdit = GetNode<HBoxContainer>("TextContainer").GetNode<CodeEdit>("CodeEdit");
        _problems = GetNode<Container>("ProblemsContainer").GetNode<TextEdit>("ProblemsDisplayer");
        popup = GetNode<Popup>("ExportOrImport");
        _codeEdit.TextChanged += Compile;
        dialog = GetNode<FileDialog>("SaveImport");
        dialog.Access=FileDialog.AccessEnum.Filesystem;
        dialog.FileSelected+=ImportOrExport;
        dialog.AddFilter("*.pw", "Pixel Wall-E File");
        popup.GetNode<Button>("OK").Pressed += OkPressed;
        export = GetNode<HBoxContainer>("ButtonsContainer").GetNode<Button>("Save");
        import = GetNode<HBoxContainer>("ButtonsContainer").GetNode<Button>("Import");
        export.Pressed += ExportButtonPressed;
        import.Pressed += ImportButtonPressed;
    }
    private void Compile()
    {
        _problems.Text = "";
        if (_codeEdit.Text != "")
        {
            scanner = new Scanner(_codeEdit.Text);
            WallE.CleanErrors();
            List<Token> tokens = scanner.ScanTokens();
            if (WallE.HasErrors())
            {
                foreach (string errors in WallE.errors)
                {
                    _problems.Text += errors + "\n";
                }
            }
        }
    }
    private void _on_back_button_down()
    {
        GetTree().ChangeSceneToPacked((PackedScene)ResourceLoader.Load("res://Resources/Scenes/MainMenu.tscn"));
    }
    //Está hecho así para no crear un fileDialog para importar y otro para Exportar
    private void ImportOrExport(string path)
    {
        if(dialog.FileMode == FileDialog.FileModeEnum.OpenFile)
        {
            Import(path);
        }
        else if(dialog.FileMode==FileDialog.FileModeEnum.SaveFile)
        {
            Export(path);
        }
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
        //Comprobar si el archivo exise
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
