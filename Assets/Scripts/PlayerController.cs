using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

//public class Test : MonoBehaviour
//{
//    Animator anim;

//    private void Start()
//    {
//        anim = GetComponent<Animator>();
//    }
//    private void Update()
//    {
//        if (Input.GetKey(KeyCode.W))
//        {
//            anim.SetBool("forward", true);
//        }        
//        else anim.SetBool("forward", false);
//        if (Input.GetKey(KeyCode.S))
//        {
//            anim.SetBool("back", true);
//        }
//        else anim.SetBool("back", false);
//        if (Input.GetKey(KeyCode.A))
//        {
//            anim.SetBool("left", true);
//        }
//        else anim.SetBool("left", false);
//        if (Input.GetKey(KeyCode.D))
//        {
//            anim.SetBool("right", true);
//        }
//        else anim.SetBool("right", false);

//    }
//}

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    List<GameObject> _guns = new List<GameObject>();

    private float speed = 10;
    float _turnSmoothTime = 0.1f;
    float _smoothVelocity;

    bool _isKill = false;
    bool _objectSpawn = false;

    Rigidbody _rb;
    Rigidbody[] _enemyRb;
    Transform _cam;
    Transform _target;
    Animator _anim;

    GameObject _ragdoll;
    GameObject _kill;

    Vector3 _enemyPosition;
    Vector3 direction;   

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GameObject.FindGameObjectWithTag("Respawn").GetComponent<Animator>();
        _cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        _target = GameObject.FindGameObjectWithTag("Enemy").transform;
        _ragdoll = GameObject.FindGameObjectWithTag("Ragdoll");
        _kill = GameObject.FindGameObjectWithTag("Text");
        _kill.SetActive(false);
        _enemyRb = _target.gameObject.GetComponentsInChildren<Rigidbody>();
        
    }
    private void Update()
    {     
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        direction = new Vector3(horizontal, 0, vertical).normalized;

        if (!_objectSpawn)
        {
            if (Vector3.Distance(transform.position, _target.position) < 8)
            {
                _kill.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _kill.SetActive(false);
                    _objectSpawn = true;
                    _isKill = true;
                    _enemyPosition = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
                }
            }
            else _kill.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (!_isKill)
        {
            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _smoothVelocity, _turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                Vector3 moveDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                
                _rb.MovePosition(transform.position + speed * moveDir.normalized * Time.deltaTime);
                _anim.SetBool("run", true);
            }
            else _anim.SetBool("run", false);
        }
        else
        {
            if (Vector3.Distance(transform.position, _target.position) > 2.5f)
            {
                speed = 15;
                _anim.SetBool("run", true);
                BodyController.FindObjectOfType<BodyController>().GetDamage(true);
                transform.LookAt(_target);
                transform.position = Vector3.MoveTowards(gameObject.transform.position, _target.position, Time.deltaTime * speed);
            }
            else
            {
                _anim.SetBool("kill", true);
                _guns[0].SetActive(true);
                _guns[1].SetActive(false);
                Invoke("ResetBool", 0.5f);
            }
        }
    }
    public void ResetBool()
    {        
        StartCoroutine("SpawnEnemy");
    }
    
    private IEnumerator SpawnEnemy()
    {
        speed = 10;
        BodyController.FindObjectOfType<BodyController>().GetDamage(false);
        foreach (Rigidbody rb in _enemyRb)
        {
            rb.isKinematic = false;
        }
        _ragdoll.GetComponent<Animator>().enabled = false;
        _isKill = false;
        _guns[1].SetActive(true);
        _guns[0].SetActive(false);
        yield return new WaitForSeconds(1f);
        _anim.SetBool("kill", false);
        yield return new WaitForSeconds(5f);
        foreach (Rigidbody rb in _enemyRb)
        {
            rb.isKinematic = true;
        }
        _ragdoll.GetComponent<Animator>().enabled = true;
        _target.position = _enemyPosition;
        _objectSpawn = false;


    }


}
