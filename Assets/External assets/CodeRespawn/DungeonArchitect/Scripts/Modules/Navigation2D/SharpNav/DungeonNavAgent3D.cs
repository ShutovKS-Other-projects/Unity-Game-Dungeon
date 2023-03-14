//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using SharpNav.Crowds;


namespace DungeonArchitect.Navigation {
	public class DungeonNavAgent3D : DungeonNavAgent {
		public float radius = 0.5f;
		public float height = 1f;
		public float maxAcceleration = 8;
		public float maxSpeed = 3f;	
		public float collisionQueryRange = 4;	
		public float pathOptimizationRange = 15;	
		public float separationWeight = 3;
		public float gravity = -10;
		
		// No. of updates per second
		public float updateFrequency = 2;
        float timeSinceLastNavUpdate = 0;
		
		CharacterController character;
		
		int agentId;
		DungeonNavMesh navMesh;
		bool running = true;
		
		public DungeonNavMesh NavMesh {
			get {
				return navMesh;
			}
		}
		
		Vector3 destination;
		SharpNav.Crowds.Agent agent;
		
		public override Vector3 Destination {
			get {
				return destination;
			}
			set { destination = value; }
		}
		
		public override Vector3 Velocity {
			get {
				if (agent == null) return Vector3.zero;
				return ToV3(agent.Vel);
			}
			set {
				agent.Vel = ToSV3(value);
			}
		}
		
		public override Vector3 Direction {
			get {
				return Velocity.normalized;
			}
		}

		public float DesiredSpeed {
			get {
				if (agent == null) return 0;
				return agent.DesiredSpeed; 
			}
		}
		
		void OnDrawGizmosSelected() {
			if (agent != null) {
				Gizmos.DrawSphere(ToV3(agent.Position), 1);
			}
		}
		
		// Use this for initialization
		void Start () {
			character = GetComponent<CharacterController>();
			navMesh = GameObject.FindObjectOfType<DungeonNavMesh>();
			if (navMesh == null)
			{
				Debug.LogWarning("Cannot build initialize dungeon navigation agent. No dungeon navigation object found in the scene. Drop in the DungeonNavigation prefab into the scene");
			} 
			else {
				// Place the player on the nearest valid nav mesh polygon
				PositionOnNearestNavMesh();
				
				var agentParams = new SharpNav.Crowds.AgentParams();
				agentParams.Radius = radius;
				agentParams.Height = height;
				agentParams.MaxAcceleration = maxAcceleration;
				agentParams.MaxSpeed = maxSpeed;
				agentParams.CollisionQueryRange = collisionQueryRange;
				agentParams.PathOptimizationRange = pathOptimizationRange;
				agentParams.SeparationWeight = separationWeight;
				agentParams.UpdateFlags = UpdateFlags.Separation | UpdateFlags.OptimizeTopo; 
				
				var position = transform.position;
				var sposition = ToSV3(position);
				if (navMesh.Crowd == null) {
					Debug.Log ("Navmesh not initialized properly.  Crowd is null");
					return;
				}
				agentId = navMesh.Crowd.AddAgent(sposition, agentParams);
				
				if (agentId >= 0) {
					agent = navMesh.Crowd.GetAgent(agentId);
				} else {
					Debug.Log ("Cannot create crowd nav agent");
				}
			}
		}
		
		void PositionOnNearestNavMesh() {
			var navPoint = navMesh.NavMeshQuery.FindNearestPoly(ToSV3(transform.position), ToSV3(Vector3.one * 6));
			transform.position = ToV3 (navPoint.Position);
		}
		
		public static SharpNav.Geometry.Vector3 ToSV3(Vector3 v) {
			return new SharpNav.Geometry.Vector3(v.x, v.y, v.z);
		}
		
		public static Vector3 ToV3(SharpNav.Geometry.Vector3 v) {
			return new Vector3(v.X, v.Y, v.Z);
		}
		
		public override void Stop() {
			if (agent != null) {
				var navPoint = navMesh.NavMeshQuery.FindNearestPoly(agent.Position, ToSV3(Vector3.one * 4));
				agent.Reset (navPoint.Polygon, navPoint.Position);
			}
			running = false;
		}
		
		public override void Resume() {
			running = true;
		}
		
		void Update() {
			
		}
		
		public override float GetRemainingDistance() {
			if (agent == null) return 0;
			var direction = (ToV3(agent.TargetPosition) - transform.position);
			direction.y = 0;
			return direction.magnitude;
		}
		
		// Update is called once per frame
		void FixedUpdate() {
            timeSinceLastNavUpdate += Time.fixedDeltaTime;
            bool recalculatePath = false;
            if (updateFrequency == 0)
            {
                recalculatePath = true;
            }
            else
            {
                float frameTime = 1.0f / Mathf.Max(1, updateFrequency);
                if (timeSinceLastNavUpdate >= frameTime)
                {
                    recalculatePath = true;
                    timeSinceLastNavUpdate = 0;
                }
            }
            ProcessMove(recalculatePath);
        }

        void ProcessMove(bool recalculatePath) {
			// Move the player towards the destination
			if (running && agent != null && character.enabled) {
				// Move the character using unity's physics
				
				// Reset the position of the character to the nav agent's position if they are too far away
				{
					var resetDistanceThreshold = 4;
					var distanceToNavAgent = (transform.position - ToV3(agent.Position)).magnitude;
					if (distanceToNavAgent > resetDistanceThreshold) {
						transform.position = ToV3(agent.Position);
					}
					
				}
				
				{
					var svelocity = agent.Vel;
					var velocity = ToV3 (svelocity);
					
					if (!character.isGrounded) {
						velocity += new Vector3(0, gravity, 0);
					}

					bool moved = false; //character.SimpleMove(velocity);
					if (!moved) {
                        if (recalculatePath)
                        {
                            var delta = velocity * Time.fixedDeltaTime;
                            character.Move(delta);
                        }
                        else
                        {
                            character.SimpleMove(velocity);
                        }
					}
					
					// Set the rotation
					var direction = velocity;
					direction.y = 0;
					var speedSq = direction.sqrMagnitude;
					if (speedSq > 0.01f) {
						transform.rotation = Quaternion.LookRotation(direction.normalized);
					}
				}

                if (recalculatePath)
                {
                    try
                    {
                        // Move the nav agent
                        var navPoint = navMesh.NavMeshQuery.FindNearestPoly(ToSV3(transform.position), ToSV3(Vector3.one * 2));
                        agent.Position = navPoint.Position;

                        navPoint = navMesh.NavMeshQuery.FindNearestPoly(ToSV3(destination), ToSV3(Vector3.one * 2));
                        agent.RequestMoveTarget(navPoint.Polygon, navPoint.Position);
                    }
                    catch (System.Exception)
                    {
                        //Debug.Log(e);
                    }
                }
			}
		}

	}
}
