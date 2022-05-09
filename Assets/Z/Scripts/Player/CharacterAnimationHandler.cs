using System;
using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;
using UnityEngine;

public class CharacterAnimationHandler : MonoBehaviour
{
    
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterGameController _character;
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
    private static readonly int MoveSpeed = Animator.StringToHash("moveSpeed");

    private void Update()
    {
     _animator.SetBool(IsGrounded, _character.isGrounded);
     _animator.SetFloat(MoveSpeed, _character.currentVelocity);
    }
}