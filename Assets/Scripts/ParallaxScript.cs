using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxScript : MonoBehaviour
{
	public float parallaxWeight;

	void Update ()
	{
		var parallaxPosition = Camera.main.transform.position * parallaxWeight;
		var position = this.transform.position;
		position.x = parallaxPosition.x;
		this.transform.position = position;
	}
}