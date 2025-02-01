using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class SpaceShipMovement : MonoBehaviour
{
    
    public float speed = 10f; // ������� �������� ��������
    public float rotationSpeed = 90f; // �������� ��������
    public float rollSpeed = 60f; // �������� �����
    public float boostMultiplier = 20f; // ��������� ���������
    public float strafeSpeed = 20f; // �������� ��������������� �����������
    public float smoothTime = 0.5f;    // ����� ��� �������� ��������� ��������

    private float currentSpeed; // ������� ��������
    private float targetSpeed; // C������� � ������� ����������/����������� �������
    private float rollAngle = 0f; // ������� ���� �����

    public float boundaryRadius = 11000f; // ������ �������
    private Vector3 centerPoint = Vector3.zero; // ����� ������� (0,0,0)

    public Transform canvasParent;
    public TextMeshProUGUI monitorText;
    public TextMeshProUGUI speedMonitorText;

    public Camera shipCamera;
    public float normalFOV = 60f; // ������� FOV
    public float boostFOV = 76f; // FOV ��� ���������
    public float transitionSpeed = 1f; // �������� �������� � ���������

    private AudioSource movementAudioSource; // �������� �����
    private bool isMoving = false; // ��������� �� �������?

    public float currentShowSpeed;
    public float targetShowSpeed;
    public float initialShowSpeed;
    public float showSpeedChangeSpeed = 1f;

    private enum ShipState { Stopped, Moving, Boosting };
    private ShipState currentState = ShipState.Stopped; // ������� ��������� �������
    

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
            Debug.LogError("AudioSource �� ������ �� ������� �������.");
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

    // ���������� ������������ ������� ������� � ������� ���� - ������ � ��������
    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float yaw = mouseX * rotationSpeed * Time.deltaTime;
        float pitch = -mouseY * rotationSpeed * Time.deltaTime;

        transform.Rotate(pitch, yaw, 0f, Space.Self);
    }

    // ���������� ��������� ������� �����/�����
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

    // ���������� �������������� ���������
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

    // ���������� ������
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

    // ��������� �������� ��� ��������� �������
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

    // ������� ��������� ������� �������� � �������
    void SmoothSpeedTransition()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / smoothTime);
    }

    // �������� ������ �� �������
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
            monitorText.gameObject.SetActive(true); // �������� �����
            monitorText.text = "�������� ���� ���������...";
        }
    }

    // ����� ������������
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
    
    // ������� ��������� ��������
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

    // ����������� �������� �� ��������
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
                speedMonitorText.text = Mathf.Round(currentShowSpeed) + " ��� ��/c";
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
                speedMonitorText.text = Mathf.Round(currentShowSpeed) + " ��� ��/c";
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
                speedMonitorText.text = Mathf.Round(currentShowSpeed) + " ��� ��/c";
                break;
        }

        if (newState != currentState)
        {
            initialShowSpeed = currentShowSpeed;

            currentState = newState;
        }
    }
}
