using UnityEngine;

public class AmbientSoundManager : MonoBehaviour
{
    private static AmbientSoundManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ���������� ������� ��� ����� �����
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
