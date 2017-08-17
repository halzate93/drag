using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private Plane frontal, lateral, currentPlane, lastPlane;

	private void Awake ()
	{
		frontal = new Plane (Vector3.back, Vector3.zero);
		lateral = new Plane (Vector3.right, Vector3.zero);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		Debug.LogWarning ("Start drag");
	}

	public void OnDrag(PointerEventData eventData)
	{
		//transform.position = eventData.position;
		Vector3 displacement = SelectDirection (eventData.delta);
		if (eventData.delta.sqrMagnitude > 1f)
			MoveUnit (displacement * Mathf.Sign (eventData.delta.x));
	}
		
	public void OnEndDrag(PointerEventData eventData)
	{
		Debug.LogWarning ("End drag!");	
	}

	private void MoveUnit (Vector3 direction)
	{
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance = 0f;
        if (currentPlane.Raycast(ray, out distance))
        {
            Vector3 hitPosition = ray.GetPoint(distance);
            transform.position = new Vector3(hitPosition.x, 1f, hitPosition.z);
        }
        //transform.Translate (direction.normalized);
	}

	private Vector3 SelectDirection (Vector2 drag)
	{
		Vector3 spatialDrag = GetCameraRelativeDrag (drag);
		Debug.DrawRay (Camera.main.transform.position, spatialDrag);
		float lateralDot = Vector3.Dot (lateral.normal, spatialDrag);
		float frontalDot = Vector3.Dot (frontal.normal, spatialDrag);

        //lastPlane = currentPlane;

        if (Mathf.Abs(lateralDot) < Mathf.Abs(frontalDot))
        {
            currentPlane = lateral;
            /*
            if(currentPlane.normal != lastPlane.normal)
            {
                lastPlane.Translate()
            }
            */
            return frontal.normal;
        }
        else
        {
            currentPlane = frontal;
            return lateral.normal;
        }
    }
			
	private Vector3 GetCameraRelativeDrag (Vector2 drag)
	{
		return Camera.main.transform.localRotation * (Vector3) drag;
	}
}
