using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InfoScene : MonoBehaviour
{
    public Button backButton; 
    public Renderer Renderer; 
    public TextMeshProUGUI InfoText; 
    public TextMeshProUGUI NameText;
    public GameObject saturnRings;
    public float rotationSpeed = 10f; // �������� �������� �����

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (Renderer == null)
        {
            GameObject sphereObject = GameObject.Find("Planet"); 
            if (sphereObject != null)
            {
                Renderer = sphereObject.GetComponent<Renderer>();
            }
            else
            {
                Debug.LogError("Sphere object not found!");
                return;
            }
        }

        
        if (InfoText == null)
        {
            Debug.LogWarning("InfoText is not assigned in the inspector!");
        }
        if (NameText == null)
        {
            Debug.LogWarning("NameText is not assigned in the inspector!");
        }


        Renderer.material = null;
        // �������� ���������������� ���������
        if (DataHolder.Sun)
        {
            LoadMaterials(new string[] { "sunMaterial" });
            NameText.text = "������";
            InfoText.text = DataHolder.SunText;
        }
        else if (DataHolder.Mercury)
        {
            LoadMaterials(new string[] { "mercuryMaterial" });
            NameText.text = "��������";
            InfoText.text = DataHolder.MercuryText;
        }
        else if (DataHolder.Venus)
        {
            LoadMaterials(new string[] { "venusMaterial", "venusAtmosphereMaterial" });
            NameText.text = "������";
            InfoText.text = DataHolder.VenusText;
        }
        else if (DataHolder.Earth)
        {
            LoadMaterials(new string[] { "EarthDefaultMaterial", "EarthCloudMaterial" });
            NameText.text = "�����";
            InfoText.text = DataHolder.EarthText;
        }
        else if (DataHolder.Moon)
        {
            LoadMaterials(new string[] { "moonTexture" });
            NameText.text = "����";
            InfoText.text = DataHolder.MoonText;
        }
        else if (DataHolder.Mars)
        {
            LoadMaterials(new string[] { "marsMaterial" });
            NameText.text = "����";
            InfoText.text = DataHolder.MarsText;
        }
        else if (DataHolder.Jupiter)
        {
            LoadMaterials(new string[] { "jupiterMaterial" });
            NameText.text = "������";
            InfoText.text = DataHolder.JupiterText;
        }
        else if (DataHolder.Saturn)
        {
            LoadMaterials(new string[] { "saturnMaterial" });
            transform.Rotate(-3f, 0, 0);
            saturnRings.SetActive(true);
            NameText.text = "������";
            InfoText.text = DataHolder.SaturnText;
        }
        else if (DataHolder.Neptune)
        {
            LoadMaterials(new string[] { "neptuneMaterial" });
            NameText.text = "������";
            InfoText.text = DataHolder.NeptuneText;
        }
        else if (DataHolder.Uranus)
        {
            LoadMaterials(new string[] { "uranusMaterial" });
            NameText.text = "����";
            InfoText.text = DataHolder.UranusText;
        }
        else Debug.LogWarning("No planet is selected!");
    }

    // �������� �������� ������ ����� ���
    void Update()
    {
         float rotationAmount = rotationSpeed * Time.deltaTime;
         transform.Rotate(0, rotationAmount, 0);
    }

    // ���������� ���������
    void LoadMaterials(string[] materialNames)
    {
        Material[] materials = new Material[materialNames.Length];

        for (int i = 0; i < materialNames.Length; i++)
        {
            Material material = Resources.Load<Material>(materialNames[i]);
            if (material != null)
            {
                if (materialNames[i] == "EarthCloudMaterial" || materialNames[i] == "venusAtmosphereMaterial")
                {
                    Color color = material.color;
                    color.a = 0.5f;
                    material.color = color;
                }
                materials[i] = material;
            }
            else
            {
                Debug.LogError("Material not found: " + materialNames[i]);
            }
        }

        Renderer.materials = materials;
    }

    // ��������� ������� �� ������ �����
    public void BackButtonClick()
    {
        if (!DataHolder.InfoEnterThroughAtlas)
        {
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            SceneManager.LoadScene("AtlasScene");
        }
        DataHolder.ResetFlags();
    }
}
