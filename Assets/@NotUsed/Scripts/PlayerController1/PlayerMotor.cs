using UnityEngine;
using UnityEngine.AI;

/* NavMeshAgent הזזת השחקן באמצעות */

[RequireComponent(typeof(NavMeshAgent))] // באופן אוטומטי ברגע שמשתמשים בקומפוננטה NavMeshAgent הוספת
public class PlayerMotor : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform target; // המטרה שהסוכן עוקב אחריה

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (target) // עדכון הסוכן כשנבחרה מטרה
        {
            agent.SetDestination(target.position);
            FaceTarget();
        }
    }

    public void MoveToPoint (Vector3 point)
    {
        agent.SetDestination(point); // הזזת השחקן למטרה
    }

    public void FollowTarget (Interactable newTarget)
    {
        agent.stoppingDistance = newTarget.radius * .8f; // (קביעת טווח מרחק עצירת הסוכן לפני המטרה (ולא בתוכה

        agent.updateRotation = false; // אם המטרה התקרבה לסוכן מעבר לטווח הוא לא יפנה אליה פנים
                                      // FaceTarget לכן במקרה כזה נבטל את הפניית הפנים האוטומטית ונשתמש בפונקציה

        target = newTarget.interactionTransform; // הזזת הסוכן למטרה
    }

    public void StopFollowTarget()
    {
        agent.stoppingDistance = 0f;
        agent.updateRotation = true;
        target = null;
    }

    void FaceTarget () // הפונקציה מגדירה שהסוכן יפנה פניו למטרה גם אם הוא עצר בטווח שנקבע לו לפני המטרה ולאחר מכן היא זזה בתוך הטווח
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
}