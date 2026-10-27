using UnityEngine;
using UnityEngine.InputSystem;

public class CheatManager : MonoBehaviour
{
    public static CheatManager Instance { get; private set; }
    public int cheatCoinAmount = 10;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
        {
            GiveCheatCoins();
        }
    }

    void GiveCheatCoins()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCapsules(cheatCoinAmount);
        }
    }
}