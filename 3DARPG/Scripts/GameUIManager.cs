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

    //游戏UI状态枚举
    private enum GameUI_State
    {
        GamePlay,Pause,GameOver,GameIsFinished
    }

    //当前UI状态
    private GameUI_State currentState;

    private void Start()
    {
        //防止切换场景出现问题：从打开菜单界面的暂停Time.timeScale=0，重新回复到Time正常状态
        SwitchUIState(GameUI_State.GamePlay);
    }

    void Update()
    {
        //把slider的值，更新为玩家血量百分比
        HealthSlider.value = GM.playerCharacter.GetComponent<Health>().CurrentHealthPercentage;
        //text的类型是字符串类型
        CoinText.text = GM.playerCharacter.Coin.ToString();
    }

    /// <summary>
    /// 切换状态，并做相应操作
    /// </summary>
    /// <param name="state">切换后的状态</param>
    private void SwitchUIState(GameUI_State state)
    {
        //禁止全部UI对象
        UI_Pause.SetActive(false);
        UI_GameOver.SetActive(false);
        UI_GameIsFinished.SetActive(false);

        //初始化时间速度为正常
        Time.timeScale = 1f;
        switch (state)
        {
            case GameUI_State.GamePlay:
                break;
            case GameUI_State.Pause:
                //时间暂停
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
    /// 用于切换显示隐藏Pause的UI
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
    /// 菜单按钮调用事件
    /// </summary>
    public void Button_MainMenu()
    {
        GM.ReturnToTheMainMenu();
    }
    /// <summary>
    /// 菜单按钮调用事件
    /// </summary>
    public void Button_Restart()
    {
        GM.Restart();
    }
    /// <summary>
    /// 用来给游戏管理器调用
    /// </summary>
    public void ShowGameOverUI()
    {
        SwitchUIState(GameUI_State.GameOver);
    }
    /// <summary>
    /// 用来给游戏管理器调用
    /// </summary>
    public void ShowGameIsFinishedUI()
    {
        SwitchUIState(GameUI_State.GameIsFinished);
    }
}
