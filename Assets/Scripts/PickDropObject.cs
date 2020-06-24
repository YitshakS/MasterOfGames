// מחלקה זו מטפלת בהרמת והנחת חפצים ע"י הדמות הראשית

using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class PickDropObject : MonoBehaviour
{
    // אוביקטים הניתנים להרמה
    public GameObject Energizer;
    public GameObject Mushroom;
    public GameObject Pikachu;

    public bool PickDropEnabled = true; // הפעלת/הפסקת אפשרות השחקן להרמת/הנחת אוביקטים
    public bool isPikachuPickable = true; // (האם פיקאצ'ו ניתן להרמה (רק כשהוא חולה
    public float minDistanceToPick; // המרחק המינימלי של הדמות הראשית מאוביקט כדי שהיא תוכל להרימו
    Vector3 objPosition; // המיקום של אוביקט ביחס לדמות הראשית לפני ההרמה כדי לדעת באיזה גובה להניחו 
    Quaternion objRotation; // הזווית של האוביקט ביחס לדמות הראשית לפני ההרמה כדי לדעת באיזו זווית להניחו
    public GameObject ObjTopick; // אוביקט שהדמות הראשית צריכה להרים
    public GameObject ObjPicked; // אוביקט שהדמות הראשית מחזיקה
    public Transform holdPoint; // נקודת ההרמה ביד של הדמות הראשית
    Animator anim; // אנימטור של הדמות הראשית הכולל גם אנימציות הרמה/הנחה

    // יחוס למחלקות אחרות
    ClickToMove ClickToMove;
    RotateTo RotateTo;

    void Start()
    {
        anim = GetComponent<Animator>();
        ClickToMove = GetComponent<ClickToMove>();
        RotateTo = GetComponent<RotateTo>();
    }

    void Update()
    {
        // אם נלחץ מקש הרווח, המשמש להרמת והנחת חפץ
        if (
            Input.GetKeyDown(KeyCode.Space) // אם נלחץ מקש הרווח
            &&
            PickDropEnabled // ולשחקן מופעלת האפשרות להרים/להניח אוביקטים
        )
        {
            if (ObjPicked) // אם הדמות הראשית מחזיקה אוביקט
            {
                ClickToMove.clickEnabled = false; // הפסקת אפשרות השחקן להזזת הדמות הראשית
                anim.SetTrigger("put"); // הפעלת אנימציית הנחה
            }
            else // אחרת אולי הדמות הראשית צריכה להרים אוביקט ואולי היא רחוקה מידי
            {
                // בדיקה מהו האוביקט הניתן להרמה הקרוב ביותר לדמות הראשית
                // נניח שהמרחק הקרוב ביותר שמצאנו עד כה גדול ממרחק הרמה
                // ומולו נתחיל לבצע השוואות מרחקי האוביקטים הניתנים להרמה 
                float minDistance = minDistanceToPick + 1f;

                if (Energizer) // (אם האנרג'ייזר קיים (עדיין לא ניתן לפקמן
                {
                    float EnergizerDistance = Vector3.Distance(transform.position, Energizer.transform.position);
                    if (minDistance > EnergizerDistance)
                    {
                        minDistance = EnergizerDistance;
                        ObjTopick = Energizer;
                    }
                }

                if (isPikachuPickable) // (אם פיקאצ'ו בריא (עדיין לא ניתן לאחות 
                {
                    float PikachuDistance = Vector3.Distance(transform.position, Pikachu.transform.position) - 0.1f;
                    if (minDistance > PikachuDistance)
                    {
                        minDistance = PikachuDistance;
                        ObjTopick = Pikachu;
                    }
                }

                if (Mushroom) // (אם הפטריה קיימת (עדיין לא ניתנה למריו
                {
                    float MushroomDistance = Vector3.Distance(transform.position, Mushroom.transform.position);
                    if (minDistance > MushroomDistance)
                    {
                        minDistance = MushroomDistance;
                        ObjTopick = Mushroom;
                    }
                }

                // אם הסוכן במרחק הרמה מהאוביקט הקרוב אליו ביותר 
                if (minDistance <= minDistanceToPick)
                {
                    ClickToMove.clickEnabled = false; // הפסקת אפשרות השחקן להזזת הדמות הראשית

                    // המיקום והזווית של האוביקט ביחס לדמות הראשית לפני ההרמה כדי לדעת באיזו גובה וזווית להניחו
                    // זוכרים את הגובה ביחס לדמות הראשית, כי בזמן ההנחה, ישנן קרקעות בגבהים שונים
                    objPosition = transform.position - ObjTopick.transform.position;
                    objRotation = ObjTopick.transform.rotation;

                    StartCoroutine(RotateAndPick()); // סיבוב הדמות הראשית כלפי האוביקט והרמתו
                }
            }
        }
    }

    // סיבוב הדמות הראשית כלפי האוביקט והרמתו
    IEnumerator RotateAndPick()
    {
        RotateTo.rotateObject = transform.gameObject; // יש לסובב את הדמות הראשית
        RotateTo.rotateToObject = ObjTopick; // אל האוביקט שעליה להרים
        yield return new WaitUntil(() => !RotateTo.rotateObject); // המתנה עד שהדמות הראשית תסיים להסתובב אל האוביקט שעליה להרים
        anim.SetTrigger("pick"); // הפעלת אנימציית הרמה
    }

    // משתמש באותה האנימציה AnimatorController ה
    // במהירות 1 כדי להריץ כרגיל להרמה
    // במהירות 1- כדי להריץ ברוורס להנחה
    // event הפונקציה הבאה מופעלת על ידי
    // המוגדר בתוך האנימציה, באותו הפרם, הן להרמה והן להנחה
    public void AnimationEventFunction()
    {
        if (!ObjPicked) // אם הדמות הראשית לא מחזיקה אוביקט
            PickUp(); // היא תרים את האוביקט שעליה להרים
        else // אחרת
            Drop(); // היא תניח את האוביקט שהיא מחזיקה
    }

    // הרמת אוביקט
    public void PickUp()
    {
        ObjPicked = ObjTopick; // האוביקט שהדמות הראשית מרימה הוא האוביקט שעליה להרים

        if (ObjPicked.GetComponent<Animator>()) // Animator אם לאוביקט יש
            ObjPicked.GetComponent<Animator>().enabled = false; // ביטול האנימציה של האוביקט כדי שהחפץ לא יזוז ביד של הדמות הראשית

        if (ObjPicked.GetComponent<NavMeshObstacle>()) // NavMeshObstacle אם לאוביקט יש
            ObjPicked.GetComponent<NavMeshObstacle>().enabled = false; // כדי שהדמות הראשית תוכל להרים את האוביקט NavMeshObstacle ביטול

        ObjPicked.transform.SetParent(holdPoint); // הזזת האוביקט תחת מיקום האחיזה ביד הדמות הראשית
        ObjPicked.transform.localPosition = Vector3.zero; // איפוס מיקום האוביקט ביחס לנקודה האחיזה
        ObjPicked.transform.localRotation = Quaternion.identity; // איפוז זווית האוביקט ביחס לנקודת האחיזה

        ClickToMove.clickEnabled = true; // החזרת אפשרות השחקן להזזת הדמות הראשית
    }

    // הנחת אוביקט
    public void Drop()
    {
        ObjPicked.transform.parent = null; // ביטול ההגדרה שהאוביקט תחת היד הדמות הראשית

        // (השבת האוביקט לגובה הקרקע (ביחס לגובה הדמות הראשית
        ObjPicked.transform.position = new Vector3(ObjPicked.transform.position.x, transform.position.y - objPosition.y, ObjPicked.transform.position.z);

        // השבת האוביקט לזווית המקורית לפני שהורם
        ObjPicked.transform.rotation = objRotation;

        if (ObjPicked.GetComponent<NavMeshObstacle>()) // NavMeshObstacle אם לאוביקט יש
            ObjPicked.GetComponent<NavMeshObstacle>().enabled = true; // השבתו לפעולה

        if (ObjPicked.GetComponent<Animator>()) // Animator אם לחפץ יש
            ObjPicked.GetComponent<Animator>().enabled = true; // השבתו לפעולה

        ObjPicked = null; // ביטול ההגדרה שהדמות הראשית מחזיקה אוביקט

        ClickToMove.clickEnabled = true; // החזרת אפשרות השחקן להזזת הדמות הראשית
    }
}