using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using TMPro;

public class Solder : MonoBehaviour
{
    // Start is called before the first frame update
    public float HP = 0;
    Color originalColor;
    Color burnedColor;
    Color fullBurnedColor;
    Transform indicator;
    List<GameObject> spheres;
    List<SolderPoint> solderPoints;
    RobotConnector robotConnector;
    public bool isReset = false;
    void Start()
    {
        indicator = transform.Find("Indicator");
        originalColor = transform.GetComponent<MeshRenderer>().material.color;
        burnedColor = new Color(0.2924528f, 0.2924528f, 0.2924528f, 1f);
        fullBurnedColor = Color.red;
        spheres = new List<GameObject>();
        solderPoints = new List<SolderPoint>();
        robotConnector = GameObject.Find("RobotConnector").GetComponent<RobotConnector>();
        transform.GetComponent<MeshRenderer>().material.color = Color.Lerp(originalColor, fullBurnedColor, HP);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rayStart = transform.position - transform.localScale.y * 0.1f * 0.5f * transform.up;
        
        RaycastHit[] hits = Physics.RaycastAll(rayStart, -transform.up, 100f);
        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject.layer == 6)
                {
                    indicator.position = hit.point;
                    if (hit.transform.name.StartsWith("Cube")){
                        break;
                    }
                    if(hit.transform.name.StartsWith("HP")){
                        break;
                    }
                }
            }
        }
        else
        {
            indicator.position = rayStart + (-transform.up * 100f);
        }
        if (isReset){
            ResetGame();
            isReset = false;
        }
    }
    void OnTriggerEnter(Collider other){
        Debug.Log(other.name);
        if(other.name.Contains("Sphere")){
            Debug.Log("Trigger Enter");
            SolderPoint solderPoint = other.GetComponent<SolderPoint>();
            solderPoint.isBurned = true;
            if (!solderPoints.Contains(solderPoint)){
                solderPoints.Add(solderPoint);
            }
            if (!spheres.Contains(other.gameObject)){
                spheres.Add(other.gameObject);
            }
        }
        if(other.name.Contains("HP")){
            HP += 0.01f;
            // check if all solder points are burned
            bool allBurned = true;
            foreach (SolderPoint solderPoint in solderPoints){
                if (solderPoint.temperature < 0.99f){
                    allBurned = false;
                    break;
                }
            }
            if (solderPoints.Count == 6 && allBurned){
                robotConnector.SendStopCmd();
                ResetGame();
            }
        }
        if(other.name.Contains("Reset")){
            ResetGame();
        }
    }
    void OnTriggerStay(Collider other){
        if(other.name.Contains("Sphere")){
            SolderPoint solderPoint = other.GetComponent<SolderPoint>();
            solderPoint.temperature += 0.01f;
            Debug.Log("Trigger Stay");
            if (HP > 0f){
                HP -= 0.01f;
                transform.GetComponent<MeshRenderer>().material.color = Color.Lerp(burnedColor, originalColor, HP);
            }else{
                transform.GetComponent<MeshRenderer>().material.color = fullBurnedColor;
            }
            
        }
        if(other.name.Contains("HP")){
            HP += 0.01f;
            if (HP > 1){
                HP = 1;
            }
            transform.GetComponent<MeshRenderer>().material.color = Color.Lerp(fullBurnedColor, originalColor, HP);
        }
        
    }
    void OnTriggerExit(Collider other){
        if(other.name.Contains("Sphere")){
            Debug.Log("Trigger Exit");
            SolderPoint solderPoint = other.GetComponent<SolderPoint>();
            solderPoint.isBurned = false;
        }
        if(other.name.Contains("HP")){
            if(solderPoints.Count == 0){
                robotConnector.SendStartCmd();
            }
        }
    }
    private void ResetGame(){
        foreach (GameObject sphere in spheres){
            sphere.GetComponent<SolderPoint>().temperature = 0f;
        }
        spheres.Clear();
        solderPoints.Clear();
    }
}
