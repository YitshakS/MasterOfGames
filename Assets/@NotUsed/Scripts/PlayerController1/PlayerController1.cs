using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController1 : MonoBehaviour
{
    public LayerMask movementMask; // מפלטר כל מה שלא ניתן ללכת עליו
    public Interactable focus; // על מה ממוקד הפוקוס של השחקן

    Camera cam; // משתנה יחוס למצלמה
    PlayerMotor motor; // משתנה יחוס למנוע שמזיז את השחקן

    void Start()
    {
        cam = Camera.main;
        motor = GetComponent<PlayerMotor>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // אם הכפתור השמאלי של העכבר נלחץ
        {
            RaycastHit hitInfo; // משתנה יחוס כדי שיקבל ערך מהקריאה הבאה לפונקציה
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hitInfo, 100, movementMask))
            {
                // משתנה היחוס מקבל את הקולידר שעליו נלחץ סמן העכבר וכן את מיקומו
                Debug.Log("We hit " + hitInfo.collider.name + " " + hitInfo.point);

                // הזזת השחקן לנקודה שעליה נלחץ סמן העכבר
                motor.MoveToPoint(hitInfo.point);

                RemoveFocus();
            }
        }

        if (Input.GetMouseButtonDown(1)) // אם הכפתור הימני של העכבר נלחץ
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hitInfo, 100))
            {
                // אם הוא נלחץ על אוביקט שניתן לקיים איתו אינטרקציה
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                if (interactable)
                    SetFocus(interactable);
            }
        }
    }

    void SetFocus(Interactable newFocus)
    {
        if (newFocus != focus) // אם הפוקוס השתנה
        {
            if (focus) // וקיים פוקוס ישן
                focus.OnDefocused(); // נבטל את הישן
            focus = newFocus; // נעדכן את החדש
            motor.FollowTarget(newFocus); // ונעדכן את המנוע שמזיז את השחקן בחדש
        }
        newFocus.OnFocused(transform);
    }

    void RemoveFocus()
    {
        if (focus) // אם קיים פוקוס
        {
            focus.OnDefocused();
            focus = null;
        }
        motor.StopFollowTarget();
    }
}