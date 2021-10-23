using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crest;

[RequireComponent(typeof(Rigidbody))] [RequireComponent(typeof(SphereCollider))]
public class FaunWaterMovingBase : MonoBehaviour
{
    [Header("Faun Base")]
    [SerializeField] private float _movingSpeed, _goingMinTime, _goingMaxTime, _rotationSpeed, 
        _minWaterDistance, _minGroundDistance;
    [SerializeField] private Vector3 _rightRotation, _leftRotation, _upRotation, _downRotation, _forwardRotation
        , _backRotation;
    private Direction _curDir;
    private float _groundHeightToEntity;
    private Rigidbody _rigidbody;
    

    protected virtual void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        FindPath();
    }

    protected virtual void Update()
    {
        var hitsInfo = Physics.RaycastAll(new Ray(transform.position, Vector2.down));
        foreach(var hitInfo in hitsInfo)
        {
            if(hitInfo.collider.TryGetComponent<Terrain>(out Terrain component))
            {
                _groundHeightToEntity = hitInfo.distance;
                break;
            }
        }

        var oceanHeight = OceanRenderer.Instance.SeaLevel;
        var oceanHeightDistance = Vector3.Distance
            (transform.position, new Vector3(transform.position.x, oceanHeight, transform.position.z));

        if(_groundHeightToEntity < _minGroundDistance)
        {
            _curDir = Direction.Up;
            StartCoroutine(RotateToPathPoint(_upRotation, Direction.Up));
        }
        else if (oceanHeightDistance < _minWaterDistance)
        {
            _curDir = Direction.Down;
            StartCoroutine(RotateToPathPoint(_downRotation, Direction.Down));
        }
    }

    protected virtual void FixedUpdate()
    {
        _rigidbody.velocity = transform.forward * _movingSpeed;
    }

    protected virtual void FindPath()
    {
        _curDir = (Direction)Random.RandomRange(0, 4);
        switch (_curDir)
        {
            case Direction.Up:
                StartCoroutine(RotateToPathPoint(_upRotation, _curDir));
                break;
            case Direction.Down:
                StartCoroutine(RotateToPathPoint(_downRotation, _curDir));
                break;
            case Direction.Right:
                StartCoroutine(RotateToPathPoint(_rightRotation, _curDir));
                break;
            case Direction.Left:
                StartCoroutine(RotateToPathPoint(_leftRotation, _curDir));
                break;
            case Direction.Forward:
                StartCoroutine(RotateToPathPoint(_forwardRotation, _curDir));
                break;
            case Direction.Back:
                StartCoroutine(RotateToPathPoint(_backRotation, _curDir));
                break;
            default:
                break;
        }
        StartCoroutine(GoingTimer(Random.Range(_goingMinTime, _goingMaxTime)));
    }

    protected virtual IEnumerator GoingTimer(float time)
    {
        yield return new WaitForSeconds(time);
        FindPath();
    }

    protected virtual IEnumerator RotateToPathPoint(Vector3 direction, Direction dir)
    {
        while(_curDir == dir)
        {
            transform.rotation =  Quaternion.Slerp(transform.rotation, Quaternion.Euler(direction), 
                Time.deltaTime * _rotationSpeed);

            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent<Terrain>(out Terrain terrain))
        {
            _curDir = Direction.Back;
            StartCoroutine(RotateToPathPoint(_backRotation, _curDir));
        }
    }
}
public enum Direction { Up, Down, Right, Left, Forward, Back}
