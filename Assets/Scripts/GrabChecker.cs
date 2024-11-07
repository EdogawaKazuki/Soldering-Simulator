using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabChecker : MonoBehaviour
{
    private DebugText debugText;
    private int debugTextIndex;
    private string debugTextPrefix = "";
    private string debugTextString = "";
    public XRBaseController controller;
    // Start is called before the first frame update
    void OnEnable()
    {
        debugTextPrefix = transform.name + ":";
        // find debug text
        debugText = GameObject.Find("DebugTextHolder").GetComponent<DebugText>();
        debugTextIndex = debugText.RegisterDebugText(debugTextPrefix);
        // find grab interactable
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    // Update is called once per frame
    void Update()
    {
        debugTextString = debugTextPrefix;
    }
    private void OnGrab(SelectEnterEventArgs args){
        Debug.Log("Grab");
        controller = args.interactorObject.transform.parent.GetComponent<XRBaseController>();
        debugTextString += "Grab: " + args.interactorObject.transform.parent.name + "\n";
        debugText.UpdateDebugText(debugTextIndex, debugTextString);
    }
    private void OnRelease(SelectExitEventArgs args){
        Debug.Log("Release");
        controller = null;
        debugTextString += "Release: " + args.interactorObject.transform.parent.name + "\n";
        debugText.UpdateDebugText(debugTextIndex, debugTextString);
    }
    
}
