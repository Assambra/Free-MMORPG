using System;
using System.Reflection;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Den.Tools;
using MapMagic.Products;

namespace MapMagic.Nodes
{
	[GeneratorMenu (menu = "Map/Portals", name = "Enter", iconName = "GeneratorIcons/PortalIn", lookLikePortal=true)] public class MatrixPortalEnter : PortalEnter<Den.Tools.Matrices.MatrixWorld> { }
	[GeneratorMenu (menu = "Map/Portals", name ="Exit", iconName = "GeneratorIcons/PortalOut", lookLikePortal=true)] public class MatrixPortalExit : PortalExit<Den.Tools.Matrices.MatrixWorld> { }
	//[GeneratorMenu (menu = "Objects/Portals", name = "Enter", iconName = "GeneratorIcons/PortalIn", lookLikePortal=true)] public class ObjectsPortalEnter : PortalEnter<TransitionsList> { }
	//[GeneratorMenu (menu = "Objects/Portals", name ="Exit", iconName = "GeneratorIcons/PortalOut", lookLikePortal=true)] public class ObjectsPortalExit : PortalExit<TransitionsList> { }
	//[GeneratorMenu (menu = "Spline/Portals", name = "Enter", iconName = "GeneratorIcons/PortalIn", lookLikePortal=true)] public class SplinePortalEnter : PortalEnter<Plugins.Splines.SplineSys> { }
	//[GeneratorMenu (menu = "Spline/Portals", name ="Exit", iconName = "GeneratorIcons/PortalOut", lookLikePortal=true)] public class SplinePortalExit : PortalExit<Plugins.Splines.SplineSys> { }


	public interface IPortalEnter<out T> : IInlet<T> where T:class
	{
		string Name {get; set; }
	}


	public interface IPortalExit<out T> :  IOutlet<T> where T:class
	{
		void AssignEnter (IPortalEnter<object> enter, Graph graph);
		IPortalEnter<T> RefreshEnter (Graph graph); //gets enter at the same time
	}


	public interface IFnPortal<out T>  { string Name { get; set; } }

	public interface IFnEnter<out T> : IFnPortal<T>, IOutlet<T>  where T: class { }  //to use objects of type IFnEnter<object>
	public interface IFnExit<out T> : IFnPortal<T>, IInlet<T>, IRelevant where T: class { } //fnExit is always generated (should be IRelevant)
	//interfaces required in draw editor, so they are stored in portals.cs, not module


	[System.Serializable]
	[GeneratorMenu(name = "Generic Portal Enter")]
	public class PortalEnter<T> : Generator, IInlet<T>, IPortalEnter<T>  where T: class, ICloneable
	{
		public string name = "Portal";
		public string Name { get{ return name; } set{ name = value; } }

		public override void Generate (TileData data, StopToken stop) { }
	}


	

	[Serializable]
	[GeneratorMenu (name ="Generic Portal Exit")]
	public class PortalExit<T> : Generator, IOutlet<T>, IPortalExit<T>, ICustomDependence, ICustomSerialize  where T: class, ICloneable 
	{
		public PortalEnter<T> enter; //older versions used to save enter as class. TODO 3.0 rename tempEnter to enter
		[NonSerialized] public PortalEnter<T> tempEnter;
		//public IPortalEnter<T> Enter => enter;

		public ulong enterId = 0; 


		public void OnBeforeSerialize (Graph graph) => enterId = tempEnter.Id;
		public void OnAfterDeserialize (Graph graph) => RefreshEnter(graph);


		public IPortalEnter<T> RefreshEnter (Graph graph)
		{
			//switching old portal to new format
			if (enter != null)
			{
				tempEnter = enter;
				enterId = enter.id;
				enter = null;
			}

			//refreshing enter
			if (tempEnter == null  ||  tempEnter.id != enterId)
			{
				Generator gen = graph.GetGeneratorById(enterId);
				if (gen != null)
					tempEnter = (PortalEnter<T>)gen;
			}

			return tempEnter;
		}

		public void AssignEnter (IPortalEnter<object> ienter, Graph graph)
		{
			//removing enter
			if (ienter == null)
			{
				tempEnter = null;
				enterId = 0;
				return;
			}

			if (!(ienter is PortalEnter<T> enter)) return;
			//TODO: other validity check

			tempEnter = enter;
			enterId = enter.id;
		}

		public override void Generate (TileData data, StopToken stop) 
		{ 
			if (tempEnter != null   &&  !stop.stop) 
			{
				data.StoreProduct(this, data.ReadInletProduct(tempEnter));
				//TODO: clone?
			}
		}

		public IEnumerable<Generator> PriorGens () 
		{
			if (tempEnter != null)
				yield return tempEnter;
		}
	}
}