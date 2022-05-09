using System;
using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;

public enum CharacterState
{
    Intro,
    Default
}

public class CharacterGameController : MonoBehaviour, ICharacterController
{
    public KinematicCharacterMotor motor;
    [Header("Exposed Variables")] public bool isGrounded = false;
    public float currentVelocity = 0f;
    [Header("Stable Movement")] public float maxStableMoveSpeed = 10f;
    public float stableMovementSharpness = 15f;
    public float orientationSharpness = 10f;

    [Header("Air Movement")] public float maxAirMoveSpeed = 15f;
    public float airAccelerationSpeed = 15f;
    public float drag = 0.1f;

    [Header("Jumping")] public bool allowJumpingWhenSliding;
    public float jumpUpSpeed = 10f;
    public float jumpScalableForwardSpeed = 10f;
    public float jumpPreGroundingGraceTime;
    public float jumpPostGroundingGraceTime;

    [Header("Misc")] public List<Collider> ignoredColliders = new();
    public float bonusOrientationSharpness = 10f;
    public Vector3 gravity = new(0, -30f, 0);
    
    private Vector3 _internalVelocityAdd = Vector3.zero;
    private bool _isCrouching;
    private bool _jumpConsumed;
    private bool _jumpedThisFrame;
    private bool _jumpRequested;
    private Vector3 _lookInputVector;
    private Vector3 _moveInputVector;
    private bool _shouldBeCrouching;
    private float _timeSinceJumpRequested = Mathf.Infinity;
    private float _timeSinceLastAbleToJump;

    private Quaternion _tmpTransientRot;

    public CharacterState CurrentCharacterState { get; private set; }

    private void Awake()
    {
        // Handle initial state
        //TransitionToState(CharacterState.Default);
        TransitionToState(CharacterState.Default);

        // Assign the characterController to the motor
        motor.CharacterController = this;
    }

    /// <summary>
    ///     (Called by KinematicCharacterMotor during its update cycle)
    ///     This is called before the character begins its movement update
    /// </summary>
    public void BeforeCharacterUpdate(float deltaTime)
    {
    }

    /// <summary>
    ///     (Called by KinematicCharacterMotor during its update cycle)
    ///     This is where you tell your character what its rotation should be right now.
    ///     This is the ONLY place where you should set the character's rotation
    /// </summary>
    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        switch (CurrentCharacterState)
        {
            case CharacterState.Default:
            {
                if (_lookInputVector.sqrMagnitude > 0f && orientationSharpness > 0f)
                {
                    // Smoothly interpolate from current to target look direction
                    var smoothedLookInputDirection = Vector3.Slerp(motor.CharacterForward, _lookInputVector,
                        1 - Mathf.Exp(-orientationSharpness * deltaTime)).normalized;

                    // Set the current rotation (which will be used by the KinematicCharacterMotor)
                    currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, motor.CharacterUp);
                }

                var currentUp = currentRotation * Vector3.up;

                var smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up,
                    1 - Mathf.Exp(-bonusOrientationSharpness * deltaTime));
                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;


                break;
            }
        }
    }

    /// <summary>
    ///     (Called by KinematicCharacterMotor during its update cycle)
    ///     This is where you tell your character what its velocity should be right now.
    ///     This is the ONLY place where you can set the character's velocity
    /// </summary>
    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        this.currentVelocity = currentVelocity.magnitude;
        switch (CurrentCharacterState)
        {
            case CharacterState.Default:
            {
                // Ground movement
                if (motor.GroundingStatus.IsStableOnGround)
                {
                    var currentVelocityMagnitude = currentVelocity.magnitude;

                    var effectiveGroundNormal = motor.GroundingStatus.GroundNormal;

                    // Reorient velocity on slope
                    currentVelocity = motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) *
                                      currentVelocityMagnitude;

                    // Calculate target velocity
                    var inputRight = Vector3.Cross(_moveInputVector, motor.CharacterUp);
                    var reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized *
                                          _moveInputVector.magnitude;
                    var targetMovementVelocity = reorientedInput * maxStableMoveSpeed;

                    // Smooth movement Velocity
                    currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity,
                        1f - Mathf.Exp(-stableMovementSharpness * deltaTime));
                }
                // Air movement
                else
                {
                    // Add move input
                    if (_moveInputVector.sqrMagnitude > 0f)
                    {
                        var addedVelocity = _moveInputVector * (airAccelerationSpeed * deltaTime);

                        var currentVelocityOnInputsPlane =
                            Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp);

                        // Limit air velocity from inputs
                        if (currentVelocityOnInputsPlane.magnitude < maxAirMoveSpeed)
                        {
                            // clamp addedVel to make total vel not exceed max vel on inputs plane
                            var newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity,
                                maxAirMoveSpeed);
                            addedVelocity = newTotal - currentVelocityOnInputsPlane;
                        }
                        else
                        {
                            // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                            if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                                addedVelocity = Vector3.ProjectOnPlane(addedVelocity,
                                    currentVelocityOnInputsPlane.normalized);
                        }

                        // Prevent air-climbing sloped walls
                        if (motor.GroundingStatus.FoundAnyGround)
                            if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
                            {
                                var perpenticularObstructionNormal = Vector3
                                    .Cross(Vector3.Cross(motor.CharacterUp, motor.GroundingStatus.GroundNormal),
                                        motor.CharacterUp).normalized;
                                addedVelocity =
                                    Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
                            }

                        // Apply added velocity
                        currentVelocity += addedVelocity;
                    }

                    // Gravity
                    currentVelocity += gravity * deltaTime;

                    // Drag
                    currentVelocity *= 1f / (1f + drag * deltaTime);
                }

                // Handle jumping
                _jumpedThisFrame = false;
                _timeSinceJumpRequested += deltaTime;
                if (_jumpRequested)
                    // See if we actually are allowed to jump
                    if (!_jumpConsumed &&
                        ((allowJumpingWhenSliding
                             ? motor.GroundingStatus.FoundAnyGround
                             : motor.GroundingStatus.IsStableOnGround) ||
                         _timeSinceLastAbleToJump <= jumpPostGroundingGraceTime))
                    {
                        // Calculate jump direction before ungrounding
                        var jumpDirection = motor.CharacterUp;
                        if (motor.GroundingStatus.FoundAnyGround && !motor.GroundingStatus.IsStableOnGround)
                            jumpDirection = motor.GroundingStatus.GroundNormal;

                        // Makes the character skip ground probing/snapping on its next update. 
                        // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                        motor.ForceUnground();

                        // Add to the return velocity and reset jump state
                        currentVelocity += jumpDirection * jumpUpSpeed -
                                           Vector3.Project(currentVelocity, motor.CharacterUp);
                        currentVelocity += _moveInputVector * jumpScalableForwardSpeed;
                        _jumpRequested = false;
                        _jumpConsumed = true;
                        _jumpedThisFrame = true;
                    }

                // Take into account additive velocity
                if (_internalVelocityAdd.sqrMagnitude > 0f)
                {
                    currentVelocity += _internalVelocityAdd;
                    _internalVelocityAdd = Vector3.zero;
                }

                
                break;
                
            }
            case CharacterState.Intro:
                // Gravity
                currentVelocity += gravity * deltaTime;

                // Drag
                currentVelocity *= 1f / (1f + drag * deltaTime);
                break;
        }
       
    }

    /// <summary>
    ///     (Called by KinematicCharacterMotor during its update cycle)
    ///     This is called after the character has finished its movement update
    /// </summary>
    public void AfterCharacterUpdate(float deltaTime)
    {
        switch (CurrentCharacterState)
        {
            case CharacterState.Default:
            {
                // Handle jump-related values
                {
                    // Handle jumping pre-ground grace period
                    if (_jumpRequested && _timeSinceJumpRequested > jumpPreGroundingGraceTime) _jumpRequested = false;

                    if (allowJumpingWhenSliding
                            ? motor.GroundingStatus.FoundAnyGround
                            : motor.GroundingStatus.IsStableOnGround)
                    {
                        // If we're on a ground surface, reset jumping values
                        if (!_jumpedThisFrame) _jumpConsumed = false;

                        _timeSinceLastAbleToJump = 0f;
                    }
                    else
                    {
                        // Keep track of time since we were last able to jump (for grace period)
                        _timeSinceLastAbleToJump += deltaTime;
                    }
                }

                
                break;
            }
        }
    }

    public void PostGroundingUpdate(float deltaTime)
    {
        isGrounded = motor.GroundingStatus.IsStableOnGround;
        // Handle landing and leaving ground
        if (motor.GroundingStatus.IsStableOnGround && !motor.LastGroundingStatus.IsStableOnGround)
            OnLanded();
        else if (!motor.GroundingStatus.IsStableOnGround && motor.LastGroundingStatus.IsStableOnGround)
            OnLeaveStableGround();
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        if (ignoredColliders.Count == 0) return true;

        if (ignoredColliders.Contains(coll)) return false;

        return true;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
        ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
        ref HitStabilityReport hitStabilityReport)
    {
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
        Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
    }

    /// <summary>
    ///     Handles movement state transitions and enter/exit callbacks
    /// </summary>
    public void TransitionToState(CharacterState newState)
    {
        var tmpInitialState = CurrentCharacterState;
        OnStateExit(tmpInitialState, newState);
        CurrentCharacterState = newState;
        OnStateEnter(newState, tmpInitialState);
    }

    /// <summary>
    ///     Event when entering a state
    /// </summary>
    public void OnStateEnter(CharacterState state, CharacterState fromState)
    {
        switch (state)
        {
            case CharacterState.Default:
            {
                break;
            }
        }
    }

    /// <summary>
    ///     Event when exiting a state
    /// </summary>
    public void OnStateExit(CharacterState state, CharacterState toState)
    {
        switch (state)
        {
            case CharacterState.Default:
            {
                break;
            }
        }
    }

    /// <summary>
    ///     This is called every frame by ExamplePlayer in order to tell the character what its inputs are
    /// </summary>
    public void SetInputs(ref PlayerCharacterInputs inputs)
    {
        // Clamp input
        var moveInputVector =
            Vector3.ClampMagnitude(new Vector3(inputs.moveAxisHorizontal, 0f, inputs.moveAxisVertical), 1f);

        // Calculate camera direction and rotation on the character plane
        var cameraPlanarDirection =
            Vector3.ProjectOnPlane( Vector3.forward, motor.CharacterUp).normalized;
        if (cameraPlanarDirection.sqrMagnitude == 0f)
            cameraPlanarDirection = Vector3.ProjectOnPlane(Vector3.up, motor.CharacterUp)
                .normalized;

        var cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, motor.CharacterUp);

        switch (CurrentCharacterState)
        {
            case CharacterState.Default:
            {
                // Move and look inputs
                _moveInputVector = cameraPlanarRotation * moveInputVector;

                _lookInputVector = _moveInputVector.normalized;


                // Jumping input
                if (inputs.shouldJump)
                {
                    _timeSinceJumpRequested = 0f;
                    _jumpRequested = true;
                }

                // Crouching input

                break;
            }
        }
    }

    public void AddVelocity(Vector3 velocity)
    {
        switch (CurrentCharacterState)
        {
            case CharacterState.Default:
            {
                _internalVelocityAdd += velocity;
                break;
            }
        }
    }

    protected void OnLanded()
    {
    }

    protected void OnLeaveStableGround()
    {
    }
}