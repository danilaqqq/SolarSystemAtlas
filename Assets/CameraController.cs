using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class CameraController : MonoBehaviour
{
    public float rotationSpeed = 10.0f; // �������� �������� ������
    private bool isLookingAround = false; // �� �������������?
    private Quaternion initialLocalRotation; // ����������� ��������� ���������� ������
    public float returnSpeed = 5.0f;    // �������� �������� ������ � �������� ���������
    private bool isReturning = false;   // ������ ������������?

    public Transform canvasParent; // ������ �� �������
    private GameObject targetObject; // ������, �� ������� �� �������
    private float lookTimer = 0f;   // �����, � ������� �������� ������ ������� �� ������
    private const float lookDuration = 0.3f; // �����, ������� ������ ������ �������� �� ������, ����� ���������� �������

    public TextMeshProUGUI monitorText; // ������ �� ������ TextMeshPro ��� ������ �� ��������

    public Texture2D crosshairTexture; // �������� �������
    public float crosshairSize = 16;   // ������ ������� �� ������

    private bool canTransition = false; // ����� ������� �� ������ �����?
    private string targetSceneName = "InfoScene"; // �������� �����, � ������� ����� �������

    // ��������� �������
    private void OnGUI()
    {
        float posX = (Screen.width - crosshairSize) / 2;
        float posY = (Screen.height - crosshairSize) / 2;

        GUI.DrawTexture(new Rect(posX, posY, crosshairSize, crosshairSize), crosshairTexture);
    }

    void Start()
    {
        initialLocalRotation = transform.localRotation;

        if (canvasParent != null)
        {
            Transform monitorCanvas = canvasParent.Find("Monitor1");
            if (monitorCanvas != null)
            {
                monitorText = monitorCanvas.Find("MonitorText").GetComponent<TextMeshProUGUI>();
            }
        }

        if (monitorText != null)
        {
            monitorText.gameObject.SetActive(false);
        }
        
        RestorePositionAndRotation();
    }

    void Update()
    {
        HandleCameraRotation();
        HandleRaycasting();

        if (canTransition && Input.GetKeyDown(KeyCode.F))
        {
            SavePositionAndRotation(); // ���������� ������� ����� ���������
            FindObjectOfType<SceneTransition>().LoadScene(targetSceneName);
        }
    }


     void HandleCameraRotation()
     {
            // ���� ������������ Alt - �������������
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isLookingAround = true;
                isReturning = false;
            }
            else
            {
                isLookingAround = false;
                isReturning = true;
            }

            // ������ ������ �������
            if (isLookingAround)
            {
                float rotationX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
                float rotationY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

                transform.Rotate(-rotationY, rotationX, 0);
                //transform.Rotate(0, rotationX, 0);
                //transform.Rotate(Vector3.up * rotationY);
            }

            // ������� ����������� � ����������� �������
            if (isReturning)
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, initialLocalRotation, returnSpeed * Time.deltaTime);

                if (Quaternion.Angle(transform.localRotation, initialLocalRotation) < 0.1f)
                {
                    transform.localRotation = initialLocalRotation;
                    isReturning = false;
                }
            }
     }

   // ����������
   void HandleRaycasting()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // ���� ��� ����� � ������
        if (Physics.Raycast(ray, out hit))
        {
            // ���� ���� ������ - �������
            if (IsPlanet(hit.collider.gameObject.name))
            {
                targetObject = hit.collider.gameObject;
                // ���� ������� �� ����� ������
                if (targetObject != hit.collider.gameObject)
                {
                    // ����� �������
                    targetObject = hit.collider.gameObject;
                    lookTimer = 0f;
                }

                // ���� ������� �� ������, ������ ������
                lookTimer += Time.deltaTime;

                // ���� ������ ���������� ������� �� ������
                if (lookTimer >= lookDuration && monitorText != null)
                {
                    // ��������� �������
                    monitorText.gameObject.SetActive(true);
                    
                    // ����� ���������� �� ���� �����
                    canTransition = true;

                    // ����� ��������� ������� ����� ����������� �����
                    DataHolder.ResetFlags();
                    // ���������� ����� �������
                    switch (targetObject.name)
                    {
                        case "Sun":
                            DataHolder.Sun = true;
                            monitorText.text = "������";
                            break;
                        case "Mercury":
                            DataHolder.Mercury = true;
                            monitorText.text = "��������";
                            break;
                        case "Venus":
                            DataHolder.Venus = true;
                            monitorText.text = "������";
                            break;
                        case "Earth":
                            DataHolder.Earth = true;
                            monitorText.text = "�����";
                            break;
                        case "Moon":
                            DataHolder.Moon = true;
                            monitorText.text = "����";
                            break;
                        case "Mars":
                            DataHolder.Mars = true;
                            monitorText.text = "����";
                            break;
                        case "Jupiter":
                            DataHolder.Jupiter = true;
                            monitorText.text = "������";
                            break;
                        case "Saturn":
                            DataHolder.Saturn = true;
                            monitorText.text = "������";
                            break;
                        case "Neptune":
                            DataHolder.Neptune = true;
                            monitorText.text = "������";
                            break;
                        case "Uranus":
                            DataHolder.Uranus = true;
                            monitorText.text = "����";
                            break;
                        default:
                            Debug.LogWarning("Unknown planet selected!");
                            break;
                    }
                }
            }
            else
            {
                // ���� �� ������� - ����� �������, �������� ��������
                ResetCanvas();
            }
        }
        else
        {
            // ��� �� �� ��� �� ����� - ����� �������, �������� ��������
            ResetCanvas();
        }
    }

    void ResetCanvas()
    {
        // ����� �������
        lookTimer = 0f;
        // �������� ��������
        if (monitorText != null && monitorText.gameObject.activeSelf)
        {
            monitorText.gameObject.SetActive(false);
        }
        canTransition = false;
    }

    // �������?
    bool IsPlanet(string name)
    {
        return name == "Earth" || name == "Mercury" || name == "Venus" || name == "Sun" || name == "Mars" || name == "Jupiter" || name == "Saturn" || name == "Neptune" || name == "Uranus" || name == "Moon";
    }

    // ���������� ������� � ����������
    void SavePositionAndRotation()
    {
        PlayerPrefs.SetFloat("ShipPosX", transform.parent.position.x);
        PlayerPrefs.SetFloat("ShipPosY", transform.parent.position.y);
        PlayerPrefs.SetFloat("ShipPosZ", transform.parent.position.z);

        PlayerPrefs.SetFloat("ShipRotX", transform.parent.rotation.eulerAngles.x);
        PlayerPrefs.SetFloat("ShipRotY", transform.parent.rotation.eulerAngles.y);
        PlayerPrefs.SetFloat("ShipRotZ", transform.parent.rotation.eulerAngles.z);
    }

    // �������������� ������� � ����������
    void RestorePositionAndRotation()
    {
        if (PlayerPrefs.HasKey("ShipPosX"))
        {
            Vector3 position = new Vector3(
                PlayerPrefs.GetFloat("ShipPosX"),
                PlayerPrefs.GetFloat("ShipPosY"),
                PlayerPrefs.GetFloat("ShipPosZ")
            );

            Vector3 rotationEuler = new Vector3(
                PlayerPrefs.GetFloat("ShipRotX"),
                PlayerPrefs.GetFloat("ShipRotY"),
                PlayerPrefs.GetFloat("ShipRotZ")
            );

            transform.parent.position = position;
            transform.parent.rotation = Quaternion.Euler(rotationEuler);
        }
    }
}
