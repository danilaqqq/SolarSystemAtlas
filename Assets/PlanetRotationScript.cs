using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlanetRotationScript : MonoBehaviour
{
    public GameObject Mercury;
    public GameObject Venus;
    public GameObject Earth;
    public GameObject Moon;
    public GameObject Mars;
    public GameObject Jupiter;
    public GameObject Saturn;
    public GameObject Uranus;
    public GameObject Neptune;

    // Скорость вращения вокруг своей оси
    public float SunRotationSpeed = 0.3125f;
    public float MercuryRotationSpeed = 0.1695f;
    public float VenusRotationSpeed = 0.0862f;
    public float EarthRotationSpeed = 10.0f;
    public float MoonRotationSpeed = 0.3448f;
    public float MarsRotationSpeed = 10.0f;
    public float JupiterRotationSpeed = 25.0f;
    public float SaturnRotationSpeed = 22.22f;
    public float UranusRotationSpeed = 14.29f;
    public float NeptuneRotationSpeed = 15.38f;


    // Скорость оборота вокруг Солнца
    public float MercuryOrbitSpeed = 0.1136f;
    public float VenusOrbitSpeed = 0.0446f;
    public float EarthOrbitSpeed = 0.0274f;
    public float MoonOrbitSpeed = 0.3448f;
    public float MarsOrbitSpeed = 0.0146f;
    public float JupiterOrbitSpeed = 0.00228f;
    public float SaturnOrbitSpeed = 0.00093f;
    public float UranusOrbitSpeed = 0.00033f;
    public float NeptuneOrbitSpeed = 0.00017f;

    //Мультиплаер скорости
    public float SpeedMultiplier = 10f;

    public LineRenderer lineRenderer;
    public int segments = 100000;  // Количество сегментов кольца
    
    public float MercuryOrbitRadius = 1216f;  // Радиус орбиты
    public float VenusOrbitRadius = 1308f;  
    public float EarthOrbitRadius = 1390f;  
    public float MoonOrbitRadius = 1.2f;  
    public float MarsOrbitRadius = 1556f;  
    public float JupiterOrbitRadius = 2657f;  
    public float SaturnOrbitRadius = 3954f;  
    public float UranusOrbitRadius = 6842f;  
    public float NeptuneOrbitRadius = 10090f;  

    void Start()
    {
        CreateAnOrbit(Mercury, MercuryOrbitRadius);
        CreateAnOrbit(Venus, VenusOrbitRadius);
        CreateAnOrbit(Earth, EarthOrbitRadius);
        CreateAnOrbit(Moon, MoonOrbitRadius);
        CreateAnOrbit(Mars, MarsOrbitRadius);
        CreateAnOrbit(Jupiter, JupiterOrbitRadius);
        CreateAnOrbit(Saturn, SaturnOrbitRadius);
        CreateAnOrbit(Uranus, UranusOrbitRadius);
        CreateAnOrbit(Neptune, NeptuneOrbitRadius);
    }

    void Update()
    {
        if (DataHolder.RotatePlanets)
        {
            //Солнце
            float rotationAmount = SunRotationSpeed * SpeedMultiplier * Time.deltaTime;
            transform.Rotate(0, rotationAmount, 0);

            //Меркурий
            RotatePlanet(Mercury, MercuryRotationSpeed, MercuryOrbitSpeed);

            //Венера
            RotatePlanet(Venus, VenusRotationSpeed, VenusOrbitSpeed);

            //Земля
            RotatePlanet(Earth, EarthRotationSpeed, EarthOrbitSpeed);

            //Луна
            rotationAmount = MoonRotationSpeed * SpeedMultiplier * Time.deltaTime;
            Moon.transform.Rotate(0, rotationAmount, 0);

            Moon.transform.RotateAround(Earth.transform.position, Vector3.up, MoonOrbitSpeed * SpeedMultiplier * Time.deltaTime);

            //Марс
            RotatePlanet(Mars, MarsRotationSpeed, MarsOrbitSpeed);

            //Юпитер
            RotatePlanet(Jupiter, JupiterRotationSpeed, JupiterOrbitSpeed);

            //Сатурн
            RotatePlanet(Saturn, SaturnRotationSpeed, SaturnOrbitSpeed);

            //Уран
            RotatePlanet(Uranus, UranusRotationSpeed, UranusOrbitSpeed);

            //Нептун
            RotatePlanet(Neptune, NeptuneRotationSpeed, NeptuneOrbitSpeed);
        }
    }

    void RotatePlanet(GameObject planet, float planetRotationSpeed, float planetOrbitSpeed)
    {
        float rotationAmount = planetRotationSpeed * SpeedMultiplier * Time.deltaTime;
        planet.transform.Rotate(0, rotationAmount, 0);

        planet.transform.RotateAround(transform.position, Vector3.up, planetOrbitSpeed * SpeedMultiplier * Time.deltaTime);
    }

    void CreateAnOrbit(GameObject planet, float orbitRadius)
    {
        lineRenderer = planet.GetComponent<LineRenderer>();

        lineRenderer.positionCount = segments + 1;
        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;
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
            float x = Mathf.Cos(angle) * orbitRadius;
            float z = Mathf.Sin(angle) * orbitRadius;

            lineRenderer.SetPosition(i, new Vector3(x, 0, z));
        }

    }
}
