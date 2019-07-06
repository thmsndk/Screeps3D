/*
 * Moba Camera Script v1.1 
 * - created by Jacob Penner 2013
 * 
 * Notes: 
 * 	- Enabling useFixedUpdate may make camera jumpy when being locked 
 * 	to a target.
 *  - Boundaries dont restrict on the y axis
 * 
 * Plans:
 * 	- Add following of terrain for camera height
 *  - Add ability to have camera look towards a target location
 *  - Add terrain height following
 * 
 * Version:
 * v1.1 
 *  - Removed the boundary list from the MobaCamera script
 *  - Created a separate static class that will contain all boundary and do calculations.
 *  - Created a Boundary component that can be attach to a boundary that will automaticly add it to the boundary list
 *  - Added cube boundaries are able to be rotated on their Y axis
 * 	- Boundaries can now be both cubes and spheres
 *  - Added Axes and Buttons to use the Input Manager instead of KeyCodes
 *  - Added Option to turn on and off use of KeyCodes
 * 
 * v0.5
 *  -Organized Code structure
 * 	-Fixed SetCameraRotation function
 *  -Restrict Camera X rotation on range from -89 to 89
 *  -Added property for currentCameraRotation
 *  -Added property for currentCameraZoomAmount
 *  -Can now set the CameraRotation and CameraZoomAmount at runtime with the
 * corresponding properties
 * 
 * v0.4
 *  -Fixed issue with camera colliding with boundaries when locked to target
 * 
 * v0.3
 * 	-Added boundaries
 * 	-Added defualt height value to camera
 * 	-Allow Camera to Change height value form defult to the locked target's height
 * 
 * v0.2
 * 	-Changed Handling of Player Input with rotation
 *  -Changed Handling of Player Input with zoom
 * 	-fix offset calculation for rotation
 * 	-Added Helper classes for better organization
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;

////////////////////////////////////////////////////////////////////////////////////////////////////
// Helper Classes
[System.Serializable]
public class Moba_Camera_Requirements
{
	// Objects that are requirements for the script to work
	public Transform pivot 	= null;
	public Transform offset = null;
	public Camera camera 	= null;
}

[System.Serializable]
public class Moba_Camera_KeyCodes
{
	// Allows camera to be rotated while pressed
	public KeyCode RotateCamera			= KeyCode.Mouse2;
	
	// Toggle lock camera to lockTargetTransform position
	public KeyCode LockCamera			= KeyCode.L;
	
	// Lock camera to lockTargetTransform  position while being pressed
	public KeyCode characterFocus		= KeyCode.Space;
	
	// Move camera based on camera direction
	public KeyCode CameraMoveLeft		= KeyCode.LeftArrow;
	public KeyCode CameraMoveRight		= KeyCode.RightArrow;
	public KeyCode CameraMoveForward	= KeyCode.UpArrow;
	public KeyCode CameraMoveBackward	= KeyCode.DownArrow;
}

[System.Serializable]
public class Moba_Camera_Axis {
	// Input Axis
	public string DeltaScrollWheel		= "Mouse ScrollWheel";
	public string DeltaMouseHorizontal	= "Mouse X";
	public string DeltaMouseVertical	= "Mouse Y";
	
	// Allows camera to be rotated while pressed
	public string button_rotate_camera			= "Moba Rotate Camera";
	
	// Toggle lock camera to lockTargetTransform position
	public string button_lock_camera			= "Moba Lock Camera";
	
	// Lock camera to lockTargetTransform  position while being pressed
	public string button_char_focus				= "Moba Char Focus";
	
	// Move camera based on camera direction
	public string button_camera_move_left		= "Moba Camera Move Left";
	public string button_camera_move_right		= "Moba Camera Move Right";
	public string button_camera_move_forward	= "Moba Camera Move Forward";
	public string button_camera_move_backward	= "Moba Camera Move Backward";
}

[System.Serializable]
public class Moba_Camera_Inputs {
	// set to true for quick testing with keycodes
	// set to false for use with Input Manager
	public bool useKeyCodeInputs		= true;
	
	public Moba_Camera_KeyCodes keycodes			= new Moba_Camera_KeyCodes();
	public Moba_Camera_Axis	axis					= new Moba_Camera_Axis();
}

[System.Serializable]
public class Moba_Camera_Settings
{	
	// Is the camera restricted to only inside boundaries
	public bool useBoundaries			= true;
	
	// Is the camera locked to a target
	public bool cameraLocked			= false;
	
	// Target for camera to move to when locked
	public Transform lockTargetTransform	= null;
	
	// Helper classes for organization
	public Moba_Camera_Settings_Movement movement = new Moba_Camera_Settings_Movement();
	public Moba_Camera_Settings_Rotation rotation = new Moba_Camera_Settings_Rotation();
	public Moba_Camera_Settings_Zoom zoom = new Moba_Camera_Settings_Zoom();
}

[System.Serializable]
public class Moba_Camera_Settings_Movement {
	// The rate the camera will transition from its current position to target
	public float lockTransitionRate		= 0.1f;

	// How fast the camera moves
	public float cameraMovementRate		= 1.0f;
	
	// Does camera move if mouse is near the edge of the screen
	public bool edgeHoverMovement		= true;
	
	// The Distance from the edge of the screen 
	public float edgeHoverOffset		= 10.0f;
	
	// The defualt value for the height of the pivot y position
	public float defualtHeight			= 0.0f;
	
	// Will set the pivot's y position to the defualtHeight when true
	public bool useDefualtHeight 		= true;
	
	// Uses the lock targets y position when camera locked is true
	public bool useLockTargetHeight		= true;
}

[System.Serializable]
public class Moba_Camera_Settings_Rotation {
	// Zoom rate does not change based on speed of mouse
	public bool constRotationRate		= false;
	
	// Lock the rotations axies
	public bool lockRotationX 			= true;
	public bool lockRotationY 			= true;
	
	// rotation that is used when the game starts
	public Vector2 defualtRotation		= new Vector2(-45.0f, 0.0f);
	
	// How fast the camera rotates
	public Vector2 cameraRotationRate	= new Vector2(100.0f, 100.0f);
}

[System.Serializable]
public class Moba_Camera_Settings_Zoom {
	// Changed direction zoomed
	public bool invertZoom				= false;
	
	// Starting Zoom value
	public float defaultZoom			= 15.0f;
	
	// Minimum and Maximum zoom values
	public float minZoom				= 10.0f;
	public float maxZoom				= 20.0f;
	
	// How fast the camera zooms in and out
	public float zoomRate				= 10.0f;
	
	// Zoom rate does not chance based on scroll speed
	public bool constZoomRate			= false;
}

////////////////////////////////////////////////////////////////////////////////////////////////////
// Moba_Camera Class
public class Moba_Camera : MonoBehaviour {
	// Use fixed update
	public bool useFixedUpdate			= false;
	
	// Helper classes
	public Moba_Camera_Requirements requirements	= new Moba_Camera_Requirements();
	public Moba_Camera_Inputs inputs				= new Moba_Camera_Inputs();
	public Moba_Camera_Settings settings			= new Moba_Camera_Settings();
		
	// The Current Zoom value for the camera; Not shown in Inspector
	private float _currentZoomAmount			= 0.0f;
	public float currentZoomAmount {
		get {
			return _currentZoomAmount;
		}
		set {
			_currentZoomAmount = value;
			changeInCamera = true;
		}
	}
	
	// the current Camera Rotation
	private Vector2 _currentCameraRotation 	= Vector3.zero;
	public Vector3 currentCameraRotation {
		get { 
			return _currentCameraRotation; 
		}
		set {
			_currentCameraRotation = value;
			changeInCamera = true;
		}
	}
	
	// True if either the zoom amount or the rotation value changed
	private bool changeInCamera			= true;
	
	// The amount the mouse has to move before the camera is rotated 
	// Only Used when constRotation rate is true
	private float deltaMouseDeadZone 	= 0.2f;
	
	// Constant values
	private const float MAXROTATIONXAXIS = 89.0f;
	private const float MINROTATIONXAXIS = -89.0f;
	
	// Use this for initialization
	void Start () {
		
		if(!requirements.pivot || !requirements.offset || !requirements.camera) {
			string missingRequirements = "";
			if(requirements.pivot == null) {
				missingRequirements += " / Pivot";
				this.enabled = false;
			}
			
			if(requirements.offset == null) {
				missingRequirements += " / Offset";
				this.enabled = false;
			}
			
			if(requirements.camera == null) {
				missingRequirements += " / Camera";
				this.enabled = false;
			}
			Debug.LogWarning("Moba_Camera Requirements Missing" + missingRequirements + ". Add missing objects to the requirement tab under the Moba_camera script in the Inspector.");
			Debug.LogWarning("Moba_Camera script requires two empty gameobjects, Pivot and Offset, and a camera." +
				"Parent the Offset to the Pivot and the Camera to the Offset. See the Moba_Camera Readme for more information on setup.");
		}
			
		// set values to the defualt values
		_currentZoomAmount 		= settings.zoom.defaultZoom;
		_currentCameraRotation 	= settings.rotation.defualtRotation;
		
		// if using the defualt height
		if(settings.movement.useDefualtHeight && this.enabled) {
			// set the pivots height to the defualt height
			Vector3 tempPos = requirements.pivot.transform.position;
			tempPos.y = settings.movement.defualtHeight;
			requirements.pivot.transform.position = tempPos;
		}
	}
	
	// Update is called once per frame
	void Update() {
		if(!useFixedUpdate) {
			CameraUpdate();	
		}
	}
	// Update is called once per frame
	void FixedUpdate () {
		if(useFixedUpdate) {
			CameraUpdate();	
		}
	}
	
	// Called from Update or FixedUpdate Depending on value of useFixedUpdate
	void CameraUpdate()
	{	
		CalculateCameraZoom();
		
		CalculateCameraRotation();
		
		CalculateCameraMovement();
		
		CalculateCameraUpdates();
		
		CalculateCameraBoundaries();
	}
	
	void CalculateCameraZoom() {
		////////////////////////////////////////////////////////////////////////////////////////////////////
		// Camera Zoom In/Out
		float zoomChange = 0.0f;
		int inverted = 1;
		
		float mouseScrollWheel = Input.GetAxis(inputs.axis.DeltaScrollWheel);
		if(mouseScrollWheel != 0.0f) 
		{
			// Set the a camera value has changed
			changeInCamera = true;
			
			if(settings.zoom.constZoomRate) {
				if(mouseScrollWheel != 0.0) {
					if(mouseScrollWheel > 0.0) zoomChange = 1;
					else zoomChange = -1;
				}
			}
			else {
				zoomChange = mouseScrollWheel;	
			}
		}
		
		// change the zoom amount based on if zoom is inverted
		if(!settings.zoom.invertZoom) inverted = -1;
		
		_currentZoomAmount += zoomChange * settings.zoom.zoomRate * inverted * Time.deltaTime;
	}
	
	void CalculateCameraRotation() {
		////////////////////////////////////////////////////////////////////////////////////////////////////
		// Camera rotate
		float changeInRotationX = 0.0f;
		float changeInRotationY = 0.0f;
		Cursor.lockState = CursorLockMode.None;
		
		if((inputs.useKeyCodeInputs)?
			(Input.GetKey(inputs.keycodes.RotateCamera)&&inputs.useKeyCodeInputs):
			(Input.GetButton(inputs.axis.button_rotate_camera))) {
			// Lock the cursor to the center of the screen and hide the cursor
			// Screen.lockCursor = true;
			if(!settings.rotation.lockRotationX) {
				float deltaMouseVertical = Input.GetAxis(inputs.axis.DeltaMouseVertical);
				if(deltaMouseVertical != 0.0) {
					if(settings.rotation.constRotationRate) {
						if(deltaMouseVertical > deltaMouseDeadZone) changeInRotationX = 1.0f;
						else if(deltaMouseVertical < -deltaMouseDeadZone) changeInRotationX = -1.0f;
					}
					else {
						changeInRotationX = deltaMouseVertical;
					}
					changeInCamera = true;
				}
			}
			
			if(!settings.rotation.lockRotationY) {
				float deltaMouseHorizontal = Input.GetAxis(inputs.axis.DeltaMouseHorizontal);
				if(deltaMouseHorizontal != 0.0f) {
					if(settings.rotation.constRotationRate) {
						if(deltaMouseHorizontal > deltaMouseDeadZone) changeInRotationY = 1.0f;
						else if(deltaMouseHorizontal < -deltaMouseDeadZone) changeInRotationY = -1.0f;
					}
					else {
						changeInRotationY = deltaMouseHorizontal;
					}
					changeInCamera = true;
				}
			}
		}
		
		// apply change in Y rotation
		_currentCameraRotation.y += changeInRotationY * settings.rotation.cameraRotationRate.y * Time.deltaTime;
		_currentCameraRotation.x += changeInRotationX * settings.rotation.cameraRotationRate.x * Time.deltaTime;
	}
	
	void CalculateCameraMovement() {
		////////////////////////////////////////////////////////////////////////////////////////////////////
		// Camera Movement : When mouse is near the screens edge
		
		// Lock / Unlock camera movement
		if((inputs.useKeyCodeInputs)?
			(Input.GetKeyDown(inputs.keycodes.LockCamera)):
			(Input.GetButtonDown(inputs.axis.button_lock_camera)) && 
			settings.lockTargetTransform != null)
		{
			if(settings.lockTargetTransform != null) 
			{
				//flip bool value
				settings.cameraLocked = !settings.cameraLocked;
			}
		}
		
		// if camera is locked or if character focus, set move pivot to target
		if(settings.lockTargetTransform != null && (settings.cameraLocked || 
			((inputs.useKeyCodeInputs)?
				(Input.GetKey(inputs.keycodes.characterFocus)):
				(Input.GetButton(inputs.axis.button_char_focus)))))
		{
			Vector3 target = settings.lockTargetTransform.position;
			if((requirements.pivot.position - target).magnitude > 0.2f) {
				if(settings.movement.useDefualtHeight 
					&& !settings.movement.useLockTargetHeight)
				{
					target.y = settings.movement.defualtHeight;	
				}
				else if (!settings.movement.useLockTargetHeight) 
				{
					target.y = requirements.pivot.position.y;
				}
				
				// Lerp between the target and current position
				requirements.pivot.position = Vector3.Lerp(requirements.pivot.position, target, settings.movement.lockTransitionRate);
			}	
		}
		else
		{
			Vector3 movementVector = new Vector3(0,0,0);
			var activeInput = false;
			if (EventSystem.current && EventSystem.current.currentSelectedGameObject)
			{
				var input = EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
				activeInput = input && input.isFocused;
			}
			
			// Move camera when mouse is near the edge of the screen
			if((Input.mousePosition.x < settings.movement.edgeHoverOffset && settings.movement.edgeHoverMovement)
				|| ((inputs.useKeyCodeInputs)?
						(!activeInput && Input.GetKey(inputs.keycodes.CameraMoveLeft)):
						(Input.GetButton(inputs.axis.button_camera_move_left))))
			{
				movementVector += requirements.pivot.transform.right;
			}
			if((Input.mousePosition.x > Screen.width - settings.movement.edgeHoverOffset  && settings.movement.edgeHoverMovement)
				|| ((inputs.useKeyCodeInputs)?
						(!activeInput && Input.GetKey(inputs.keycodes.CameraMoveRight)):
						(Input.GetButton(inputs.axis.button_camera_move_right))))
			{
				movementVector -= requirements.pivot.transform.right;
			}
			if((Input.mousePosition.y < settings.movement.edgeHoverOffset  && settings.movement.edgeHoverMovement)
				|| ((inputs.useKeyCodeInputs)?
						(!activeInput && Input.GetKey(inputs.keycodes.CameraMoveBackward)):
						(Input.GetButton(inputs.axis.button_camera_move_backward))))
			{
				movementVector += requirements.pivot.transform.forward;
			}
			if((Input.mousePosition.y > Screen.height - settings.movement.edgeHoverOffset  && settings.movement.edgeHoverMovement)
				|| ((inputs.useKeyCodeInputs)?
						(!activeInput && Input.GetKey(inputs.keycodes.CameraMoveForward)):
						(Input.GetButton(inputs.axis.button_camera_move_forward))))
			{
				movementVector -= requirements.pivot.transform.forward;
			}
			
			requirements.pivot.position += movementVector.normalized * settings.movement.cameraMovementRate * Time.deltaTime;
			
			
			// Lerp between the z position if magnitude is greater than value
			Vector3 target = Vector3.zero;
			Vector3 current = new Vector3(0, requirements.pivot.position.y, 0);
			
			
			if(settings.movement.useDefualtHeight)
				target.y = settings.movement.defualtHeight;
			else
				target.y = requirements.pivot.position.y;
			
			if((target - current).magnitude > 0.2f){
				Vector3 shift = Vector3.Lerp(current, target, settings.movement.lockTransitionRate);
				requirements.pivot.position = new Vector3(requirements.pivot.position.x, shift.y, requirements.pivot.position.z);
			}
		}	
	}
	
	void CalculateCameraUpdates() {
		////////////////////////////////////////////////////////////////////////////////////////////////////
		// Update the camera position relative to the pivot if there was a change in the camera transforms
		
		// if there is no change in the camera exit update
		if(!changeInCamera) return;
		
		// Check if the fMaxZoomVal is greater than the fMinZoomVal
		if(settings.zoom.maxZoom < settings.zoom.minZoom)
			settings.zoom.maxZoom = settings.zoom.minZoom + 1;
		
		// Check if Camera Zoom is between the min and max
		if(_currentZoomAmount < settings.zoom.minZoom)
			_currentZoomAmount = settings.zoom.minZoom;
		if(_currentZoomAmount > settings.zoom.maxZoom)
			_currentZoomAmount = settings.zoom.maxZoom;
		
		// Restrict rotation X value
		if(_currentCameraRotation.x > MAXROTATIONXAXIS) 
			_currentCameraRotation.x = MAXROTATIONXAXIS;
		else if(_currentCameraRotation.x < MINROTATIONXAXIS)
			_currentCameraRotation.x = MINROTATIONXAXIS;
		
		// Calculate the new position of the camera
		// rotate pivot by the change int camera 
		Vector3 forwardRotation = Quaternion.AngleAxis(_currentCameraRotation.y, Vector3.up) * Vector3.forward;
		requirements.pivot.transform.rotation = Quaternion.LookRotation(forwardRotation);
		
		//requirements.pivot.transform.Rotate(Vector3.up, changeInRotationY);
		
		Vector3 CamVec = requirements.pivot.transform.TransformDirection(Vector3.forward);
		
		// Apply Camera Rotations
		CamVec = Quaternion.AngleAxis(_currentCameraRotation.x, requirements.pivot.transform.TransformDirection(Vector3.right)) * CamVec;
		//CamVec = Quaternion.AngleAxis(_currentCameraRotation.y, Vector3.up) * CamVec;
		
		// Move camera along CamVec by ZoomAmount
		requirements.offset.position = CamVec * _currentZoomAmount + requirements.pivot.position;
		
		// Make Camera look at the pivot
		requirements.offset.transform.LookAt(requirements.pivot);
		
		// reset the change in camera value to false
		changeInCamera = false;
		
	}
	
	void CalculateCameraBoundaries() {
		if(settings.useBoundaries && !
		  ((inputs.useKeyCodeInputs)?
			(Input.GetKey(inputs.keycodes.CameraMoveRight)):
			(Input.GetButton(inputs.axis.button_camera_move_right)))) 
		{
			// check if the pivot is not in a boundary
			if(!Moba_Camera_Boundaries.isPointInBoundary(requirements.pivot.position)) {
				// Get the closet boundary to the pivot
				Moba_Camera_Boundary boundary = Moba_Camera_Boundaries.GetClosestBoundary(requirements.pivot.position);
				if(boundary != null) {
					// set the pivot's position to the closet point on the boundary
					requirements.pivot.position = Moba_Camera_Boundaries.GetClosestPointOnBoundary(boundary, requirements.pivot.position);
				}
			}
		}
	}
	
	//////////////////////////////////////////////////////////////////////////////////////////
	// Class functions
	
	//////////////////////////////////////////////////////////////////////////////////////////
	// Set Variables from outside script
	public void SetTargetTransform(Transform t) {
		if(transform != null) {
			settings.lockTargetTransform = t;	
		}
	}
	
	public void SetCameraRotation(Vector2 rotation) {
		currentCameraRotation = new Vector2(rotation.x, rotation.y);
	}
	
	public void SetCameraRotation(float x, float y) {
		currentCameraRotation = new Vector2(x, y);
	}
	
	public void SetCameraZoom(float amount) {
		currentZoomAmount = amount;
	}
	
	//////////////////////////////////////////////////////////////////////////////////////////
	// Get Variables from outside script
	public Camera GetCamera() {
		return requirements.camera;
	}
}
