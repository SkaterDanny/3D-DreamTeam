﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CreateAssetMenu()]
public class MeleeWeapon : Item {

	AudioSource[] audioSources;

	Moves activeMove;


	// The hitbox object with values
	[System.Serializable]
	public class Hitbox {
		public float damage; // Damage of the sphere collider
		public float delay;
		public Vector3 offset; // Offset of the sphere collider
		public Vector3 size = new Vector3(1, 1, 1); // Radius of the sphere collider
		public float maxDuration = 1; // the duration for the active hitbox
		[HideInInspector] public float duration; // the time + duration of the hitbox
		[HideInInspector] public List<Humanoid> damagedHumanoids; // A list of all the damaged humanoids so it can only hit the target ocne
		public bool linkWithNext;
		public bool combo; // If it should continue on the next hitbox instead of starting at 0 hitbox again
		public float comboTime; // the duration to trigger the net combo attack
		public int attackAnimation; // the ID of the animation for the animator controller
		public int swingSoundID; // the ID of the sound it should play once the hitbox is active
		public int hitSoundID; // the ID of the sound it should play once the hitbox is active
		[HideInInspector] public bool playingSwingSound; // To make sure the sound doesn't play multible times
		[HideInInspector] public bool playingHitSound; // To make sure the sound doesn't play multible times
	}

	// The hixboxes from the attack move to check what/where to damage
	[System.Serializable]
	public class Moves {

		[HideInInspector] public Transform caster;
		
		public float maxCooldown; // In seconds
		[HideInInspector] public float cooldown = 0;

		[SerializeField]
		public Hitbox[] hitboxes;
		int currentHitboxIndex;
		[HideInInspector] public int CurrentHitboxIndex {
			get { return currentHitboxIndex; }
			set {
				//Reset the list
				if (!hitboxes [currentHitboxIndex].linkWithNext)
					activeHitboxes = new List<Hitbox> ();

				//Set the new value
				currentHitboxIndex = value % hitboxes.Length;

				//Add the hitbox to the list
				activeHitboxes.Add( hitboxes[currentHitboxIndex] );

				//Reset hitbox values for a clean new use
				hitboxes[currentHitboxIndex].damagedHumanoids = new List<Humanoid> ();
				hitboxes [currentHitboxIndex].playingHitSound = false;
				hitboxes [currentHitboxIndex].playingSwingSound = false;

				//Set the duration of the time it was set
				hitboxes[currentHitboxIndex].duration = Time.time + hitboxes[currentHitboxIndex].maxDuration + hitboxes[currentHitboxIndex].delay;

				//Check if the hitbox has multible boxes that need to be linked
				if (hitboxes [currentHitboxIndex].linkWithNext)
					CurrentHitboxIndex++;
			}
		}
		[HideInInspector] public List<Hitbox> activeHitboxes;
	}




	// The attack moves (Edited in the Unity editor)
	[SerializeField]
	public Moves[] moves;


	public override void UpdateItem() {
		
		if (activeMove != null) {

			Active = false;
			Transform caster = activeMove.caster;
			foreach (Hitbox hitbox in activeMove.activeHitboxes) {
				
				if (Time.time > hitbox.duration - hitbox.maxDuration && Time.time < hitbox.duration) {
					Active = true;
					Debug.DrawRay (caster.position + caster.up, hitbox.size, Color.red);

					// Check for targets to hit
					RaycastHit[] hits = Physics.BoxCastAll (caster.position + caster.TransformDirection (Vector3.Scale(hitbox.offset, caster.localScale)), Vector3.Scale(hitbox.size, caster.localScale) / 2, caster.forward, new Quaternion(), (hitbox.size.z / 2) * caster.localScale.z);

					foreach (RaycastHit hit in hits) {

						// Damage the humanoid inside the target (if it exists)
						if (hit.transform != caster.transform && hit.transform.gameObject.GetComponent <Humanoid> () && hitbox.damagedHumanoids.IndexOf (hit.transform.gameObject.GetComponent <Humanoid> ()) == -1) {
							hitbox.damagedHumanoids.Add (hit.transform.gameObject.GetComponent <Humanoid> ());
							hit.transform.gameObject.GetComponent <Humanoid> ().Health -= hitbox.damage;

							// Play the audio
							if (!hitbox.playingHitSound) {
								hitbox.playingHitSound = true;
								PlayAudio (hitbox.hitSoundID);
							}
						}
					}
				}
			}
		}
	}


	//Play a Death sound once the Enemy / Player dies
	void PlayAudio(int input)
	{
		if (activeMove != null) {
			
			audioSources = activeMove.caster.GetComponents<AudioSource> ();

			if (input >= 0 && audioSources.Length > input)
				audioSources [input].Play ();
			else if (input >= 0)
				Debug.Log ("The caster does not own sound ID '" + input + "'.");
		}
	}



	//Tell the animator to play an animation
	void FireAnimation() {
		animator.SetInteger ("AttackState", activeMove.hitboxes[activeMove.CurrentHitboxIndex].attackAnimation);
		animator.SetTrigger ("Attack");
	}




	// Tell the character you want it to use this tool and what move of his weapon he should use
	public override void use (Transform caster, int toolMove) {


		if (activeMove == null || Time.time > activeMove.hitboxes[activeMove.CurrentHitboxIndex].duration + activeMove.hitboxes[activeMove.CurrentHitboxIndex].comboTime) {
			activeMove = moves [toolMove];
			activeMove.caster = caster;
			activeMove.CurrentHitboxIndex = 0;

			FireAnimation ();
		}

		if ((Time.time > activeMove.hitboxes[activeMove.CurrentHitboxIndex].duration && Time.time <= activeMove.hitboxes[activeMove.CurrentHitboxIndex].duration + activeMove.hitboxes[activeMove.CurrentHitboxIndex].comboTime)) {
			
			if (activeMove.hitboxes [activeMove.CurrentHitboxIndex].combo) {
				activeMove.CurrentHitboxIndex++;
			
				FireAnimation ();
			}
		}
	}
}