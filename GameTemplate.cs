using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using HandyUtilities;

public abstract class GameTemplate : MonoBehaviour
{
    public bool wipeSave = false;
    public string storeURL;
    public string removeAdsProductID = "com.test.removeads";
    public string shareText = "I scored {0} in Zig Zag Rush! -place link here-";
    public string faceBookPage = "www.facebook.com";
 
    [SerializeField]
    UIManager m_UIManager;
    [SerializeField]
    SoundManager m_soundManager;
#if UNITY_PURCHASER
    [SerializeField]
    Purchaser m_purchaser;
#endif

    // Events:

    [SerializeField]
    UnityEvent m_onGameOver;
    [SerializeField]
    UnityEvent m_onRunStart;
    [SerializeField]
    UnityEvent m_onRetryPressed;
    [SerializeField]
    UnityEvent m_onGameLoaded;
    [SerializeField]
    NumEvent m_onNewRecord = new NumEvent();
    [SerializeField]
    NumEvent m_onCoinAmountChange = new NumEvent();
    [SerializeField]
    NumEvent m_onScoreChange = new NumEvent();


    // Private members:

    UserData m_userData;
    protected static GameTemplate m_instance;
    protected int m_score;
    protected bool m_secondLifeUsed;
    protected const string USER_DATA_PREFS_KEY = "userdata";

    // Puplic properties:
#if UNITY_PURCHASER
    public static Purchaser purchaser { get { return m_instance.m_purchaser; } }
#endif

    public static UserData userData { get { return m_instance.m_userData; } }
    public static SoundManager sound { get { return m_instance.m_soundManager; } }
    public static UnityEvent onGameOver { get { return m_instance.m_onGameOver; } }
    public static NumEvent onNewRecord { get { return m_instance.m_onNewRecord; } }
    public static NumEvent onCoinAmountChange { get { return m_instance.m_onCoinAmountChange; } }
    public static NumEvent onScoreChange { get { return m_instance.m_onScoreChange; } }
    public static UnityEvent onRunStart { get { return m_instance.m_onRunStart; } }
    public static UnityEvent onContinue { get { return m_instance.m_onRetryPressed; } }
    public static UnityEvent onGameLoaded { get { return m_instance.m_onGameLoaded; } }
    public static UIManager UI { get { return m_instance.m_UIManager; } }
    public static GameTemplate instance { get { return m_instance; } }
    public static string screenShotPath { get; private set; }

    public static int score
    {
        get { return m_instance.m_score; }
        set
        {
            m_instance.m_score = value;
            onScoreChange.Invoke(value);
        }
    }

    public static int coins
    {
        get { return userData.coins; }
        set
        {
            userData.coins = value;
            onCoinAmountChange.Invoke(value);
        }
    }


    public virtual void Init()
    {
        Application.targetFrameRate = 60;
        if (wipeSave)
            PlayerPrefs.DeleteAll();
        m_instance = this;
        LoadSave();
        LoadComponents();
        onGameLoaded.Invoke();
        userData.attemptsCount++;
        UI.Init();
        sound.Init();

        Social.localUser.Authenticate(OnAuthenticate);
    }

    void OnAuthenticate(bool success)
    {

    }

    public virtual void LoadComponents()
    {
#if UNITY_PURCHASER
        m_purchaser.Init();
#endif
        m_UIManager.Init();
        m_soundManager.Init();
    }

    public static void TakeScreenshot()
    {
        screenShotPath = Application.persistentDataPath + "/screenshot.png";
        Application.CaptureScreenshot("screenshot.png");
    }

    public virtual void Continue()
    {
        var s = score;
        m_instance.m_secondLifeUsed = true;
        onContinue.Invoke();
        score = s;
    }

    public virtual void Pause()
    {
        Time.timeScale = 0;
    }

    public virtual void Resume()
    {
        Time.timeScale = 1;
    }

    public virtual void StartRun()
    {
        userData.attemptsCount++;
        m_instance.m_secondLifeUsed = false;
        onRunStart.Invoke();
    }

    public virtual void LoadSave()
    {
        if (PlayerPrefs.HasKey(USER_DATA_PREFS_KEY))
        {
            m_instance.m_userData = JsonUtility.FromJson<UserData>(PlayerPrefs.GetString(USER_DATA_PREFS_KEY));
        }
        else
        {
            m_instance.m_userData = new UserData();
        }

    }

    public virtual void Mute(bool mute)
    {
        userData.mute = mute;
        SaveGame();
    }

    public virtual bool IsCharacterLocked(int index)
    {
        return userData.characterLockData[index];
    }

    public virtual void CheckHighScore(int record)
    {
        if (userData.highscore < record)
        {
            onNewRecord.Invoke(record);
            userData.highscore = record;
            SaveGame();
        }
    }

    public virtual void SaveGame()
    {
        PlayerPrefs.SetString(USER_DATA_PREFS_KEY, JsonUtility.ToJson(m_instance.m_userData));
        PlayerPrefs.Save();
    }

   
    [System.Serializable]
    public class NumEvent : UnityEvent<int>
    {
        public NumEvent() { }
    }
}

public interface ICharacter
{
    int price { get; }
    ICharacter prefab { get; }
    Sprite icon { get; }
    string name { get; }
    bool unlocked { get; set; }
    int index { get; set; }
    void Spawn();
    GameObject gameObject { get; }
}
