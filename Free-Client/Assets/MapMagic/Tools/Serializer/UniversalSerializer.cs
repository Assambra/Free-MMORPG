using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

[assembly:System.Runtime.CompilerServices.InternalsVisibleTo("Tests.Editor")]

namespace Den.Tools.Serialization
{
	public class UniversalSerializer
	{
		string data;

		public void Serialize (object obj)
		{
			data = JsonUtility.ToJson(obj, true);
		}

		public void Deserialize (object obj)
		{
			JsonUtility.FromJsonOverwrite(data, obj);
		}


		public object Clone (object obj)
		{
			return null;
		}
	}
}