using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public bool isReset = false;
    [SerializeField]
    private float offsetY = -0.2f;
    [SerializeField]
    private float offsetZ = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isReset){
            ResetPOV();
        }
    }

    public void ResetPOV(){
        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + offsetY, Camera.main.transform.position.z + offsetZ);
        // transform.localRotation = Quaternion.identity;
        transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
        transform.SetParent(null);
        isReset = false;
    }
}
