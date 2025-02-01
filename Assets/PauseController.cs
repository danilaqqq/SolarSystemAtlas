using Newtonsoft.Json.Bson;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuUI; 
    public Slider rotationSpeedSlider; 
    public PlanetRotationScript RotScript;
    private bool isPaused = false;
    TextMeshProUGUI buttonText;
    
    public TextMeshProUGUI hintsText;

    // ���������
    private string hints =
        "W, S - ������, �����\n" +
        "A, D - �����, ������\n" +
        "Q, E - ���� �����, ������\n" +
        "Z - ������� �����\n" +
        "F - ���������� � �������\n" +
        "������ - ���������\n" +
        "Shift - ������\n" +
        "Esc - �����";

    private void Start()
    {
        rotationSpeedSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void Update()
    {
        // ���� Escape - �����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        Hints();
    }

    // ������� ����������� ����
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; 
        isPaused = false; 
    }

    // ������� ���������� ���� �� �����
    void PauseGame()
    {
        pauseMenuUI.SetActive(true); 
        Time.timeScale = 0f; 
        isPaused = true; 
    }

    // ������� ������ �� ����
    public void QuitGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        FindObjectOfType<SceneTransition>().LoadScene("MainMenu");
    }

    // ������� �� ������ ��������
    public void RotateToggle()
    {
        if (DataHolder.RotatePlanets == false)
        {
            DataHolder.RotatePlanets = true;
            buttonText = pauseMenuUI.transform.Find("RotationButton").Find("RotButText").GetComponent<TextMeshProUGUI>();
            buttonText.text = "��������: ���";
        }
        else
        {
            DataHolder.RotatePlanets = false;
            buttonText = pauseMenuUI.transform.Find("RotationButton").Find("RotButText").GetComponent<TextMeshProUGUI>();
            buttonText.text = "��������: ����";
        }
    }

    // ���������� RotationSpeed
    private void OnSliderValueChanged(float value)
    {
        RotScript.SpeedMultiplier = value;
    }

    // ����������� ���������
    private void Hints()
    {
        if (!isPaused)
        {
            hintsText.text = hints;
            hintsText.enabled = true; 
        }
        else
        {
            hintsText.enabled = false;
        }
    }
}