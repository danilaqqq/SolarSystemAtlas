using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuInteractions : MonoBehaviour
{
    public GameObject MiniSun;
    public GameObject MiniMercury;
    public GameObject MiniVenus;
    public GameObject MiniEarth;
    public GameObject MiniMars;

    public LineRenderer lineRenderer;
    public int segments = 100;

    public float MercuryOrbitRadius = 1.5f;  
    public float VenusOrbitRadius = 2.2f;  
    public float EarthOrbitRadius = 2.9f;  
    public float MarsOrbitRadius = 3.6f;  

    // Скорость оборота вокруг Солнца
    public float MercuryOrbitSpeed = 11f;
    public float VenusOrbitSpeed = 4.4f;
    public float EarthOrbitSpeed = 2.7f;
    public float MarsOrbitSpeed = 1.4f;

    // Скорость вращения вокруг своей оси
    public float SunRotationSpeed = 3.1f;
    public float MercuryRotationSpeed = 1.6f;
    public float VenusRotationSpeed = 0.8f;
    public float EarthRotationSpeed = 100f;
    public float MarsRotationSpeed = 100f;

    public float SpeedMultiplier = 150;

    // Start is called before the first frame update
    void Start()
    {
        CreateAnOrbit(MiniMercury, MercuryOrbitRadius);
        CreateAnOrbit(MiniVenus, VenusOrbitRadius);
        CreateAnOrbit(MiniEarth, EarthOrbitRadius);
        CreateAnOrbit(MiniMars, MarsOrbitRadius);
    }

    // Update is called once per frame
    void Update()
    {
        float rotationAmount = SunRotationSpeed * SpeedMultiplier * Time.deltaTime;
        MiniSun.transform.Rotate(0, rotationAmount, 0);

        //Меркурий
        RotatePlanet(MiniMercury, MercuryRotationSpeed, MercuryOrbitSpeed);

        //Венера
        RotatePlanet(MiniVenus, VenusRotationSpeed, VenusOrbitSpeed);

        //Земля
        RotatePlanet(MiniEarth, EarthRotationSpeed, EarthOrbitSpeed);

        //Марс
        RotatePlanet(MiniMars, MarsRotationSpeed, MarsOrbitSpeed);
    }

    public void StartGame()
    {
        FindObjectOfType<SceneTransition>().LoadScene("SampleScene");
    }

    public void EnterAtlas()
    {
        FindObjectOfType<SceneTransition>().LoadScene("AtlasScene");
    }

    public void ExitGame()
    {
        Application.Quit();

        // Если в редакторе, выход в редактор
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    void CreateAnOrbit(GameObject planet, float orbitRadius)
    {
        lineRenderer = planet.GetComponent<LineRenderer>();

        lineRenderer.positionCount = segments + 1;
        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.useWorldSpace = true;

        Material orbitMaterial = new Material(Shader.Find("Unlit/Color"));
        orbitMaterial.color = Color.white;
        lineRenderer.material = orbitMaterial;

        lineRenderer.receiveShadows = false;
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        float angleStep = 360f / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * (i * angleStep);
            float x = Mathf.Cos(angle) * orbitRadius + 9.5f; 
            float y = Mathf.Sin(angle) * orbitRadius - 1.5f; 
            float z = 0; 

            lineRenderer.SetPosition(i, new Vector3(x, y, z));
        }
    }

    void RotatePlanet(GameObject planet, float planetRotationSpeed, float planetOrbitSpeed)
    {
        float rotationAmount = planetRotationSpeed * SpeedMultiplier * Time.deltaTime;
        planet.transform.Rotate(0, rotationAmount, 0); 

        Vector3 sunPosition = new Vector3(9.5f, -1.5f, 0); 
        planet.transform.RotateAround(sunPosition, Vector3.forward, planetOrbitSpeed * SpeedMultiplier * Time.deltaTime); 
    }

}
