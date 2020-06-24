// מחלקה זו מטפלת בהזזת הדמות הראשית לעבר הנקודה שעליה הקליק השחקן

using UnityEngine;
using UnityEngine.AI;

public class ClickToMove : MonoBehaviour
{
	public float walkSpeed = 1f; // מהירות הליכה
	public float jumpSpeed = .1f; // מהירות קפיצה
	public AudioSource audioWalk; // קול פסיעות על צמחיה
	public AudioSource audioWalkOnBridge; // קול פסיעות על גשר
	public AudioSource audioJump; // קול מאמץ בקפיצה
	public bool clickEnabled = true; // הפעלת/הפסקת אפשרות השחקן להזזת הדמות הראשית

	bool isOnBridge = false; // האם הדמות הראשית על גשר
	string prevMeshLinkName = ""; // תבוצע רק אנימציית קפיצה אחת, גם אם הסוכן עדיין לא סיים לעבור OffMeshLink לודא שבכל מעבר
	Animator anim; // אנימטור של הדמות הראשית הכולל גם אנימציות הליכה/קפיצה
	NavMeshAgent agent; // סוכן המוצא את המסלול ליעד באמצעות בינה מלאכותית
	RaycastHit hitInfo; // מידע על הנקודה שעליה הקליק השחקן

	void Start()
	{
		anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
	}

	void Update()
	{
		if (
			Input.GetMouseButtonDown(0) // אם נלחץ הקליק השמאלי של העכבר
			&&
			clickEnabled // ולשחקן מופעלת האפשרות להזזת הדמות הראשית
		)
		{

			// הטלת קרן מנקודת ההקלקה על המסך לנקודה בעולם התלת מימד
			// מידע כמו מקור הקרן וכיוון הקרן ScreenPointToRay מקבל מהפונקציה ray
			// Debug.Log(ray.origin + " " + ray.direction);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			// המרת הנקודה מהמסך לנקודה בעולם התלת מימד
			// מידע כמו יעד הקרן ושם העצם שאליו היא מגיעה, כלומר העצם שעליו הוקלק Raycast מקבל מהפונקציה hitInfo
			// Debug.Log(hitInfo.point + " " + hitInfo.collider.name);
			Physics.Raycast(ray, out hitInfo);

			agent.destination = hitInfo.point; // הזזת הסוכן אל היעד
		}

		// אם הסוכן לא אמור לנוע
		if (
			!agent.pathPending // התכנית לא באמצע תהליך חישוב מסלול לסוכן
			&&
			agent.remainingDistance <= agent.stoppingDistance // הסוכן הגיע ליעד או למרחק מהיעד שהוא אמור לעצור
			)
		{
			// איפוס המסלול, אחרת, אם אחרי שהוא הגיע ליעד משהו דחף אותו, הוא ינסה שוב ושוב להגיע ליעד
			agent.ResetPath();
		}

		// אם הסוכן כן אמור לנוע
		if (
			agent.remainingDistance > agent.stoppingDistance // (הסוכן לא הגיע ליעד (המרחק שנשאר גדול ממרחק העצירה
			||
			agent.velocity.sqrMagnitude > 0f // (משהו דוחף אותו (המהירות גדולה מאפס
			)
		{
			agent.isStopped = false; // הפעלת אפשרות הסוכן לזוז

			if (agent.isOnOffMeshLink) // אם הסוכן בקפיצה
			{
				// תבוצע רק אנימציית קפיצה אחת, גם אם הסוכן עדיין לא סיים לעבור OffMeshLink בכל מעבר
				if (prevMeshLinkName != agent.currentOffMeshLinkData.offMeshLink.name)
				{
					prevMeshLinkName = agent.currentOffMeshLinkData.offMeshLink.name; // זכירת שם המעבר הנוכחי לאטירציה הבאה
					audioWalk.Stop(); // הפסקת קול פסיעות על צמחיה
					audioWalkOnBridge.Stop(); // הפסקת קול פסיעות על גשר
					agent.speed = jumpSpeed; // הגדרת מהירות הסוכן למהירות קפיצה
				//	anim.applyRootMotion = true; // אנימציית הקפיצה תזיז את הסוכן
					audioJump.Play(); // הפעלת קול קפיצה
					anim.SetTrigger("jump"); // הפעלת אנימציית קפיצה
				}
			}
			else // אם הסוכן בהליכה
			{
				agent.speed = walkSpeed; // הגדרת מהירות הסוכן למהירות הליכה
				anim.SetBool("walk", true); // הפעלת אנימציית הליכה
				if (!isOnBridge && !audioWalk.isPlaying) // אם הסוכן על צמחיה (לא על גשר) ועדיין לא מתנגן קול פסיעות על צמחיה 
				{
					audioWalkOnBridge.Stop(); // הפסקת קול פסיעות על גשר
					audioWalk.Play(); // הפעלת קול פסיעות על צמחיה
				}
				if (isOnBridge && !audioWalkOnBridge.isPlaying) // אם הסוכן על גשר ועדיין לא מתנגן קול פסיעות על גשר
				{
					audioWalk.Stop(); // הפסקת קול פסיעות על צמחיה
					audioWalkOnBridge.Play(); // הפעלת קול פסיעות על גשר
				}
			}
		}
		else // אחרת הסוכן עומד
		{
			agent.isStopped = true; // הפסקת אפשרות הסוכן לזוז

			anim.SetBool("walk", false); // הפסקת אנימציית הליכה
			audioWalk.Stop(); // הפסקת קול פסיעות על צמחיה
			audioWalkOnBridge.Stop(); // הפסקת קול פסיעות על גשר

			//	עצירת מהירות גוף הדמות הראשית
			//	Rigidbody rb;
			//	rb = GetComponent<Rigidbody>();
			//	rb.velocity = Vector3.zero;
			//	rb.angularVelocity = Vector3.zero;
		}
	}

	void OnTriggerEnter(Collider other) // (אם הדמות הראשית נכנסת לגשר (לצורך שינוי קול הפסיעות
	{
		if (other.gameObject.layer == 9)
			isOnBridge = true;
	}

	void OnTriggerExit(Collider other) // (אם הדמות הראשית יוצאת מגשר (לצורך שינוי קול הפסיעות
	{
		if (other.gameObject.layer == 9)
			isOnBridge = false;
	}
}