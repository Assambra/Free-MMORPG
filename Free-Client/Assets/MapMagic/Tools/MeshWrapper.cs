using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Den.Tools
{
	[System.Serializable]
	public class MeshWrapper
	{
		public Vector3[] verts;
		public Vector3[] normals;
		public Vector2[] uv; public Vector2[] uv2; public Vector2[] uv3; public Vector2[] uv4;
		public Color[] colors;
		public Vector4[] tangents;

		//public List<Vector4>[] uvs4; //IDK what is it and how it corresponds to uv4. Just cleaning for maintanance

		public int[] tris;

		[System.NonSerialized] public int vertCounter = 0; //for appending mesh manually
		[System.NonSerialized] public int triCounter = 0;


		public MeshWrapper () { }
		public MeshWrapper (MeshWrapper src)
		{
			if (src.verts != null) { verts = new Vector3[src.verts.Length]; System.Array.Copy(src.verts, verts, verts.Length); }
			if (src.normals != null) { normals = new Vector3[src.normals.Length]; System.Array.Copy(src.normals, normals, normals.Length); }
			if (src.uv != null) { uv = new Vector2[src.uv.Length]; System.Array.Copy(src.uv, uv, uv.Length); }
			if (src.uv2 != null) { uv2 = new Vector2[src.uv2.Length]; System.Array.Copy(src.uv2, uv2, uv2.Length); }
			if (src.uv3 != null) { uv3 = new Vector2[src.uv3.Length]; System.Array.Copy(src.uv3, uv3, uv3.Length); }
			if (src.uv4 != null) { uv4 = new Vector2[src.uv4.Length]; System.Array.Copy(src.uv4, uv4, uv4.Length); }
			if (src.colors != null) { colors = new Color[src.colors.Length]; System.Array.Copy(src.colors, colors, colors.Length); }
			if (src.tangents != null) { tangents = new Vector4[src.tangents.Length]; System.Array.Copy(src.tangents, tangents, tangents.Length); }
			if (src.tris != null) { tris = new int[src.tris.Length]; System.Array.Copy(src.tris, tris, tris.Length); }
		}


		public void Restart ()
		/// Doesn't flush arrays, but resets mesh appending
		{
			vertCounter = 0;
			triCounter = 0;
		}


		public void SetChannels (byte[] channels)
		{
			int length = channels.Length / 8;
			
			uv = new Vector2[length]; 
			uv2 = new Vector2[length]; 
			uv3 = new Vector2[length]; 
			uv4 = new Vector2[length]; 

			for (int v=0; v<length; v++) 
			{
				uv[v] = new Vector2(channels[v*8 + 1]/32f, channels[v*8 + 2]/32f);
				uv2[v] = new Vector2(channels[v*8 + 3]/32f, channels[v*8 + 4]/32f);
				uv3[v] = new Vector2(channels[v*8 + 5]/32f, channels[v*8 + 6]/32f);
				uv4[v] = new Vector2(channels[v*8 + 7]/32f, channels[v*8 + 7]/32f);
			}
		}

		public void ApplyTo (Mesh mesh)
		{
			mesh.Clear();

			mesh.vertices = verts;
			mesh.normals = normals;
			if (uv != null) mesh.uv = uv;
			if (uv2 != null) mesh.uv2 = uv2;
			if (uv3 != null) mesh.uv3 = uv3;
			if (uv4 != null) mesh.uv4 = uv4;
			if (colors != null) mesh.colors = colors;
			if (tangents != null) mesh.tangents = tangents;

			/*if (uvs4 != null)
			for (int i=0; i<uvs4.Length; i++)
				if (uvs4[i] != null) mesh.SetUVs(i,uvs4[i]);*/

			//mesh.triangles = tris;
			mesh.SetTriangles(tris, 0, false);  //twice faster
		}

		public void ReadMesh (Mesh mesh)
		{
			verts = mesh.vertices;
			normals = mesh.normals;
			uv = mesh.uv; uv2 = mesh.uv2; uv3 = mesh.uv3; uv4 = mesh.uv4;
			tangents = mesh.tangents;
			colors = mesh.colors;

			tris = mesh.triangles;

			if (normals != null && normals.Length == 0) normals = null;
			if (tangents != null && tangents.Length == 0) tangents = null;
			if (colors != null && colors.Length == 0) colors = null;
			if (uv != null && uv.Length == 0) uv = null; 
			if (uv2 != null && uv2.Length == 0) uv2 = null; 
			if (uv3 != null && uv3.Length == 0) uv3 = null; 
			if (uv4 != null && uv4.Length == 0) uv4 = null;
		}

		public void Append (MeshWrapper addMesh, Vector3 offset=new Vector3(), float size=1, float height=1) //custom verts length can be larger than addMesh vert count
		{
			/*for (int v=0; v<addMesh.verts.Length; v++) //TODO test iteration inside "if"
			{
				if (customVerts==null) verts[vertCounter+v] = addMesh.verts[v];// + offset;
				else verts[vertCounter+v] = customVerts[v];

				if (normals!=null && addMesh.normals!=null)
				{
					if (customNormals==null) normals[vertCounter+v] = addMesh.normals[v];
					else normals[vertCounter+v] = customNormals[v];
				}

				if (uv!=null && addMesh.uv!=null) uv[vertCounter+v] = addMesh.uv[v];
				if (uv2!=null && addMesh.uv2!=null) uv2[vertCounter+v] = addMesh.uv2[v];
				if (uv3!=null && addMesh.uv3!=null) uv3[vertCounter+v] = addMesh.uv3[v];
				if (uv4!=null && addMesh.uv4!=null) uv4[vertCounter+v] = addMesh.uv4[v];
				if (colors!=null && addMesh.colors!=null) colors[vertCounter+v] = addMesh.colors[v];
				if (tangents!=null && addMesh.tangents!=null) tangents[vertCounter+v] = addMesh.tangents[v];
			}*/


			//this definitely works faster:

			//standard case (verts+normals+uv)
			if (normals!=null && uv!=null && addMesh.normals!=null && normals.Length!=0 && addMesh.normals.Length!=0 && addMesh.uv!=null && uv.Length!=0 && addMesh.uv.Length!=0)
			{
				for (int v=0; v<addMesh.verts.Length; v++)
				{
					int v2 = vertCounter+v;
					verts[v2] = new Vector3(addMesh.verts[v].x*size + offset.x, addMesh.verts[v].y*size*height + offset.y, addMesh.verts[v].z*size + offset.z);
					normals[v2] = addMesh.normals[v];
					uv[v2] = addMesh.uv[v];
				}
			}

			//special cases when there's no normals or uvs
			else
			{
				for (int v=0; v<addMesh.verts.Length; v++)
					verts[vertCounter+v] = new Vector3(addMesh.verts[v].x*size + offset.x, addMesh.verts[v].y*size*height + offset.y, addMesh.verts[v].z*size + offset.z);

				if (normals != null && addMesh.normals != null && normals.Length!=0 && addMesh.normals.Length!=0)
				{
					for (int v=0; v<addMesh.normals.Length; v++)
						normals[vertCounter+v] = addMesh.normals[v];
				}

				if (uv!=null && addMesh.uv!=null && uv.Length!=0 && addMesh.uv.Length!=0)
					for (int v=0; v<addMesh.verts.Length; v++)
						uv[vertCounter+v] = addMesh.uv[v];
			}
			
			//additional cases
			if (uv2!=null && addMesh.uv2!=null && uv2.Length!=0 && addMesh.uv2.Length!=0)
				for (int v=0; v<addMesh.verts.Length; v++)
					uv2[vertCounter+v] = addMesh.uv2[v];

			if (uv3!=null && addMesh.uv3!=null && uv3.Length!=0 && addMesh.uv3.Length!=0)
				for (int v=0; v<addMesh.verts.Length; v++)
					uv3[vertCounter+v] = addMesh.uv3[v];

			if (uv4!=null && addMesh.uv4!=null && uv4.Length!=0 && addMesh.uv4.Length!=0)
				for (int v=0; v<addMesh.verts.Length; v++)
					uv4[vertCounter+v] = addMesh.uv4[v];

			if (uv!=null && addMesh.uv!=null && uv.Length!=0 && addMesh.uv.Length!=0)
				for (int v=0; v<addMesh.verts.Length; v++)
					uv[vertCounter+v] = addMesh.uv[v];

			if (colors!=null && addMesh.colors!=null && colors.Length!=0 && addMesh.colors.Length!=0)
				for (int v=0; v<addMesh.verts.Length; v++)
					colors[vertCounter+v] = addMesh.colors[v];

			if (tangents!=null && addMesh.tangents!=null && tangents.Length!=0 && addMesh.tangents.Length!=0)
				for (int v=0; v<addMesh.verts.Length; v++)
					tangents[vertCounter+v] = addMesh.tangents[v];

			//tris
			for (int t=0; t<addMesh.tris.Length; t++)
				tris[triCounter+t] = addMesh.tris[t] + vertCounter;

			//counters
			vertCounter += addMesh.verts.Length;
			triCounter += addMesh.tris.Length;
		}

		public void RotateMirror (int rotation, bool mirror) //rotation in 90-degree
		{
			//setting mesh rot-mirror params
			bool mirrorX = false;
			bool mirrorZ = false;
			bool rotate = false;
			
			switch (rotation)
			{
				case 90: rotate = true; mirrorX = true; break;
				case 180: mirrorX = true; mirrorZ = true; break;
				case 270: rotate = true; mirrorZ = true; break;
			}
			
			if (mirror) mirrorX = !mirrorX;
			
			//rotating verts
			for (int v=0; v<verts.Length; v++)
			{ 
				Vector3 pos = verts[v];
				
				if (rotate)
				{
					float temp = pos.x;
					pos.x = pos.z;
					pos.z = temp;
					
				}

				if (mirrorX) { pos.x = -pos.x; }
				if (mirrorZ) { pos.z = -pos.z; } 
				
				verts[v] = pos;

				//normals
				if (normals != null) 
				{
					Vector3 normal = normals[v];

					if (rotate)
					{
						float temp = normal.x;
						normal.x = normal.z;
						normal.z = temp;
					}
					
					if (mirrorX) { normal.x = -normal.x; }
					if (mirrorZ) { normal.z = -normal.z; } 

					normals[v] = normal;
				}

				//tangents
				if (tangents != null) 
				{
					Vector4 tangent = tangents[v];

					if (rotate)
					{
						float temp = tangent.x;
						tangent.x = tangent.z;
						tangent.z = temp;
					}
					
					if (mirrorX) { tangent.x = -tangent.x; }
					if (mirrorZ) { tangent.z = -tangent.z; } 

					tangents[v] = tangent;
				}
			}
			
			//mirroring tris
			if (mirror) 
				for (int t=0; t<tris.Length; t++) 
					for (int i=0; i<tris.Length; i+=3) 
			{
				int temp = tris[i];
				tris[i] = tris[i+2];
				tris[i+2] = temp;
			}
		}


	}
}