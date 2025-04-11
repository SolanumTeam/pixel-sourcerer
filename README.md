# pixel-sourcerer

A source image generator made in Godot, for Godot

## Description

Pixel Sourcerer can generate source images from a map image and any overlay images. Source images can be used together with a color lookup table and simple shader to provide an easy way to modify your pixel art characters' appearence without having to create a different set of sprites with the same animations for each different skin you want to add.

You are free to use any code in this project in your own projects, including the shader and the image generation code! But please mention the source if you do so.

## Instructions

1. Upload a map image (image that has the same format as the lookup table, but each pixel has a different color)
2. Upload all overlay images you wish to convert (images for each animation frame and each pixel should have the same color as the corresponding pixel in the map image)
3. Click on "Generate" to generate the source images
4. Upload a lookup image to visualize the final result with the shader (optional)
5. Select the file format you wish to use for the source images and click on "Save". Your images will be saved in the selected directory as {file name you wrote} + {number} (example: source1.png, source2.png...)

## Credits

- Pixel Sourcerer by Randoraz (Solanum Team)
- Font by [Kenney](https://kenney.nl/)
- Inpired by [aarthificial](https://www.youtube.com/watch?v=HsOKwUwL1bE&pp=ygUdcmV1c2FibGUgcGl4ZWwgYXJ0IGFuaW1hdGlvbnM%3D)
