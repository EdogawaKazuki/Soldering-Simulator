using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.VR;
using UnityEngine.XR.Interaction.Toolkit;
using System.ComponentModel;

public class CollisionChecker : MonoBehaviour
{
    private XRBaseController controller;
    public GrabChecker grabChecker;
    private DebugText debugText;
    private int debugTextIndex;
    float vibrationIntensity = 100f;

    string debugTextPrefix = "";
    string debugTextString = "";
    // Start is called before the first frame update
    void OnEnable()
    {
        debugTextPrefix = transform.name + ":";
        // find debug text
        debugText = GameObject.Find("DebugTextHolder").GetComponent<DebugText>();
        debugTextIndex = debugText.RegisterDebugText(debugTextPrefix);

    }

    // Update is called once per frame
    void Update()
    {
        // debugText.UpdateDebugText(debugTextIndex, debugTextPrefix + debugTextString);
        debugTextString = debugTextPrefix;
        controller = grabChecker.controller;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Enter");
        // Get the collision contact point
        ContactPoint contact = collision.GetContact(0);
        
        // Calculate penetration depth (how deep the collision is)
        float penetrationDepth = Mathf.Clamp01(contact.separation * -1) * vibrationIntensity;
        vibration(penetrationDepth);
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("Collision Stay");
        // Get the collision contact point
        ContactPoint contact = collision.GetContact(0);
        float penetrationDepth = Mathf.Clamp01(contact.separation * -1) * vibrationIntensity;
        vibration(penetrationDepth);
        
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Collision Exit");
        // Stop vibration when collision ends
        if (controller != null){
            controller.SendHapticImpulse(0, 0);
        }
        debugTextString += "Collision Exit\n";
        debugText.UpdateDebugText(debugTextIndex, debugTextPrefix + debugTextString);
    }
    private void vibration(float penetrationDepth){
        
        // Calculate penetration depth (how deep the collision is)
        if(penetrationDepth > 0){
            if(penetrationDepth > 1f){
                penetrationDepth = 1f;
            }   
            if (controller != null){
                controller.SendHapticImpulse(penetrationDepth, penetrationDepth);
            }
            debugTextString += "Vibration: " + penetrationDepth + "\n";
            debugText.UpdateDebugText(debugTextIndex, debugTextPrefix + debugTextString);
        }
    }
}
