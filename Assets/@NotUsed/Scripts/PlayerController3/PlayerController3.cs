using UnityEngine;

public class PlayerController3 : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] float GoSpeed = 1f;
    [SerializeField] float RotateSpeed = 100f;
 // [SerializeField] float JumpPoewr = 1f;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // עצירת אנימציות מפרם קודם
        anim.SetBool("Go", false);
        anim.SetBool("Left", false);
        anim.SetBool("Right", false);
        anim.SetBool("Jump", false);

        // קליטת מקשים מהשחקן
        float Trans = Input.GetAxis("Vertical");
        float Rotate = Input.GetAxis("Horizontal");

        // צעידה לפנים
        if (Trans > 0) {
            anim.SetBool("Go", true);
            transform.Translate(0, 0, Trans * GoSpeed * Time.deltaTime);
        }

        // סיבוב לאחור
        if (Trans < 0)
        {
            anim.SetBool("Left", true);
            transform.Rotate(0, transform.rotation.y - 180, 0);
            return;
        }

        // סיבוב לשמאל
        if (Rotate < 0)
        {
            anim.SetBool("Left", true);
            transform.Rotate(0, - RotateSpeed * Time.deltaTime, 0);
        }
 
        // סיבוב לימין
        if (Rotate > 0)
        {
            anim.SetBool("Right", true);
            transform.Rotate(0, RotateSpeed * Time.deltaTime, 0);
        }

        // קפיצה
        if (Input.GetAxis("Jump") > 0)
        {
            anim.SetBool("Jump", true);
        }
    }
}
