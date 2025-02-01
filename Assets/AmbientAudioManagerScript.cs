using UnityEngine;

public class AmbientSoundManager : MonoBehaviour
{
    private static AmbientSoundManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Сохранение объекта при смене сцены
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
