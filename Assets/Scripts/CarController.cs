using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider wheelCollider_FR;
    public WheelCollider wheelCollider_FL;
    public WheelCollider wheelCollider_BR;
    public WheelCollider wheelCollider_BL;

    [Header("Wheel Mesh Transform")]
    public Transform wheelMeshTransform_FR;
    public Transform wheelMeshTransform_FL;
    public Transform wheelMeshTransform_BR;
    public Transform wheelMeshTransform_BL;

    [Header("Car Settings")]
    public float torque = 1500f;
    public float steeringAgnle = 35f;
    public float brakeTorque = 5000f;

    [Header("Car Effect")]
    public Material breakLightMaterial;
    public Light headLight_L, headLight_R;
    public float hiIntensity = 30f;
    public float loIntensity = 15f;

    [Header("Input Actions")]
    public InputActionReference moveInputAction;
    public InputActionReference breakAction;
    public InputActionReference lightActions;

    private Vector2 moveInput;
    private bool isBraking = false;
    private int lightState = 0; // 0: off, 1: low beam, 2: high beam

    private void OnEnable()
    {
        lightActions.action.Enable();
        lightActions.action.performed += ToggleLights;
    }

    private void ToggleLights(InputAction.CallbackContext context)
    {
        lightState++;

        if (lightState > 2) lightState = 0;

        switch (lightState)
        {
            case 0:
                headLight_L.intensity = 0f;
                headLight_R.intensity = 0f;
                break;
            case 1:
                headLight_L.intensity = loIntensity;
                headLight_R.intensity = loIntensity;
                break;
            case 2:
                headLight_L.intensity = hiIntensity;
                headLight_R.intensity = hiIntensity;
                break;

        }
    }

    private void Update()
    {
        moveInput = moveInputAction.action.ReadValue<Vector2>();
        isBraking = breakAction.action.IsPressed();

        WheelVisuals();


        if (isBraking)
        {
            WheelBreak(brakeTorque);
            BreakLight(true);
        }
        else
        {
            WheelBreak(0);
            BreakLight(false);
        }

        if(moveInput.y<-0.1f) BreakLight(true);
    }

    private void FixedUpdate()
    {
        float steering = moveInput.x * steeringAgnle;

        wheelCollider_FL.steerAngle = steering;
        wheelCollider_FR.steerAngle = steering;

        float motor = moveInput.y * torque;
        wheelCollider_BL.motorTorque = motor;
        wheelCollider_BR.motorTorque = motor;

    }

    private void SingleWheelUpdater(WheelCollider collider, Transform mesh)
    {
        collider.GetWorldPose(out Vector3 pos, out Quaternion rot);

        mesh.position = pos;
        mesh.rotation = rot;
    }

    private void WheelVisuals()
    {
        SingleWheelUpdater(wheelCollider_FR, wheelMeshTransform_FR);
        SingleWheelUpdater(wheelCollider_FL, wheelMeshTransform_FL);
        SingleWheelUpdater(wheelCollider_BR, wheelMeshTransform_BR);
        SingleWheelUpdater(wheelCollider_BL, wheelMeshTransform_BL);
    }

    private void WheelBreak(float breakTorque)
    {
        wheelCollider_FR.brakeTorque = breakTorque;
        wheelCollider_FL.brakeTorque = breakTorque;
        wheelCollider_BR.brakeTorque = breakTorque;
        wheelCollider_BL.brakeTorque = breakTorque;
    }

    private void BreakLight(bool isBreaking)
    {
        if (isBreaking)
        {
            breakLightMaterial.EnableKeyword("_EMISSION");
        }
        else
        {
            breakLightMaterial.DisableKeyword("_EMISSION");
        }
    }
}
