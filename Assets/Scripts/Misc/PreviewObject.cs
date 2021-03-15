using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewObject : MonoBehaviour
{
    [SerializeField] private GameObject previewObject;

    void Start()
    {
        Texture2D previewTexture = UnityEditor.AssetPreview.GetAssetPreview(previewObject);
        GetComponent<Image>().sprite = Sprite.Create(previewTexture, new Rect(0, 0, previewTexture.width, previewTexture.height), new Vector2(0.5f, 0.5f));
        Destroy(this);
    }

}
