using System;
using Rewired;
using UnityEngine;

[Serializable]
public struct PlayerCharacterInputs
{
    public float moveAxisVertical;
    public float moveAxisHorizontal;
    public bool shouldJump;
}
public class CharacterInputController : MonoBehaviour
{
    public PlayerCharacterInputs CharacterInputs;
    private const string HorizontalInput = "MoveHorizontal";
    private const string VerticalInput = "MoveVertical";
    private const string JumpInput = "Jump";

    public CharacterGameController character;

    private Player _player;

    private Vector2 _touchInput;
    private bool _jumpInput;

    private void Awake()
    {
        _player = ReInput.players.GetPlayer(0);
    }
    
    private void Update()
    {
        HandleKeyboardCharacterInput();
        
    }
    public void OnTouchJoystick(Vector2 input)
    {
        _touchInput = input;
    }

    public void OnPressJump(bool value)
    {
        _jumpInput = value;
    }
    
    
    private void HandleKeyboardCharacterInput()
    {
        CharacterInputs = new PlayerCharacterInputs
        {
            // Build the CharacterInputs struct
            moveAxisVertical = _player.GetAxis(HorizontalInput) + -_touchInput.x,
            moveAxisHorizontal = _player.GetAxis(VerticalInput)+ _touchInput.y,
            shouldJump = _player.GetButton(JumpInput) || _jumpInput
        };

        // Apply inputs to character
        character.SetInputs(ref CharacterInputs);
    }
    

}