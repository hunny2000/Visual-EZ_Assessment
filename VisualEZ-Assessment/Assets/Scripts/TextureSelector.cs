using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System.IO;
using UnityEngine.UI;

public class TextureSelector : MonoBehaviour
{
    public GameObject UI_ShowCase;
    public Material TempMat;
    public GameObject EditWall;
    public List<Texture2D> texture;
    [SerializeField] Shader CheckerShader;

    string TextureURL;
    List<JSONNode> DATA;

    string dir;

    void Awake()
    {
        dir = Path.Combine(Application.dataPath,  "Resources");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        texture = new List<Texture2D>();
        DATA = new List<JSONNode>();
        TextureURL = "https://quicklook.orientbell.com/Task/gettiles.php";

        StartCoroutine(GetTileData());
        LoadTextures();
    }

    public void CreateMaterial(string MatName)
    {
        TempMat = new Material(CheckerShader);
        TempMat.name = MatName;
        TempMat.SetTexture("Texture_0", Texture2D.grayTexture);
        TempMat.SetTexture("Texture_1", Texture2D.whiteTexture);
        UI_ShowCase.GetComponent<MeshRenderer>().material = TempMat;
    }

    public void LoadTextures()
    {
        //Load
        DirectoryInfo Dir = new DirectoryInfo(dir);
        FileInfo[] info = Dir.GetFiles("*.png");
        int TexturesCount = info.Length;

        Debug.Log("TexturesCount" + TexturesCount);

        //use texture count from local path for 'for loop' and add textures to textures list
        for (int i = 0; i < TexturesCount; i++)
        {
            var texture_ = Resources.Load<Texture2D>("" + i);
            texture.Add(texture_);
            Debug.Log(texture_.name);
        }
    }

    IEnumerator GetTileData()
    {
        

        UnityWebRequest TileRequest = UnityWebRequest.Get(TextureURL);
        yield return TileRequest.SendWebRequest();
        
        if(TileRequest.isNetworkError || TileRequest.isHttpError)
        {
            Debug.LogError(TileRequest.error);
            yield break;
        }

        JSONNode TilesListInfo = JSON.Parse(TileRequest.downloadHandler.text);

        for(int i = 0; i < TilesListInfo.Count; i++)
        {
            DATA.Add(TilesListInfo[i]);
            Debug.Log(TilesListInfo[i]["url"]);
        }


        //UI
        for(int i = 0; i <DATA.Count; i++)
        {
            Debug.Log(DATA[i]["url"]);
            string TileTextureURL = DATA[i]["url"];

            UnityWebRequest TileTextureRequest = UnityWebRequestTexture.GetTexture(TileTextureURL);
            yield return TileTextureRequest.SendWebRequest();

            //Debug.Log(TileInfo["url"].Value);
            if (TileTextureRequest.isNetworkError || TileTextureRequest.isHttpError)
            {
                Debug.LogError(TileTextureRequest.error);
                yield break;
            }


            var isDownloaded = GetBool(i.ToString());
            Debug.Log(i + "" + isDownloaded);
            if (!isDownloaded)
            {
                byte[] results = TileTextureRequest.downloadHandler.data;

                var dir_ = Path.Combine(dir, i.ToString() + ".png");
                File.WriteAllBytes(dir_, results);
                Debug.Log("DIR_" + dir_);
                
                texture.Add(DownloadHandlerTexture.GetContent(TileTextureRequest));
                SetBool(i.ToString(), true);
            }
        }

        
    }
    public static void SetBool(string key, bool state)
    {
        PlayerPrefs.SetInt(key, state ? 1 : 0);
    }

    public static bool GetBool(string key)
    {
        int value = PlayerPrefs.GetInt(key);
        return value == 1 ? true : false;
    }
}