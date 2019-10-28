using System.Collections;
using System.Collections.Generic;
using Anima2D;
using Enemy;
using UnityEngine;

public class SnowRiceFieldArm : MonoBehaviour
{
    public GameObject Attaker;

    private LargeEnemy _body;
    private SpriteMeshInstance _spriteMeshInstance;
    private Bone2D _bone2D;

    private Quaternion _roat;
    private Vector2 _pos;

    // Use this for initialization
    void Start()
    {
        _body = GameObject.Find("SnowRiceField").GetComponent<LargeEnemy>();
        _spriteMeshInstance = this.gameObject.GetComponent<SpriteMeshInstance>();
        _bone2D = _spriteMeshInstance.bones[0];
        _roat = _bone2D.transform.rotation;
        _pos = _bone2D.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        _roat = _bone2D.transform.rotation;
        _pos = _bone2D.transform.position;
    }

    public void ShotRice()
    {
        GameObject attacking = Instantiate(Attaker, _pos, _roat);
        attacking.GetComponent<Bullet>().Parent = this.gameObject;
        attacking.GetComponent<Bullet>().AttackPower = _body.AttackPower;
        attacking.GetComponent<Bullet>().IsRight = -this._body.IsRight;
    }
}