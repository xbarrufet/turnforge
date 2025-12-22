using Godot;
using System;

public partial class SurvivorSelectionRow : Button
{

	public string CharacterName { get; private set; }
	public bool IsSelected { get; private set; }

	public void Setup(string name, Texture2D avatar)
	{
		CharacterName = name;
		GetNode<Label>("HBoxContainer/SurvivorNameLabel").Text = name;
		GetNode<TextureRect>("HBoxContainer/SurvivorAvatar").Texture = avatar;
	}

	// Al clicar el botó (senyal "pressed"), canviem l'estat visual
	public void ToggleSelection()
	{
		IsSelected = !IsSelected;
		// Canviar color de fons o mostrar el check
		Modulate = IsSelected ? Colors.Green : Colors.White; 
	}
}
