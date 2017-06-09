using UnityEngine;
using System.Collections;
using HandyUtilities;
public class CameraBackgroundMesh : MonoBehaviour
{


	void Start ()
    {
        var cam = Camera.main;
        transform.localScale = cam.GetSize();
	}

}
