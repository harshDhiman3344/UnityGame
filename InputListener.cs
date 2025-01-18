using UnityEngine;
using UnityEngine.InputSystem;

public class InputListener : MonoBehaviour
{
    public Car car;
    
    // Reference to the InputActionAsset in the inspector
    public InputActionAsset inputActions;

    private InputAction throttleAction;
    private InputAction steeringAction;
    private InputAction breakingAction;

    private void OnEnable()
    {
        // Check if the InputActionAsset is assigned in the Inspector
        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset is not assigned!");
            return;
        }

        // Get the input actions from the InputActionAsset
        var carActionMap = inputActions.FindActionMap("Car");
        if (carActionMap == null)
        {
            Debug.LogError("ActionMap 'Car' not found in InputActionAsset!");
            return;
        }

        // Find the specific actions
        throttleAction = carActionMap.FindAction("Throttle");
        steeringAction = carActionMap.FindAction("Steering");
        breakingAction = carActionMap.FindAction("Break");

        // Check if any action is null
        if (throttleAction == null || steeringAction == null || breakingAction == null)
        {
            Debug.LogError("One or more actions ('Throttle', 'Steering', 'Break') not found in ActionMap 'Car'!");
            return;
        }

        // Enable the input actions
        throttleAction.Enable();
        steeringAction.Enable();
        breakingAction.Enable();

        // Subscribe to the action events
        throttleAction.performed += Throttle;
        steeringAction.performed += Steering;
        breakingAction.performed += Breaking;
    }

    private void OnDisable()
    {
        // Ensure the actions are not null before unsubscribing
        if (throttleAction != null)
            throttleAction.performed -= Throttle;
        if (steeringAction != null)
            steeringAction.performed -= Steering;
        if (breakingAction != null)
            breakingAction.performed -= Breaking;

        // Disable the input actions
        if (throttleAction != null)
            throttleAction.Disable();
        if (steeringAction != null)
            steeringAction.Disable();
        if (breakingAction != null)
            breakingAction.Disable();
    }

    private void Throttle(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();
        Debug.Log($"Throttle value received: {value}");
        car.throttle = value;
    }

    private void Steering(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();
        Debug.Log($"Steering value received: {value}");
        car.steering = value;
    }

    private void Breaking(InputAction.CallbackContext ctx)
    {
        bool value = ctx.ReadValueAsButton();
        Debug.Log($"Breaking value received: {value}");
        car.breaking = value;
    }
}
