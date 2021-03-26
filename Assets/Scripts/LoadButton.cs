using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadButton : MonoBehaviour
{
    private SaveGame activeSaveGame;
    [SerializeField] private Image saveImage;
    [SerializeField] private TextMeshProUGUI saveTitle;

    public void LoadSaveGameToButton(SaveGame save)
    {
        if(ES3.FileExists(save.SaveGameName + ".png"))
        {
            Texture2D texture = ES3.LoadImage(save.SaveGameName + ".png");
            Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.05f, 0.05f), 100f);
            saveImage.sprite = newSprite;
        }

        saveTitle.text = save.SaveGameName;
        activeSaveGame = save;
    }

    public void LoadGame()
    {
        GameObject.Find("Canvas").GetComponent<MenuManager>().LoadSaveGame(activeSaveGame.SaveGameName);
    }
}
