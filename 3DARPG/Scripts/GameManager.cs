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
        //ͨ����ǩѰ����ҽ�ɫ
        playerCharacter = GameObject.FindWithTag("Player").GetComponent<Character>();
    }
    private void Update()
    {
        if (gameIsOver) return;
        
        //�������esc�͵�����ͣUI
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameUIManager.TogglePauseUI();
        }
        //������״̬ΪDead����Ϸ����
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
    /// ��spwner��unity�¼��е���,��Ϸ���
    /// </summary>
    public void GameIsFinished()
    {
        gameUIManager.ShowGameIsFinishedUI();
    }
    /// <summary>
    /// ������UIManager����
    /// </summary>
    public void ReturnToTheMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    /// <summary>
    /// ������UIManager����
    /// </summary>
    public void Restart()
    {
        //���س�������õ�ǰ����ĳ�����name��
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
