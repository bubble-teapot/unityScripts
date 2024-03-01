using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterController _cc;
    public float moveSpeed = 5f;
    private Vector3 _movementVelocity;
    private PlayerInput _playerInput;
    //��ֱ���ٶȷ���
    private float _verticalVelocity;
    //������С
    public float Gravity = -9.8f;

    private Animator _animator;

    //Ӳ����
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

    //�Ӷ����ϵ�DamageCaster�ű�
    private DamageCaster _damageCaster;

    //�ڽ�ɫ�ϵĳ�����������
    private Vector3 impactOnCharacter;

    //�޵�״̬
    public bool IsInvincible;
    public float invincivleDuration = 2f;

    //������������ʱ��
    private float attackAnimationDuration;

    //�����������ٶ�
    public float SlideSpeed = 9f;

    //״̬ö��
    public enum CharacterState
    {
        Normal,Attacking,Dead,BeingHit,Slide,Spawn
    }
    //��ǰ״̬
    public CharacterState CurrentState;

    //���ɳ���ʱ��
    public float SpawnDuration = 2f;
    //��ǰ���ɵ��˵���ʱ
    private float currentSpawnTime;

    //Material animation
    //�������Կ�
    private MaterialPropertyBlock _materialPropertyBlock;
    //Ƥ����Ⱦ��
    private SkinnedMeshRenderer _skinnedMeshRenderer;

    //������
    public GameObject ItemToDrop; 

    private void Awake()
    {
        //��ȡ���
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        //�ں��ӽڵ��ȡ�˽ű�
        _damageCaster = GetComponentInChildren<DamageCaster>();

        //��ȡ���ӽڵ�Ĳ�����Ⱦ��
        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        //��ȡ���Կ飬��¼��_materialPropertyBlock
        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);

        //����ǵ���
        if (!IsPlayer)
        {
            _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            TargetPlayer = GameObject.FindWithTag("Player").transform;
            _navMeshAgent.speed = moveSpeed;
            //��ʼ��״̬Ϊ ����������
            SwitchStateTo(CharacterState.Spawn);
        }
        else
        {
            _playerInput = GetComponent<PlayerInput>();
        }
    }
    /// <summary>
    /// �������ˮƽ�ƶ��������͸ı䶯��
    /// </summary>
    private void CalculatePlayerMovement()
    {
        //������������£��л�����״̬
        if(_playerInput.MouseButtonDown && _cc.isGrounded)
        {
            SwitchStateTo(CharacterState.Attacking);
            return;
        }else if(_playerInput.SpaceKeyDown&& _cc.isGrounded)//������˿ո񣬲����ڵ��ϣ��л�������״̬
        {
            SwitchStateTo(CharacterState.Slide);
            return;
        }

        //�ƶ�����ֵ
        _movementVelocity.Set(_playerInput.HorizontalInput,0f,_playerInput.VerticalInput);
        //��׼��Vector3����ֹ�Խ����ƶ�ʱ�ٶȱ�졣
        _movementVelocity.Normalize();
        //�ƶ��������� ��תһ�£����ֺ�������ĽǶ�һ��,y����ת-45��
        _movementVelocity = Quaternion.Euler(0, -45f, 0) * _movementVelocity;

        //�ı䶯���Ĳ�����ʹ���Ŷ���,���뷽�������ľ���ֵ
        _animator.SetFloat("Speed", _movementVelocity.magnitude);

        //�����ƶ��ٶȣ������򣩣���_movementVelocity����
        _movementVelocity = _movementVelocity * moveSpeed * Time.deltaTime;

        //������ҳ���
        if(_movementVelocity!=Vector3.zero)//���ƶ�ʱ�Ÿ��ĳ��򣬷����ƶ�ʱ_movementVelocityΪ0���Ḵλ����
        {
            transform.rotation = Quaternion.LookRotation(_movementVelocity);
        }

        _animator.SetBool("AirBorne", !_cc.isGrounded);
    }
   
    /// <summary>
    /// ������˶���
    /// </summary>
    private void CalculateEnemyMovement()
    {
        //�����Һ͵��˵ľ������_navMeshAgent.stoppingDistance
        if (Vector3.Distance(TargetPlayer.position, transform.position) >= _navMeshAgent.stoppingDistance)
        {
            //����Ŀ�꣬��ʼ����
            _navMeshAgent.SetDestination(TargetPlayer.position);
            //_navMeshAgent.SetDestination(new Vector3(10,0,10));
            //�����ƶ�����
            _animator.SetFloat("Speed", 0.2f);
        }
        else
        {   
            //Ŀ�����ó��Լ�
            _navMeshAgent.SetDestination(transform.position);
            _animator.SetFloat("Speed", 0f);

            //�л�Ϊ����״̬
            SwitchStateTo(CharacterState.Attacking);
        }
    }
    
    //����ƶ��ͼ����������֡��
    private void FixedUpdate()
    {
        switch (CurrentState)
        {
            case CharacterState.Normal:
                if (IsPlayer)
                {
                    //����������ˮƽ�ƶ�����
                    CalculatePlayerMovement();
                }
                else
                {   //������˶���
                    CalculateEnemyMovement();
                }
                break;
            case CharacterState.Attacking:
                if (IsPlayer)
                {
                    //����ڹ�������ʱ���ڣ�����
                    if (Time.time < _attackStartTime + AttackSlideDuration)
                    {
                        //��ʼ��������ȥ��ʱ��
                        float timePassed = Time.time - _attackStartTime;
                        //��ȥ��ʱ��ռ����ʱ��İٷֱ�
                        float lerpTime = timePassed / AttackSlideDuration;
                        //���ٶȽ��в�ֵ���㣬��ʼ�ٶ�Ϊtransform.forward * AttackSlideSpeed�����ٶ�0
                        _movementVelocity = Vector3.Lerp(transform.forward * AttackSlideSpeed, Vector3.zero, lerpTime);
                    }
                    //����ڹ���״̬�£����������£���������ڵ��棬��������
                    if (_playerInput.MouseButtonDown && _cc.isGrounded)
                    {
                        //��ȡ��ǰ����״̬ʱ�Ĳ��ŵĶ�������������
                        string currentClipName = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                        //��ȡ��ǰ����״̬����Ϣ�� ��׼������ʱ��[0-1]
                        attackAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                        //����������һ���������������ҵ�ǰ�����Ѿ�������50%��70%���ͼ����л�������״̬
                        if(currentClipName!= "LittleAdventurerAndie_ATTACK_03")
                        {
                            //����������룬���ⲻ��Ҳ����������SwitchStateTo��ʼʱ�����ˣ��˲�����ʡ��
                            //_playerInput.MouseButtonDown = false;
                            SwitchStateTo(CharacterState.Attacking);
                            //ʹ��ҿ���������ʱ�ı䷽���λ��
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
                //���㻬���ƶ��ٶ�
                _movementVelocity = transform.forward * SlideSpeed * Time.deltaTime;
                break;
            case CharacterState.Spawn:
                //��ʼ����ʱ
                currentSpawnTime -= Time.deltaTime;
                //����ʱС��0ʱ��
                if (currentSpawnTime <= 0)
                {
                    SwitchStateTo(CharacterState.Normal);
                }
                break;
        }
        //������
        //�����������������ģ>0.2f
        if (impactOnCharacter.magnitude > 0.2f)
        {
            //���ƶ�������ֵ
            _movementVelocity = impactOnCharacter * Time.deltaTime;
        }
        //��ֵ���㣨���Σ����𽥼�С��0����С�����ȿ����������
        impactOnCharacter = Vector3.Lerp(impactOnCharacter, Vector3.zero, Time.deltaTime * 5);


        if (IsPlayer)
        {
            //�����ɫ���ڵ���
            if (_cc.isGrounded == false)
            {   //ʩ��100%����
                _verticalVelocity = Gravity;
            }
            else
            {   //�ڵ���͸�30%����
                _verticalVelocity = Gravity * 0.3f;
            }
            //ˮƽ�ƶ�����+���������ƶ�����
            _movementVelocity += _verticalVelocity * Vector3.up * Time.deltaTime;

            //ʹ��CharacterController�ƶ����
            _cc.Move(_movementVelocity);

            //ֹͣ�ƶ�,��������
            _movementVelocity = Vector3.zero;
        }
        else
        {
            //ʹ��CharacterController�ƶ����
            //_cc.Move(_movementVelocity);

            //ֹͣ�ƶ�,��������
            //_movementVelocity = Vector3.zero;
        }

    }

    /// <summary>
    /// �л�״̬
    /// </summary>
    /// <param name="newState"></param>
    public void SwitchStateTo(CharacterState newState)
    {
        if (IsPlayer)
        {
            //���������¼
            _playerInput.ClearCache();
        }


        //Exiting state
        switch (CurrentState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                //��ֹ����û���Ǹ�֡�¼����ֶ��ٽ�ֹһ��
                if (_damageCaster != null)
                {
                    DisableDamageCaster();
                }
                //�˳�����״̬ʱ����������ǣ���ֹͣ���й�����Ч
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
                //�˳�����״̬ʱ���޵�ʧЧ
                IsInvincible = false;
                break;
        }
        //Entering state
        switch (newState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                //����ǵ���
                if (!IsPlayer)
                {
                    //������ת����Ԫ�� ����Ϊ ����������Ŀ��λ��-�Լ�λ��=����Ŀ��λ�õ�������
                    Quaternion newRotation = Quaternion.LookRotation(TargetPlayer.position-transform.position);
                    transform.rotation = newRotation;
                }

                _animator.SetTrigger("Attack");

                if(IsPlayer)
                {
                    //��¼������ʼ��ʱ��
                    _attackStartTime = Time.time;
                }
                break;
            case CharacterState.Dead:
                //��ֹ�ƶ�
                _cc.enabled = false;
                //������������
                _animator.SetTrigger("Dead");
                //ʬ���ܽ�
                StartCoroutine(MaterialDissolve());
                break;
            case CharacterState.BeingHit:
                _animator.SetTrigger("BeingHit");
                //�������Ҿ������޵У�Ȼ���ӳ�ʧЧ
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
                //���ɵ���״̬ʱ����������Ϊ�޵�
                IsInvincible = true;
                //����ʱ��ʼ��
                currentSpawnTime = SpawnDuration;
                //�������˸��ֵ�Э��
                StartCoroutine(MaterialAppear());
                break;
        }
        //����л�����¼��ǰ״̬Ϊ��״̬
        CurrentState = newState;

        //test
        Debug.Log(IsPlayer+"SwitchState To" + CurrentState);
    }
    /// <summary>
    /// ������������֡�¼����л���Normal״̬
    /// </summary>
    public void SlideAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    /// <summary>
    /// Combo 01���������¼�����
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
    /// ��Ϊ��ת��ʵ�ʵ���Health�ķ���
    /// </summary>
    /// <param name="damege"></param>
    /// <param name="attackerPos">��λײ����</param>
    public void ApplyDamage(int damege,Vector3 attackerPos=new Vector3())
    {
        //����޵У�����
        if (IsInvincible)
        {
            return;
        }
        if (_health!=null)
        {
            //����health�ű������˵ķ���
            _health.ApplyDamage(damege);
        }
        //�������ɫ�������
        if (!IsPlayer)
        {
            //��ȡ������EnemyVFXManager���ܻ�����
            GetComponent<EnemyVFXManeger>().PlayBeingHitVFX(attackerPos);
        }

        //����Э�̣�����˸0.2��
        StartCoroutine(MaterialBlink());

        //�������ң�����BeingHit״̬�����˲���Ӱ��
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
    /// �ӳ�ȡ���޵�
    /// </summary>
    /// <returns></returns>
    IEnumerator DelayCancelInvincible()
    {
        yield return new WaitForSeconds(invincivleDuration);
        IsInvincible = false;
    }

    /// <summary>
    /// ����impactOnCharacter
    /// </summary>
    /// <param name="attackerPos"></param>
    /// <param name="force"></param>
    private void AddImpact(Vector3 attackerPos,float force)
    {
        //�������
        Vector3 impactDir = transform.position - attackerPos;
        impactDir.Normalize();
        impactDir.y = 0;
        impactOnCharacter = impactDir * force;
    }

    /// <summary>
    /// ��Ϊ����֡���¼�������DamageCaster
    /// </summary>
    public void EnableDamageCaster()
    {
        //��ֹ����û��_damageCaster
        if (_damageCaster!=null)
            _damageCaster.EnableDamageCaster();
    }

    /// <summary>
    /// ��Ϊ����֡���¼�
    /// </summary>
    public void DisableDamageCaster()
    {
        //��ֹû��_damageCaster
        if (_damageCaster != null)
            _damageCaster.DisableDamageCaster();
    }

    /// <summary>
    /// ��˸0.2���Э�̺���
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
    /// �ȴ�2���ʼ�����ܽ�
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
            //�޸��ܽ�ֵ,��ֵ����(��ʼֵ��Ŀ��ֵ���ٷֱ�)���õ���ǰʱ����ܽ�ֵ
            dissolveHight = Mathf.Lerp(dissolveHight_start, dissolveHight_target, currentDissolveTime / dissloveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            //ֹͣһ֡��ʱ��͹�ȥ��Time.deltaTime
            yield return null;
        }
        //����һ������
        DropItem();
    }
    /// <summary>
    /// ����һ����Ʒ
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
        //���Ż�Ѫ��Ч
        GetComponent<PlayerVFXManager>().PlayHealVFX();
    }
    private void AddCoin(int coin)
    {
        Coin += coin;
    }

    /// <summary>
    /// ˮƽת��Ŀ��
    /// </summary>
    public void RotateToTarget()
    {
        if (CurrentState != CharacterState.Dead)
        {
            transform.LookAt(TargetPlayer, Vector3.up);
        }
    }
    /// <summary>
    /// ���ϸ��֣���������ʱ����
    /// </summary>
    /// <returns></returns>
    IEnumerator MaterialAppear()
    {
        //��¼������ʱ��
        float dissolveTimeDuration = SpawnDuration;
        //��¼��ǰ���ֹ�ȥ��ʱ��
        float currentDissolveTime = 0;
        float dissolveHight_start = -10f;
        float dissolveHight_target = 20f;
        float dissoleHight;

        //����Ƥ�����ܽ��ѡ��
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
        //�ر�Ƥ�����ܽ��ѡ��
        _materialPropertyBlock.SetFloat("_enableDissolve", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }
}
