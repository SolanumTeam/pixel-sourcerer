using Godot;
using System;

public partial class ImageData : GodotObject
{
	public Image Image { get; set; }
	public ImageTexture Texture { get; set; }
	public string FileName { get; set; }
	public Vector2 Scale { get; set; }

	public ImageData() { }

	public ImageData(Image image, string fileName)
	{
		Image = image;
		FileName = fileName;
		Texture = ImageTexture.CreateFromImage(Image);

		float scale = Mathf.Min(128f / Texture.GetHeight(), 128f / Texture.GetWidth());
		Scale = new Vector2(scale, scale);
	}
}
