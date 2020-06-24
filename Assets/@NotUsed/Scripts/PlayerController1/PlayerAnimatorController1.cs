using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimatorController1 : MonoBehaviour
{

    Animator animator;
    NavMeshAgent agent;
    const float locmationAnimationSmoothTime = .1f;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float speedPercent = (agent.velocity.magnitude / agent.speed) / 10;
        animator.SetFloat("speedPercent", speedPercent, locmationAnimationSmoothTime, Time.deltaTime);
    }
}
