
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using Den.Tools;
using Den.Tools.GUI;


//namespace MapMagic.Core 
namespace Den.Tools.Matrices
{
	public static class StripeFieldDrawer
	{
		[Draw.Editor(typeof(MatrixOps.Stripe))]
		public static void DrawStripe (MatrixOps.Stripe stripe)
		{
			using (Cell.LineStd) Draw.Label("Test");
			using (Cell.LineStd) Draw.Label("Test");
		}
	}
}