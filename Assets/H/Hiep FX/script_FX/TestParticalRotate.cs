using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestParticalRotate : MonoBehaviour
{

	public ParticleSystem system;
	Vector3 lastEmit;
	public float delta = 1;
    public float gap = 0.5f;
    int dir = 1;
	
    void Start()
    {
        lastEmit = transform.position;
    }

    public void Update()
    {

        if (Vector3.Distance(lastEmit, transform.position) > delta)
        {
            //Gizmos.color = Color.green;
            var pos = transform.position;
            var newpos = pos + (transform.right * gap * dir);
            dir *= -1;
			
			system.startRotation = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
			
            ParticleSystem.EmitParams ep = new ParticleSystem.EmitParams();
            ep.position = newpos;
            //Debug.Log("rotation ep: "+ep.rotation);
            system.Emit(ep, 1);
            lastEmit = pos;
        }

    }
}