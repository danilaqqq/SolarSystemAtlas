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

    // Подсказки
    private string hints =
        "W, S - вперед, назад\n" +
        "A, D - влево, вправо\n" +
        "Q, E - крен влево, вправо\n" +
        "Z - возврат крена\n" +
        "F - информация о планете\n" +
        "Пробел - ускорение\n" +
        "Shift - осмотр\n" +
        "Esc - пауза";

    private void Start()
    {
        rotationSpeedSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void Update()
    {
        // Если Escape - пауза
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

    // Функция продолжения игры
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; 
        isPaused = false; 
    }

    // Функция постановки игры на паузу
    void PauseGame()
    {
        pauseMenuUI.SetActive(true); 
        Time.timeScale = 0f; 
        isPaused = true; 
    }

    // Функция выхода из игры
    public void QuitGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        FindObjectOfType<SceneTransition>().LoadScene("MainMenu");
    }

    // Нажатие на кнопку вращения
    public void RotateToggle()
    {
        if (DataHolder.RotatePlanets == false)
        {
            DataHolder.RotatePlanets = true;
            buttonText = pauseMenuUI.transform.Find("RotationButton").Find("RotButText").GetComponent<TextMeshProUGUI>();
            buttonText.text = "Вращение: ВКЛ";
        }
        else
        {
            DataHolder.RotatePlanets = false;
            buttonText = pauseMenuUI.transform.Find("RotationButton").Find("RotButText").GetComponent<TextMeshProUGUI>();
            buttonText.text = "Вращение: ВЫКЛ";
        }
    }

    // Обновление RotationSpeed
    private void OnSliderValueChanged(float value)
    {
        RotScript.SpeedMultiplier = value;
    }

    // Отображение подсказок
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