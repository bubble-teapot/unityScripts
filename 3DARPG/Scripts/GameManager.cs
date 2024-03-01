using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameUIManager gameUIManager;
    public Character playerCharacter;
    private bool gameIsOver;
    private void Awake()
    {
        //通过标签寻找玩家角色
        playerCharacter = GameObject.FindWithTag("Player").GetComponent<Character>();
    }
    private void Update()
    {
        if (gameIsOver) return;
        
        //如果按下esc就弹出暂停UI
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameUIManager.TogglePauseUI();
        }
        //如果玩家状态为Dead，游戏结束
        if (playerCharacter.CurrentState == Character.CharacterState.Dead)
        {
            gameIsOver = true;
            GameOver();
        }
    }

    private void GameOver()
    {
        gameUIManager.ShowGameOverUI();
    }
    /// <summary>
    /// 在spwner的unity事件中调用,游戏完成
    /// </summary>
    public void GameIsFinished()
    {
        gameUIManager.ShowGameIsFinishedUI();
    }
    /// <summary>
    /// 用来给UIManager调用
    /// </summary>
    public void ReturnToTheMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    /// <summary>
    /// 用来给UIManager调用
    /// </summary>
    public void Restart()
    {
        //加载场景（获得当前激活的场景的name）
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
