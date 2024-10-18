using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine.Profiling;

using Den.Tools;
using MapMagic.Products;

namespace MapMagic.Nodes
{
	[System.Serializable]
	public class Auxiliary
	{ 
		public string name = "Group";
		public string comment = "Drag in generators to group them";
		public Color color = new Color(0.625f, 0.625f, 0.625f, 1);

		public Vector2 guiPos;
		public Vector2 guiSize = new Vector2(100,100);
	}


	[System.Serializable]
	public class Group : Auxiliary
	{ 
		
	}

	[System.Serializable]
	public class Comment : Auxiliary
	{ 
		public Comment ()
		{
			name = "Comment";
			color = new Color(255f/256f, 182f/256f, 72f/256f, 1); //Amber
		}
	}
}
