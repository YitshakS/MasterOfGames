// מחלקה זו מטפלת באינטראקציה בין הדמות הראשית לשאר הדמויות

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    public float interactionDistance = 1f; // המרחק המקסימלי של הדמות הראשית מאוביקט, כדי שתוכל להתקיים ביניהם אינטראקציה
    public Color gizmosColor = Color.yellow; // צבע לבדיקה באמצעות הגיזמו, של המרחק מהדמות הראשית, בו יכולה להתקיים אינטראקציה
    public float timeDialogBubbleAppears = 5f; // זמן השהייה של הצגת בועת דו שיח המכילה טקסט

    // קולות
    public AudioSource audioMarioMushroom; // מריו אוכל פטריה
    public AudioSource audioMarioCoin; // מריו תופס מטבע
    public AudioSource audioPacManGhost; // פקמן אוכל רוח רפאים

    // שיוך לדמויות ולחפצים
    public GameObject Bridge;
    public GameObject Mario;
    public GameObject Coin;
    public GameObject Mushroom;
    public GameObject PacMan;
    public GameObject Ghost;
    public GameObject Energizer;
    public GameObject Pikachu;
    public GameObject Nurse;

    // שיוך לאוביקטים שהדמות הראשית תסתכל עליהם בתחילת חלק מהאינטקארציות, כדי שהן יראו היטב לשחקן
    public GameObject LookAt1; // אינטראקציה עם מריו
    public GameObject LookAt2; // אינטראקציה עם פקמן
    public GameObject LookAt3; // אינטראקציה עם פיקאצ'ו
    public GameObject LookAt4; // אינטראקציה לפני סוף המשחק

    // שיוך לאינטראקציות של תחילת וסוף המשחק
    public GameObject gameObjectStartOfGame;
    public GameObject gameObjectEndOfGame;

    // שיוך לקנבס של כל דמות
    Canvas MarioCanvas;
    Canvas PacManCanvas;
    Canvas PikachuCanvas;
    Canvas NurseCanvas;

    // שיוך לטקסט בקנבס של כל דמות
    Text MarioText;
    Text PacManText;
    Text PikachuText;
    Text NurseText;

    // שיוך לאנימטור של כל דמות
    Animator MarioAnimator;
    Animator PacManAnimator;
    Animator GhostAnimator;
    Animator PikachuAnimator;

    // של כל דמות NavMeshAgent שיוך ל
    NavMeshAgent PlayerAgent;
    NavMeshAgent MarioAgent;
    NavMeshAgent PacManAgent;
    NavMeshAgent PikachuAgent;

    // של כל דמות NavMeshObstacle שיוך ל
    // ClickToMove השיוק לדמות הראשית נמצא במחלקה
    NavMeshObstacle MarioNavMeshObstacle;
    NavMeshObstacle PacManNavMeshObstacle;
    NavMeshObstacle PikachuNavMeshObstacle;

    // דגלים עבור פונקציות, המייצגים האם פונקציה של אינטראקציה עדיין לא החלה, באמצע, או הסתיימה
    enum FunctionState
    {
        before, middle, end
    }
    FunctionState functionGiveMarioMushroom = FunctionState.before;
    FunctionState functionGivePacManEnergizer = FunctionState.before;
    FunctionState functionGiveNursePikachu = FunctionState.before;
    FunctionState functionAllCharactersToCenter = FunctionState.before;
    FunctionState functionEndOfGame = FunctionState.before;

    // יחוס למחלקות אחרות
    CameraController CameraController;
    ClickToMove ClickToMove;
    PickDropObject PickDropObject;
    RotateTo RotateTo;

    // בדיקה באמצעות הגיזמו, של המרחק מהדמות הראשית, בו יכולה להתקיים אינטראקציה
    void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmosColor;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }

    void Start()
    {
        // שיוך לקנבס של כל דמות
        MarioCanvas = Mario.GetComponentInChildren<Canvas>();
        PacManCanvas = PacMan.GetComponentInChildren<Canvas>();
        PikachuCanvas = Pikachu.GetComponentInChildren<Canvas>();
        NurseCanvas = Nurse.GetComponentInChildren<Canvas>();

        // שיוך לטקסט בקנבס של כל דמות
        MarioText = Mario.GetComponentInChildren<Text>();
        PacManText = PacMan.GetComponentInChildren<Text>();
        PikachuText = Pikachu.GetComponentInChildren<Text>();
        NurseText = Nurse.GetComponentInChildren<Text>();

        // שיוך לאנימטור של כל דמות
        MarioAnimator = Mario.GetComponentInChildren<Animator>();
        PacManAnimator = PacMan.GetComponentInChildren<Animator>();
        GhostAnimator = Ghost.GetComponentInChildren<Animator>();
        PikachuAnimator = Pikachu.GetComponentInChildren<Animator>();

        // של כל דמות NavMeshAgent שיוך ל
        PlayerAgent = GetComponent<NavMeshAgent>();
        MarioAgent = Mario.GetComponent<NavMeshAgent>();
        PacManAgent = PacMan.GetComponent<NavMeshAgent>();
        PikachuAgent = Pikachu.GetComponent<NavMeshAgent>();

        // של כל דמות NavMeshObstacle שיוך ל 
        MarioNavMeshObstacle = Mario.GetComponent<NavMeshObstacle>();
        PacManNavMeshObstacle = PacMan.GetComponentInChildren<NavMeshObstacle>();
        PikachuNavMeshObstacle = Pikachu.GetComponent<NavMeshObstacle>();

        // יחוס למחלקות אחרות
        CameraController = Camera.main.GetComponent<CameraController>();
        ClickToMove = GetComponent<ClickToMove>();
        PickDropObject = GetComponent<PickDropObject>();
        RotateTo = GetComponent<RotateTo>();

//      StartCoroutine(Test()); // הפעלת פונקציה לבדיקת המשחק
    }

    void Update()
    {
        // אם נלחץ מקש כלשהו בתחילת המשחק
        if (gameObjectStartOfGame && Input.anyKey)
        {
            Destroy(gameObjectStartOfGame); // השמדת קנבס הפתיחה
        }

        // סיבוב בועות דו שיח למצלמה
        MarioCanvas.transform.rotation = Quaternion.LookRotation(MarioCanvas.transform.position - Camera.main.transform.position);
        PacManCanvas.transform.rotation = Quaternion.LookRotation(PacManCanvas.transform.position - Camera.main.transform.position);
        PikachuCanvas.transform.rotation = Quaternion.LookRotation(PikachuCanvas.transform.position - Camera.main.transform.position);
        NurseCanvas.transform.rotation = Quaternion.LookRotation(NurseCanvas.transform.position - Camera.main.transform.position);

        // שלב 1 - משימות: הדמות הראשית צריכה להביא לכל אחת מהדמויות אוביקט מתאים
        if (
            functionGiveMarioMushroom != FunctionState.end // אם הפטריה טרם ניתנה למריו
            ||
            functionGivePacManEnergizer != FunctionState.end // או האנרג'ייזר טרם ניתן לפקמן
            ||
            functionGiveNursePikachu != FunctionState.end // או פיקאצ'ו טרם ניתן לאחות
            )
        {
            // אם הדמות הראשית קרובה למריו
            if (Vector3.Distance(Mario.transform.position, transform.position) <= interactionDistance)
            {
                if (functionGiveMarioMushroom == FunctionState.before) // אם הפטריה טרם ניתנה למריו
                {
                    MarioText.text = ReverseString("ולהגיע למטבע\nהלוואי שיכולתי לגדול"); // הגדרת טקסט בבועת דו שיח
                    MarioCanvas.enabled = true; // הצגת הבועה
                    if (
                        !Mushroom.transform.parent // (אם הפטריה מונחת (השחקן הניח אותה
                        &&
                        Vector3.Distance(Mario.transform.position, Mushroom.transform.position) <= interactionDistance // קרוב למריו
                        )
                    {
                        StartCoroutine(GiveMarioMushroom()); // הפעלת אינטראקציית נתינת פטריה למריו
                    }
                }
            }
            else // אחרת
                MarioCanvas.enabled = false; // הסתרת הבועה

            // אם הדמות הראשית קרובה לפקמן
            if (Vector3.Distance(PacMan.transform.position, transform.position) <= interactionDistance)
            {
                if (functionGivePacManEnergizer == FunctionState.before) // אם האנרג'ייזר טרם ניתן לפקמן
                {
                    PacManText.text = ReverseString("רודפת אחרי\nרוח הרפאים הזו\nהצילו!"); // הגדרת טקסט בבועת דו שיח

                    // הזזת הבועה בהתאם לפקמן שזז במעגל
                    PacManCanvas.transform.position = new Vector3(PacMan.transform.Find("PacMan").transform.position.x,
                                                                  PacManCanvas.transform.position.y,
                                                                  PacMan.transform.Find("PacMan").transform.position.z);

                    PacManCanvas.enabled = true; // הצגת הבועה
                    if (
                        !Energizer.transform.parent // (אם האנרג'ייזר מונח (השחקן הניח אותו
                        &&
                        Vector3.Distance(PacMan.transform.position, Energizer.transform.position) <= interactionDistance // קרוב לפקמן
                        )
                    {
                        StartCoroutine(GivePacManEnergizer()); // הפעלת אינטראקציית נתינת אנרג'ייזר לפקמן
                    }
                }
            }
            else
                PacManCanvas.enabled = false; // הסתרת הבועה

            // אם הדמות הראשית קרובה לפיקאצ'ו
            if (Vector3.Distance(Pikachu.transform.position, transform.position) <= interactionDistance
                &&
                !(PickDropObject.ObjPicked && PickDropObject.ObjPicked == Pikachu) // ודמות הראשית לא מרימה את פיקאצ'ו
               )
            {
                if (
                    functionGiveNursePikachu == FunctionState.before // אם פיקאצ'ו טרם ניתן לאחות
                    &&
                    Vector3.Distance(Pikachu.transform.position, Nurse.transform.position) > interactionDistance // ופיקאצ'ו רחוק מהאחות
                   )
                {
                    PikachuText.text = ReverseString("כואב לי!\nאיי איי..."); // הגדרת טקסט בבועת דו שיח
                    PikachuCanvas.enabled = true; // הצגת הבועה
                }
            }
            else // אחרת
                PikachuCanvas.enabled = false; // הסתרת הבועה

            // אם הדמות הראשית קרובה לאחות
            if (Vector3.Distance(Nurse.transform.position, transform.position) <= interactionDistance)
            {
                if (functionGiveNursePikachu == FunctionState.before) // אם פיקאצ'ו טרם ניתן לאחות
                {
                    if (Vector3.Distance(Nurse.transform.position, Pikachu.transform.position) > interactionDistance) // אם פיקאצ'ו רחוק מהאחות
                    {
                        NurseText.text = ReverseString("במה אוכל לעזור?\nהאם אתה מרגיש טוב?\nהיי"); // הגדרת טקסט בבועת דו שיח
                        NurseCanvas.enabled = true; // הצגת הבועה
                    }
                    else // אחרת
                    {
                        if (PickDropObject.ObjPicked && PickDropObject.ObjPicked == Pikachu) // אם השחקן מרים את פיקאצ'ו
                        {
                            NurseText.text = ReverseString("הוא נראה ממש חולה\nפיקא'צו מסכנצ'יק\nאוי"); // הגדרת טקסט בבועת דו שיח
                            NurseCanvas.enabled = true; // הצגת הבועה
                        }
                        else // אם השחקן הניח את פיקאצ'ו קרוב לאחות
                        {
                            StartCoroutine(GiveNursePikachu()); // הפעלת אינטראקציית נתינת פיקאצ'ו לאחות
                        }
                    }
                }
            }
            else // אחרת
                NurseCanvas.enabled = false; // הסתרת הבועה
        }

        else // אחרת שלב 1 הסתיים
        // שלב 2 - שליחת כל הדמויות לנקודת הפרחת הכדור הפורח והצגת דו שיח
        {
            if (functionAllCharactersToCenter == FunctionState.before) // שלב 2 טרם החל
            {
                functionAllCharactersToCenter = FunctionState.middle; // סימון שהפונקציה החלה כדי שתבוצע רק פעם אחת

                ClickToMove.clickEnabled = false; // הפסקת אפשרות השחקן להזזת הדמות הראשית
                PickDropObject.PickDropEnabled = false; // הפסקת אפשרות השחקן להרמת/הנחת אוביקטים

                // שליחת כל הדמויות לנקודת הפרחת הכדור הפורח

                PlayerAgent.enabled = true;
                PlayerAgent.destination = new Vector3(-1.66f, .85f, -2.3f);

                MarioNavMeshObstacle.enabled = false;
                MarioAgent.enabled = true;
                MarioAgent.destination = new Vector3(-2f, .87f, -1.5f);

                PacManNavMeshObstacle.enabled = false;
                PacManAgent.enabled = true;
                PacManAgent.destination = new Vector3(-1.65f, .87f, -1.2f);

                PikachuNavMeshObstacle.enabled = false;
                PikachuAgent.enabled = true;
                PikachuAgent.destination = new Vector3(-1.27f, .87f, -1.4f);
            }
            else // אחרת הדמויות שולחו לנקודת הפרחת הכדור הפורח

            if ( // אם לא כל הדמויות הגיעו ליעדן
                PlayerAgent.pathPending || PlayerAgent.remainingDistance > PlayerAgent.stoppingDistance
                ||
                MarioAgent.pathPending || MarioAgent.remainingDistance > MarioAgent.stoppingDistance
                ||
                PacManAgent.pathPending || PacManAgent.remainingDistance > PacManAgent.stoppingDistance
                ||
                PikachuAgent.pathPending || PikachuAgent.remainingDistance > PikachuAgent.stoppingDistance
            )
            {
                // אם דמות נעה, הפעלת אנימציית הליכה, אחרת הפסקת אנימציית הליכה
                // ClickToMove לדמות הראשית אין צורך להגדיר זאת, כי זה כבר מוגדר במחלקה
                MarioAnimator.SetBool("walk", MarioAgent.velocity.magnitude > 0f);
                PacManAnimator.SetBool("walk", PacManAgent.velocity.magnitude > 0f);
                PikachuAnimator.SetBool("walk", PikachuAgent.velocity.magnitude > 0f);
            }
            else // כל הדמויות הגיעו ליעדן
            {
                functionAllCharactersToCenter = FunctionState.end;

                // ברגע שהדמות האחרונה מגיעה ליעדה התנאי הקודם מתקיים
                // לכן צריך להפסיק גם את אנימציית ההליכה שלה
                MarioAnimator.SetBool("walk", false);
                PacManAnimator.SetBool("walk", false);
                PikachuAnimator.SetBool("walk", false);

                if (functionEndOfGame == FunctionState.before) // אינטראקציית סיום המשחק טרם החלה
                    StartCoroutine(EndOfGame()); // הפעלת אינטראקציית סיום המשחק
            }
        }
    }

    // היפוך הטקסט שבקנבס, כי יוניטי לא תומך בכתיבה מימין לשמאל
    string ReverseString(string str)
    {
        char[] charArray = str.ToString().ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    // אינטראקציית נתינת פטריה למריו
    IEnumerator GiveMarioMushroom()
    {
        functionGiveMarioMushroom = FunctionState.middle; // סימון שהפונקציה החלה כדי שתבוצע רק פעם אחת
        ClickToMove.clickEnabled = false; // הפסקת אפשרות השחקן להזזת הדמות הראשית
        PickDropObject.PickDropEnabled = false; // הפסקת אפשרות השחקן להרמת/הנחת אוביקטים
        transform.LookAt(LookAt1.transform); // הסטת מבט הדמות הראשית לאוביקט
        CameraController.angleCurrent = 180f; // סיבוב המצלמה ביחס לדמות הראשית כך שהבועות יראו היטב
        MarioCanvas.enabled = false; // הסתרת הבועה
        MarioAnimator.SetTrigger("bigStart"); // העברת אנימציה למצב עמידה בבטלה
        yield return new WaitForSeconds(3); // השהייה
        MarioText.text = ReverseString("היא בדיוק מה שהייתי צריך\nתודה רבה על הפטריה!"); // הגדרת הטקסט
        MarioCanvas.enabled = true; // הצגת הבועה
        yield return new WaitForSeconds(timeDialogBubbleAppears); // השהייה
        MarioCanvas.enabled = false; // הסתרת הבועה
        Destroy(Mushroom); // השמדת הפטריה
        audioMarioMushroom.Play(); // ניגון קול גדילה מפטריה
        Mario.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f); // הגדלה ראשונה
        yield return new WaitForSeconds(.3f); // השהייה
        Mario.transform.localScale = new Vector3(0.030f, 0.030f, 0.030f); // הגדלה שניה
        yield return new WaitForSeconds(.3f); // השהייה
        Mario.transform.localScale = new Vector3(0.035f, 0.035f, 0.035f); // הגדלה שלישית
        yield return new WaitForSeconds(1); // השהיה
        MarioAnimator.SetTrigger("bigEnd"); // מעבר אנימציה למצב קפיצה פעם אחת ולאחר מכן למצב עמידה בבטלה
        yield return new WaitForSeconds(.7f); // השהייה
        Destroy(Coin); // השמדת המטבע
        audioMarioCoin.Play(); // ניגון קול תפיסת מטבע
        yield return new WaitForSeconds(3); // השהייה

        // החזרת הקאנבס לפרופורציה אחרי שגדל עם מריו
        MarioCanvas.transform.localScale = new Vector3(0.008f, 0.008f, 0.008f);
        MarioCanvas.transform.position = new Vector3(MarioCanvas.transform.position.x, 1.65f, MarioCanvas.transform.position.z);

        MarioText.text = ReverseString("ולחזור למשחק שלי\nגם אני רוצה לצאת מפה"); // הגדרת הטקסט
        MarioCanvas.enabled = true; // הצגת הבועה
        yield return new WaitForSeconds(timeDialogBubbleAppears); // השהייה
        MarioText.text = ReverseString("את הנסיכה פיץ'\nאני צריך להציל"); // הגדרת הטקסט
        yield return new WaitForSeconds(timeDialogBubbleAppears); // השהייה
        MarioText.text = ReverseString("אבל בעברי הייתי נגר\nכיום אני מוכר כשרברב"); // הגדרת הטקסט
        yield return new WaitForSeconds(timeDialogBubbleAppears); // השהייה
        MarioText.text = ReverseString("לך בנגרות\nאשמח לעזור"); // הגדרת הטקסט
        yield return new WaitForSeconds(timeDialogBubbleAppears); // השהייה
        MarioText.text = ReverseString("מקרשים וחבלים\nהגשר הזה עשוי"); // הגדרת הטקסט
        yield return new WaitForSeconds(timeDialogBubbleAppears); // השהייה
        MarioText.text = ReverseString("ולקשור אותו בעזרת החבלים\nאני יכול ליצור מהקרשים סל"); // הגדרת הטקסט
        yield return new WaitForSeconds(timeDialogBubbleAppears); // השהייה
        MarioCanvas.enabled = false; // הסתרת הבועה
        ClickToMove.clickEnabled = true; // ביטול הפסקת אפשרות השחקן להזזת הדמות הראשית
        PickDropObject.PickDropEnabled = true; // ביטול הפסקת אפשרות השחקן להרמת/הנחת אוביקטים
        functionGiveMarioMushroom = FunctionState.end; // סימון שהפונקציה הסתיימה כדי להמשיך לשלב הבא
    }

    // אינטראקציית נתינת אנרג'ייזר לפקמן
    IEnumerator GivePacManEnergizer()
    {
        functionGivePacManEnergizer = FunctionState.middle; // סימון שהפונקציה החלה כדי שתבוצע רק פעם אחת
        ClickToMove.clickEnabled = false; // הפסקת אפשרות השחקן להזזת הדמות הראשית
        PickDropObject.PickDropEnabled = false; // הפסקת אפשרות השחקן להרמת/הנחת אוביקטים
        transform.LookAt(PacMan.transform); // הסטת מבט הדמות הראשית לאוביקט
        CameraController.angleCurrent = 180f; // סיבוב המצלמה ביחס לדמות הראשית כך שהבועות יראו היטב
        PacManCanvas.enabled = false; // הסתרת הבועה
        PacManAnimator.applyRootMotion = false; // הפסקת הגדרה שאנימציה מזיזה את פקמן, כי בהמשך הוא צריך לנוע באמצעות הסוכן
        PacManAnimator.SetTrigger("idle"); // העברת אנימציה למצב עמידה בבטלה
        PacMan.transform.Find("PacMan").transform.localPosition = Vector3.zero; // איפוס המיקום של פקמן, שהשתנה בעקבות אנימציית הסיבוב
        PacMan.transform.Find("PacMan").transform.localRotation = Quaternion.Euler(0, 180, 0); // סיבוב פקמן ב 180 מעלות
        PacManCanvas.transform.localPosition = new Vector3(0, PacManCanvas.transform.localPosition.y, 0); // הזזת הבועה מעל פקמן
        GhostAnimator.enabled = false; // הפסקת אנימציה של רוח הרפאים
        Ghost.transform.localRotation = Quaternion.Euler(0, 60, 0); // קביעת מיקום רוח הרפאים
        PacManText.text = ReverseString("הוא בדיוק מה שהייתי צריך\nתודה רבה על האנרג'ייזר!"); // הגדרת הטקסט
        PacManCanvas.enabled = true; // הצגת הבועה
        yield return new WaitForSeconds(timeDialogBubbleAppears); // השהייה
        PacManCanvas.enabled = false; // הסתרת הבועה
        Destroy(Energizer); // השמדת הפטריה
        GhostAnimator.enabled = true; // ביטול הפסקת אנימציה של רוח הרפאים
        yield return new WaitForSeconds(3f); // השהייה
        Destroy(Ghost); // השמדת רוח הרפאים
        audioPacManGhost.Play(); // ניגון קול השמדת רוח הרפאים
        yield return new WaitForSeconds(3); // השהייה
        PacMan.transform.Find("PacMan").transform.localRotation = Quaternion.identity; // ביטול סיבוב פקמן ב 180 מעלות
//      PacMan.transform.Find("PacMan").eulerAngles = Vector3.zero;
//      PacMan.transform.Find("PacMan").rotation = Quaternion.identity;
        yield return new WaitForSeconds(3); // השהייה
        PacManText.text = ReverseString("ולחזור למשחק שלי\nגם אני רוצה לצאת מפה"); // הגדרת הטקסט
        PacManCanvas.enabled = true; // הצגת הבועה
        yield return new WaitForSeconds(timeDialogBubbleAppears); // השהייה
        PacManText.text = ReverseString("גברת פפר פקמן\nאני רוצה לחזור לאשתי"); // הגדרת הטקסט
        yield return new WaitForSeconds(timeDialogBubbleAppears); // השהייה
        PacManText.text = ReverseString("באה מפיצה שחסר בה משולש\nאומרים שההשראה לצורה שלי"); // הגדרת הטקסט
        yield return new WaitForSeconds(timeDialogBubbleAppears); // השהייה
        PacManText.text = ReverseString("בצורה המיוחדת שיש לי\nאשמח לעזור לך"); // הגדרת הטקסט
        yield return new WaitForSeconds(timeDialogBubbleAppears); // השהייה
        PacManCanvas.enabled = false; // הסתרת הבועה
        ClickToMove.clickEnabled = true; // ביטול הפסקת אפשרות השחקן להזזת הדמות הראשית
        PickDropObject.PickDropEnabled = true; // ביטול הפסקת אפשרות השחקן להרמת/הנחת אוביקטים
        functionGivePacManEnergizer = FunctionState.end; // סימון שהפונקציה הסתיימה כדי להמשיך לשלב הבא
    }

    // אינטראקציית נתינת פיקאצ'ו לאחות
    IEnumerator GiveNursePikachu()
    {
        functionGiveNursePikachu = FunctionState.middle; // סימון שהפונקציה החלה כדי שתבוצע רק פעם אחת
        ClickToMove.clickEnabled = false; // הפסקת אפשרות השחקן להזזת הדמות הראשית
        PickDropObject.PickDropEnabled = false; // הפסקת אפשרות השחקן להרמת/הנחת אוביקטים
        PickDropObject.isPikachuPickable = false; // הפסקת אפשרות השחקן להרמת פיקאצ'ו

        // הרחקת השחקן מפיקא'צו כדי שהבועות יראו היטב
        float OriginalPikachuRadius = PikachuNavMeshObstacle.radius;
        PikachuNavMeshObstacle.radius = 2f;
        yield return new WaitForSeconds(3);
        PikachuNavMeshObstacle.radius = OriginalPikachuRadius;

        transform.LookAt(LookAt3.transform); // הסטת מבט הדמות הראשית לאוביקט
        CameraController.angleCurrent = 180f; // סיבוב המצלמה ביחס לדמות הראשית כך שהבועות יראו היטב

        NurseText.text = ReverseString("עד שיחלים\nאטפל בפיקא'צו\nאל תדאג"); // הגדרת הטקסט
        yield return new WaitForSeconds(timeDialogBubbleAppears); // השהייה
        NurseCanvas.enabled = false; // הסתרת הבועה
        PikachuAnimator.SetTrigger("idle"); // העברת אנימציה למצב עמידה בבטלה 

        // העמדת פיקאצ'ו
        Pikachu.transform.eulerAngles = Vector3.zero;
        Pikachu.transform.rotation = Quaternion.identity;

        // סיבוב הפנים של פיקאצ'ו לדמות הראשית 
        RotateTo.rotateObject = Pikachu;
        RotateTo.rotateToObject = transform.gameObject;
        float OriginalRotateToSpeed = RotateTo.rotateObjectSpeed;
        RotateTo.rotateObjectSpeed = 5f;
        yield return new WaitUntil(() => !RotateTo.rotateObject);
        RotateTo.rotateObjectSpeed = OriginalRotateToSpeed;

    //  Pikachu.transform.LookAt(transform); // העמדת פיקאצ'ו וסיבוב הפנים שלו לדמות הראשית

        // פתיחת העיניים של פיקאצ'ו
        Material[] m = Pikachu.GetComponentInChildren<SkinnedMeshRenderer>().materials;
        m[3].SetTextureScale(m[3].GetTexturePropertyNames()[0], new Vector2(1f, 1f));
        m[3].SetTextureOffset(m[3].GetTexturePropertyNames()[0], new Vector2(0f, 0f));

        PikachuCanvas.GetComponent<RectTransform>().localPosition = new Vector3(0f, 3.3f, 0f); // הזזת הבועה מעל פיקאצ'ו
        PikachuText.text = ReverseString("שהצלת אותי\nתודה רבה"); // הגדרת הטקסט
        PikachuCanvas.enabled = true; // הצגת הבועה
        yield return new WaitForSeconds(timeDialogBubbleAppears); // השהייה
        PikachuText.text = ReverseString("ולחזור למשחק שלי\nלצאת מפה\nגם אני רוצה"); // הגדרת הטקסט
        yield return new WaitForSeconds(timeDialogBubbleAppears); // השהייה
        PikachuText.text = ReverseString("אש קטצ'אם\nלמאמן שלי\nאני מתגתגע"); // הגדרת הטקסט
        yield return new WaitForSeconds(timeDialogBubbleAppears); // השהייה
        PikachuText.text = ReverseString("זרמים חשמליים\nאני יודע ליצור\nאשמח לעזור לך"); // הגדרת הטקסט
        yield return new WaitForSeconds(timeDialogBubbleAppears); // השהייה
        PikachuCanvas.enabled = false; // הסתרת הבועה
        ClickToMove.clickEnabled = true; // ביטול הפסקת אפשרות השחקן להזזת הדמות הראשית
        PickDropObject.PickDropEnabled = true; // ביטול הפסקת אפשרות השחקן להרמת/הנחת אוביקטים
        functionGiveNursePikachu = FunctionState.end; // סימון שהפונקציה הסתיימה כדי להמשיך לשלב הבא
    }

    // אינטראקציית סיום המשחק
    IEnumerator EndOfGame()
    {
        functionEndOfGame = FunctionState.middle; // סימון שהפונקציה החלה כדי שתבוצע רק פעם אחת
        Destroy(Bridge); // השמדת גשר החבלים שמריו כביכול הפך לסל וקשר לכדור הפורח

        transform.LookAt(LookAt4.transform); // הסטת מבט הדמות הראשית לאוביקט
        CameraController.angleCurrent = 180f; // סיבוב המצלמה ביחס לדמות הראשית כך שהבועות יראו היטב
        CameraController.zoomCurrent = 2.55f; // זום המצלמה כך שהבועות יראו היטב

        // הסטת מבט הדמויות לדמות הראשית
        Mario.transform.LookAt(transform);
        PacMan.transform.LookAt(transform);
        Pikachu.transform.LookAt(transform);

        MarioText.text = ReverseString("לכדור הפורח\nואקשור אותו בעזרת החבלים\nאני אבנה מהגשר סל"); // הגדרת הטקסט
        MarioCanvas.enabled = true; // הצגת הבועה
        yield return new WaitForSeconds(timeDialogBubbleAppears); // השהייה
        MarioCanvas.enabled = false; // הסתרת הבועה

        PacManText.text = ReverseString("אני אהיה הכדור הפורח"); // הגדרת הטקסט
        PacManCanvas.enabled = true; // הצגת הבועה
        yield return new WaitForSeconds(timeDialogBubbleAppears); // השהייה
        PacManCanvas.enabled = false; // הסתרת הבועה

        PikachuText.text = ReverseString("כדי להרים את הכדור הפורח\nעם הזרמים החשמליים\nאני אייצר חום"); // הגדרת הטקסט
        PikachuCanvas.enabled = true; // הצגת הבועה
        yield return new WaitForSeconds(timeDialogBubbleAppears); // השהייה
        PikachuCanvas.enabled = false; // הסתרת הבועה

        yield return new WaitForSeconds(3); // השהייה

        // הסתרת אוביקטים מעולם המשחק, המופיעים בסרטון הסיום
        gameObject.SetActive(false);
        Mario.SetActive(false);
        PacMan.SetActive(false);
        Pikachu.SetActive(false);
        //Camera.main.gameObject.SetActive(false);

        // הצגת סרטון הסיום
        gameObjectEndOfGame.SetActive(true);

        functionEndOfGame = FunctionState.end; // סימון שהפונקציה הסתיימה
    }

    // פונקציה לבדיקת המשחק
    IEnumerator Test()
    {
        float OriginalTimeDialog = timeDialogBubbleAppears;
        timeDialogBubbleAppears = .1f;

        StartCoroutine(GiveMarioMushroom());
        yield return new WaitUntil(() => functionGiveMarioMushroom == FunctionState.end);

        StartCoroutine(GivePacManEnergizer());
        yield return new WaitUntil(() => functionGivePacManEnergizer == FunctionState.end);

        Vector3 OriginalPikachuposition = Pikachu.transform.position;
        StartCoroutine(GiveNursePikachu());
        yield return new WaitUntil(() => functionGiveNursePikachu == FunctionState.end);
        Pikachu.transform.position = OriginalPikachuposition;

        yield return new WaitUntil(() => functionAllCharactersToCenter == FunctionState.end);

        timeDialogBubbleAppears = OriginalTimeDialog;

        StartCoroutine(EndOfGame());
    }
}