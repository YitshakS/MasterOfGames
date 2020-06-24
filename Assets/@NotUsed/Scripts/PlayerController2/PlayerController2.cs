using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] float GoSpeed = 1f;
    [SerializeField] float RotateSpeed = 100f;
    [SerializeField] float JumpPoewr = 1f;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        anim.SetFloat("vertical", Input.GetAxis("Vertical") * GoSpeed * Time.deltaTime); // צעידה לפנים
        anim.SetFloat("horizontal", Input.GetAxis("Horizontal") * RotateSpeed * Time.deltaTime); // סיבוב
        anim.SetFloat("jump", Input.GetAxis("Jump") * JumpPoewr * Time.deltaTime); // קפיצה
    }
}
