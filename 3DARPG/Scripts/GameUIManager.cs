using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public GameManager GM;
    //TMPro.TextMeshProUGUI 
    public TextMeshProUGUI CoinText;
    //UnityEngine.UI.Slider
    public Slider HealthSlider;

    public GameObject UI_Pause;
    public GameObject UI_GameOver;
    public GameObject UI_GameIsFinished;

    //��ϷUI״̬ö��
    private enum GameUI_State
    {
        GamePlay,Pause,GameOver,GameIsFinished
    }

    //��ǰUI״̬
    private GameUI_State currentState;

    private void Start()
    {
        //��ֹ�л������������⣺�Ӵ򿪲˵��������ͣTime.timeScale=0�����»ظ���Time����״̬
        SwitchUIState(GameUI_State.GamePlay);
    }

    void Update()
    {
        //��slider��ֵ������Ϊ���Ѫ���ٷֱ�
        HealthSlider.value = GM.playerCharacter.GetComponent<Health>().CurrentHealthPercentage;
        //text���������ַ�������
        CoinText.text = GM.playerCharacter.Coin.ToString();
    }

    /// <summary>
    /// �л�״̬��������Ӧ����
    /// </summary>
    /// <param name="state">�л����״̬</param>
    private void SwitchUIState(GameUI_State state)
    {
        //��ֹȫ��UI����
        UI_Pause.SetActive(false);
        UI_GameOver.SetActive(false);
        UI_GameIsFinished.SetActive(false);

        //��ʼ��ʱ���ٶ�Ϊ����
        Time.timeScale = 1f;
        switch (state)
        {
            case GameUI_State.GamePlay:
                break;
            case GameUI_State.Pause:
                //ʱ����ͣ
                Time.timeScale = 0;
                UI_Pause.SetActive(true);
                break;
            case GameUI_State.GameIsFinished:
                UI_GameIsFinished.SetActive(true);
                break;
            case GameUI_State.GameOver:
                UI_GameOver.SetActive(true);
                break;
        }
        currentState = state;
    }
    /// <summary>
    /// �����л���ʾ����Pause��UI
    /// </summary>
    public void TogglePauseUI()
    {
        if (currentState == GameUI_State.GamePlay)
        {
            SwitchUIState(GameUI_State.Pause);
        }else if (currentState == GameUI_State.Pause)
        {
            SwitchUIState(GameUI_State.GamePlay);
        }
    }
    /// <summary>
    /// �˵���ť�����¼�
    /// </summary>
    public void Button_MainMenu()
    {
        GM.ReturnToTheMainMenu();
    }
    /// <summary>
    /// �˵���ť�����¼�
    /// </summary>
    public void Button_Restart()
    {
        GM.Restart();
    }
    /// <summary>
    /// ��������Ϸ����������
    /// </summary>
    public void ShowGameOverUI()
    {
        SwitchUIState(GameUI_State.GameOver);
    }
    /// <summary>
    /// ��������Ϸ����������
    /// </summary>
    public void ShowGameIsFinishedUI()
    {
        SwitchUIState(GameUI_State.GameIsFinished);
    }
}
