using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] GameObject WorldEditCanvasPrefab;

    [SerializeField] RawImage RawImage_;
    [SerializeField] GameObject texturePanel;
    [SerializeField] GameObject SelectionPanel;
    TextureSelector textureSelector;

    List<Texture2D> SelectedTextures;

    bool TURN = true; // Turn Raycast On or Off

    void Start()
    {
        SelectedTextures = new List<Texture2D>();
        textureSelector = Camera.main.GetComponent<TextureSelector>();

        //textureSelector.CreateAndApplyMaterial();
        ListTextures();
    }

    private void Update()
    {
        var camera = Camera.main;
        var screenRay = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0) && TURN)
        {
            if (Physics.Raycast(screenRay, out hit, float.PositiveInfinity))
            {
                textureSelector.EditWall = hit.collider.gameObject;

                GameObject EditCanvas = Instantiate(WorldEditCanvasPrefab, textureSelector.EditWall.transform);
                EditCanvas.GetComponentInChildren<Button>().onClick.AddListener(() => EditWallTexture(textureSelector.EditWall));
                Debug.Log(hit.collider.name);

                TURN = false;
            }
        }
    }

    void ListTextures()
    {
        foreach (Texture2D texture in textureSelector.texture)
        {
            RawImage image = Instantiate(RawImage_, texturePanel.transform);
            image.name = texture.name;
            image.gameObject.AddComponent<Button>().onClick.AddListener(() => SelectTexture(texture));

            image.texture = texture;
        }
    }

    public void SelectTexture(Texture2D texture)
    {
        SelectedTextures.Add(texture);
        
        for(int i = 0; i < SelectedTextures.Count; i++)
        {
            SelectedTextures[i].name = i.ToString();
        }
        Debug.Log("Texture_" + texture.name);
        textureSelector.TempMat.SetTexture("Texture_" + texture.name, texture);

    }

    public void EditWallTexture(GameObject Wall)
    {
        TURN = false;
        textureSelector.CreateMaterial(Wall.name);
        SelectionPanel.SetActive(true);
    }
    public void Done()
    {
        SelectedTextures.Clear();

        textureSelector.EditWall.GetComponent<MeshRenderer>().material = textureSelector.TempMat;
        Destroy(textureSelector.EditWall.GetComponentInChildren<Canvas>().gameObject);
        SelectionPanel.SetActive(false);

        TURN = true;
    }
}
