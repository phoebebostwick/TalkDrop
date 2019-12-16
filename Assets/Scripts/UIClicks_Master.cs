 using UnityEngine;
 using UnityEngine.EventSystems;
 
 public class UIClicks_Master : MonoBehaviour, IPointerClickHandler {
     public void OnPointerClick(PointerEventData data) {
         // This will only execute if the objects collider was the first hit by the click's raycast

         
         Debug.Log(gameObject.name + ": I was clicked!");


     }
 }