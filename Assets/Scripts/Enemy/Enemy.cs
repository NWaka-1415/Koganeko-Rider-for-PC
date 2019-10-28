using Controllers;
using DG.Tweening;
using UnityEngine;

namespace Enemy
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private LayerMask _groundLayerMask;
        [SerializeField] private GameObject _rightWallJudgment;
        [SerializeField] private GameObject _leftWallJudgment;
        [SerializeField] private GameObject _damageTextUI;
        [SerializeField] private GameObject _attaker;
        [SerializeField] private EnemyType _enemyType;
        [SerializeField] private AudioClip[] _attackAudioClips;
        [SerializeField] private AudioClip _destoryAudioClip;
        [SerializeField] private ContactFilter2D _contactFilter2D;

        private float _delta = 0f;
        private Rigidbody2D _rigidbody2D;
        private Animator _animator;
        private AudioSource _audioSource;

        //Status
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

        private int _isRight = 1; //左向きの場合は-1
        private EnemyPattern _pattern;

        //Attack
        private bool _isAttackState = false;
        private bool _isAttacking = false;
        private float _attackingDelta = 0f;
        private int _prevAttackInx;

        private int _attackerCount = 0;

        private bool _isAttackInterval = false;
        private float _attackInterval = 0f;

        //Down
        private bool _isDown = false;
        private bool _isDownAudio = false;
        private float _downDelta = 0f;

        //Patrol
        private bool _isPatrol = false;
        private float _patrolDelta;
        private bool _isTurned = false;

        private bool _isWalking = false;

        private int _floatingCount = 0;

        // Use this for initialization
        void Start()
        {
            Init();
            SetStatus();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (GameSceneController.instance.IsGameStart)
            {
                /*Debug.Log("<color=blue>" + gameObject.name + "'s State:" + _state + ", Onground:" + OnGround() +
                      "</color>");*/
                if (!_isAttackState && !_isDown)
                {
                    //Debug.Log("R:" + _isRight.ToString());
                    if (_rightWallJudgment.GetComponent<WallJudgment>().Judgment)
                    {
                        _rightWallJudgment.GetComponent<WallJudgment>().Judgment = false;
                        _isRight *= -1;
                    }

                    if (_leftWallJudgment.GetComponent<WallJudgment>().Judgment)
                    {
                        _leftWallJudgment.GetComponent<WallJudgment>().Judgment = false;
                        _isRight *= -1;
                    }

                    //Debug.Log("After R:" + _isRight.ToString());

                    transform.localScale = new Vector3(_size * _isRight * -1, transform.lossyScale.y,
                        transform.localScale.z);
                    if (_enemyType == EnemyType.Floating)
                    {
                        FloatungAction();
                    }
                }

                if (_isDown)
                {
                    _downDelta += Time.deltaTime;
                    if (_downDelta >= 1f)
                    {
                        GameSceneController.instance.AddWillGetExp(this._experiencePoint);
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

                if (OnGround() && !_isDown)
                {
                    if (_state == State.ShortDistAttack)
                    {
                        if (_pattern == EnemyPattern.Ikilipse)
                        {
                            float rand = Random.Range(0, 10);
                            if (rand < 2)
                            {
                                Attack(2);
                            }
                        }
                        else if (_pattern == EnemyPattern.Blender)
                        {
                            //float rand = Random.Range(0, 10);
                            /*
                        if (!_isAttackState)
                        {
                            _prevAttackInx = rand < 5 ? 0 : 1;
                        }
                        
                        Attack(_prevAttackInx);
                        */
                            Attack(1);
                        }
                        else if (_pattern == EnemyPattern.Skyper)
                        {
                            Attack(0);
                        }
                        else if (_pattern == EnemyPattern.FireChrome)
                        {
                            Attack(1);
                        }
                    }
                    else if (_state == State.LongDistAttack)
                    {
                        if (_pattern == EnemyPattern.Ikilipse)
                        {
                            if (!_isAttackState)
                            {
                                float rand = Random.Range(0, 10);
                                _prevAttackInx = rand < 5f ? 0 : 1;
                            }

                            Attack(_prevAttackInx);
                        }
                        else if (_pattern == EnemyPattern.Blender)
                        {
                            Walk();
                        }
                        else if (_pattern == EnemyPattern.Skyper)
                        {
                            Walk();
                        }
                        else if (_pattern == EnemyPattern.FireChrome)
                        {
                            Attack(0);
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
                if (_enemyType == EnemyType.Floating)
                {
                    GameObject floating = this.transform.Find("Move").gameObject;
                    _floatingCount = 90;
                    floating.transform.localPosition = new Vector3(0f, 1f, 0f);
                }

                switch (_pattern)
                {
                    case EnemyPattern.Ikilipse:
                        switch (attackIndex)
                        {
                            case 0:
                                if (!_isAttacking)
                                {
                                    _animator.SetTrigger("isAttack");
                                    _isAttacking = true;
                                    _attackingDelta = 0f;
                                    _isAttackState = true;
                                    _attackerCount = 0;
                                }
                                else
                                {
                                    if (_attackingDelta >= 2f)
                                    {
                                        _isAttacking = false;
                                        _isAttackInterval = false;
                                        _isAttackState = false;
                                    }
                                    else if (_attackingDelta >= 1.10f)
                                    {
                                        if (_attackerCount == 0)
                                        {
                                            _audioSource.clip = _attackAudioClips[0];
                                            _audioSource.Play();
                                            GameObject attaking = Instantiate(_attaker,
                                                transform.position + new Vector3(_isRight * 1.5f, 0f, 0f),
                                                transform.rotation);
                                            attaking.GetComponent<Bullet>().Parent = this.gameObject;
                                            attaking.GetComponent<Bullet>().AttackPower = _attackPower;
                                            attaking.GetComponent<Bullet>().IsRight = _isRight;
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
                                break;
                            case 2:
                                //攻撃ではなくワープ
                                if (!_isAttacking)
                                {
                                    _isAttacking = true;
                                    _isAttackState = true;
                                    _attackerCount = 0;

                                    float moveX = Random.Range(0f, 100f);

                                    _attackingDelta = 0f;
                                    _audioSource.clip = _attackAudioClips[1];
                                    _audioSource.Play();
                                    this.transform.DOScale(0.1f, 0.4f).OnComplete(() =>
                                    {
                                        this.transform.position = new Vector3(moveX, transform.position.y + 5f, 0f);
                                        this.transform.DOScale(_size, 0.4f).OnComplete(() =>
                                        {
                                            _isAttacking = false;
                                            _isAttackInterval = false;
                                            _isAttackState = false;
                                        });
                                    });
                                }

                                break;
                        }

                        break;
                    case EnemyPattern.Blender:
                        switch (attackIndex)
                        {
                            case 0:
                                //体当たり
                                if (!_isAttacking)
                                {
                                    _animator.SetBool("isAttack", true);
                                    _isAttacking = true;
                                    _isAttackState = true;
                                    _attackingDelta = 0f;
                                }
                                else
                                {
                                    if (_attackingDelta >= 1f)
                                    {
                                        _animator.SetBool("isAttack", false);
                                        _isAttackInterval = false;
                                        _isAttacking = false;
                                        _isAttackState = false;
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
                                    _animator.SetBool("isAttack", true);
                                    _isAttacking = true;
                                    _isAttackState = true;
                                    _attackingDelta = 0f;
                                }
                                else
                                {
                                    if (_attackingDelta >= 1f)
                                    {
                                        _animator.SetBool("isAttack", false);
                                        _isAttackInterval = false;
                                        _isAttacking = false;
                                        _isAttackState = false;
                                        _attackerCount = 0;
                                    }
                                    else if (_attackingDelta >= 0.3f)
                                    {
                                        if (_attackerCount == 1)
                                        {
                                            _audioSource.clip = _attackAudioClips[0];
                                            _audioSource.Play();
                                            GameObject instanceAttacker =
                                                Instantiate(_attaker,
                                                    transform.position + new Vector3(_isRight * 0.65f, 0f),
                                                    transform.rotation);
                                            instanceAttacker.GetComponent<Bullet>().Parent = this.gameObject;
                                            instanceAttacker.GetComponent<Bullet>().AttackPower = this._attackPower;
                                            instanceAttacker.GetComponent<Bullet>().IsRight = this._isRight;
                                            _attackerCount = 2;
                                        }

                                        _attackingDelta += Time.deltaTime;
                                    }
                                    else if (_attackingDelta >= 0.2f)
                                    {
                                        if (_attackerCount == 0)
                                        {
                                            _audioSource.clip = _attackAudioClips[0];
                                            _audioSource.Play();
                                            GameObject instanceAttacker =
                                                Instantiate(_attaker,
                                                    transform.position + new Vector3(_isRight * 0.65f, 0f),
                                                    transform.rotation);
                                            instanceAttacker.GetComponent<Bullet>().Parent = this.gameObject;
                                            instanceAttacker.GetComponent<Bullet>().AttackPower = this._attackPower;
                                            instanceAttacker.GetComponent<Bullet>().IsRight = this._isRight;
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
                        }

                        break;
                    case EnemyPattern.Skyper:
                        switch (attackIndex)
                        {
                            case 0:
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
                                    if (_attackingDelta >= 1f)
                                    {
                                        _isAttacking = false;
                                        _isAttackState = false;
                                        _isAttackInterval = false;
                                        _attackingDelta = 0f;
                                        _attackerCount = 0;
                                    }
                                    else if (_attackingDelta >= 0.8f)
                                    {
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
                        }

                        break;
                    case EnemyPattern.FireChrome:
                        switch (attackIndex)
                        {
                            case 0:
                                //突進
                                if (!_isAttacking)
                                {
                                    _animator.SetTrigger("isAttack");
                                    _isAttacking = true;
                                    _isAttackState = true;
                                    _attackingDelta = 0f;
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
                                    _attackingDelta = 0f;
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
                    Instantiate(_damageTextUI, transform.position + new Vector3(rand, 2f, 0f), transform.rotation);
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
                        _audioSource.clip = _destoryAudioClip;
                        _audioSource.Play();
                        _animator.SetTrigger("isDown");
                        if (_pattern == EnemyPattern.Blender)
                        {
                            _animator.SetBool("isAttack", false);
                        }

                        _isDownAudio = true;
                        _isDown = true;
                        _downDelta = 0f;
                    }
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

        void FloatungAction()
        {
            GameObject floating = this.transform.Find("Move").gameObject;
            floating.transform.localPosition = new Vector3(0f, Mathf.Sin(_floatingCount / 360f), 0f);
            _floatingCount += 10;
        }

        void Init()
        {
            _animator = GetComponent<Animator>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _audioSource = GetComponent<AudioSource>();
            _size = transform.localScale.x;
            if (this.gameObject.name.Contains("Ikilipse"))
            {
                _pattern = EnemyPattern.Ikilipse;
            }
            else if (this.gameObject.name.Contains("Blender"))
            {
                _pattern = EnemyPattern.Blender;
            }
            else if (this.gameObject.name.Contains("Skyper"))
            {
                _pattern = EnemyPattern.Skyper;
            }
            else if (this.gameObject.name.Contains("FireChrome"))
            {
                _pattern = EnemyPattern.FireChrome;
            }
        }

        void SetStatus()
        {
            switch (_pattern)
            {
                case EnemyPattern.Ikilipse:
                    _maxHp = 410;
                    _attackPower = 100;
                    _defencePower = 50;
                    _experiencePoint = 10;
                    for (int i = 0; i < _level; i++)
                    {
                        _maxHp += 50;
                        _attackPower += 10;
                        _defencePower += 10;
                        _experiencePoint += 2;
                    }

                    _hitPoint = _maxHp;
                    break;
                case EnemyPattern.Blender:
                    _maxHp = 320;
                    _attackPower = 80;
                    _defencePower = 20;
                    _experiencePoint = 5;
                    for (int i = 0; i < _level; i++)
                    {
                        _maxHp += 15;
                        _attackPower += 10;
                        _defencePower += 10;
                        _experiencePoint += 2;
                    }

                    _hitPoint = _maxHp;
                    break;
                case EnemyPattern.Skyper:
                    //最弱モンスター予定
                    _maxHp = 120;
                    _attackPower = 70;
                    _defencePower = 10;
                    _experiencePoint = 1;
                    for (int i = 0; i < _level; i++)
                    {
                        _maxHp += 10;
                        _attackPower += 5;
                        _defencePower += 5;
                        _experiencePoint += 1;
                    }

                    _hitPoint = _maxHp;
                    break;
                case EnemyPattern.FireChrome:
                    _maxHp = 510;
                    _attackPower = 200;
                    _defencePower = 55;
                    _experiencePoint = 18;
                    for (int i = 0; i < _level; i++)
                    {
                        _maxHp += 25;
                        _attackPower += 20;
                        _defencePower += 10;
                        _experiencePoint += 4;
                    }

                    _hitPoint = _maxHp;
                    break;
            }
        }

        public bool OnGround()
        {
            if (_enemyType == EnemyType.Floating)
            {
                GameObject floating = this.transform.Find("Move").gameObject;
                return floating.GetComponent<CapsuleCollider2D>().IsTouching(_contactFilter2D) || Physics2D.Linecast(
                           transform.position - new Vector3(0, _lineCastEndPoint - 1, 0),
                           transform.position - new Vector3(0, _lineCastEndPoint, 0),
                           _groundLayerMask);
            }

            return _rigidbody2D.IsTouching(_contactFilter2D);
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

        public Rigidbody2D Rigidbody2D
        {
            get { return _rigidbody2D; }
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

        public EnemyPattern Pattern
        {
            get { return _pattern; }
        }

        public State States
        {
            get { return _state; }
            set { _state = value; }
        }

        /*
     * ------------------------------
     * State
     * ------------------------------
     */
        public enum State
        {
            Walk,
            Stop,
            LongDistAttack,
            ShortDistAttack,
            Patrol
        }

        public enum EnemyPattern
        {
            Ikilipse,
            Blender,
            Skyper,
            FireChrome
        }

        enum EnemyType
        {
            Standing,
            Floating
        }
    }
}