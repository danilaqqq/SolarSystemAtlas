using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class SpaceShipMovement : MonoBehaviour
{
    
    public float speed = 10f; // Базовая скорость движения
    public float rotationSpeed = 90f; // Скорость поворота
    public float rollSpeed = 60f; // Скорость крена
    public float boostMultiplier = 20f; // Множитель ускорения
    public float strafeSpeed = 20f; // Скорость горизонтального перемещения
    public float smoothTime = 0.5f;    // Время для плавного изменения скорости

    private float currentSpeed; // Текущая скорость
    private float targetSpeed; // Cкорость к которой ускоряется/замедляется корабль
    private float rollAngle = 0f; // Текущий угол крена

    public float boundaryRadius = 11000f; // Радиус границы
    private Vector3 centerPoint = Vector3.zero; // Центр отсчета (0,0,0)

    public Transform canvasParent;
    public TextMeshProUGUI monitorText;
    public TextMeshProUGUI speedMonitorText;

    public Camera shipCamera;
    public float normalFOV = 60f; // Обычный FOV
    public float boostFOV = 76f; // FOV при ускорении
    public float transitionSpeed = 1f; // Скорость перехода к ускорению

    private AudioSource movementAudioSource; // Источник звука
    private bool isMoving = false; // Двигается ли корабль?

    public float currentShowSpeed;
    public float targetShowSpeed;
    public float initialShowSpeed;
    public float showSpeedChangeSpeed = 1f;

    private enum ShipState { Stopped, Moving, Boosting };
    private ShipState currentState = ShipState.Stopped; // Текущее состояние корабля
    

    void Start()
    {
        currentSpeed = speed;
        targetSpeed = speed;

        currentShowSpeed = 0f;
        targetShowSpeed = 0f;
        initialShowSpeed = 0f;
        
        movementAudioSource = GetComponent<AudioSource>();
        if (movementAudioSource == null)
        {
            Debug.LogError("AudioSource не найден на объекте корабля.");
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
           
        }
        else
        {
            HandleRotation();
        }
        HandleMovement();
        HandleStrafing();
        HandleRoll();
        HandleBoost();
        SmoothSpeedTransition();
        BoundaryCheck();
        HandleAudio();
        HandleSpaceshipState();
    }

    // Управление направлением взгляда корабля с помощью мыши - тангаж и рысканье
    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float yaw = mouseX * rotationSpeed * Time.deltaTime;
        float pitch = -mouseY * rotationSpeed * Time.deltaTime;

        transform.Rotate(pitch, yaw, 0f, Space.Self);
    }

    // Управление движением корабля вперёд/назад
    void HandleMovement()
    {
        float moveInput = Input.GetAxis("Vertical");
        Vector3 forward = transform.forward * moveInput * currentSpeed * Time.deltaTime;

        if (moveInput != 0)
        {
            //isMoving = true;
        }

        transform.position += forward;
    }

    // Управление горизонтальным движением
    void HandleStrafing()
    {
        float strafeInput = Input.GetAxis("Horizontal");
        Vector3 right = transform.right * strafeInput * strafeSpeed * Time.deltaTime;

        if (strafeInput != 0)
        {
            //isMoving = true;
        }

        transform.position += right;
    }

    // Управление креном
    private void HandleRoll()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            rollAngle += rollSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            rollAngle -= rollSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.Z))
        {
            rollAngle = Mathf.Lerp(rollAngle, 0f, 3 * Time.deltaTime);
        }

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, rollAngle);
    }

    // Ускорение движения при удержании пробела
    void HandleBoost()
    {
        if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.W))
        {
            targetSpeed = speed * boostMultiplier;
            shipCamera.fieldOfView = Mathf.Lerp(shipCamera.fieldOfView, boostFOV, Time.deltaTime * transitionSpeed);
            isMoving = true;
        }
        else
        {
            targetSpeed = speed;
            shipCamera.fieldOfView = Mathf.Lerp(shipCamera.fieldOfView, normalFOV, Time.deltaTime * transitionSpeed);
        }
    }

    // Плавное изменение текущей скорости к целевой
    void SmoothSpeedTransition()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / smoothTime);
    }

    // Проверка вылета за границы
    void BoundaryCheck()
    {
        float distanceFromCenter = Vector3.Distance(transform.position, centerPoint);
        monitorText.gameObject.SetActive(false);
        if (distanceFromCenter > boundaryRadius)
        {
            Vector3 directionToCenter = (transform.position - centerPoint).normalized;
            transform.position = centerPoint + directionToCenter * boundaryRadius;
            
            Transform monitorCanvas = canvasParent.Find("Monitor2");
            if (monitorCanvas != null)
            {
                monitorText = monitorCanvas.Find("MonitorText").GetComponent<TextMeshProUGUI>();
            }
            monitorText.gameObject.SetActive(true); // Включаем текст
            monitorText.text = "Достгнут край галактики...";
        }
    }

    // Аудио сопроождение
    void HandleAudio()
    {
        float targetVolume = isMoving ? 0.75f : 0.2f;
        float volumeChangeSpeed = 0.5f;

        movementAudioSource.volume = Mathf.MoveTowards(movementAudioSource.volume, targetVolume, volumeChangeSpeed * Time.deltaTime);

        if (isMoving && !movementAudioSource.isPlaying)
        {
            movementAudioSource.Play();
        }
        else if (!isMoving && movementAudioSource.volume <= 0f && movementAudioSource.isPlaying)
        {
            movementAudioSource.Stop();
        }

        isMoving = false;
    }
    
    // Плавное изменение скорости
    private float ChangeShowSpeed(float current, float start, float target, float deltaSpeed)
    {
        float distance = Mathf.Abs(target - current);

        if (distance < 0.01f) return target;

        float totalDistance = Mathf.Abs(target - start);

        float easeFactor = Mathf.Min(1.0f, distance / totalDistance);
        float delta = deltaSpeed * easeFactor;

        if (current < target)
            return Mathf.Min(current + delta, target);
        else
            return Mathf.Max(current - delta, target); 
    }

    // Отображение скорости на мониторе
    void HandleSpaceshipState()
    {
        ShipState newState;

        if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.W))
        {
            newState = ShipState.Boosting;
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            newState = ShipState.Moving;
        }
        else
        {
            newState = ShipState.Stopped;
        }

        switch (newState)
        {
            case ShipState.Stopped:
                targetShowSpeed = 0f;
                currentShowSpeed = ChangeShowSpeed(currentShowSpeed, initialShowSpeed, targetShowSpeed, showSpeedChangeSpeed);
                
                Transform speedMonitor = canvasParent.Find("SpeedMonitor");
                if (speedMonitor != null)
                {
                    speedMonitorText = speedMonitor.Find("MonitorText").GetComponent<TextMeshProUGUI>();
                }
                speedMonitorText.gameObject.SetActive(true); 
                speedMonitorText.text = Mathf.Round(currentShowSpeed) + " млн км/c";
                break;
            
            case ShipState.Moving:
                targetShowSpeed = 5f;
                currentShowSpeed = ChangeShowSpeed(currentShowSpeed, initialShowSpeed, targetShowSpeed, showSpeedChangeSpeed/10);
                
                speedMonitor = canvasParent.Find("SpeedMonitor");
                if (speedMonitor != null)
                {
                    speedMonitorText = speedMonitor.Find("MonitorText").GetComponent<TextMeshProUGUI>();
                }
                speedMonitorText.gameObject.SetActive(true); 
                speedMonitorText.text = Mathf.Round(currentShowSpeed) + " млн км/c";
                break;
            
            case ShipState.Boosting:
                targetShowSpeed = 100f;
                currentShowSpeed = ChangeShowSpeed(currentShowSpeed, initialShowSpeed, targetShowSpeed, showSpeedChangeSpeed);
                
                speedMonitor = canvasParent.Find("SpeedMonitor");
                if (speedMonitor != null)
                {
                    speedMonitorText = speedMonitor.Find("MonitorText").GetComponent<TextMeshProUGUI>();
                }
                speedMonitorText.gameObject.SetActive(true);
                speedMonitorText.text = Mathf.Round(currentShowSpeed) + " млн км/c";
                break;
        }

        if (newState != currentState)
        {
            initialShowSpeed = currentShowSpeed;

            currentState = newState;
        }
    }
}
