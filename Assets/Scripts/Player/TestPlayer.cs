using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTargetPosition();
    }
	public void UpdateTargetPosition()
	{
		Vector3 newPosition = Vector3.zero;

		newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		newPosition.z = 0;

        transform.position = newPosition;
    }
}
