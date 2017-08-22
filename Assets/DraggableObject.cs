using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private Plane frontal, lateral, currentPlane;
    private new Rigidbody rigidbody;
    private Vector3 targetPosition;

    private Vector3 Position
    {
        get { return rigidbody.position; }
    }

	private void Awake ()
	{
        rigidbody = GetComponent<Rigidbody>();
        frontal = new Plane (Vector3.back, Vector3.zero);
		lateral = new Plane (Vector3.right, Vector3.zero);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		Debug.LogWarning ("Start drag");
    }

	public void OnDrag(PointerEventData eventData)
	{
		Vector3 displacement = SelectDirection (eventData.delta);
        if (eventData.delta.sqrMagnitude > 1f)
            MoveUnit(displacement * Mathf.Sign (eventData.delta.x));
	}
		
	public void OnEndDrag(PointerEventData eventData)
	{
        targetPosition = new Vector3(Mathf.Round(Position.x), Position.y, Mathf.Round(Position.z));
    }

    private void MoveUnit (Vector3 direction)
	{
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (currentPlane.Raycast(ray, out distance))
        {
            Vector3 hitPosition = ray.GetPoint(distance);
            targetPosition = new Vector3(hitPosition.x, 1f, hitPosition.z);
        }
	}

    private void FixedUpdate()
    {
        Vector3 movement = targetPosition - rigidbody.position;
        rigidbody.velocity = movement / Time.fixedDeltaTime;
    }

    private Vector3 SelectDirection (Vector2 drag)
	{
		Vector3 spatialDrag = GetCameraRelativeDrag (drag);
		float lateralDot = Vector3.Dot (lateral.normal, spatialDrag);
		float frontalDot = Vector3.Dot (frontal.normal, spatialDrag);
        Vector3 normal;

        if (Mathf.Abs(lateralDot) < Mathf.Abs(frontalDot))
        {
            SetCurrentPlane(ref lateral);
            normal = frontal.normal;
        }
        else
        {
            SetCurrentPlane(ref frontal);
            normal = lateral.normal;
        }

        return normal;
    }

    private void SetCurrentPlane(ref Plane planeToAssign)
    {
        planeToAssign.SetNormalAndPosition(planeToAssign.normal, transform.position);
        currentPlane = planeToAssign;
    }

    private Vector3 GetCameraRelativeDrag (Vector2 drag)
	{
		return Camera.main.transform.localRotation * (Vector3) drag;
	}
}
