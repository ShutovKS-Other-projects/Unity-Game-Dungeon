//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using SharpNav.Crowds;

namespace DungeonArchitect.Navigation {
	public class DungeonNavAgent2D : DungeonNavAgent {
		public float radius = 0.5f;
		public float height = 1f;
		public float maxAcceleration = 8;
		public float maxSpeed = 3f;	
		public float collisionQueryRange = 4;	
		public float pathOptimizationRange = 15;	
		public float separationWeight = 3;
		public Vector2 navAgentCollisionOffset;

		// No. of updates per second
		public float updateFrequency = 2;

		int agentId;
		DungeonNavMesh navMesh;
		Rigidbody2D rigidBody2D;
		bool running = true;
		Vector3 previousDirection = Vector3.zero;

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
				return previousDirection;
			}
		}

		public float DesiredSpeed {
			get {
				if (agent == null) return 0;
				return agent.DesiredSpeed; 
			}
		}

		Vector3 _debugNavDest = Vector3.zero;
		void OnDrawGizmosSelected() {
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(destination, 0.1f);
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(FlipYZ(_debugNavDest), 0.1f);
			if (agent != null) {
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(FlipYZ(ToV3(agent.Position)), radius);
			}
		}

		Vector3 FlipYZ(Vector3 v) {
			return new Vector3(v.x, v.z, v.y);
		}

		void Awake() {
			rigidBody2D = GetComponent<Rigidbody2D>();
			if (rigidBody2D == null) {
				Debug.LogWarning("Rigid Body 2D not assigned to nav agent game object");
			}
		}

		// Use this for initialization
		void Start () {
			//transform.position = FlipYZ(transform.position);
            navMesh = GameObject.FindObjectOfType<DungeonNavMesh>();
			if (navMesh == null || navMesh.NavMeshQuery == null)
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

				var position = ActorPosition3D + new Vector3(navAgentCollisionOffset.x, 0, navAgentCollisionOffset.y);
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

		Vector3 ActorPosition3DX {
			get { return (transform.position); }
			set { 
				transform.position = value;
			}
		}

		Vector3 ActorPosition3D {
			get { return FlipYZ(transform.position); }
			set { 
				var flipped = FlipYZ (value);
				flipped.z = 0;
				transform.position = flipped;
			}
		}


		void PositionOnNearestNavMesh() {
			var navPoint = navMesh.NavMeshQuery.FindNearestPoly(ToSV3(ActorPosition3D), ToSV3(Vector3.one * 6));
			ActorPosition3D = ToV3 (navPoint.Position);
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
			rigidBody2D.velocity = Vector2.zero;
			running = false;
		}

		public override void Resume() {
			running = true;
		}

		void Update() {

		}

		public override float GetRemainingDistance() {
			if (agent == null) return 0;
			var direction = (ToV3(agent.TargetPosition) - ActorPosition3D);
			direction.y = 0;
			return direction.magnitude;
		}

		// Update is called once per frame
		void FixedUpdate() {
			// Move the player towards the destination
			if (running && agent != null) {
				// Move the character using unity's physics

				// Reset the position of the character to the nav agent's position if they are too far away
				{
					var resetDistanceThreshold = 4;
					var distanceToNavAgent = (ActorPosition3D - ToV3(agent.Position)).magnitude;
					if (distanceToNavAgent > resetDistanceThreshold) {
						ActorPosition3D = ToV3(agent.Position);
					}

				}

				{
					var svelocity = agent.Vel;
					var velocity = ToV3 (svelocity);

					rigidBody2D.velocity = FlipYZ(velocity);

					if (velocity.sqrMagnitude > 0.01f) {
						previousDirection = velocity.normalized;
					}
				}

				try {
					// Move the nav agent
					var navPoint = navMesh.NavMeshQuery.FindNearestPoly(ToSV3(ActorPosition3D), ToSV3(Vector3.one * 2));
					agent.Position = navPoint.Position;

					navPoint = navMesh.NavMeshQuery.FindNearestPoly(ToSV3(FlipYZ(destination)), ToSV3(Vector3.one * 2));
					agent.RequestMoveTarget(navPoint.Polygon, navPoint.Position);

					_debugNavDest = ToV3 (navPoint.Position);
				} catch(System.Exception) {
					//Debug.Log(e);
				}
			}
		}
	}
}
