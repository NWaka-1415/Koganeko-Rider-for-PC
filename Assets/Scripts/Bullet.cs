using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
using Values;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed = 25f;
    [SerializeField] private float _destoryTime = 2.5f;
    [SerializeField] private BulletType _bulletType;

    private AudioSource _audioSource;
    private Rigidbody2D _rigidbody2D;
    private GameObject _parent;

    private string _thisTag;

    private int _isRight = 1;
    private int _attackPower;


    private float _totalTime;

    // Use this for initialization
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _totalTime = 0f;
        _thisTag = _parent.tag;
        if (_bulletType == BulletType.FireBallType)
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("parentTag:" + _parent.tag);
        Vector3 scale = transform.localScale;

        if (_bulletType == BulletType.FireBallType)
        {
            //プレイヤー追従型
            GameObject player = GameObject.FindWithTag(Tag.Player);
            Vector3 target = player.transform.position - this.transform.position;
            float dirX = target.x / Mathf.Abs(target.x);
            float dirY = target.y / Mathf.Abs(target.y);
            float angle = Mathf.Atan(target.y / target.x);
            float vY = _bulletSpeed * Mathf.Abs(Mathf.Sin(angle)) * dirY;
            float vX = _bulletSpeed * Mathf.Abs(Mathf.Cos(angle)) * dirX;
            _rigidbody2D.velocity = new Vector2(vX, vY);
        }
        else
        {
            //一直線型
            _rigidbody2D.velocity = transform.right * _bulletSpeed * _isRight;
        }

        transform.localScale = new Vector3(Mathf.Abs(scale.x) * _isRight * -1, scale.y, 0f);

        _totalTime += Time.deltaTime;
        if (_totalTime >= _destoryTime)
        {
            Destroy(gameObject);
        }
    }

    void Attack()
    {
        if (_bulletType == BulletType.FireBallType)
        {
            _audioSource.Play();
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_thisTag == Tag.Player)
        {
            if (other.gameObject.CompareTag(Tag.Enemy))
            {
                other.GetComponent<Enemy.Enemy>().Damaged(_attackPower);
                Attack();
            }
            else if (other.gameObject.CompareTag(Tag.LargeEnemy))
            {
                other.GetComponent<LargeEnemy>().Damaged(_attackPower);
                Attack();
            }
        }
        else if (_thisTag == Tag.Enemy || _thisTag == Tag.LargeEnemyWepon)
        {
            if (other.gameObject.CompareTag(Tag.Player))
            {
                other.GetComponent<Player>().Damaged(_attackPower);
                Attack();
            }
        }
    }

    /*-----------------------------------------------------
     * Set & Get
     * ----------------------------------------------------
     */
    public int IsRight
    {
        get => _isRight;
        set
        {
            if (value == 1 || value == -1) _isRight = value;
        }
    }

    public int AttackPower
    {
        get => _attackPower;
        set => _attackPower = value >= 0 ? value : 0;
    }

    public GameObject Parent
    {
        get => _parent;
        set => _parent = value;
    }

    //------------------------------------------------------
    enum BulletType
    {
        StraightLineType,
        FireBallType
    }
}