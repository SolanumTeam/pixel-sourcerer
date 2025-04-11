using Godot;
using System;
using static Main;

public partial class Dropdown : Control
{
	public static Dropdown Instance;
	public EFileFormat FileFormat;

	private Button button { get; set; }
	private ItemList itemList { get; set; }


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;

		button = GetNode<Button>("Button");
		itemList = GetNode<ItemList>("ItemList");

		button.Pressed += OnDropdownPressed;
		itemList.ItemSelected += OnFileFormatSelected;

		itemList.AddItem("PNG");
		itemList.AddItem("JPG");
		itemList.Select(0);

		OnFileFormatSelected(0);
	}

	private void OnDropdownPressed()
	{
		itemList.Visible = !itemList.Visible;
	}

	private void OnFileFormatSelected(long index)
	{
		FileFormat = (EFileFormat)index;
		button.Text = Enum.GetName(typeof(EFileFormat), FileFormat);
		itemList.Hide();
	}
}
