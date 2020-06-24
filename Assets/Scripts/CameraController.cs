// מחלקה זו מטפלת במעקב המצלמה אחר הדמות הראשית, וכן בשליטת השחקן על המצלמה
// ניתן להזיז את המצלמה באמצעות גלגלת העכבר, וכן באמצעות חיצי ומקשי המקלדת

using UnityEngine;

public class CameraController : MonoBehaviour
{
    // (המטרה שהמצלמה עוקבת אחריה (הדמות הראשית
    public Transform target;

    // זווית נטית המצלמה כלפי מעלה/מטה
    public float rotationX = .25f;

    // מיקום המצלמה ימינה/שמאלה במעגל סביב הדמות הראשית
    public float angleCurrent = 0f; // זווית נוכחית של מיקום המצלמה ביחס לדמות הראשית
    public float angleSpeed = 5f; // מהירות סיבוב המצלמה

    // גובה המצלמה
    public float heightCurrent = .3f; // הגובה הנוכחי
    public float heightMin = 0f; // הגובה הכי נמוך
    public float heightMax = 3f; // הגובה הכי גבוה
    public float heightSpeed = .1f; // מהירות שינוי הגובה

    // זום על הדמות הראשית
    public float zoomCurrent = .4f; // הזום הנוכחי
    public float zoomMin = .4f; // הזום הכי קרוב
    public float zoomMax = 3f; // הזום הכי רחוק
    public float zoomSpeed = .1f; // מהירות שינוי הזום

    void Update()
    {
        // נותן את הזמן מהפרם הקודם עד הפרם הנוכחי Time.deltaTime 
        // (משמש לכך שהתנועה תבוצע בצורה חלקה (ולא תלויה במהירות החומרה של המחשב עליו רץ המשחק
        // 60fps ההכפלה ב 60 מגדירה 60 פרמים לשניה כלומר

        // מסובבים את המצלמה לימין הדמות הראשית D חץ ימני או מקש
        // מסובבים את המצלמה לשמאל הדמות הראשית A חץ שמאלי או מקש
        angleCurrent -= Input.GetAxis("Horizontal") * angleSpeed * Time.deltaTime * 60f;
        angleCurrent = (angleCurrent % 360f + 360f) % 360f; // 360 תחימת המספר (המעלות) בין 0 ל

        // מעלים את המצלמה W חץ למעלה או מקש
        // מורידים את המצלמה X חץ למטה או מקש
        heightCurrent += Input.GetAxis("Vertical") * heightSpeed * Time.deltaTime * 60f;
        heightCurrent = Mathf.Clamp(heightCurrent, heightMin, heightMax); // תחימת המספר שלא יחרוג מהגבולות שנקבעו

        // גלגלת עכבר לפנים מקרבת זום לדמות הראשית
        // גלגלת עכבר לאחור מרחקת זום מהדמות הראשית
        zoomCurrent -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime * 60f;
        zoomCurrent = Mathf.Clamp(zoomCurrent, zoomMin, zoomMax); // תחימת המספר שלא יחרוג מהגבולות שנקבעו
    }

    // עדכון מצלמת מעקב אחרי שכל פונקציות העדכון נקראו
    void LateUpdate()
    {

        // הגדרת מיקום המצלמה מבחינת זום וגובה
        /*
           Vector3 newPosition = target.position - target.forward * zoomCurrent + Vector3.up;
           newPosition.y = target.position.y + heightCurrent;
           transform.position = newPosition;
        */

        transform.position = new Vector3(
            (target.position - target.forward * zoomCurrent).x,
            target.position.y + heightCurrent,
            (target.position - target.forward * zoomCurrent).z
        );

        // הגדרת מיקום המצלמה במעגל שסביב הדמות הראשית
        transform.RotateAround(target.position, Vector3.up, angleCurrent);

        // הגדרה למצלמה 'להתסתכל על' הדמות הראשית
        transform.LookAt(target.position + Vector3.up * rotationX);
    }
}