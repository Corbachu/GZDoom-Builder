#region ================== Namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.VisualModes
{
	public class VisualCamera
	{
		#region ================== Constants

		private const float ANGLE_FROM_MOUSE = 0.0001f;
		public const float MAX_ANGLEZ_LOW = 100f / Angle2D.PIDEG;
		public const float MAX_ANGLEZ_HIGH = (360f - 100f) / Angle2D.PIDEG;
		
		#endregion

		#region ================== Variables

		// Properties
		private Vector3D position;
		private Vector3D target;
		private Vector3D movemultiplier;
		private float anglexy, anglez;
		private Sector sector;
		
		#endregion

		#region ================== Properties

		public Vector3D Position { get { return position; } set { position = value; } }
		public Vector3D Target { get { return target; } }
		public float AngleXY { get { return anglexy; } set { anglexy = value; } }
		public float AngleZ { get { return anglez; } set { anglez = value; } }
		public Sector Sector { get { return sector; } internal set { sector = value; } }
		public Vector3D MoveMultiplier { get { return movemultiplier; } set { movemultiplier = value; } }
		
		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public VisualCamera()
		{
			// Initialize
			this.movemultiplier = new Vector3D(1.0f, 1.0f, 1.0f);
			this.position = position;
			this.anglexy = 0.0f;
			this.anglez = Angle2D.PI;
			this.sector = null;
			
			PositionAtThing();
		}
		
		#endregion

		#region ================== Methods

		// Mouse input
		internal void ProcessMouseInput(Vector2D delta)
		{
			// Change camera angles with the mouse changes
			anglexy -= delta.x * ANGLE_FROM_MOUSE;
			anglez += delta.y * ANGLE_FROM_MOUSE;

			// Normalize angles
			anglexy = Angle2D.Normalized(anglexy);
			anglez = Angle2D.Normalized(anglez);

			// Limit vertical angle
			if(anglez < MAX_ANGLEZ_LOW) anglez = MAX_ANGLEZ_LOW;
			if(anglez > MAX_ANGLEZ_HIGH) anglez = MAX_ANGLEZ_HIGH;
		}

		// Key input
		internal void ProcessMovement(Vector3D deltavec)
		{
			// Calculate camera direction vectors
			Vector3D camvec = Vector3D.FromAngleXYZ(anglexy, anglez);

			// Position the camera
			position += deltavec;
			
			// Target the camera
			target = position + camvec;
		}

		// This applies the position and angle from the 3D Camera Thing
		// Returns false when it couldn't find a 3D Camera Thing
		public bool PositionAtThing()
		{
			Thing modething = null;

			// Find a 3D Mode thing
			foreach(Thing t in General.Map.Map.Things)
				if(t.Type == General.Map.Config.Start3DModeThingType) modething = t;

			// Found one?
			if(modething != null)
			{
				int z = 0;
				if(sector != null)
					z = (int)position.z - sector.FloorHeight;

				// Position camera here
				modething.DetermineSector();
				position = modething.Position + new Vector3D(0.0f, 0.0f, 96.0f);
				anglexy = modething.Angle + Angle2D.PI;
				anglez = Angle2D.PI;
				return true;
			}
			else
			{
				return false;
			}
		}
		
		// This applies the camera position and angle to the 3D Camera Thing
		// Returns false when it couldn't find a 3D Camera Thing
		public bool ApplyToThing()
		{
			Thing modething = null;
			
			// Find a 3D Mode thing
			foreach(Thing t in General.Map.Map.Things)
				if(t.Type == General.Map.Config.Start3DModeThingType) modething = t;

			// Found one?
			if(modething != null)
			{
				int z = 0;
				if(sector != null)
					z = (int)position.z - sector.FloorHeight;

				// Position the thing to match camera
				modething.Move((int)position.x, (int)position.y, z);
				modething.Rotate(anglexy - Angle2D.PI);
				return true;
			}
			else
			{
				return false;
			}
		}
		
		#endregion
	}
}