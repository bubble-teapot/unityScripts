using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float HorizontalInput;
    public float VerticalInput;

    //可更改，此处更改为按键，和变量名无关
    public bool MouseButtonDown;

    public bool SpaceKeyDown;
    private void Update()
    {
        ////如果MouseButtonDown为false，并且时间没有暂停
        //if (!MouseButtonDown && Time.timeScale != 0)
        //{
        //    //获取鼠标左键是否按下
        //    MouseButtonDown = Input.GetMouseButtonDown(0);
        //}

        //更改上面的输入
        //如果MouseButtonDown为false，并且时间没有暂停
        if (!MouseButtonDown && Time.timeScale != 0)
        {
            //获取j键是否按下
            MouseButtonDown = Input.GetKeyDown(KeyCode.J);
        }

        //获取空格是否按下,当空格没按下并且游戏没暂停
        if (!SpaceKeyDown && Time.timeScale != 0)
        {
            SpaceKeyDown = Input.GetKeyDown(KeyCode.Space);
        }

        //获取水平和垂直输入，范围为：-1 0 1
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");

    }


    private void OnDisable()
    {
        //重置输入值，防止下次启动时自动移动，当角色死亡时，我们无法移动
        ClearCache();
    }
    /// <summary>
    /// 重置输入缓存
    /// </summary>
    public void ClearCache()
    {
        //重置输入值
        HorizontalInput = 0;
        VerticalInput = 0;

        MouseButtonDown = false;
        SpaceKeyDown = false;
    }
}
