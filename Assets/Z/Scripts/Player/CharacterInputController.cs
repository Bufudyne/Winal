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

    private void Awake()
    {
        _player = ReInput.players.GetPlayer(0);
    }
    
    private void Update()
    {
        HandleCharacterInput();
    }
    
    private void HandleCharacterInput()
    {
        CharacterInputs = new PlayerCharacterInputs
        {
            // Build the CharacterInputs struct
            moveAxisVertical = _player.GetAxis(HorizontalInput),
            moveAxisHorizontal = _player.GetAxis(VerticalInput),
            shouldJump = _player.GetButton(JumpInput)
        };

        // Apply inputs to character
        character.SetInputs(ref CharacterInputs);
    }
    

}