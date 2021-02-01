using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Door : MonoBehaviour, IInteractable
{
    private Animator _animator;

    [SerializeField]
    private string _openStateName = "";

    [SerializeField]
    private string _closeStateName = "";

    [SerializeField]
    private bool _openByDefault = false;

    [SerializeField]
    private AudioClip _openingSound = null;

    [SerializeField]
    private AudioClip _closingSound = null;

    private AudioSource _audioSource;

    private bool _isOpen;

    private bool _isAnimating = false;

    private void Start()
    {
        _isOpen = _openByDefault;
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        if (_isAnimating)
        {
            return;
        }

        if (_isOpen)
        {
            _animator.Play(_closeStateName);
        }
        else
        {
            _animator.Play(_openStateName);
        }
        _isOpen = !_isOpen;
    }

    public void SetIsAnimating()
    {
        _isAnimating = !_isAnimating;
    }

    public void PlaySound()
    {
        if (_isOpen)
        {
            if (_openingSound !=null)
            {
                _audioSource.clip =_openingSound;
                _audioSource.Play();
            }
        }
        else
        {
            if (_closingSound != null)
            {
                _audioSource.clip = _closingSound;
                _audioSource.Play();
            }
        }
    }
}
