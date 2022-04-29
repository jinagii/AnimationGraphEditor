using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class TextureLoader : MonoBehaviour
{
    Texture2D _texture;

    public string[] _textureNames;

    private int _textureIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        _texture = new Texture2D(256, 256, TextureFormat.ASTC_6x6, true);
        _textureIndex = 0;

        ExtractTextureContent();

        var meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material.mainTexture = _texture;
        }

        _dataPath = Application.dataPath;
        LoadTexture();
    }

    void ExtractTextureContent()
    {
        for(int i = 0;i < 3; i++)
        {
            string asset = $"/Textures/{_textureNames[i]}";
            string assetpath = $"{asset}.bytes";
            string path = Application.dataPath + assetpath;
            if (!File.Exists(path))
            {
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/{asset}.png");
                if (texture != null)
                {
                    var bytes = texture.GetRawTextureData<byte>();
                    File.WriteAllBytes(path, bytes.ToArray());
                }
            }
        }
    }

    byte[] _textureData = new byte[40096];
    bool _isLoaded = false;
    string _dataPath = null;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LoadTexture();
        }

        if (_isLoaded)
        {
            var meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                Texture2D texture2D = meshRenderer.material.mainTexture as Texture2D;
                if (texture2D != null)
                {
                    var rawData = texture2D.GetRawTextureData<byte>();
                    rawData.CopyFrom(_textureData);
                    texture2D.Apply();
                }
            }

            _isLoaded = false;
        }
    }

    void LoadTexture()
    {
        _textureIndex++;
        if (_textureIndex == 2)
            _textureIndex = 0;

        Task task = Task.Run(() =>
        {
            string asset = $"/Textures/{_textureNames[_textureIndex]}";
            string assetpath = $"{asset}.bytes";
            string path = _dataPath + assetpath;

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                fs.Read(_textureData, 0, (int)fs.Length);
            }

            _isLoaded = true;

        });
    }
}
