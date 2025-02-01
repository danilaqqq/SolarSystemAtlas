using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AtlasInteractions : MonoBehaviour
{
    string targetPlanet;

    // Start is called before the first frame update
    void Start()
    {
        DataHolder.InfoEnterThroughAtlas = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Получение объекта на который наведен курсор
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;

                GameObject parentObject = clickedObject.transform.parent.parent.gameObject;

                targetPlanet = parentObject.name.Substring(0, parentObject.name.Length - 5);

                Debug.Log(targetPlanet);
            }
        }
    }

    // Возвращение в главное меню
    public void ReturnToMainMenu()
    {
        FindObjectOfType<SceneTransition>().LoadScene("MainMenu");
        DataHolder.ResetFlags();
        DataHolder.InfoEnterThroughAtlas = false;
    }

    // Переход на сцену с информацией
    public void GoToInfoScene() 
    {
        switch (targetPlanet)
        {
            case "Sun":
                DataHolder.Sun = true;
                break;
            case "Mercury":
                DataHolder.Mercury = true;
                break;
            case "Venus":
                DataHolder.Venus = true;
                break;
            case "Earth":
                DataHolder.Earth = true;
                break;
            case "Moon":
                DataHolder.Moon = true;
                break;
            case "Mars":
                DataHolder.Mars = true;
                break;
            case "Jupiter":
                DataHolder.Jupiter = true;
                break;
            case "Saturn":
                DataHolder.Saturn = true;
                break;
            case "Neptune":
                DataHolder.Neptune = true;
                break;
            case "Uranus":
                DataHolder.Uranus = true;
                break;
            default:
                Debug.LogWarning("Unknown planet selected!");
                break;
        }

        FindObjectOfType<SceneTransition>().LoadScene("InfoScene");
    }
}
