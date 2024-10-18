using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

using Den.Tools;
using Den.Tools.GUI;


namespace MapMagic.Nodes.GUI
{
	public static class GroupDraw
	{
		private static Generator[] draggedGroupNodes;
		private static Auxiliary[] draggedGroups;
		private static Vector2[] initialGroupNodesPos;
		private static Vector2[] initialGroupGroupsPos;
		private static GUIStyle miniNameStyle;
		private static GUIStyle commentStyle;
		private static GUIStyle commentMiniStyle;
		private static GUIStyle groupStyle;
		private static GUIStyle groupMiniStyle;
		private static GUIStyle groupSrcStyle;

		public static void DragGroup (Group group, Generator[] allGens=null, Auxiliary[] allAux=null)
		{
			//dragging
			if (!UI.current.layout)
			{
				if (DragDrop.TryDrag(group, UI.current.mousePos))
				{
					for (int i=0; i<draggedGroupNodes.Length; i++)
					{
						Generator gen = draggedGroupNodes[i];

						gen.guiPosition = initialGroupNodesPos[i] + DragDrop.totalDelta;

						//moving generators cells
						if (UI.current.cellObjs.TryGetCell(gen, "Generator", out Cell genCell))
						{
							genCell.worldPosition = gen.guiPosition;
							genCell.CalculateSubRects();
						}
					}

					for (int i=0; i<draggedGroups.Length; i++)
					{
						Auxiliary aux = draggedGroups[i];

						aux.guiPos = initialGroupGroupsPos[i] + DragDrop.totalDelta;

						//moving generators cells
						if (UI.current.cellObjs.TryGetCell(aux, "Group", out Cell genCell))
						{
							genCell.worldPosition = aux.guiPos;
							genCell.CalculateSubRects();
						}
					}

					group.guiPos = DragDrop.initialRect.position + DragDrop.totalDelta;
					Cell.current.worldPosition = group.guiPos;
					Cell.current.CalculateSubRects();
				}

				if (DragDrop.TryRelease(group, UI.current.mousePos))
				{
					draggedGroupNodes = null;
					initialGroupNodesPos = null;

					draggedGroups = null;
					initialGroupGroupsPos = null;

					#if UNITY_EDITOR //this should be an editor script, but it doesnt mentioned anywhere
					UnityEditor.EditorUtility.SetDirty(GraphWindow.current.graph);
					#endif
				}

				if (DragDrop.TryStart(group, UI.current.mousePos, Cell.current.InternalRect))
				{
					draggedGroupNodes = GetContainedGenerators(group, allGens);
					draggedGroups = GetContainedAux(group, allAux);
					
					initialGroupNodesPos = new Vector2[draggedGroupNodes.Length];
					for (int i=0; i<draggedGroupNodes.Length; i++)
						initialGroupNodesPos[i] = draggedGroupNodes[i].guiPosition;

					initialGroupGroupsPos = new Vector2[draggedGroups.Length];
					for (int i=0; i<draggedGroups.Length; i++)
						initialGroupGroupsPos[i] = draggedGroups[i].guiPos;
				}

				Rect cellRect = Cell.current.InternalRect;
				if (DragDrop.ResizeRect(group, UI.current.mousePos, ref cellRect, minSize:new Vector2(100,100)))
				{
					group.guiPos = cellRect.position;
					group.guiSize = cellRect.size;

					Cell.current.InternalRect = cellRect;
					Cell.current.CalculateSubRects();
				}
			}
		}

		public static void DrawGroupOutdated (Group group, bool isMini=false)
		{
			float miniFactor = !isMini ? 1 : 0.5f/GraphWindow.miniZoom; //group controls are slightly smaller than in fullscreen

			//CellObject.SetObject(Cell.current, group);
			if (UI.current.layout)
				UI.current.cellObjs.ForceAdd(group, Cell.current, "Group");

			Texture2D tex = UI.current.textures.GetColorizedTexture("MapMagic/Group", group.color);
			GUIStyle style = UI.current.textures.GetElementStyle(tex);
			Draw.Element(style);

			Cell.EmptyRowPx(5*miniFactor);
			using (Cell.Row)
			{
				if (isMini && miniNameStyle == null)
				{
					miniNameStyle = new GUIStyle(UI.current.styles.bigLabel);
					miniNameStyle.fontSize = (int)(miniNameStyle.fontSize/GraphWindow.miniZoom*0.6f);
				}

				Cell.EmptyLinePx(5*miniFactor);
				GUIStyle labelStyle = !isMini ? UI.current.styles.bigLabel : miniNameStyle;
				using (Cell.LinePx(24*miniFactor)) Draw.EditableLabelRight(ref group.name, style:labelStyle);
				//using (Cell.LineStd)	Draw.EditableLabelRight(ref group.comment);
				Cell.EmptyLinePx(3*miniFactor);
				using (Cell.LineStd)
				{
					using (Cell.RowPx(0))
					{
						if (!isMini)
						{
							using (Cell.RowPx(20)) Draw.Icon(UI.current.textures.GetTexture("DPUI/Chevrons/Down"), scale:0.5f*miniFactor);
							using (Cell.RowPx(35)) Draw.Label("Color");
						}

						if (Draw.Button("", visible:false)) GroupRightClick.DrawGroupColorSelector(group);
					}
				}
			}
		}


		public static void DrawGroup (Group group, bool isMini=false)
		{
			if (UI.current.layout)
				UI.current.cellObjs.ForceAdd(group, Cell.current, "Group");

			//background
			Texture2D tex = UI.current.textures.GetColorizedTexture("MapMagic/Group", group.color);
			GUIStyle style = UI.current.textures.GetElementStyle(tex);
			Draw.Element(style);

			//text style
			if (!isMini && groupStyle == null)
			{
				groupSrcStyle = new GUIStyle(UI.current.styles.bigLabel);
				groupSrcStyle.fontSize = 16;
				groupStyle = new GUIStyle(groupSrcStyle);
				UI.current.styles.AddStyleToResize(groupStyle);
			}

			if (isMini && groupMiniStyle == null)
			{
				groupMiniStyle = new GUIStyle(UI.current.styles.bigLabel);
				groupMiniStyle.fontSize = 12; //(int)(16*0.8f);
				//UI.current.styles.AddStyleToResize(commentStyle); //not resizing mini style
			}

			//contents
			using (Cell.Padded(4))
				DrawContents(group, groupStyle, groupMiniStyle, groupSrcStyle, isMini, additionalLabelHeight:8);
		}


		public static void DrawComment (Comment group, bool isMini=false)
		{
			if (UI.current.layout)
				UI.current.cellObjs.ForceAdd(group, Cell.current, "Group");

			//background
			Texture2D tex = UI.current.textures.GetColorizedTexture("MapMagic/Comment", group.color);
			RectOffset commentBorders = new RectOffset(10,40,4,40);
			GUIStyle style = UI.current.textures.GetElementStyle(tex, borders:commentBorders);
			RectOffset commentShadow = new RectOffset(9,5,3,9);
			Draw.Element(style, commentShadow);

			//text style
			if (!isMini && commentStyle == null)
			{
				commentStyle = new GUIStyle(UnityEditor.EditorStyles.wordWrappedLabel);
				commentStyle.fontSize = StylesCache.defaultFontSize;
				UI.current.styles.AddStyleToResize(commentStyle);
			}

			if (isMini && commentMiniStyle == null)
			{
				commentMiniStyle = new GUIStyle(UI.current.styles.labelWordWrap);
				commentMiniStyle.fontSize = (int)(StylesCache.defaultFontSize*0.8f);
				//UI.current.styles.AddStyleToResize(commentStyle); //not resizing mini style
			}

			//contents
			DrawContents(group, commentStyle, commentMiniStyle, UnityEditor.EditorStyles.wordWrappedLabel, isMini, additionalLabelHeight:2);
		}


		private static void DrawContents (Auxiliary group, GUIStyle fullStyle, GUIStyle miniStyle, GUIStyle srcStyle, bool isMini=false, int additionalLabelHeight=2)
		{
			Cell labelCellParent;
			using (Cell.LineStd)
			{
				GUIStyle labelStyle = !isMini ? fullStyle : miniStyle;

				float labelHeight = labelStyle.CalcHeight(new GUIContent(group.name),  Cell.current.InternalRect.width*UI.current.scrollZoom.zoom.x - 4) / UI.current.scrollZoom.zoom.y;

				labelCellParent = Cell.current;

				using (Cell.LinePx(labelHeight+additionalLabelHeight)) 
					group.name = Draw.EditableLabelText(group.name, style:labelStyle);
			}

			//Cell.EmptyLinePx(spaceFromLabelToButtons);

			using (Cell.LinePx(18))
			{
				Cell.EmptyRowPx(4);

				using (Cell.RowPx(18)) 
				{
					//Cell.EmptyRowPx(3); //hacky offset
					//using (Cell.RowPx(18))
						Draw.EditableLabelButton(UI.current.textures.GetTexture("DPUI/Icons/Edit"), labelCellParent:labelCellParent, iconScale:0.5f);
				}

				using (Cell.RowPx(18))
				if (Draw.Button("", icon: UI.current.textures.GetTexture("DPUI/Icons/Pallete"), visible: false, iconScale: 0.5f))
					GroupRightClick.DrawGroupColorSelector(group);
			}

			Cell.EmptyLine();

			Cell.EmptyLinePx(4);
		}



		public static void ResizeComment (Comment comment)
		{
			if (!UI.current.layout)
			{
				if (DragDrop.TryDrag(comment, UI.current.mousePos))
				{
					comment.guiPos = DragDrop.initialRect.position + DragDrop.totalDelta;
					Cell.current.worldPosition = comment.guiPos;
					Cell.current.CalculateSubRects();
				}

				if (DragDrop.TryRelease(comment, UI.current.mousePos))
				{
					draggedGroupNodes = null;
					initialGroupNodesPos = null;

					#if UNITY_EDITOR //this should be an editor script, but it doesnt mentioned anywhere
					UnityEditor.EditorUtility.SetDirty(GraphWindow.current.graph);
					#endif
				}

				DragDrop.TryStart(comment, UI.current.mousePos, Cell.current.InternalRect);

				Rect cellRect = Cell.current.InternalRect;
				//cellRect = cellRect.Extended(-4, -12, -2, -12);
				if (DragDrop.ResizeRect(comment, UI.current.mousePos, ref cellRect, minSize:new Vector2(40,40), additionalBottomRightMargin:new RectOffset(4,0,4,0)))
				{
					//cellRect = cellRect.Extended(4, 12, 2, 12);

					comment.guiPos = cellRect.position;
					comment.guiSize = cellRect.size;

					Cell.current.InternalRect = cellRect;
					Cell.current.CalculateSubRects();
				}
			}
		}
		


		public static Generator[] GetContainedGenerators (Group group, Generator[] all)
		/// Removes dragged-off gens and adds new ones
		{
			List<Generator> generators = new List<Generator>();
			Rect rect = new Rect(group.guiPos, group.guiSize);
			for (int g=0; g<all.Length; g++)
			{
				if (!rect.Contains(all[g].guiPosition, all[g].guiSize)) continue;
				generators.Add(all[g]);
			}

			return generators.ToArray();
		}


		public static Generator[] GetContainedGenerators (Group group, Graph graph)
			{ return GetContainedGenerators(group, graph.generators); }


		public static Auxiliary[] GetContainedAux (Group group, Auxiliary[] all)
		/// Removes dragged-off gens and adds new ones
		{
			List<Auxiliary> generators = new List<Auxiliary>();
			Rect rect = new Rect(group.guiPos, group.guiSize);
			for (int g=0; g<all.Length; g++)
			{
				if (!rect.Contains(all[g].guiPos, all[g].guiSize)) continue;
				generators.Add(all[g]);
			}

			return generators.ToArray();
		}


		public static void RemoveGroupContents (Group group, Graph graph)
		/// Called from editor. Removes the enlisted generators on group remove
		{
			GraphWindow.RecordCompleteUndo();

			Generator[] containedGens = GetContainedGenerators(group, graph.generators);

			for (int i=0; i<containedGens.Length; i++)
				graph.Remove(containedGens[i]);
		}
	}
}