using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace  Den.Tools
{

public static class VoxelMatrixEditor
{
	//Sample scene GUI template
	/*
	[CustomEditor(typeof(VoxelMatrixBehavior))]
	public class VoxelMatrixSceneview : Editor
	{
		public bool edit;
		public int editHeight;
		public int editType;

		public override void OnInspectorGUI ()
		{
			DrawDefaultInspector();

			EditorGUILayout.BeginVertical();
			EditorGUILayout.Space();
			edit = EditorGUILayout.Toggle("Edit", edit,  GUI.skin.button);
			editHeight = EditorGUILayout.IntField("Edit Height", editHeight);
			editType = EditorGUILayout.IntField("Edit Type", editType);
			EditorGUILayout.EndVertical();
		}

		public void OnSceneGUI () 
		{
			VoxelMatrixBehavior voxelMatrix = (VoxelMatrixBehavior)target;

			VoxelMatrixEditor.DrawVoxelMatrix(voxelMatrix.matrix);

			if (voxelMatrix.edit)
				VoxelMatrixEditor.DrawVoxelAbsCursor(voxelMatrix.matrix, ref voxelMatrix.editHeight, (byte)voxelMatrix.editType);
		}
	}
	*/


	public static void DrawVoxelMatrix (Matrix3D<byte> matrix, bool isWire = true, Vector3 worldPos=new Vector3())
	{
		CoordDir min = matrix.cube.Min;
		CoordDir max = matrix.cube.Max;

		Handles.color = new Color(1,1,1,0.5f);
		for (int x=min.x; x<max.x; x++)
			for (int y=min.y; y<max.y; y++)
				for (int z=min.z; z<max.z; z++)
				{
					if (matrix[x,y,z] != 0)
					{
						if (!isWire)
							Handles.CubeHandleCap(0, new Vector3(x+0.5f, y+0.5f, z+0.5f) + worldPos, Quaternion.identity, 1, EventType.Repaint);
						else
							Handles.DrawWireCube(new Vector3(x+0.5f, y+0.5f, z+0.5f) + worldPos, Vector3.one);
					}
				}
	}


	public static void DrawVoxelAbsCursor (Matrix3D<byte> matrix, ref int heightPlane, byte setType, Vector3 worldOffset=new Vector3()) =>
		DrawVoxelAbsCursor(matrix.array, matrix.cube.offset, matrix.cube.size, ref heightPlane, setType, Vector3.zero);


	public static bool DrawVoxelAbsCursor (byte[] matrix, CoordDir matrixOffset, CoordDir matrixSize, ref int heightPlane, byte setType, Vector3 worldOffset=new Vector3())
	//Returns true if edit performed
	{
		UnityEditor.SceneView sceneView = UnityEditor.SceneView.lastActiveSceneView;
		if (sceneView == null || sceneView.camera == null)
			return false;
			
		//disabling selection
		UnityEditor.HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

		//changing height
		if (Event.current.isScrollWheel  &&  Event.current.alt)
		{
			if (Event.current.delta.y>0) heightPlane--;
			if (Event.current.delta.y<0) heightPlane++;
			Event.current.Use();
		}


		//aiming
		Vector2 mousePos = Event.current.mousePosition;
		mousePos = new Vector2(mousePos.x/sceneView.camera.pixelWidth, mousePos.y/sceneView.camera.pixelHeight);
		mousePos *= EditorGUIUtility.pixelsPerPoint;
		mousePos.y = 1 - mousePos.y;
		Ray ray = sceneView.camera.ViewportPointToRay(mousePos);
		ray.origin -= worldOffset;

		Plane plane = new Plane(Vector3.up, new Vector3(0,heightPlane,0));
		plane.Raycast(ray, out float enter);
		Vector3 point = ray.GetPoint(enter);
		//Handles.CubeHandleCap(0, point, Quaternion.identity, 0.33f, EventType.Repaint);


		//rounding to block
		Coord coord = Coord.Round(point - new Vector3(0.5f,0.5f,0.5f));
		CoordDir coordDir = new CoordDir(coord.x, heightPlane, coord.z);

		//checkingh if coord is within matrix
		CoordCube cube = new CoordCube(matrixOffset, matrixSize);
		bool isInRange = cube.Contains(coordDir);
		if (!isInRange)
			Handles.color = Color.red;
		else
			Handles.color = Color.green;
		
		//placing block
		bool editPerformed = false;
		int mouseButton = -1;
		if (Event.current.type == EventType.MouseDown) mouseButton = Event.current.button;
		
		if (isInRange && !Event.current.alt)
		{
			if (mouseButton == 0) 
				matrix[cube.GetPos(coordDir)] = setType;
				//matrix[coordDir] = setType;
			if (mouseButton == 1) 
				matrix[cube.GetPos(coordDir)] = 0;
				//matrix[coordDir] = 0;
		}

		//drawing handle
		Handles.CubeHandleCap(0, new Vector3(coordDir.x+0.5f, coordDir.y+0.5f, coordDir.z+0.5f)+worldOffset, Quaternion.identity, 1, EventType.Repaint);

		//drawing coord
		//if (Event.current.alt)
		Handles.Label(point+worldOffset, coordDir.ToString(writeDir:false));

		UnityEditor.SceneView.lastActiveSceneView.Repaint();

		return editPerformed;
	}

	public static (int,int) DrawVoxelCursor (Matrix3D<byte> matrix, Vector3 worldOffset=new Vector3()) => 
		DrawVoxelCursor(matrix.array, matrix.cube.offset, matrix.cube.size, worldOffset);


	public static (int,int) DrawVoxelCursor (byte[] matrix, CoordDir matrixOffset, CoordDir matrixSize, Vector3 worldOffset=new Vector3())
	//Returns kind of change was performed: 0 no change, 1 adding, -1 erasing
	//And array element that was changed
	{
		UnityEditor.SceneView sceneView = UnityEditor.SceneView.lastActiveSceneView;
		if (sceneView == null || sceneView.camera == null)
			return (0,0);
			
		//disabling selection
		UnityEditor.HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

		//aiming
		Vector2 mousePos = Event.current.mousePosition;
		mousePos = new Vector2(mousePos.x/sceneView.camera.pixelWidth, mousePos.y/sceneView.camera.pixelHeight);
		mousePos *= EditorGUIUtility.pixelsPerPoint;
		mousePos.y = 1 - mousePos.y;
		Ray ray = sceneView.camera.ViewportPointToRay(mousePos);
		ray.origin -= worldOffset;

		//plane for test purpose
		/*Plane plane = new Plane(Vector3.up, new Vector3(0,0,0));
		plane.Raycast(ray, out float enter);
		Vector3 planepoint = ray.GetPoint(enter);
		Handles.CubeHandleCap(0, planepoint, Quaternion.identity, 0.33f, EventType.Repaint);*/

		//intersecting voxels
		CoordDir min = matrixOffset;
		CoordDir max = matrixOffset+matrixSize;
		CoordCube cube = new CoordCube(matrixOffset, matrixSize);

		float closestDist = float.MaxValue;
		CoordDir closestCoord = new CoordDir(0,0,0);
		Vector3 closestPoint = new Vector3(0,0,0);
		bool isHit = false;
		CoordDir normal = new CoordDir();

		for (int x=min.x; x<max.x; x++)
			for (int y=min.y; y<max.y; y++)
				for (int z=min.z; z<max.z; z++)
				{
					int num = cube.GetPos(x,y,z);
					if (matrix[num] == 0)
						continue;

					bool hit = Collisions.RayhitBoundingBox(ray, new Vector3(x, y, z), new Vector3(x+1, y+1, z+1), out Vector3 point);
					if (!hit)
						continue;

					isHit = true;
					float dist = Vector3.Distance(ray.origin, point);
					if (dist < closestDist)
					{
						closestDist = dist;
						closestCoord = new CoordDir(x,y,z);
						closestPoint = point;
					}
				}
		closestPoint += worldOffset;

		//mising voxels
		bool isEmpty = false;
		if (!isHit)
		{
			//checking if empty
			isEmpty = true;
			for (int i=0; i<matrix.Length; i++)
				if (matrix[i] > 0)
				{
					isEmpty = false;
					break;
				}

			if (isEmpty)
			{
				closestCoord = new CoordDir(0,0,0);
				if (!cube.Contains(closestCoord))
					closestCoord = cube.Min;
				normal = new CoordDir(0,0,0);
			}

			else
			{
				UnityEditor.SceneView.lastActiveSceneView.Repaint();
				return (0, 0);
			}
		}

		//calculating side
		Vector3 center = new Vector3(closestCoord.x+0.5f, closestCoord.y+0.5f, closestCoord.z+0.5f)+worldOffset;
		Vector3 dir = (closestPoint-center).normalized;
		int maxDir = 0;
		float maxDirVal = 0;
		for (int i=0; i<3; i++)
			if (Mathf.Abs(dir[i]) > maxDirVal)
			{
				maxDirVal = Mathf.Abs(dir[i]);
				maxDir = i;
			}
		int dirSide = dir[maxDir] > 0 ? 1 : -1;
	
		//dir normal
		if (!isEmpty)
			normal[maxDir] = dirSide;

		//rotation for highlight
		Vector3 up = Vector3.up;
		if (maxDir == 1) up = Vector3.right;
		up *= dirSide;
		Vector3 right = Vector3.Cross(normal.pos, up);
		Quaternion rotation = 
			!isEmpty ? 
			Quaternion.LookRotation(normal.pos, up) :
			Quaternion.identity;

		//finding highlight coord and size
		Vector3 size = Vector3.one;
		for (int i=0; i<3; i++)
			if (i==maxDir) 
				size[i] = 0.01f;
		Vector3 pos = normal.pos/2 + center;

		//checking if in range
		bool isInRange = cube.Contains(closestCoord);
		bool isNeigInRange = cube.Contains(closestCoord+normal);

		//drawing highlight
		Handles.color = isInRange ? Color.green : Color.red;
		Handles.DrawWireCube(center, Vector3.one);

		Handles.color = isNeigInRange ? Color.green : Color.red;
		up /= 2; right /= 2;
		Handles.DrawAAConvexPolygon(pos + up + right,
									pos + up - right,
									pos - up - right,
									pos - up + right);

		//drawing coord
		//if (Event.current.alt)
		float handleSize = HandleUtility.GetHandleSize(center);
		Handles.Label(center - Vector3.up*0.5f, closestCoord.ToString(writeDir:false), EditorStyles.miniButton);

		//editing
		int edit = 0;
		int coord = 0;
		int mouseButton = -1;
		if (Event.current.type == EventType.MouseDown) mouseButton = Event.current.button;

		if (!Event.current.alt)
		{
			if (mouseButton == 0  &&  !Event.current.shift  &&  isNeigInRange) 
				{ coord = cube.GetPos(closestCoord+normal); edit = 1; }

			if (mouseButton == 1  &&  isInRange) 
				{ coord = cube.GetPos(closestCoord); edit = -1; }

			if (mouseButton == 0  &&  Event.current.shift  &&  isInRange) 
				{ coord = cube.GetPos(closestCoord); edit = 1; }
		}

		UnityEditor.SceneView.lastActiveSceneView.Repaint();

		return (edit, coord);
	}


	//public static bool DrawQuadrantCursor (Vector3 cubeMin, Vector3 cubeMax, Vector3 worldOffset=new Vector3())
}
}