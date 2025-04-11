using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;

public partial class Main : Node
{
	// Map
	private FileDialog mapFileDialog { get; set; }
	private Button mapButton { get; set; }
	private Sprite2D mapSprite { get; set; }
	private Image mapImage { get; set; }
	private Texture2D mapTexture { get; set; }

	// Overlay
	private FileDialog overlayFileDialog { get; set; }
	private Button overlayButton { get; set; }
	private Sprite2D overlaySprite { get; set; }
	private List<ImageData> overlayImages { get; set; }
	private ItemList overlayItemList { get; set; }
	private int overlaySelectedItemIndex { get; set; }

	// Source
	private FileDialog saveSourceFileDialog { get; set; }
	private Button generateButton { get; set; }
	private Button saveButton { get; set; }
	private Sprite2D sourceSprite { get; set; }
	private List<ImageData> sourceImages { get; set; }
	private ImageTexture sourceTexture { get; set; }
	private ItemList sourceItemList { get; set; }
	private int sourceSelectedItemIndex { get; set; }

	// Preview (Lookup)
	private FileDialog lookupFileDialog { get; set; }
	private Button lookupButton { get; set; }
	private Sprite2D previewSprite { get; set; }
	private Image lookupImage { get; set; }
	private ImageTexture lookupTexture { get; set; }
	private ShaderMaterial shader { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get nodes
		mapFileDialog = GetNode<FileDialog>("Control/MapFileDialog");
		mapButton = GetNode<Button>("Control/MapButton");
		mapSprite = GetNode<Sprite2D>("MapSprite");

		overlayFileDialog = GetNode<FileDialog>("Control/OverlayFileDialog");
		overlayButton = GetNode<Button>("Control/OverlayButton");
		overlaySprite = GetNode<Sprite2D>("OverlaySprite");
		overlayItemList = GetNode<ItemList>("Control/OverlayItemList");

		saveSourceFileDialog = GetNode<FileDialog>("Control/SaveSourceFileDialog");
		generateButton = GetNode<Button>("Control/GenerateButton");
		saveButton = GetNode<Button>("Control/SaveButton");
		sourceSprite = GetNode<Sprite2D>("SourceSprite");
		sourceItemList = GetNode<ItemList>("Control/SourceItemList");

		lookupFileDialog = GetNode<FileDialog>("Control/LookupFileDialog");
		lookupButton = GetNode<Button>("Control/LookupButton");
		previewSprite = GetNode<Sprite2D>("PreviewSprite");

		shader = new ShaderMaterial();
		shader.Shader = GD.Load<Shader>("res://shaders/main.gdshader");

		// Customize file dialogs
		mapFileDialog.Access = FileDialog.AccessEnum.Filesystem;
		overlayFileDialog.Access = FileDialog.AccessEnum.Filesystem;
		saveSourceFileDialog.Access = FileDialog.AccessEnum.Filesystem;
		lookupFileDialog.Access = FileDialog.AccessEnum.Filesystem;

		mapFileDialog.Title = "Upload map";
		overlayFileDialog.Title = "Upload overlay(s)";
		saveSourceFileDialog.Title = "Save source(s)";
		lookupFileDialog.Title = "Upload lookup";

		// Connect signals
		mapFileDialog.FileSelected += OnMapFileSelected;
		overlayFileDialog.FilesSelected += OnOverlayFilesSelected;
		saveSourceFileDialog.FileSelected += OnSourceFilesSaved;
		lookupFileDialog.FileSelected += OnLookupFileSelected;

		mapButton.Pressed += OnMapButtonPressed;
		overlayButton.Pressed += OnOverlayButtonPressed;
		generateButton.Pressed += GenerateSourceImages;
		saveButton.Pressed += OnSaveButtonPressed;
		lookupButton.Pressed += OnLookupButtonPressed;

		overlayItemList.ItemSelected += OnOverlayItemSelected;
		sourceItemList.ItemSelected += OnSourceItemSelected;


		// Initialize variables
		overlaySelectedItemIndex = -1;
		sourceSelectedItemIndex = -1;

		overlayImages = new List<ImageData>();
		sourceImages = new List<ImageData>();


		// Disable Generate and Save buttons
		generateButton.Disabled = true;
		saveButton.Disabled = true;
	}

	private void OnMapButtonPressed()
	{
		mapFileDialog.Show();
	}

	private void OnOverlayButtonPressed()
	{
		overlayFileDialog.Show();
	}

	private void OnSaveButtonPressed()
	{
		saveSourceFileDialog.Show();
	}

	private void OnLookupButtonPressed()
	{
		lookupFileDialog.Show();
	}


	private void OnMapFileSelected(string path)
	{
		mapImage = new Image();
		Error error = mapImage.Load(path);
		if (error != 0)
		{
			GD.PrintErr($"Error loading image at {path}");
			return;
		}

		mapTexture = ImageTexture.CreateFromImage(mapImage);

		float scale = Mathf.Min(128f / mapTexture.GetHeight(), 128f / mapTexture.GetWidth());

		mapSprite.Texture = mapTexture;
		mapSprite.Scale = new Vector2(scale, scale);

		if (overlayImages.Count > 0)
		{
			saveButton.Disabled = true;
			generateButton.Disabled = false;
		}
	}

	private void OnOverlayFilesSelected(string[] paths)
	{
		overlayImages.Clear();
		overlayItemList.Clear();

		foreach (string path in paths)
		{
			Image overlayImage = new Image();
			Error error = overlayImage.Load(path);
			if (error != 0)
			{
				GD.PrintErr($"Error loading image at {path}");
				return;
			}

			string[] substrings = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
			string fileName = substrings[substrings.Length - 1];
			overlayImages.Add(new ImageData(overlayImage, fileName));
			int itemIndex = overlayItemList.AddItem(fileName);
			overlayItemList.SetItemTooltipEnabled(itemIndex, false);
		}

		overlayItemList.Select(0);
		OnOverlayItemSelected(0);

		if (mapImage != null)
		{
			saveButton.Disabled = true;
			generateButton.Disabled = false;
		}
	}

	private void OnSourceFilesSaved(string path)
	{
		int lastDotIndex = path.LastIndexOf('.');
		if (lastDotIndex != -1)
		{
			path = path.Substring(0, lastDotIndex);
		}

		switch(Dropdown.Instance.FileFormat)
		{
			case EFileFormat.PNG:
				{
					for (int i = 0; i < sourceImages.Count; i++)
					{
						sourceImages[i].Image.SavePng($"{path}{i + 1}.png");
					}
					break;
				}
			case EFileFormat.JPG:
				{
					for (int i = 0; i < sourceImages.Count; i++)
					{
						sourceImages[i].Image.SaveJpg($"{path}{i + 1}.jpg");
					}
					break;
				}
			case EFileFormat.Webp:
				{
					for (int i = 0; i < sourceImages.Count; i++)
					{
						sourceImages[i].Image.SaveWebp($"{path}{i + 1}.webp");
					}
					break;
				}
			default:
				{
					GD.PrintErr("Unhandled file format");
					return;
				}
		}
	}

	private void OnLookupFileSelected(string path)
	{
		lookupImage = new Image();
		Error error = lookupImage.Load(path);
		if (error != 0)
		{
			GD.PrintErr($"Error loading image at {path}");
			return;
		}

		lookupTexture = ImageTexture.CreateFromImage(lookupImage);

		if (sourceImages.Count > 0)
		{
			previewSprite.Texture = sourceImages[sourceSelectedItemIndex].Texture;

			float scale = Mathf.Min(128f / previewSprite.Texture.GetHeight(), 128f / previewSprite.Texture.GetWidth());
			previewSprite.Scale = new Vector2(scale, scale);

			shader.SetShaderParameter("look_up", lookupTexture);
			previewSprite.Material = shader;
		}
	}


	private void OnOverlayItemSelected(long index)
	{
		int item = (int)index;
		if (item == overlaySelectedItemIndex)
			return;

		overlaySelectedItemIndex = item;
		overlaySprite.Texture = overlayImages[item].Texture;
		overlaySprite.Scale = overlayImages[item].Scale;
	}

	private void OnSourceItemSelected(long index)
	{
		int item = (int)index;
		if (item == sourceSelectedItemIndex)
			return;

		sourceSelectedItemIndex = item;
		sourceSprite.Texture = sourceImages[item].Texture;

		sourceSprite.Scale = sourceImages[item].Scale;

		if (lookupTexture != null)
		{
			previewSprite.Texture = sourceImages[item].Texture;
			previewSprite.Scale = sourceImages[item].Scale;

			shader.Set("shader_parameter/look_up", lookupTexture);
			previewSprite.Material = shader;
		}
	}


	private void GenerateSourceImages()
	{
		sourceImages.Clear();
		sourceItemList.Clear();

		foreach (ImageData overlayImage in overlayImages)
		{
			Image sourceImage = new Image();
			sourceImage.CopyFrom(overlayImage.Image);

			for (int row = 0; row < sourceImage.GetHeight(); row++)
			{
				for (int column = 0; column < sourceImage.GetWidth(); column++)
				{
					Color pixel = sourceImage.GetPixel(column, row);
					Vector2I? mapPixelUV = GetMatchingPixelUVFromMap(pixel);
					if (mapPixelUV != null)
					{
						float x = (float)mapPixelUV.Value.X / (float)(sourceImage.GetWidth() - 1);
						float y = (float)mapPixelUV.Value.Y / (float)(sourceImage.GetHeight() - 1);

						sourceImage.SetPixel(column, row, new Color(x, y, 0, pixel.A));
					}
				}
			}

			sourceImages.Add(new ImageData(sourceImage, overlayImage.FileName));
			int itemIndex = sourceItemList.AddItem(overlayImage.FileName);
			sourceItemList.SetItemTooltipEnabled(itemIndex, false);
		}

		sourceItemList.Select(0);
		OnSourceItemSelected(0);

		generateButton.Disabled = true;
		saveButton.Disabled = false;
	}

	private Vector2I? GetMatchingPixelUVFromMap(Color pixelColor)
	{
		for (int row = 0; row < mapImage.GetHeight(); row++)
		{
			for (int column = 0; column < mapImage.GetWidth(); column++)
			{
				if (pixelColor == mapImage.GetPixel(column, row))
				{
					return new Vector2I(column, row);
				}
			}
		}

		return null;
	}

	public enum EFileFormat
	{
		PNG,
		JPG,
		Webp
	}
}
