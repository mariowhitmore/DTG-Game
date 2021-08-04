using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Checkpoint : MonoBehaviour
{
    public enum Checkpoints{Start=0, Grapple1=1}
    public Checkpoints ThisCheckpoint;
    public CheckpointIndex checkpointIndex;
    public float RespawnEscalation = 1f;
    public Renderer thisRenderer;
    public Light2D ThisLight;
    public bool IsActiveCheckpoint = false;
    public float LightingTransitionTime = 2.5f; 
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position + new Vector3(0, RespawnEscalation, 0), 4f/32);
    }
    public void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == "Player"&&!IsActiveCheckpoint) {
            IsActiveCheckpoint = true;
            // Dims Previous Checkpoint 
            if (SaveManagement.saveData.CheckpointIndex!=0) checkpointIndex.NumberToCheckpoint[SaveManagement.saveData.CheckpointIndex].Deactivate();
            
            // Update Save Slot
            SaveManagement.saveData.Position[0] = transform.position.x;
            SaveManagement.saveData.Position[1] = transform.position.y+RespawnEscalation;
            SaveManagement.saveData.CheckpointIndex = CPIToInt(ThisCheckpoint);
            
            // Glow Current Checkpoint
            StartCoroutine("PullTheLeverKronk");
        }
    }
    // Restores From Save Without Animation 
    public void SaveRestore(){
        ThisLight.intensity = 1f;
        thisRenderer.material.SetFloat("_Brightness",1f);
        IsActiveCheckpoint = true;
    }
    // Deactivate to be called from outside of current object
    public void Deactivate(){
        IsActiveCheckpoint = false;
        StartCoroutine("WrongLever");
    }

    // Checkpoint Index enum to Integer for saving to json file
    public static int CPIToInt(Checkpoints checkpointIndex) {
        switch (checkpointIndex){
            case Checkpoints.Start :
                return 0;
            case Checkpoints.Grapple1 :
                return 1;
            default :
                return 0;
        }
        
    }

    // Glow Coroutine
    IEnumerator PullTheLeverKronk(){
        
        float AnimEndPoint = Time.time + LightingTransitionTime;
        
        while (AnimEndPoint > Time.time) {
            float LV = (Time.time - AnimEndPoint)/LightingTransitionTime+1f;
            ThisLight.intensity = Mathf.Lerp(0f, 1f, LV);
            thisRenderer.material.SetFloat("_Brightness",LV);
            yield return null;
        }
        ThisLight.intensity = 1f;
        thisRenderer.material.SetFloat("_Brightness",1f);
        
        yield break;
    }
    // Dimming Coroutine
    IEnumerator WrongLever(){
        float AnimEndPoint = Time.time + LightingTransitionTime;
        while (AnimEndPoint > Time.time) {
            float LV = (Time.time - AnimEndPoint)/LightingTransitionTime+1f;
            ThisLight.intensity = Mathf.Lerp(1f, 0f, LV);
            thisRenderer.material.SetFloat("_Brightness",Mathf.Lerp(1f,0f,LV));
            yield return null;
        }
        ThisLight.intensity = 0f;
        thisRenderer.material.SetFloat("_Brightness",0);
        yield break;
    }
}