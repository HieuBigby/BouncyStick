using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Player : MonoBehaviour
{ 
    private bool canMove;

    [SerializeField]
    private AudioClip _moveClip, _pointClip, _scoreClip, _loseClip;

    [SerializeField]
    private GameObject _explosionPrefab;
    
    [SerializeField]
    private float _speed;

    private void Awake()
    {
        canMove = false;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _ballRigidbody2D = _transBall.GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        GameManager.Instance.GameStarted += GameStarted;
        GameManager.Instance.GameEnded += OnGameEnded;
    }

    private void OnDisable()
    {
        GameManager.Instance.GameStarted -= GameStarted;
        GameManager.Instance.GameEnded -= OnGameEnded;
    }

    private void GameStarted()
    {
        canMove = true;
    }

    private void Update()
    {
        if (canMove)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0; // Ensure the z position is 0 since we're in 2D

                // Move the ball to the mouse position immediately
                _transBall.position = mousePosition;

                // Update the previous ball position
                _previousBallPosition = _transBall.position;
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0; // Ensure the z position is 0 since we're in 2D

                // Move the ball using Rigidbody2D
                _ballRigidbody2D.MovePosition(mousePosition);

                // Calculate ball velocity
                _ballVelocity = (_transBall.position - _previousBallPosition) / Time.deltaTime;
                _previousBallPosition = _transBall.position;
            }

            // Check if the stick goes out of the screen
            if (transform.position.y < Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y)
            {
                EndGame();
            }
        }
    }

    private void EndGame()
    {
        Destroy(Instantiate(_explosionPrefab, transform.position, Quaternion.identity), 3f);
        AudioManager.Instance.PlaySound(_loseClip);
        GameManager.Instance.EndGame();
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Vector2 ballPosition = _transBall.position;
            Vector2 forceDirection = ((Vector2)_previousBallPosition - ballPosition).normalized;

            // Use ball velocity to determine the force magnitude
            float forceMagnitude = _ballVelocity.magnitude * _force;
            _rigidbody2D.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);

            //Debug.Log("Add force to ball in direction: " + forceDirection + " with magnitude: " + forceMagnitude);
        }
    }

    [SerializeField] private float _force = 1f;
    [SerializeField] private Transform _transBall;
    private Vector3 _previousBallPosition;
    private Vector3 _ballVelocity;

    private Rigidbody2D _rigidbody2D;
    private Rigidbody2D _ballRigidbody2D;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(Constants.Tags.SCORE))
        {
            GameManager.Instance.UpdateScore();
            AudioManager.Instance.PlaySound(_scoreClip);
            collision.gameObject.GetComponent<Score>().OnGameEnded();
        }

        if(collision.CompareTag(Constants.Tags.OBSTACLE))
        {
            EndGame();
        }
    }

    [SerializeField] private float _destroyTime;

    public void OnGameEnded()
    {
        StartCoroutine(Rescale());
    }

    private IEnumerator Rescale()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        Vector3 scaleOffset = endScale - startScale;
        float timeElapsed = 0f;
        float speed = 1 / _destroyTime;
        var updateTime = new WaitForFixedUpdate();
        while (timeElapsed < 1f)
        {
            timeElapsed += speed * Time.fixedDeltaTime;
            transform.localScale = startScale + timeElapsed * scaleOffset;
            yield return updateTime;
        }

        Destroy(gameObject);
    }
}