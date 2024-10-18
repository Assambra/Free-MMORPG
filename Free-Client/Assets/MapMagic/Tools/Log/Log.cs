using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using UnityEngine;

namespace Den.Tools
{
	public static class Log
	{
		public class Entry : IDisposable
		{
			public string name;
			public string id;
			public string idName; //id should be unique, while name shouldn't. Like id=thread, idName=coord for tile
			public long startTicks;
			public long disposeTicks;
			public (string,string)[] fieldValues;
			public List<Entry> subs;
			public bool guiExpanded;

			public void Dispose () => Log.DisposeGroup();

			public int Count
			{get{
				int count = 1;
				if (subs!=null) 
					foreach (Entry sub in subs)
						count += sub.Count;
				return count;
			}}

			public IEnumerable<Entry> SubsRecursive ()
			{
				if (subs==null) yield break;
				foreach (Entry sub in subs)
				{
					yield return sub;

					if (sub.subs != null)
						foreach (Entry subSub in sub.SubsRecursive())
							yield return subSub;
				}
			}
		}

		public static bool enabled = false; 
		private static long unityStartTime = 0;

		public static Entry root = new Entry() {name="Root"};
		private static Entry activeGroup = root; //not among openedGroups  //TODO: make a dictionary thread->group
		private static List<Entry> openedGroups = new List<Entry>();

		private static Entry tempGroup = new Entry(); //to return when recording disabled

		public const string defaultId = "Default";

		
		public static void AddThreaded (string name) => 
			Add(name, Thread.CurrentThread.ManagedThreadId.ToString(), null, null, null);

		public static void AddThreaded (string name, params (string,object)[] additional) => 
			Add(name, Thread.CurrentThread.ManagedThreadId.ToString(), null, null, additional);

		public static void AddThreaded (string name, string idName, params (string,object)[] additional) => 
			Add(name, Thread.CurrentThread.ManagedThreadId.ToString(), idName, null, additional);

		public static void Add (string name, string id, object obj) =>
			Add(name, id, null, obj, null);

		public static void Add (string name, params (string,object)[] additional) =>
			Add(name, null, null, additional);

		public static void Add (string name) =>
			Add(name, null, null, null);

		public static void Add (string name, string id, string idName, object obj, params (string,object)[] additional)
		{
			if (!enabled) return;

			if (id == null)
				id = defaultId;

			Entry entry = new Entry() {name=name, id=id};

			if (unityStartTime == 0)
				unityStartTime = System.Diagnostics.Process.GetCurrentProcess().StartTime.Ticks;
			long currentTime = DateTime.Now.Ticks; //todo: minimize operations after DateTime.Now
			entry.startTicks = currentTime - unityStartTime;

			entry.idName = idName;

			if (obj != null)
				entry.fieldValues = ReadValues(obj);

			if (additional != null)
			{
				(string,string)[] sVals = new (string, string)[additional.Length];
				for (int i=0; i<sVals.Length; i++)
				{
					sVals[i] = (additional[i].Item1, null);
					object item2 = additional[i].Item2;
					if (item2 is string sItem2)
						sVals[i].Item2 = sItem2;
					else
						sVals[i].Item2 = item2.ToString();
				}
				if (entry.fieldValues==null || entry.fieldValues.Length==0) 
					entry.fieldValues = sVals;
				else
					entry.fieldValues = ArrayTools.Append(entry.fieldValues, sVals);
			}
			
			lock (root)
			{
				if (activeGroup.subs == null) activeGroup.subs = new List<Entry>();
				activeGroup.subs.Add(entry);
			}
		}


		public static Entry Group (string name, string id=defaultId)
		{
			if (!enabled) return tempGroup;

			Entry entry = new Entry() {name=name, id=id};

			if (activeGroup.subs == null) activeGroup.subs = new List<Entry>();
			activeGroup.subs.Add(entry);

			openedGroups.Add(activeGroup);
			activeGroup = entry;

			if (unityStartTime == 0)
				unityStartTime = System.Diagnostics.Process.GetCurrentProcess().StartTime.Ticks;
			long currentTime = DateTime.Now.Ticks; //todo: minimize operations after DateTime.Now
			entry.startTicks = currentTime - unityStartTime;

			return entry;
		}


		private static void DisposeGroup ()
		{
			if (!enabled) return;

			long currentTime = DateTime.Now.Ticks;
			long unityStartTime = System.Diagnostics.Process.GetCurrentProcess().StartTime.Ticks;
			activeGroup.disposeTicks = currentTime - unityStartTime;

			activeGroup = openedGroups[openedGroups.Count-1];
			openedGroups.RemoveAt(openedGroups.Count-1);
		}


		private static (string,string)[] ReadValues (object obj)
		{
			Type type = obj.GetType();
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

			(string,string)[] fieldValues = new (string,string)[fields.Length];

			for (int i=0; i<fields.Length; i++)
			{
				string name = fields[i].Name;
				string value = fields[i].GetValue(obj).ToString();

				fieldValues[i] = (name,value);
			}

			return fieldValues;
		}


		public static void Clear () => root.subs.Clear();

		public static int Count => root.Count;

		public static IEnumerable AllEntries ()  //all except root
		{
			foreach (Entry sub in root.SubsRecursive())
				yield return sub;
		}


		public static HashSet<string> UsedThreads ()
		{
			HashSet<string> usedIds = new HashSet<string>();

			foreach (Entry sub in AllEntries())
				usedIds.Add(sub.id);

			return usedIds;
		}

		public static Dictionary<string,int> UsedThreadsNums ()
		///Same as UsedThreads, but also returns number of last entry for this thread
		{
			Dictionary<string,int> usedIdsNums = new Dictionary<string,int>();

			int i=0;
			foreach (Entry sub in AllEntries())
			{
				if (!usedIdsNums.ContainsKey(sub.id))
					usedIdsNums.Add(sub.id, i);
				else
					usedIdsNums[sub.id] = i;

				i++;
			}

			return usedIdsNums;
		}
	}
}