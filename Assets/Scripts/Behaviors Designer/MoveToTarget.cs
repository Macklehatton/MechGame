using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class MoveToTarget : Action
{
    public SharedTransform target;
    public NavMeshAgent agent;
    
	public override void OnStart()
	{
		
	}

	public override TaskStatus OnUpdate()
	{
        if (target != null)
        {
            agent.SetDestination(target.Value.position);
        }        

		return TaskStatus.Success;
	}
}