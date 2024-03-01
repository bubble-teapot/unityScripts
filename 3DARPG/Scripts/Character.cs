using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterController _cc;
    public float moveSpeed = 5f;
    private Vector3 _movementVelocity;
    private PlayerInput _playerInput;
    //垂直的速度方向
    private float _verticalVelocity;
    //重力大小
    public float Gravity = -9.8f;

    private Animator _animator;

    //硬币数
    public int Coin;

    //Health
    private Health _health;

    //Enemy
    public bool IsPlayer = true;
    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    private Transform TargetPlayer;

    //Player slides
    private float _attackStartTime;
    public float AttackSlideDuration = 0.4f;
    public float AttackSlideSpeed = 0.06f;

    //子对象上的DamageCaster脚本
    private DamageCaster _damageCaster;

    //在角色上的冲击方向和力量
    private Vector3 impactOnCharacter;

    //无敌状态
    public bool IsInvincible;
    public float invincivleDuration = 2f;

    //攻击动画持续时间
    private float attackAnimationDuration;

    //滑动，滚动速度
    public float SlideSpeed = 9f;

    //状态枚举
    public enum CharacterState
    {
        Normal,Attacking,Dead,BeingHit,Slide,Spawn
    }
    //当前状态
    public CharacterState CurrentState;

    //生成持续时间
    public float SpawnDuration = 2f;
    //当前生成敌人倒计时
    private float currentSpawnTime;

    //Material animation
    //材质属性块
    private MaterialPropertyBlock _materialPropertyBlock;
    //皮肤渲染器
    private SkinnedMeshRenderer _skinnedMeshRenderer;

    //治疗球
    public GameObject ItemToDrop; 

    private void Awake()
    {
        //获取组件
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        //在孩子节点获取此脚本
        _damageCaster = GetComponentInChildren<DamageCaster>();

        //获取孩子节点的材质渲染器
        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        //获取属性块，记录到_materialPropertyBlock
        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);

        //如果是敌人
        if (!IsPlayer)
        {
            _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            TargetPlayer = GameObject.FindWithTag("Player").transform;
            _navMeshAgent.speed = moveSpeed;
            //初始化状态为 正在在生成
            SwitchStateTo(CharacterState.Spawn);
        }
        else
        {
            _playerInput = GetComponent<PlayerInput>();
        }
    }
    /// <summary>
    /// 计算玩家水平移动向量，和改变动画
    /// </summary>
    private void CalculatePlayerMovement()
    {
        //如果鼠标左键按下，切换攻击状态
        if(_playerInput.MouseButtonDown && _cc.isGrounded)
        {
            SwitchStateTo(CharacterState.Attacking);
            return;
        }else if(_playerInput.SpaceKeyDown&& _cc.isGrounded)//如果按了空格，并且在地上，切换到滑动状态
        {
            SwitchStateTo(CharacterState.Slide);
            return;
        }

        //移动方向赋值
        _movementVelocity.Set(_playerInput.HorizontalInput,0f,_playerInput.VerticalInput);
        //标准化Vector3，防止对角线移动时速度变快。
        _movementVelocity.Normalize();
        //移动方向向量 旋转一下，保持和摄像机的角度一致,y轴旋转-45°
        _movementVelocity = Quaternion.Euler(0, -45f, 0) * _movementVelocity;

        //改变动画的参数，使播放动画,传入方向向量的绝对值
        _animator.SetFloat("Speed", _movementVelocity.magnitude);

        //计算移动速度（带方向），用_movementVelocity保存
        _movementVelocity = _movementVelocity * moveSpeed * Time.deltaTime;

        //控制玩家朝向
        if(_movementVelocity!=Vector3.zero)//当移动时才更改朝向，否则不移动时_movementVelocity为0，会复位朝向
        {
            transform.rotation = Quaternion.LookRotation(_movementVelocity);
        }

        _animator.SetBool("AirBorne", !_cc.isGrounded);
    }
   
    /// <summary>
    /// 计算敌人动作
    /// </summary>
    private void CalculateEnemyMovement()
    {
        //如果玩家和敌人的距离大于_navMeshAgent.stoppingDistance
        if (Vector3.Distance(TargetPlayer.position, transform.position) >= _navMeshAgent.stoppingDistance)
        {
            //设置目标，开始导航
            _navMeshAgent.SetDestination(TargetPlayer.position);
            //_navMeshAgent.SetDestination(new Vector3(10,0,10));
            //播放移动动画
            _animator.SetFloat("Speed", 0.2f);
        }
        else
        {   
            //目标设置成自己
            _navMeshAgent.SetDestination(transform.position);
            _animator.SetFloat("Speed", 0f);

            //切换为攻击状态
            SwitchStateTo(CharacterState.Attacking);
        }
    }
    
    //玩家移动和计算放在物理帧中
    private void FixedUpdate()
    {
        switch (CurrentState)
        {
            case CharacterState.Normal:
                if (IsPlayer)
                {
                    //计算计算玩家水平移动向量
                    CalculatePlayerMovement();
                }
                else
                {   //计算敌人动作
                    CalculateEnemyMovement();
                }
                break;
            case CharacterState.Attacking:
                if (IsPlayer)
                {
                    //如果在攻击滑动时间内，滑动
                    if (Time.time < _attackStartTime + AttackSlideDuration)
                    {
                        //开始攻击，过去的时间
                        float timePassed = Time.time - _attackStartTime;
                        //过去的时间占持续时间的百分比
                        float lerpTime = timePassed / AttackSlideDuration;
                        //给速度进行插值运算，开始速度为transform.forward * AttackSlideSpeed，到速度0
                        _movementVelocity = Vector3.Lerp(transform.forward * AttackSlideSpeed, Vector3.zero, lerpTime);
                    }
                    //如果在攻击状态下，鼠标继续按下，并且玩家在地面，触发连击
                    if (_playerInput.MouseButtonDown && _cc.isGrounded)
                    {
                        //获取当前动画状态时的播放的动画剪辑的名字
                        string currentClipName = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                        //获取当前动画状态的信息的 标准化持续时间[0-1]
                        attackAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                        //如果不是最后一个攻击动作，并且当前动作已经进行了50%到70%，就继续切换到攻击状态
                        if(currentClipName!= "LittleAdventurerAndie_ATTACK_03")
                        {
                            //重置鼠标输入，避免不按也继续连击，SwitchStateTo开始时重置了，此步可以省略
                            //_playerInput.MouseButtonDown = false;
                            SwitchStateTo(CharacterState.Attacking);
                            //使玩家可以在连击时改变方向和位置
                            //CalculatePlayerMovement();
                        }
                    }

                }
                break;
            case CharacterState.BeingHit:

                break;
            case CharacterState.Dead:
                return;
            case CharacterState.Slide:
                //计算滑动移动速度
                _movementVelocity = transform.forward * SlideSpeed * Time.deltaTime;
                break;
            case CharacterState.Spawn:
                //开始倒计时
                currentSpawnTime -= Time.deltaTime;
                //倒计时小于0时，
                if (currentSpawnTime <= 0)
                {
                    SwitchStateTo(CharacterState.Normal);
                }
                break;
        }
        //被击中
        //如果方向力量向量的模>0.2f
        if (impactOnCharacter.magnitude > 0.2f)
        {
            //给移动向量赋值
            _movementVelocity = impactOnCharacter * Time.deltaTime;
        }
        //插值运算（变形），逐渐减小到0，减小速率先快后慢的曲线
        impactOnCharacter = Vector3.Lerp(impactOnCharacter, Vector3.zero, Time.deltaTime * 5);


        if (IsPlayer)
        {
            //如果角色不在地面
            if (_cc.isGrounded == false)
            {   //施加100%重力
                _verticalVelocity = Gravity;
            }
            else
            {   //在地面就给30%重力
                _verticalVelocity = Gravity * 0.3f;
            }
            //水平移动向量+重力方向移动向量
            _movementVelocity += _verticalVelocity * Vector3.up * Time.deltaTime;

            //使用CharacterController移动玩家
            _cc.Move(_movementVelocity);

            //停止移动,重置移速
            _movementVelocity = Vector3.zero;
        }
        else
        {
            //使用CharacterController移动玩家
            //_cc.Move(_movementVelocity);

            //停止移动,重置移速
            //_movementVelocity = Vector3.zero;
        }

    }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="newState"></param>
    public void SwitchStateTo(CharacterState newState)
    {
        if (IsPlayer)
        {
            //清除按键记录
            _playerInput.ClearCache();
        }


        //Exiting state
        switch (CurrentState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                //防止动画没到那个帧事件，手动再禁止一次
                if (_damageCaster != null)
                {
                    DisableDamageCaster();
                }
                //退出攻击状态时，如果是忘记，就停止所有攻击特效
                if (IsPlayer)
                {
                    GetComponent<PlayerVFXManager>().StopBlade();
                }
                break;
            case CharacterState.BeingHit:
                break;
            case CharacterState.Dead:
                return;
            case CharacterState.Slide:
                break;
            case CharacterState.Spawn:
                //退出生成状态时，无敌失效
                IsInvincible = false;
                break;
        }
        //Entering state
        switch (newState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                //如果是敌人
                if (!IsPlayer)
                {
                    //计算旋转的四元数 参数为 方向向量（目标位置-自己位置=朝向目标位置的向量）
                    Quaternion newRotation = Quaternion.LookRotation(TargetPlayer.position-transform.position);
                    transform.rotation = newRotation;
                }

                _animator.SetTrigger("Attack");

                if(IsPlayer)
                {
                    //记录攻击开始的时间
                    _attackStartTime = Time.time;
                }
                break;
            case CharacterState.Dead:
                //禁止移动
                _cc.enabled = false;
                //播放死亡动画
                _animator.SetTrigger("Dead");
                //尸体溶解
                StartCoroutine(MaterialDissolve());
                break;
            case CharacterState.BeingHit:
                _animator.SetTrigger("BeingHit");
                //如果是玩家就设置无敌，然后延迟失效
                if (IsPlayer)
                {
                    IsInvincible = true;
                    StartCoroutine(DelayCancelInvincible());
                }
                break;
            case CharacterState.Slide:
                _animator.SetTrigger("Slide");
                break;
            case CharacterState.Spawn:
                //生成敌人状态时，敌人设置为无敌
                IsInvincible = true;
                //倒计时初始化
                currentSpawnTime = SpawnDuration;
                //启动敌人浮现的协程
                StartCoroutine(MaterialAppear());
                break;
        }
        //完成切换，记录当前状态为新状态
        CurrentState = newState;

        //test
        Debug.Log(IsPlayer+"SwitchState To" + CurrentState);
    }
    /// <summary>
    /// 滑动动画结束帧事件，切换回Normal状态
    /// </summary>
    public void SlideAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    /// <summary>
    /// Combo 01动画结束事件函数
    /// </summary>
    public void AttackAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

     public void BeingHitAnimationEnds()
     {
        SwitchStateTo(CharacterState.Normal);
     }

    /// <summary>
    /// 作为中转，实际调用Health的方法
    /// </summary>
    /// <param name="damege"></param>
    /// <param name="attackerPos">定位撞击点</param>
    public void ApplyDamage(int damege,Vector3 attackerPos=new Vector3())
    {
        //如果无敌，返回
        if (IsInvincible)
        {
            return;
        }
        if (_health!=null)
        {
            //调用health脚本的受伤的方法
            _health.ApplyDamage(damege);
        }
        //如果本角色不是玩家
        if (!IsPlayer)
        {
            //获取并调用EnemyVFXManager的受击函数
            GetComponent<EnemyVFXManeger>().PlayBeingHitVFX(attackerPos);
        }

        //开启协程，在闪烁0.2秒
        StartCoroutine(MaterialBlink());

        //如果是玩家，进入BeingHit状态，敌人不受影响
        if (IsPlayer)
        {
            SwitchStateTo(CharacterState.BeingHit);
            AddImpact(attackerPos, 10f);
        }
        else
        {
            AddImpact(attackerPos, 2.5f);
        }
    }

    /// <summary>
    /// 延迟取消无敌
    /// </summary>
    /// <returns></returns>
    IEnumerator DelayCancelInvincible()
    {
        yield return new WaitForSeconds(invincivleDuration);
        IsInvincible = false;
    }

    /// <summary>
    /// 计算impactOnCharacter
    /// </summary>
    /// <param name="attackerPos"></param>
    /// <param name="force"></param>
    private void AddImpact(Vector3 attackerPos,float force)
    {
        //冲击方向
        Vector3 impactDir = transform.position - attackerPos;
        impactDir.Normalize();
        impactDir.y = 0;
        impactOnCharacter = impactDir * force;
    }

    /// <summary>
    /// 作为动画帧的事件，启动DamageCaster
    /// </summary>
    public void EnableDamageCaster()
    {
        //防止敌人没有_damageCaster
        if (_damageCaster!=null)
            _damageCaster.EnableDamageCaster();
    }

    /// <summary>
    /// 作为动画帧的事件
    /// </summary>
    public void DisableDamageCaster()
    {
        //防止没有_damageCaster
        if (_damageCaster != null)
            _damageCaster.DisableDamageCaster();
    }

    /// <summary>
    /// 闪烁0.2秒的协程函数
    /// </summary>
    /// <returns></returns>
    IEnumerator MaterialBlink()
    {
        _materialPropertyBlock.SetFloat("_blink", 0.4f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
        yield return new WaitForSeconds(0.2f);
        _materialPropertyBlock.SetFloat("_blink", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }
    /// <summary>
    /// 等待2秒后开始渐渐溶解
    /// </summary>
    /// <returns></returns>
    IEnumerator MaterialDissolve()
    {
        yield return new WaitForSeconds(2f);

        float dissloveTimeDuration = 2f;
        float currentDissolveTime = 0;
        float dissolveHight_start = 20f;
        float dissolveHight_target = -10f;
        float dissolveHight;

        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        while (currentDissolveTime < dissloveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            //修改溶解值,插值运算(开始值，目标值，百分比)，得到当前时间的溶解值
            dissolveHight = Mathf.Lerp(dissolveHight_start, dissolveHight_target, currentDissolveTime / dissloveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            //停止一帧，时间就过去了Time.deltaTime
            yield return null;
        }
        //掉出一个东西
        DropItem();
    }
    /// <summary>
    /// 掉出一个物品
    /// </summary>
    public void DropItem()
    {
        if (ItemToDrop != null)
        {
            Instantiate(ItemToDrop, transform.position, Quaternion.identity);
        }
    }

    public void PickUpItem(PickUp item)
    {
        switch (item.type)
        {
            case PickUp.PickUpType.Heal:
                AddHealth(item.value);
                break;
            case PickUp.PickUpType.Coin:
                AddCoin(item.value);
                break;
        }
    }
    private void AddHealth(int health)
    {
        _health.AddHealth(health);
        //播放回血特效
        GetComponent<PlayerVFXManager>().PlayHealVFX();
    }
    private void AddCoin(int coin)
    {
        Coin += coin;
    }

    /// <summary>
    /// 水平转向目标
    /// </summary>
    public void RotateToTarget()
    {
        if (CurrentState != CharacterState.Dead)
        {
            transform.LookAt(TargetPlayer, Vector3.up);
        }
    }
    /// <summary>
    /// 材料浮现，敌人生成时调用
    /// </summary>
    /// <returns></returns>
    IEnumerator MaterialAppear()
    {
        //记录浮现总时长
        float dissolveTimeDuration = SpawnDuration;
        //记录当前浮现过去的时长
        float currentDissolveTime = 0;
        float dissolveHight_start = -10f;
        float dissolveHight_target = 20f;
        float dissoleHight;

        //开启皮肤可溶解的选项
        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
        while (currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissoleHight = Mathf.Lerp(dissolveHight_start, dissolveHight_target, currentDissolveTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissoleHight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }
        //关闭皮肤可溶解的选项
        _materialPropertyBlock.SetFloat("_enableDissolve", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }
}
