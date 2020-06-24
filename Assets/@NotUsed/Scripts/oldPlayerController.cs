using UnityEngine;

public class oldPlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 0;

    enum MoveDir
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    void Update()
    {
        GetInput();
    }

    void GetInput()
    {
        if (Input.GetAxis("Horizontal") > 0.2f)
        {
            Move(MoveDir.Right);
        }

        if (Input.GetAxis("Horizontal") < -0.2f)
        {
            Move(MoveDir.Left);
        }

        if (Input.GetAxis("Vertical") > 0.2f)
        {
            Move(MoveDir.Up);
        }
        if (Input.GetAxis("Vertical") < -0.2f)
        {
            Move(MoveDir.Down);
        }

    }

    void Move(MoveDir moveDir)
    {
        var movement = Vector3.zero;
        switch (moveDir)
        {
            case MoveDir.Down:
                movement = -transform.forward;
                break;
            case MoveDir.Up:
                movement = transform.forward;
                break;
            case MoveDir.Left:
                transform.Rotate(new Vector3(0, -1f, 0));
                break;
            case MoveDir.Right:
                transform.Rotate(new Vector3(0, 1f, 0));
                break;
            default:
                return;
        }

        transform.Translate(movement * speed * Time.deltaTime);
    }
}