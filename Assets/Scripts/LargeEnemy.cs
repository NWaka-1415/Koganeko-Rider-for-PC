using System.Collections;
using System.Collections.Generic;
using Anima2D;
using UnityEngine;

public class LargeEnemy : MonoBehaviour
{
    //腕などが別個に存在する大型エネミー
    //Enemy継承すりゃよかったわ
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private GameObject _rightWallJudgment;
    [SerializeField] private GameObject _leftWallJudgment;
    [SerializeField] private GameObject _damageTextUI;
    [SerializeField] private AudioClip[] _attackAudioClips;
    [SerializeField] private AudioClip _destoryAudioClip;
    [SerializeField] private ContactFilter2D _contactFilter2D;

    [SerializeField] private GameObject _armR;
    [SerializeField] private GameObject _armL;
    private SearchArea _searchArea;

    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private AudioSource _audioSource;
    private PoseManager _poseManager;

    /*
     * -----------------------
     * Status
     * -----------------------
     */
    [SerializeField, Range(0, 100)] private int _level;
    [SerializeField, Range(0, 200)] private float _lineCastEndPoint;
    [SerializeField] private float _walkSpeed;
    private int _hitPoint;
    private int _maxHp;
    private int _attackPower;
    private int _defencePower;
    private float _attackSpeed;
    private float _size;
    private float _jumpPower;
    private State _state;
    private int _experiencePoint;

    private int _isRight = -1; //左向きの場合は-1 作者からみた向き
    private EnemyPattern _pattern;

    /*
     * -----------------------
     * Attack
     * -----------------------
     */
    private int _prevAttackInx = 0;

    private bool _isAttackState = false;

    private bool _isAttackInterval = false;
    private float _attackInterval = 0f;

    private bool _isPreAttack = false;
    private bool _isAttacking = false;
    private bool _isAttacked = false;

    private float _preDelta = 0f;
    private float _attackingDelta = 6f;
    private float _attackedDelta = 0f;

    private int _attackerCount = 0;

    private float _shotDelta = 0.25f;

    private float _delta = 0f;

    /*
     * ----------------------
     * Down
     * ----------------------
     */
    private bool _isDown = false;
    private bool _isDownAudio = false;
    private float _downDelta = 0f;

    //Patrol
    private bool _isPatrol = false;
    private float _patrolDelta = 0f;
    private bool _isTurned = false;

    private bool _isWalking = false;

    // Use this for initialization
    void Start()
    {
        Init();
        SetStatus();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameObject.Find("GameManager").GetComponent<GameSceneManager>().IsGameStart)
        {
            Debug.Log(gameObject.name + "'s HP : " + _hitPoint.ToString());
            if (!_isAttackState)
            {
                //Debug.Log("R:" + _isRight.ToString());
                if (_rightWallJudgment.GetComponent<WallJudgment>().Judg)
                {
                    _rightWallJudgment.GetComponent<WallJudgment>().Judg = false;
                    _isRight *= -1;
                }

                if (_leftWallJudgment.GetComponent<WallJudgment>().Judg)
                {
                    _leftWallJudgment.GetComponent<WallJudgment>().Judg = false;
                    _isRight *= -1;
                }

                //Debug.Log("After R:" + _isRight.ToString());

                transform.localScale = new Vector3(_size * _isRight * -1, transform.lossyScale.y,
                    transform.localScale.z);
            }

            /*
            if (_hitPoint <= 0)
            {
                _isDown = true;
                _state = State.Stop;
                _animator.SetTrigger("isDown");
            }
            */
            if (_isDown)
            {
                _downDelta += Time.deltaTime;
                if (_downDelta >= 1f)
                {
                    GameObject.Find("GameManager").GetComponent<GameSceneManager>()
                        .AddWillGetExp(this._experiencePoint);
                    Destroy(gameObject);
                }
            }

            if (_attackInterval >= 6f)
            {
                _isAttackInterval = true;
                _attackInterval = 0f;
            }
            else if (!_isAttackInterval && _attackInterval < 6f)
            {
                _attackInterval += Time.deltaTime;
            }

            //Debug.Log(gameObject.name + "'s State:" + _state);
            //Debug.Log(gameObject.name + "'s OnGround:" + OnGround().ToString());
            if (OnGround() && !_isDown)
            {
                //Debug.Log("OnGround");
                if (_state == State.ShortDistAttack)
                {
                    if (_pattern == EnemyPattern.SnowRiceField)
                    {
                        //Attack(1);
                    }
                    else if (_pattern == EnemyPattern.Suspenders)
                    {
                        if (!_isAttackState)
                        {
                            float rand = Random.Range(0, 10);
                            _prevAttackInx = rand < 5f ? 0 : 1;
                        }

                        Attack(_prevAttackInx);
                    }
                    else if (_pattern == EnemyPattern.DarkHorse)
                    {
                        if (!_isAttackState)
                        {
                            float rand = Random.Range(0, 10);
                            _prevAttackInx = rand < 6f ? 0 : 1;
                        }

                        Attack(_prevAttackInx);
                    }
                    else if (_pattern == EnemyPattern.Python)
                    {
                        Attack(1);
                    }
                }
                else if (_state == State.LongDistAttack)
                {
                    if (_pattern == EnemyPattern.SnowRiceField)
                    {
                        Attack(0);
                    }
                    else if (_pattern == EnemyPattern.Suspenders)
                    {
                        Walk();
                    }
                    else if (_pattern == EnemyPattern.DarkHorse)
                    {
                        Walk();
                    }
                    else if (_pattern == EnemyPattern.Python)
                    {
                        if (!_isAttackState)
                        {
                            float rand = Random.Range(0, 10);
                            if (rand < 5f)
                            {
                                _prevAttackInx = 2;
                            }
                            else
                            {
                                _prevAttackInx = 0;
                            }
                        }

                        Attack(_prevAttackInx);
                    }
                }
                else if (_state == State.Walk)
                {
                    Walk();
                }
                else if (_state == State.Patrol)
                {
                    Patrol();
                }
                else if (_state == State.Stop)
                {
                    Stop();
                }
            }
            else
            {
                _state = State.Stop;
                Stop();
            }
        }
    }

    void Walk()
    {
        //Debug.Log("isR:" + _isRight.ToString());
        _isWalking = true;
        _animator.SetBool("isWalk", true);
        _rigidbody2D.velocity = new Vector2(_walkSpeed * _isRight, _rigidbody2D.velocity.y);
    }

    void Stop()
    {
        _isWalking = false;
        _animator.SetBool("isWalk", false);
        _rigidbody2D.velocity = new Vector2(0f, _rigidbody2D.velocity.y);
        _isPatrol = false;
        _patrolDelta = 0f;
    }

    void Attack(int attackIndex)
    {
        _animator.SetBool("isWalk", false);
        if (_isAttackInterval)
        {
            switch (_pattern)
            {
                //Patternによって敵の種類が決定
                case EnemyPattern.SnowRiceField:
                    //スノーライスフィールド
                    switch (attackIndex)
                    {
                        case 0:

                            if (_isAttacked)
                            {
                                if (_attackedDelta >= 1.5f)
                                {
                                    _isAttacked = false;
                                    _attackedDelta = 0f;
                                    _isAttackState = false;
                                    _isAttackInterval = false;
                                }
                                else
                                {
                                    _attackedDelta += Time.deltaTime;
                                }
                            }
                            else if (_isAttacking)
                            {
                                if (_attackingDelta >= 3f)
                                {
                                    _isAttacking = false;
                                    _attackingDelta = 0f;
                                    _isAttacked = true;
                                }
                                else
                                {
                                    _attackingDelta += Time.deltaTime;
                                    if (_delta >= _shotDelta)
                                    {
                                        _delta = 0f;
                                        _audioSource.clip = _attackAudioClips[0];
                                        _audioSource.Play();
                                        _armR.GetComponent<SnowRiceFieldArm>().ShotRice();
                                        _armL.GetComponent<SnowRiceFieldArm>().ShotRice();
                                    }
                                    else
                                    {
                                        _delta += Time.deltaTime;
                                    }
                                }
                            }
                            else if (!_isPreAttack)
                            {
                                _preDelta = 0f;
                                _animator.SetTrigger("isAttack00");
                                _isPreAttack = true;
                                _isAttackState = true;
                            }
                            else if (_isPreAttack)
                            {
                                if (_preDelta >= 2f)
                                {
                                    _isPreAttack = false;
                                    _preDelta = 0f;
                                    _attackingDelta = 0f;
                                    _isAttacking = true;
                                    _delta = 0f;
                                }
                                else
                                {
                                    _preDelta += Time.deltaTime;
                                }
                            }

                            break;
                    }

                    break;
                case EnemyPattern.Suspenders:
                    //サスペンダー
                    switch (attackIndex)
                    {
                        case 0:

                            //Debug.Log("armR:" + _armR.gameObject.name);
                            CapsuleCollider2D armRCaps = _armR.transform.Find("Arm_R_Hand/New bone")
                                .GetComponent<CapsuleCollider2D>();
                            if (!_isAttacking)
                            {
                                _animator.SetTrigger("isAttack00");
                                _isAttacking = true;
                                _attackingDelta = 0f;
                                _isAttackState = true;
                            }
                            else
                            {
                                if (_attackingDelta >= 1.75f)
                                {
                                    _isAttacking = false;
                                    armRCaps.enabled = false;
                                    _isAttackInterval = false;
                                    _isAttackState = false;
                                    _attackerCount = 0;
                                }
                                else if (_attackingDelta >= 0.5f)
                                {
                                    armRCaps.enabled = true;
                                    if (_attackerCount == 0)
                                    {
                                        _audioSource.clip = _attackAudioClips[0];
                                        _audioSource.Play();
                                        _attackerCount = 1;
                                    }

                                    _attackingDelta += Time.deltaTime;
                                }
                                else
                                {
                                    _attackingDelta += Time.deltaTime;
                                }
                            }

                            break;
                        case 1:
                            CapsuleCollider2D legRCaps = transform
                                .Find("Leg_R_Bone/Leg_R/Foot_R/New bone (1)/FootAttack")
                                .GetComponent<CapsuleCollider2D>();
                            CircleCollider2D legRCir = transform.Find("Leg_R_Bone/Leg_R/Foot_R/New bone (1)")
                                .GetComponent<CircleCollider2D>();
                            if (!_isAttacking)
                            {
                                _animator.SetTrigger("isAttack01");
                                _isAttacking = true;
                                _attackingDelta = 0f;
                                _isAttackState = true;
                                //足のコライダーON
                                legRCaps.enabled = true;
                                legRCir.enabled = false;
                                _audioSource.clip = _attackAudioClips[1];
                                _audioSource.Play();
                            }
                            else
                            {
                                if (_attackingDelta >= 70f / 60f)
                                {
                                    _isAttacking = false;
                                    _isAttackInterval = false;
                                    _isAttackState = false;
                                    legRCir.enabled = true;
                                }
                                else if (_attackingDelta >= 1f)
                                {
                                    legRCaps.enabled = false;
                                    _attackingDelta += Time.deltaTime;
                                }
                                else
                                {
                                    _attackingDelta += Time.deltaTime;
                                }
                            }

                            break;
                        case 2:
                            if (!_isAttacking)
                            {
                                _rigidbody2D.AddForce(new Vector2(_isRight * 5000f, 0f));
                                _isAttacking = true;
                                _attackingDelta = 0f;
                                _isAttackState = true;
                            }
                            else
                            {
                                if (_attackingDelta >= 2f)
                                {
                                    _isAttacking = false;
                                    _isAttackInterval = false;
                                    _isAttackState = false;
                                }
                                else
                                {
                                    _attackingDelta += Time.deltaTime;
                                }
                            }

                            break;
                    }

                    break;
                case EnemyPattern.DarkHorse:
                    switch (attackIndex)
                    {
                        case 0:
                            /*CapsuleCollider2D armRCaps = _armR.transform.Find("Arm_R_Bone/Arm_R_Hand/Arm_RHand_Bone")
                                .GetComponent<CapsuleCollider2D>();*/
                            if (!_isAttacking)
                            {
                                _animator.SetTrigger("isAttack");
                                _isAttacking = true;
                                _isAttackState = true;
                                _attackingDelta = 0f;
                                _attackerCount = 0;
                            }
                            else
                            {
                                if (_attackingDelta >= 80f / 60f)
                                {
                                    _isAttacking = false;
                                    _isAttackState = false;
                                    _isAttackInterval = false;
                                    //armRCaps.enabled = false;
                                    _attackerCount = 0;
                                }
                                else if (_attackingDelta >= 45f / 60f)
                                {
                                    if (_attackerCount == 1)
                                    {
                                        _audioSource.clip = _attackAudioClips[0];
                                        _audioSource.Play();
                                        _attackerCount = 2;
                                    }

                                    _attackingDelta += Time.deltaTime;
                                }
                                else if (_attackingDelta >= 40f / 60f)
                                {
                                    if (_attackerCount == 0)
                                    {
                                        //armRCaps.enabled = true;
                                        _attackerCount = 1;
                                    }

                                    _attackingDelta += Time.deltaTime;
                                }
                                else
                                {
                                    _attackingDelta += Time.deltaTime;
                                }
                            }

                            break;
                        case 1:
                            /*BoxCollider2D armBox = _armR.transform.Find("Arm_R_Bone/Arm_R_Hand/Arm_RHand_Bone")
                                .GetComponent<BoxCollider2D>();*/
                            if (!_isAttacking)
                            {
                                _animator.SetTrigger("isAttack01");
                                _isAttacking = true;
                                _isAttackState = true;
                                _attackedDelta = 0f;
                            }
                            else
                            {
                                if (_attackingDelta >= 100f / 60f)
                                {
                                    //armBox.enabled = false;
                                    _isAttacking = false;
                                    _isAttackState = false;
                                    _isAttackInterval = false;
                                    _attackerCount = 0;
                                }
                                else if (_attackingDelta >= 1.5f)
                                {
                                    if (_attackerCount == 0)
                                    {
                                        _audioSource.clip = _attackAudioClips[1];
                                        _audioSource.Play();
                                    }

                                    _attackingDelta += Time.deltaTime;
                                }
                                else if (_attackingDelta >= 0.5f)
                                {
                                    //armBox.enabled = true;
                                    _attackingDelta += Time.deltaTime;
                                }
                                else
                                {
                                    _attackingDelta += Time.deltaTime;
                                }
                            }

                            break;
                    }

                    break;
                case EnemyPattern.Python:
                    switch (attackIndex)
                    {
                        case 0:
                            if (!_isAttacking)
                            {
                                _animator.SetTrigger("isAttack");
                                _isAttacking = true;
                                _isAttackState = true;
                            }
                            else
                            {
                                if (_attackingDelta >= 1.5f)
                                {
                                    _isAttacking = false;
                                    _isAttackState = false;
                                    _isAttackInterval = false;
                                    _attackingDelta = 0f;
                                }
                                else
                                {
                                    _attackingDelta += Time.deltaTime;
                                }
                            }

                            break;
                        case 1:
                            if (!_isAttacking)
                            {
                                _animator.SetTrigger("isAttack01");
                                _isAttacking = true;
                                _isAttackState = true;
                            }
                            else
                            {
                                if (_attackingDelta >= 0.5f)
                                {
                                    _isAttacking = false;
                                    _isAttackState = false;
                                    _isAttackInterval = false;
                                    _attackingDelta = 0f;
                                }
                                else
                                {
                                    _attackingDelta += Time.deltaTime;
                                }
                            }

                            break;
                        case 2:
                            if (!_isAttacking)
                            {
                                _animator.SetTrigger("isAttack02");
                                _isAttacking = true;
                                _isAttackState = true;
                            }
                            else
                            {
                                if (_attackingDelta >= 1f)
                                {
                                    _isAttacking = false;
                                    _isAttackState = false;
                                    _isAttackInterval = false;
                                    _attackingDelta = 0f;
                                }
                                else
                                {
                                    _attackingDelta += Time.deltaTime;
                                }
                            }

                            break;
                    }

                    break;
            }
        }
    }

    public void Damaged(int damage)
    {
        if (!_isDown)
        {
            float rand = Random.Range(-2f, 2f);
            GameObject instanceText =
                Instantiate(_damageTextUI, transform.position + new Vector3(rand, 5f, 0f), transform.rotation);
            instanceText.GetComponent<DamageTextUI>().Parent = this.gameObject;
            if (_defencePower < damage)
            {
                _hitPoint = _hitPoint - (damage - _defencePower);
                instanceText.GetComponent<DamageTextUI>().DamagedValue = damage - _defencePower;
            }
            else
            {
                _hitPoint -= 1;
                instanceText.GetComponent<DamageTextUI>().DamagedValue = 1;
            }

            //_animator.SetBool("isDamaged", true);
            if (_hitPoint <= 0)
            {
                _hitPoint = 0;
                if (!_isDownAudio)
                {
                    _animator.SetTrigger("isDown");

                    _audioSource.clip = _destoryAudioClip;
                    _audioSource.Play();
                }

                _isDown = true;
                _isDownAudio = true;
                _downDelta = 0f;
            }
        }
    }

    void Patrol()
    {
        if (_patrolDelta >= 6f)
        {
            _patrolDelta = 0f;
            _isPatrol = false;
            _isTurned = false;
        }
        else if (_patrolDelta > 3f)
        {
            _isPatrol = true;
            if (!_isTurned)
            {
                _isTurned = true;
                _isRight *= -1;
                //Debug.Log("反転");
            }

            _patrolDelta += Time.deltaTime;
            Walk();
        }
        else
        {
            _isPatrol = true;
            _patrolDelta += Time.deltaTime;
            Walk();
        }
    }

    void Init()
    {
        _state = State.Stop;
        _size = transform.localScale.x;
        _attackInterval = 6f;
        if (this.gameObject.name.Contains("SnowRiceField"))
        {
            //スノーライスフィールド
            _pattern = EnemyPattern.SnowRiceField;
        }
        else if (this.gameObject.name.Contains("Suspenders"))
        {
            //サスペンダー
            _pattern = EnemyPattern.Suspenders;
        }
        else if (this.gameObject.name.Contains("DarkHorse"))
        {
            //害悪馬(DarkHorse)
            _pattern = EnemyPattern.DarkHorse;
        }
        else if (this.gameObject.name.Contains("Python"))
        {
            _pattern = EnemyPattern.Python;
        }
        else
        {
            _pattern = 0;
        }

        _poseManager = GetComponent<PoseManager>();
        _animator = GetComponent<Animator>();
        Debug.Log("Animator:" + _animator);
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    void SetStatus()
    {
        switch (_pattern)
        {
            case EnemyPattern.SnowRiceField:
                _maxHp = 11000;
                _attackPower = 120;
                _defencePower = 90;
                _experiencePoint = 25;
                for (int i = 0; i < _level; i++)
                {
                    _maxHp += 100;
                    _attackPower += 20;
                    _defencePower += 20;
                    _experiencePoint += 10;
                }

                _hitPoint = _maxHp;
                break;
            case EnemyPattern.Suspenders:
                _maxHp = 9800;
                _attackPower = 80;
                _defencePower = 60;
                _experiencePoint = 20;
                for (int i = 0; i < _level; i++)
                {
                    _maxHp += 50;
                    _attackPower += 10;
                    _defencePower += 42;
                    _experiencePoint += 9;
                }

                _hitPoint = _maxHp;
                break;
            case EnemyPattern.DarkHorse:
                _maxHp = 10000;
                _attackPower = 200;
                _defencePower = 120;
                _experiencePoint = 35;
                for (int i = 0; i < _level; i++)
                {
                    _maxHp += 100;
                    _attackPower += 20;
                    _defencePower += 10;
                    _experiencePoint += 15;
                }

                _hitPoint = _maxHp;
                break;
            case EnemyPattern.Python:
                _maxHp = 15000;
                _attackPower = 420;
                _defencePower = 120;
                _experiencePoint = 50;
                for (int i = 0; i < _level; i++)
                {
                    _maxHp += 150;
                    _attackPower += 90;
                    _defencePower += 50;
                    _experiencePoint += 20;
                }

                _hitPoint = _maxHp;
                break;
        }
    }


    public bool OnGround()
    {
        return Physics2D.Linecast(transform.position - new Vector3(0, _lineCastEndPoint - 1, 0),
            transform.position - new Vector3(0, _lineCastEndPoint, 0),
            _groundLayerMask);
    }

    /*
     * --------------------------------
     * Set & Get
     * --------------------------------
     */
    public int IsRight
    {
        get { return _isRight; }
        set
        {
            if (value != 1 && value != -1)
            {
                _isRight = 1;
            }
            else
            {
                _isRight = value;
            }
        }
    }

    public int AttackPower
    {
        get { return _attackPower; }
    }

    public int Level
    {
        get { return _level; }
        set { _level = value; }
    }

    public bool IsDown
    {
        get { return _isDown; }
    }

    public bool IsPatrol
    {
        get { return _isPatrol; }
    }

    public bool IsAttackState
    {
        get { return _isAttackState; }
    }

    public State States
    {
        get { return _state; }
        set { _state = value; }
    }

    /*
     * --------------------------------
     * State
     * --------------------------------
     */
    public enum State
    {
        Walk,
        Stop,
        LongDistAttack,
        ShortDistAttack,
        Patrol
    }

    private enum EnemyPattern
    {
        SnowRiceField,
        Suspenders,
        DarkHorse,
        Python
    }
}