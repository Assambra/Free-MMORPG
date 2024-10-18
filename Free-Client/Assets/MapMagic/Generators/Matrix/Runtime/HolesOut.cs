using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Den.Tools;
using Den.Tools.GUI;
using Den.Tools.Matrices;
using MapMagic.Core;
using MapMagic.Products;
using MapMagic.Terrains;

using UnityEngine.Profiling;

#if UN_MapMagic
using uNature.Core.Extensions.MapMagicIntegration;
using uNature.Core.FoliageClasses;
#endif


namespace MapMagic.Nodes.MatrixGenerators
{
	[Serializable]
	[GeneratorMenu(
		menu = "Map/Output", 
		name = "Holes", 
		section=2, 
		colorType = typeof(MatrixWorld), 
		iconName="GeneratorIcons/HeightOut",
		helpLink = "https://gitlab.com/denispahunov/mapmagic/-/wikis/MatrixGenerators/Holes")]
	public class  HolesOutput2112 : OutputGenerator, IInlet<MatrixWorld>
	{
		[Val(name="Tolerance")] public float tolerance = 0.5f;

		public override (string, int) GetCodeFileLine () => GetCodeFileLineBase();  //to get here with right-click on generator

		public OutputLevel outputLevel = OutputLevel.Draft | OutputLevel.Main;
		public override OutputLevel OutputLevel { get{ return outputLevel; } }

		public override void Generate (TileData data, StopToken stop)
		{
			//loading source
			if (stop!=null && stop.stop) return;
			MatrixWorld src = data.ReadInletProduct(this);
			if (src == null) return; 

			//adding to finalize
			if (stop!=null && stop.stop) return;
			if (enabled)
			{
				data.StoreOutput(this, typeof(HolesOutput2112), this, src);  //adding src since it's not changing
				data.MarkFinalize(Finalize, stop);
			}
			else 
				data.RemoveFinalize(finalizeAction);
		}


		public static FinalizeAction finalizeAction = Finalize; //class identified for FinalizeData
		public static void Finalize (TileData data, StopToken stop)
		{
			int fullSize = data.area.full.rect.size.x;
			int activeSize = data.area.active.rect.size.x;
			int margins = data.area.Margins;
			int resolution = activeSize-1; //data.globals.holesRes;
			//there's no way to change holes resolution with script yey. It's always height-1.
			//otherwise everything is ready to use it

			bool[,] holes2D = new bool[resolution, resolution];

			foreach ((HolesOutput2112 output, MatrixWorld product, MatrixWorld biomeMask) 
				in data.Outputs<HolesOutput2112,MatrixWorld,MatrixWorld> (typeof(HolesOutput2112), inSubs:true) )
			{
				if (product == null) 
					continue;

				for (int x = 0; x < activeSize; x++)
					for (int z = 0; z < activeSize; z++)
				{
					if (stop!=null && stop.stop) return;

					int pos = (z+margins)*fullSize + (x+margins);
					float val = product.arr[pos];
					float biomeVal = biomeMask!=null ? biomeMask.arr[pos] : 1;

					int holesX = (int)(1f*x/activeSize * resolution + 0.5f);
					int holesZ = (int)(1f*z/activeSize * resolution + 0.5f);
					if (val*biomeVal > output.tolerance)
						holes2D[holesZ,holesX] = false;
					else
						holes2D[holesZ,holesX] = true;
				}
			}

			//pushing to apply
			if (stop!=null && stop.stop) return;
			ApplyData applyData = new ApplyData() {holes2D=holes2D};
			Graph.OnOutputFinalized?.Invoke(typeof(HolesOutput2112), data, applyData, stop);
			data.MarkApply(applyData);
		}


		public class ApplyData : IApplyData
		{
			public bool[,] holes2D;

			public void Apply (Terrain terrain)
			{
				if (terrain==null || terrain.Equals(null) || terrain.terrainData==null) return; //chunk removed during apply
				TerrainData data = terrain.terrainData;

				//data.holesResolution = holes2D.GetLength(0);  //there's no way to change holes resolution with script yey. It's always height-1.
				data.SetHoles(0, 0, holes2D); 
				terrain.Flush();
			}

			public static ApplyData Empty 
				{get{ return new ApplyData() { holes2D = new bool[33,33] }; }}

			public int Resolution {get{ return holes2D.GetLength(0); }}
		}

		public override void ClearApplied (TileData data, Terrain terrain)
		{
			TerrainData terrainData = terrain.terrainData;
			Vector3 terrainSize = terrainData.size;

			int res = terrainData.holesResolution;
			bool[,] holes = new bool[res,res];

			for (int x = 0; x < res; x++)
				for (int z = 0; z < res; z++)
					holes[z,x] = true;

			terrainData.SetHoles(0,0,holes);

		}
	}
}