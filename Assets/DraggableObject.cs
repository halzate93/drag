using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private float tileDistance = 1f;
    [SerializeField]
    private float thresholdToCenter = 0.2f;

    private Plane frontal, lateral, currentPlane;
    private Vector3 normal;
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
		SelectDirection (eventData.delta);
        if (eventData.delta.sqrMagnitude > 1f)
            MoveUnit(eventData.position);
	}
		
	public void OnEndDrag(PointerEventData eventData)
	{
        targetPosition = new Vector3(GetSnapPosition(Position.x), Position.y, GetSnapPosition(Position.z));
    }

    private float GetSnapPosition(float position)
    {
        return tileDistance * Mathf.Round(position / tileDistance);
    }

    private void MoveUnit (Vector3 position)
	{
        Ray ray = Camera.main.ScreenPointToRay(position);
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

    private void SelectDirection (Vector2 drag)
	{
		Vector3 spatialDrag = GetCameraRelativeDrag (drag);
		float lateralDot = Vector3.Dot (lateral.normal, spatialDrag);
		float frontalDot = Vector3.Dot (frontal.normal, spatialDrag);

        if (Mathf.Abs(lateralDot) < Mathf.Abs(frontalDot))
            TrySetPlaneAndNormal(ref lateral, frontal.normal);
        else
            TrySetPlaneAndNormal(ref frontal, lateral.normal);
    }

    private void TrySetPlaneAndNormal(ref Plane planeToAssign, Vector3 normalToAssign)
    {
        if(currentPlane.normal != planeToAssign.normal)
        {
            float residue = Vector3.Scale(normal, Position).magnitude % tileDistance;
            if(residue > (1-(thresholdToCenter/2)) || residue < (0 + (thresholdToCenter/2)))
                planeToAssign = SetPlaneAndNormal(planeToAssign, normalToAssign);
        }
    }

    private Plane SetPlaneAndNormal(Plane planeToAssign, Vector3 normalToAssign)
    {
        planeToAssign.SetNormalAndPosition(planeToAssign.normal, transform.position);
        currentPlane = planeToAssign;
        normal = normalToAssign;
        return planeToAssign;
    }

    private Vector3 GetCameraRelativeDrag (Vector2 drag)
	{
		return Camera.main.transform.localRotation * (Vector3) drag;
	}
}
