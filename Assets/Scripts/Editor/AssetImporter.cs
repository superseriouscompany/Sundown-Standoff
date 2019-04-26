using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetImporter : AssetPostprocessor {
	void OnPreprocessTexture() {
		var textureImporter = (TextureImporter)assetImporter;
		textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
		textureImporter.filterMode = FilterMode.Point;
		textureImporter.spriteImportMode = SpriteImportMode.Multiple;
		textureImporter.mipmapEnabled = false;
	}

	// https://forum.unity.com/threads/sprite-editor-automatic-slicing-by-script.320776/
	void OnPostprocessTexture(Texture2D texture) {
		return;
		Debug.Log("Texture2D: (" + texture.width + "x" + texture.height + ")");

		int spriteSize = 64;
		int colCount = texture.width / spriteSize;
		int rowCount = texture.height / spriteSize;

		List<SpriteMetaData> metas = new List<SpriteMetaData>();

		for (int r = 0; r < rowCount; ++r) {
			for (int c = 0; c < colCount; ++c) {
				SpriteMetaData meta = new SpriteMetaData();
				meta.rect = new Rect(c * spriteSize, r * spriteSize, spriteSize, spriteSize);
				meta.name = c + "-" + r;
				metas.Add(meta);
			}
		}

		Debug.Log($"Got this many sprites: {metas.Count}");

		TextureImporter textureImporter = (TextureImporter)assetImporter;
		textureImporter.spritesheet = metas.ToArray();
	}

	void OnPostprocessSprites(Texture2D texture, Sprite[] sprites) {
		return;
		Debug.Log("Sprites: " + sprites.Length);
	}
}
