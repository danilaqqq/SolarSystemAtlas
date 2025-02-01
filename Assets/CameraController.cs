using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class CameraController : MonoBehaviour
{
    public float rotationSpeed = 10.0f; // Скорость вращения камеры
    private bool isLookingAround = false; // Мы осматриваемся?
    private Quaternion initialLocalRotation; // Изначальная локальная ориентация камеры
    public float returnSpeed = 5.0f;    // Скорость возврата камеры в исходное положение
    private bool isReturning = false;   // Камера возвращается?

    public Transform canvasParent; // Ссылка на корабль
    private GameObject targetObject; // Объект, на который мы смотрим
    private float lookTimer = 0f;   // Время, в течение которого камера смотрит на объект
    private const float lookDuration = 0.3f; // Время, которое камера должна смотреть на объект, чтобы отобразить монитор

    public TextMeshProUGUI monitorText; // Ссылка на объект TextMeshPro для текста на мониторе

    public Texture2D crosshairTexture; // Текстура прицела
    public float crosshairSize = 16;   // Размер прицела на экране

    private bool canTransition = false; // Можем перейти на другую сцену?
    private string targetSceneName = "InfoScene"; // Название сцены, к которой можно перейти

    // Отрисовка прицела
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
            SavePositionAndRotation(); // Сохранение позиции перед переходом
            FindObjectOfType<SceneTransition>().LoadScene(targetSceneName);
        }
    }


     void HandleCameraRotation()
     {
            // Если удерживается Alt - осматриваемся
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

            // Осмотр внутри корабля
            if (isLookingAround)
            {
                float rotationX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
                float rotationY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

                transform.Rotate(-rotationY, rotationX, 0);
                //transform.Rotate(0, rotationX, 0);
                //transform.Rotate(Vector3.up * rotationY);
            }

            // Плавное возвращение к изначальной позиции
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

   // Рейкастинг
   void HandleRaycasting()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Если луч попал в объект
        if (Physics.Raycast(ray, out hit))
        {
            // Если этот объект - планета
            if (IsPlanet(hit.collider.gameObject.name))
            {
                targetObject = hit.collider.gameObject;
                // Если смотрим на новый объект
                if (targetObject != hit.collider.gameObject)
                {
                    // Сброс таймера
                    targetObject = hit.collider.gameObject;
                    lookTimer = 0f;
                }

                // Пока смотрим на объект, таймер растет
                lookTimer += Time.deltaTime;

                // Если камера достаточно смотрит на объект
                if (lookTimer >= lookDuration && monitorText != null)
                {
                    // Создается монитор
                    monitorText.gameObject.SetActive(true);
                    
                    // Можно переходить на инфо сцену
                    canTransition = true;

                    // Сброс выбранной планеты перед назначением новой
                    DataHolder.ResetFlags();
                    // Назначение новой планеты
                    switch (targetObject.name)
                    {
                        case "Sun":
                            DataHolder.Sun = true;
                            monitorText.text = "Солнце";
                            break;
                        case "Mercury":
                            DataHolder.Mercury = true;
                            monitorText.text = "Меркурий";
                            break;
                        case "Venus":
                            DataHolder.Venus = true;
                            monitorText.text = "Венера";
                            break;
                        case "Earth":
                            DataHolder.Earth = true;
                            monitorText.text = "Земля";
                            break;
                        case "Moon":
                            DataHolder.Moon = true;
                            monitorText.text = "Луна";
                            break;
                        case "Mars":
                            DataHolder.Mars = true;
                            monitorText.text = "Марс";
                            break;
                        case "Jupiter":
                            DataHolder.Jupiter = true;
                            monitorText.text = "Юпитер";
                            break;
                        case "Saturn":
                            DataHolder.Saturn = true;
                            monitorText.text = "Сатурн";
                            break;
                        case "Neptune":
                            DataHolder.Neptune = true;
                            monitorText.text = "Нептун";
                            break;
                        case "Uranus":
                            DataHolder.Uranus = true;
                            monitorText.text = "Уран";
                            break;
                        default:
                            Debug.LogWarning("Unknown planet selected!");
                            break;
                    }
                }
            }
            else
            {
                // Если не планета - сброс таймера, удаление монитора
                ResetCanvas();
            }
        }
        else
        {
            // Луч ни во что не попал - сброс таймера, удаление монитора
            ResetCanvas();
        }
    }

    void ResetCanvas()
    {
        // Сброс таймера
        lookTimer = 0f;
        // Сокрытие монитора
        if (monitorText != null && monitorText.gameObject.activeSelf)
        {
            monitorText.gameObject.SetActive(false);
        }
        canTransition = false;
    }

    // Планета?
    bool IsPlanet(string name)
    {
        return name == "Earth" || name == "Mercury" || name == "Venus" || name == "Sun" || name == "Mars" || name == "Jupiter" || name == "Saturn" || name == "Neptune" || name == "Uranus" || name == "Moon";
    }

    // Сохранение позиции и ориентации
    void SavePositionAndRotation()
    {
        PlayerPrefs.SetFloat("ShipPosX", transform.parent.position.x);
        PlayerPrefs.SetFloat("ShipPosY", transform.parent.position.y);
        PlayerPrefs.SetFloat("ShipPosZ", transform.parent.position.z);

        PlayerPrefs.SetFloat("ShipRotX", transform.parent.rotation.eulerAngles.x);
        PlayerPrefs.SetFloat("ShipRotY", transform.parent.rotation.eulerAngles.y);
        PlayerPrefs.SetFloat("ShipRotZ", transform.parent.rotation.eulerAngles.z);
    }

    // Восстанавление позиции и ориентации
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
