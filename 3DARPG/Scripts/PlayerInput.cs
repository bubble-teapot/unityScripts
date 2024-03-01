using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float HorizontalInput;
    public float VerticalInput;

    //�ɸ��ģ��˴�����Ϊ�������ͱ������޹�
    public bool MouseButtonDown;

    public bool SpaceKeyDown;
    private void Update()
    {
        ////���MouseButtonDownΪfalse������ʱ��û����ͣ
        //if (!MouseButtonDown && Time.timeScale != 0)
        //{
        //    //��ȡ�������Ƿ���
        //    MouseButtonDown = Input.GetMouseButtonDown(0);
        //}

        //�������������
        //���MouseButtonDownΪfalse������ʱ��û����ͣ
        if (!MouseButtonDown && Time.timeScale != 0)
        {
            //��ȡj���Ƿ���
            MouseButtonDown = Input.GetKeyDown(KeyCode.J);
        }

        //��ȡ�ո��Ƿ���,���ո�û���²�����Ϸû��ͣ
        if (!SpaceKeyDown && Time.timeScale != 0)
        {
            SpaceKeyDown = Input.GetKeyDown(KeyCode.Space);
        }

        //��ȡˮƽ�ʹ�ֱ���룬��ΧΪ��-1 0 1
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");

    }


    private void OnDisable()
    {
        //��������ֵ����ֹ�´�����ʱ�Զ��ƶ�������ɫ����ʱ�������޷��ƶ�
        ClearCache();
    }
    /// <summary>
    /// �������뻺��
    /// </summary>
    public void ClearCache()
    {
        //��������ֵ
        HorizontalInput = 0;
        VerticalInput = 0;

        MouseButtonDown = false;
        SpaceKeyDown = false;
    }
}
