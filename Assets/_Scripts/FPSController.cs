using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerBody;

    [SerializeField]
    private Camera _fpsCam = null;

    [Range(15.0f, 90.0f)]
    [SerializeField]
    private float _maxCamRotation = 60.0f;

    [SerializeField]
    private float _maxInteractionRange = 2.0f;

    private float _bodyRot = 0;
    private float _camRot = 0;

    private Rigidbody _rigidbody;

    [Header("Movement configuration")]
    [SerializeField]
    private bool _canRun = true;

    [SerializeField]
    private float _walkingSpeed = 5.0f;

    [SerializeField]
    private float _runningSpeed = 10.0f;

    [Range(1.0f, 15.0f)]
    [SerializeField]
    private float _sensitivity = 1.0f;

    [Header("Jump configuration")]
    [SerializeField]
    private bool _canJump = true;

    [SerializeField]
    private GameObject _groundCheck = null;

    [SerializeField]
    private int _amountOfBonusJumps = 1;

    private int _currentRemainingJumps;

    [SerializeField]
    private float _jumpForce = 5.0f;

    private bool _isGrounded;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _currentRemainingJumps = _amountOfBonusJumps;
    }

    private void Update()
    {
        #region Rotation
        _bodyRot += Input.GetAxis("Mouse X");
        _camRot += Input.GetAxis("Mouse Y");

        if (_camRot != 0 || _bodyRot != 0)
        {
            RotateCamera();
        }
        #endregion

        #region Jump Checks
        if (_canJump)
        {
            if (_currentRemainingJumps <= 0 && CheckGrounded())
            {
                _currentRemainingJumps = _amountOfBonusJumps;
            }
            if (Input.GetButtonDown("Jump") && CheckJump())
            {
                Jump();
            }
        }
        #endregion

        #region Check Interaction
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteraction();
        }
        #endregion
    }

    private void FixedUpdate()
    {
        #region Walking Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (x != 0 || z != 0)
        {
            Move(x, z);
        }
        #endregion
    }

    private void Move(float xDir, float zDir)
    {
        Vector3 direction = new Vector3(xDir, 0, zDir);
        float speed = _walkingSpeed;

        if (_canRun && Input.GetKey(KeyCode.LeftShift))
        {
            speed = _runningSpeed;
        }

        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void RotateCamera()
    {
        float camLock = _maxCamRotation / _sensitivity;
        _camRot = Mathf.Clamp(_camRot, -camLock, camLock);

        Vector3 camDir = new Vector3(-_camRot, 0f, 0f) * _sensitivity;
        Vector3 bodyDir = new Vector3(0, _bodyRot, 0) * _sensitivity;

        _fpsCam.transform.localRotation = Quaternion.Euler(camDir);
        transform.rotation = Quaternion.Euler(bodyDir);
    }

    #region Jump Methods
    private bool CheckGrounded()
    {
        RaycastHit hit;
        return Physics.Raycast(_groundCheck.transform.position, _groundCheck.transform.TransformDirection(Vector3.down), out hit, 0.2f);
    }

    private bool CheckJump()
    {
        if (CheckGrounded())
        {
            _currentRemainingJumps = _amountOfBonusJumps;
            return true;
        }
        else if(_currentRemainingJumps > 0)
        {
            _currentRemainingJumps--;
            return true;
        }
        return false;
    }

    private void Jump()
    {
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
        _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }
    #endregion

    private void TryInteraction()
    {
        RaycastHit hit;

        if (Physics.Raycast(_fpsCam.transform.position, _fpsCam.transform.forward, out hit, _maxInteractionRange))
        {
            Debug.Log(hit.collider.gameObject.name);
            IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact();
                return;
            }
            else
            {
                interactable = hit.collider.GetComponentInParent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                    return;
                }
            }
        }
    }
}
