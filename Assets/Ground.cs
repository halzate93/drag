using UnityEngine;

public class Ground : MonoBehaviour
{
	[SerializeField]
	private GameObject totem;

	private bool isDragging = false;
	private Vector2 startPosition;
	private Vector2 lastPosition;
	private Vector2 currentPosition;
	private Plane plane;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + Vector3.forward);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position + Vector3.right);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, transform.position + (Vector3.up/2));
	}

	private void Update()
	{
		if (isDragging)
		{
			lastPosition = currentPosition;
			currentPosition = Input.mousePosition;
			Vector2 delta = (lastPosition - currentPosition).normalized;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			float distance = 0f;
			if (plane.Raycast(ray, out distance))
			{
				Vector3 hitPosition = ray.GetPoint(distance);
				totem.transform.position = new Vector3(hitPosition.y, 0.15f, hitPosition.z);
				Debug.Log(hitPosition);
			}
		}

		if (Input.GetMouseButtonDown(0))
		{
			if (!isDragging)
			{
				isDragging = true;
				currentPosition = startPosition = Input.mousePosition;
				plane = new Plane(Camera.main.transform.forward, 0);
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			isDragging = false;
		}
	}
}