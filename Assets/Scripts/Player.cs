using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Controllers;
using UnityEngine;
using UnityEngine.UI;
using Values;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public float Speed = 10f;
    public float JumpForce = 1100f;
    public float AvoidanceForce = 10000f;
    public float StoppingTime = 0.45f;
    public LayerMask GroundLayerMask;
    public GameObject AttackerBullet;
    [SerializeField] private GameObject _shield;
    [SerializeField] private GameObject _damageTextUI;
    [SerializeField] private PlayerPattern _playerPattern;
    [SerializeField] private AudioClip[] _attackerAudioClips;
    [SerializeField] private ContactFilter2D _contactFilter2D;

    //Comp
    private Rigidbody2D _rigidbody2D;
    private CapsuleCollider2D _capsuleCollider2D;
    private Animator _animator;
    private AudioSource _audioSource;

    private Vector2 _startPos;
    private Vector2 _previousPos;
    private Vector2 _dir = Vector2.zero;
    Vector2 _flickDir = Vector2.zero;

    //Touch;
    private bool _isStartTouch;
    private bool _isTouchMoving;
    private bool _isTapped;
    private bool _isStationary;
    private float _totalFlickTime = 0f;
    private float _startToEndedTime = 0f;
    private const float FlickPerDisplayHeight = 0.025f;
    private const float SwipePerDisplayWidth = 0.10f;
    private const float ValidWidth = 0.25f;
    private const float FlickLimitTime = 0.1f;
    private float _acceleration;

    //Managers;
    private GameObject _gameManager;

    //Attack
    private float _attackInterval;
    private float _intervalCount;
    private bool _isAttackInterval;
    private bool _isAttacking;
    private float _attackingDelta;
    private GameObject[] _searchedEnemy;
    private GameObject[] _searchedLargeEnemy;
    private GameObject[] _searchedAllEnemy;
    private int _combo; //可能コンボ数
    private int _attackCount = 0;
    private int _tapCount = 0;

    //Guard
    private const float GuardLimitTime = 2f; //ガード実行可能時間
    private float _guardTime; //実際にガードをしている時間
    private float _guardStartTime;
    private const float GuardStartLimit = 0.15f;
    private const float GuardIntervalLimit = 2f; //ガードを実行可能時間限界まで使った際の，次ガードを使えるようになるまでの時間(秒)
    private float _guardInterval;
    [Range(0.0f, 1.0f)] private float _shieldPower; //ガード時の軽減率
    private bool _isGuard; //ガードしてる？
    private Color _shieldColor;
    private Text _guardText; //ガードに関連する情報を表示するTextコンポーネント

    //PlayerStatus
    private int _hitPoint;
    private int _maxHitPoint;
    private int _attackPower;
    private int _defencePower;
    private float _attackSpeed;

    private int _isRight;
    private bool _isJump;
    private bool _isDamaging;
    private float _stoppedTime = 0f;
    private float _size;

    // Use this for initialization
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        SetManager();
        SetStatus();
        _size = transform.localScale.x;
        _isDamaging = false;
        _isJump = false;
        _isStartTouch = false;
        _isTouchMoving = false;
        _isTapped = false;
        _isStationary = false;
        SetShield();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameSceneController.instance.IsGameStart) return;
        if (GameSceneController.instance.GameClear) return;
        if (!GameSceneController.instance.GameOver)
        {
            RefreshGuardText();
            _guardInterval -= Time.deltaTime;
            if (_guardInterval <= 0f) _guardInterval = 0f;

            _animator.SetBool("isFalling", OnGround());
            if (OnGround())
            {
                _capsuleCollider2D.enabled = true;
                _isJump = false;
            }
            else
            {
                _animator.SetBool("isWalk", false);
                _animator.SetTrigger("Jump");
            }

            if (_stoppedTime >= StoppingTime)
            {
                _animator.SetBool("isDamaged", false);
                _isDamaging = false;
                _stoppedTime = 0f;
            }
            else
            {
                _stoppedTime += Time.deltaTime;
            }

            if (_intervalCount >= _attackInterval)
            {
                _isAttackInterval = true;
                _intervalCount = 0f;
            }
            else
            {
                if (!_isAttackInterval) _intervalCount += Time.deltaTime;
            }

            if (!_isDamaging)
            {
//                Touched();
                Control();
            }
        }
        else
        {
            _animator.SetBool("isFalling", true);
            _animator.SetBool("isDamaged", true);
        }
    }

    void Flicked()
    {
        float flickX = Mathf.Abs(_flickDir.x);
        float flickY = Mathf.Abs(_flickDir.y);
        float direction = Mathf.Pow((Mathf.Pow(flickX, 2) + Mathf.Pow(flickY, 2)), 0.5f);
        //加速度を出す

        if (_totalFlickTime <= FlickLimitTime && !_isTapped)
        {
            if (Mathf.Abs(_dir.y) > Mathf.Abs(_dir.x))
            {
                if (_dir.y > Screen.height * FlickPerDisplayHeight)
                {
                    //this is Flick
                    Jump();
                }
                else if (_dir.y < -Screen.height * FlickPerDisplayHeight)
                {
                    //_capsuleCollider2D.enabled = false;
                }
            }

            else if (Mathf.Abs(this._dir.x) > Mathf.Abs(this._dir.y))
            {
                //左右に瞬間移動・着地中のみ・非攻撃時のみ利用可能
                if (OnGround() && !_isAttacking)
                {
                    if (_dir.x > Screen.width * FlickPerDisplayHeight)
                    {
                        Teleportation();
                    }
                    else if (_dir.x < -Screen.width * FlickPerDisplayHeight)
                    {
                        Teleportation(false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 瞬間移動する
    /// 左向きの場合はfalseを渡すこと
    /// </summary>
    /// <param name="isRight"></param>
    private void Teleportation(bool isRight = true)
    {
        _rigidbody2D.MovePosition(
            new Vector2(transform.localPosition.x + 5f * (isRight ? 1f : -1f), transform.localPosition.y));
    }

    //Touch Swipe and Flick
    void Touched()
    {
        /*
         * FlickとSwipeを方向で分けずに，斜めにも対応させること
         */
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    this._startPos = touch.position;
                    _isStartTouch = true;
                    _isTapped = false;
                    _isStationary = false;
                    break;
                case TouchPhase.Moved:
                    _isTouchMoving = true;
                    _isStationary = false;

                    if (Vector3.Distance(touch.position, _previousPos) <= Screen.width * 0.1f)
                    {
                        GameUiController.instance.ShowDebugMessage("Stationary in Moved");
                        if (_isStartTouch)
                        {
                            if (!_isTouchMoving) _guardStartTime += Time.deltaTime;
                            _isTouchMoving = false;
                            if (_guardStartTime >= GuardStartLimit) _isStationary = true;
                        }
                    }

                    _isStartTouch = false;
                    this._dir = touch.position - this._startPos;
                    _totalFlickTime += Time.deltaTime;
                    break;
                case TouchPhase.Stationary:
                    //長押し
                    //_gameUiManager.ShowDebugMessage("Stationary");
                    if (!_isStartTouch) break;
                    if (!_isTouchMoving) _guardStartTime += Time.deltaTime;
                    if (_guardStartTime >= GuardStartLimit) _isStationary = true;
                    break;
                case TouchPhase.Ended:
                    _isTapped = !_isTouchMoving;
                    if (_isJump || _isStationary) _isTapped = false;
                    _isTouchMoving = false;
                    _isStationary = false;
                    if (OverallController.instance.JumpPattern == OverallController.JumpPatterns.Flick)
                    {
                        Flicked();
                    }

                    this._dir = Vector2.zero;
                    _totalFlickTime = 0f;
                    _guardStartTime = 0f;
                    break;
            }

            GameUiController.instance.ShowDebugMessage("isStationary:" + _isStationary);
            if (_isTapped)
            {
                _tapCount += 1;
            }

            if (_tapCount > 0 && !_isAttacking)
            {
                //攻撃
                _animator.SetBool("isWalk", false);
                _animator.SetTrigger("AttackStart");
                _tapCount -= 1;
            }
            else if (!_isAttacking)
            {
                //移動
                if (this._dir.x > Screen.width * SwipePerDisplayWidth && !RightWall())
                {
                    _isRight = 1;
                    this._rigidbody2D.velocity = new Vector2(Speed, this._rigidbody2D.velocity.y);
                    this.transform.localScale = new Vector3(_size, _size, _size);
                    _animator.SetBool("isWalk", true);
                }
                else if (this._dir.x < -Screen.width * SwipePerDisplayWidth && !LeftWall())
                {
                    _isRight = -1;
                    this._rigidbody2D.velocity = new Vector2(-Speed, this._rigidbody2D.velocity.y);
                    this.transform.localScale = new Vector3(-_size, _size, _size);
                    _animator.SetBool("isWalk", true);
                }
                else
                {
                    Guard();
                }
            }
        }
        else
        {
            this._rigidbody2D.velocity = new Vector2(0f, this._rigidbody2D.velocity.y);
            _animator.SetBool("isWalk", false);
        }
    }

    private void Control()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _tapCount++;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Guard();
        }

        if (_tapCount > 0 && !_isAttacking)
        {
            _animator.SetBool("isWalk", false);
            _animator.SetTrigger("AttackStart");
            _tapCount--;
        }
    }

    /// <summary>
    /// 動きます
    /// </summary>
    /// <param name="isRight"></param>
    private void Move(bool isRight = true)
    {
        //移動
        if (isRight && !RightWall())
        {
            _isRight = 1;
            this._rigidbody2D.velocity = new Vector2(Speed, this._rigidbody2D.velocity.y);
            this.transform.localScale = new Vector3(_size, _size, _size);
            _animator.SetBool("isWalk", true);
        }
        else if (!isRight && !LeftWall())
        {
            _isRight = -1;
            this._rigidbody2D.velocity = new Vector2(-Speed, this._rigidbody2D.velocity.y);
            this.transform.localScale = new Vector3(-_size, _size, _size);
            _animator.SetBool("isWalk", true);
        }
    }

    public void Jump()
    {
        if (!OnGround() || _isAttacking) return;
        _isJump = true;
        this._rigidbody2D.AddForce(this.transform.up * this.JumpForce);
    }

    public void Attack(int attackIndex)
    {
        /*
         * 
         */
        _searchedEnemy = GameObject.FindGameObjectsWithTag(Tag.Enemy);

        _searchedLargeEnemy = GameObject.FindGameObjectsWithTag(Tag.LargeEnemy);

        _searchedAllEnemy = new GameObject[_searchedEnemy.Length + _searchedLargeEnemy.Length];
        int i = 0;
        if (_searchedEnemy.Length != 0)
        {
            for (i = 0; i < _searchedEnemy.Length; i++)
            {
                _searchedAllEnemy[i] = _searchedEnemy[i];
            }
        }

        if (_searchedLargeEnemy.Length != 0)
        {
            for (int j = 0; j < _searchedLargeEnemy.Length; j++)
            {
                _searchedAllEnemy[i + j] = _searchedLargeEnemy[j];
            }
        }

        float distX = 0;

        if (_searchedAllEnemy.Length != 0)
        {
            distX = transform.position.x - _searchedAllEnemy[0].transform.position.x;
            foreach (GameObject enemy in _searchedAllEnemy)
            {
                if (Mathf.Abs(transform.position.x - enemy.transform.position.x) < Mathf.Abs(distX))
                {
                    distX = transform.position.x - enemy.transform.position.x;
                }
            }

            if (distX >= 0)
            {
                _isRight = -1;
            }
            else
            {
                _isRight = 1;
            }
        }

        transform.localScale = new Vector3(_size * _isRight, _size, _size);
        /*
         * 
         */
        if (!_isAttacking)
        {
            _isAttacking = true;
        }
        else

        {
            if (attackIndex == 1)
            {
                _isAttacking = false;
            }
        }
    }

    public void Guard()
    {
        _isGuard = _isStationary;
        if (_guardTime >= GuardLimitTime)
        {
            //ガード実行時間がガード実行可能時間を超えた際にはガード終了
            _isGuard = false;
            _guardInterval = GuardIntervalLimit;
            _guardTime = 0;
        }

        //Debug.Log("ガード：" + _guardInterval);
        if (_guardInterval > 0f) _isGuard = false;

        SpriteRenderer shieldSprite = _shield.GetComponent<SpriteRenderer>();
        float alpha;

        if (_isGuard)
        {
            _guardTime += Time.deltaTime;

            alpha = _shieldColor.a;
        }
        else
        {
            _guardTime = 0f;

            alpha = 0f;
        }

        shieldSprite.color = new Color(_shieldColor.r, _shieldColor.g, _shieldColor.b, alpha);
    }

    private void RefreshGuardText()
    {
        if (_guardInterval == 0)
        {
            _guardText.text = "";
        }
        else
        {
            _guardText.text = String.Format("ガード可能まで：{0:F1}秒", _guardInterval);
        }
    }

    public void Damaged(int damage)
    {
        int thisDefencePower = _defencePower;
        if (_isGuard)
        {
            thisDefencePower = (int) Mathf.Floor(_defencePower * (1 + _shieldPower));
        }

        float rand = Random.Range(-3f, 3f);
        GameObject instanceText =
            Instantiate(_damageTextUI, transform.position + new Vector3(rand, 2f, 0f), transform.rotation);
        instanceText.GetComponent<DamageTextUI>().Parent = this.gameObject;
        GameUiController.instance.FromHp = _hitPoint;

        if (thisDefencePower < damage)
        {
            _hitPoint = _hitPoint - (damage - thisDefencePower);
            instanceText.GetComponent<DamageTextUI>().DamagedValue = damage - thisDefencePower;
        }
        else
        {
            _hitPoint -= 1;
            instanceText.GetComponent<DamageTextUI>().DamagedValue = 1;
        }

        if (!_isDamaging)
        {
            _isDamaging = true;
            _stoppedTime = 0f;
        }

        _animator.SetBool("isDamaged", true);
        if (_hitPoint <= 0)
        {
            _hitPoint = 0;
            if (!GameSceneController.instance.GameClear) GameSceneController.instance.GameOver = true;
        }

        GameUiController.instance.ToHp = _hitPoint;
        GameUiController.instance.SetReducing();
    }

    public void ShotRabbitTankBullet()
    {
        _audioSource.clip = _attackerAudioClips[3];
        _audioSource.Play();
        GameObject attacking = Instantiate(AttackerBullet, transform.position + new Vector3(0.5f * _isRight, 0.5f),
            transform.rotation);
        attacking.GetComponent<Bullet>().Parent = this.gameObject;
        attacking.GetComponent<Bullet>().AttackPower = this._attackPower;
        attacking.GetComponent<Bullet>().IsRight = this._isRight;
    }

    public void PlayAudio(int clipIndex)
    {
        _audioSource.clip = _attackerAudioClips[clipIndex];
        _audioSource.Play();
    }

    public int GetHP(int index)
    {
        //0を受け取れば最大HP，1であれば現在のHPを返す
        return index == 0 ? _maxHitPoint : _hitPoint;
    }

    void SetHP(int hp)
    {
        _maxHitPoint = hp;
        _hitPoint = _maxHitPoint;
    }

    void SetShield()
    {
        _guardStartTime = 0f;
        _guardInterval = 0f;
        _guardTime = 0f;

        _guardText = GameObject.Find("GuardIntervalText").GetComponent<Text>();

        _shieldColor = _shield.GetComponent<SpriteRenderer>().color;
        //最初は見えなくしておく
        _shield.GetComponent<SpriteRenderer>().color = new Color(_shieldColor.r, _shieldColor.g, _shieldColor.b, 0f);
    }

    void SetStatus()
    {
        SetHP(OverallController.instance.MaxPlayerHp);
        _attackPower = OverallController.instance.AttackPlayerPower;
        _defencePower = OverallController.instance.DefencePlayerPower;
        _shieldPower = OverallController.instance.ShieldPower;
        _combo = OverallController.instance.ComboPlayer;
        switch (_playerPattern)
        {
            case PlayerPattern.RabbitTank:
                _attackInterval = 0.05f;
                break;
        }
    }

    void SetManager()
    {
        _gameManager = GameObject.Find("GameManager");
    }

/*
 * --------------------------
 * Set & Get
 * --------------------------
 */
    public int IsRight
    {
        get => _isRight;
        set => _isRight = value;
    }

    public int TapCount
    {
        get => _tapCount;
        set => _tapCount = value;
    }

    public int AttackPower => _attackPower;

    public bool IsGuard => _isGuard;

/*
 * --------------------------
 * 接地判定
 * --------------------------
 */
    public bool OnGround()
    {
        return _rigidbody2D.velocity.y >= -0.15f && _rigidbody2D.velocity.y <= 0.15f;

        /*
        return _rigidbody2D.IsTouching(_contactFilter2D) || Physics2D.Linecast(
                   transform.position - new Vector3(0, _capsuleCollider2D.size.y / 4),
                   transform.position - new Vector3(0, _capsuleCollider2D.size.y * 3 / 5, 0), GroundLayerMask);
                   */
    }

    bool LeftWall()
    {
        return Physics2D.Linecast(transform.position,
            transform.position - new Vector3(_capsuleCollider2D.size.x * 3 / 5, 0, 0), GroundLayerMask);
    }

    bool RightWall()
    {
        return Physics2D.Linecast(transform.position,
            transform.position + new Vector3(_capsuleCollider2D.size.x * 3 / 5, 0, 0), GroundLayerMask);
    }

/*
 * ---------------------
 * State
 * ---------------------
 */
    public enum PlayerPattern
    {
        RabbitTank
    }
}