using Godot;
using System;

public partial class MainMenuVisual : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void _on_exit_btn_button_down()
	{
		GetTree().Quit();
	}
	public void _on_about_btn_button_down()
	{
		Panel about = GetNode<Panel>("AboutPanel");
		VBoxContainer menu = GetNode<VBoxContainer>("vbcMenu");
		menu.Visible = false;
		about.Visible = true;
	}
	private void _on_start_btn_button_down()
	{
		GetTree().ChangeSceneToPacked((PackedScene)ResourceLoader.Load("res://Resources/Scenes/CompilerScene.tscn"));
	}
	public void _on_back_btn_button_down()
	{
		Panel about = GetNode<Panel>("AboutPanel");
		VBoxContainer menu = GetNode<VBoxContainer>("vbcMenu");
		menu.Visible = true;
		about.Visible = false;
	}
	public void _on_documentation_btn_pressed()
	{
		GetTree().ChangeSceneToPacked((PackedScene)ResourceLoader.Load("res://Resources/Scenes/Manual.tscn"));
	}
}
