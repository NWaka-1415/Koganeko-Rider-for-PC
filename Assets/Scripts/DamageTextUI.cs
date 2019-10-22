using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Values;

public class DamageTextUI : MonoBehaviour
{
    [SerializeField] private float _fadeoutSpeed = 1f;
    [SerializeField] private float _moveValue = 0.4f;
    private Text _text;
    private int _damagedValue;
    private GameObject _parent;

    [Range(0f, 255f)] private float _red;
    [Range(0f, 255f)] private float _green;
    [Range(0f, 255f)] private float _blue;

    private float _alpha;

    // Use this for initialization
    void Start()
    {
        _text = GetComponentInChildren<Text>();
        _alpha = _text.color.a;
        _text.text = _damagedValue.ToString();
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        _alpha -= _fadeoutSpeed * Time.deltaTime;
        _text.color = new Color(_red, _green, _blue, _alpha);
        transform.position += Vector3.up * _moveValue * Time.deltaTime;
        if (_alpha <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    void Init()
    {
        if (_parent.CompareTag(Tag.Player))
        {
            _red = 255f;
            _green = 0f;
            _blue = 0f;
        }
        else if (_parent.CompareTag(Tag.Enemy) || _parent.CompareTag(Tag.LargeEnemy))
        {
            _red = 0f;
            _green = 150f;
            _blue = 255f;
        }
    }

    public int DamagedValue
    {
        get { return _damagedValue; }
        set { _damagedValue = value; }
    }

    public GameObject Parent
    {
        get { return _parent; }
        set { _parent = value; }
    }
}