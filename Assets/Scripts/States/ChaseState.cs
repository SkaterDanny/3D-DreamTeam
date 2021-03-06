﻿using UnityEngine;
using System.Collections;

public class ChaseState : State {

    //At which speed the Enemy has to chase the Player at
	private float chaseSpeed = 10;
    private WeaponScript weaponScript;
    //Attack Range
    private float GivenDistanceToTarget = 7.5f;
	private Animator enemyAnimator;

    void Start()
    {
        weaponScript = GetComponentInChildren<WeaponScript>();
		enemyAnimator = GetComponentInChildren<Animator>();
    }

    public override void Act(){
        /*
        if (weaponScript.isAttackingGetSet)
        {
            chaseSpeed = 0;
        }
        else
        {
            chaseSpeed = 10;
        }
        */

		if(NavMeshAgentGetter().velocity != Vector3.zero){
			enemyAnimator.SetBool("isMoving", true);
		}

		if(health > 0){
        //Setting the speed and Destination on the Agent Manager for the NavMash Pathfinding
        NavMeshAgentSpeedSetter(chaseSpeed);
        NavMeshAgentDestinationSetter(targetGetter().transform.position);
        }
	}
	
	public override void Reason(){
        //Total Distance from Player to Enemy
		float distanceToTarget = Vector3.Distance(targetGetter().transform.position, transform.position);
		enemyAnimator.SetBool("isMoving", false);

        //If the Target is within Attack Range
		if(distanceToTarget < GivenDistanceToTarget)
			GetComponent<StateMachine>().SetState( StateID.Attack);
	}
}
