using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 3f; // המרחק של האוביקט מהשחקן שיחשב אינטרקציה
    public Color gizmosColor = Color.yellow;
    public Transform interactionTransform;

    bool isFocus = false; // האם האוביקט בטווח אינטרקציה מהשחקן
    Transform player;

    bool hasInteracted = false;

    public virtual void Interact ()
    {
        // יש להגדיר פונקציה זו בהתאם לאוביקט שאיתו קיימת אינטרקציה
        Debug.Log("Interacting with " + transform.name);
    }

    private void Update()
    {
        if (isFocus && !hasInteracted) // אם יש פוקוס ואין איטנרקציה
        {
            float distance = Vector3.Distance(player.position, interactionTransform.position); // המרחק בין האוביקט לשחקן
            if (distance <= radius) // אם המרחק בטווח הרדיוס
            {
                Interact();
                hasInteracted = true;
            }
        }
    }

    // כאשר האוביקט בטווח אינטרקציה מהשחקן
    public void  OnFocused (Transform playerTransdorm)
    {
        player = playerTransdorm;
        isFocus = true;
        hasInteracted = false;
    }
    
    // כאשר האוביקט מחוץ לטווח אינטרציה מהשחקן
    public void OnDefocused()
    {
        player = null;
        isFocus = false;
        hasInteracted = false;
    }

    // פונקציה שלא קשורה למשחק. משמשת כבקרה להצגת הרדיוס מהאוביקט
    void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmosColor;
        Gizmos.DrawWireSphere(interactionTransform.position, radius);
    }
}