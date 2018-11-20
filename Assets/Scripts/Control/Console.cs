using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Console : MonoBehaviour {

	private static Console thisConsole;

	[SerializeField]
	public bool printResults = false;

	float lastPrint = 0;
	float increment = 10;
	float mod = 1000 * 1000;

	SortedList<string, SortedList<string, Function>> scripts;

	void Awake()
	{
		if(thisConsole == null)
		{
			DontDestroyOnLoad(this);
			thisConsole = this;

			scripts = new SortedList<string, SortedList<string, Function>>();
		}
		else
		{
			Destroy(this.gameObject);
		}
	}

	public static Function_Instance Start(string ref_class, string ref_function)
	{
		if(!thisConsole.scripts.ContainsKey(ref_class))
		{
			thisConsole.scripts.Add(ref_class, new SortedList<string, Function>());
		}
		if(!thisConsole.scripts[ref_class].ContainsKey(ref_function))
		{
			thisConsole.scripts[ref_class].Add(ref_function, new Function());
		}
		
		return new Function_Instance(thisConsole.scripts[ref_class][ref_function]);
	}

	void Update()
	{
		if(Time.time > lastPrint && printResults)
		{
			lastPrint += increment;

			string result = "Performance Marker at " + Time.time + "\n";

			float timeSpent = 0;
			foreach(KeyValuePair<string, SortedList<string, Function>> classes in scripts)
			{
				foreach(KeyValuePair<string, Function> func in classes.Value)
				{
					timeSpent += func.Value.stats.lifeTotal;
				}
			}

			foreach(KeyValuePair<string, SortedList<string, Function>> classes in scripts)
			{
				float classt = 0;
				float classi = 0;
				foreach(KeyValuePair<string, Function> func in classes.Value)
				{
					classt += func.Value.stats.lifeTotal;
					classi += func.Value.stats.lifeInstances;
				}

				result += "Class: " + classes.Key + ", " + (classt / timeSpent) * 100 + "%, Avg: " + (classt / classi) * mod + "\n";

				foreach(KeyValuePair<string, Function> func in classes.Value)
				{
					result += "\tFunction: " + func.Key + ", " + (func.Value.stats.lifeTotal / classt) * 100 + "%, Avg: " + func.Value.stats.lifeAverage * mod + ", Min: " + func.Value.stats.lifeMin * mod + ", Max: " + func.Value.stats.lifeMax * mod + "\n";

					foreach(KeyValuePair<string, Statistics> fun_flag in func.Value.flagStats)
					{
						result += "\t\tFlag: " + fun_flag.Key + ", Avg: " + fun_flag.Value.lifeAverage * mod + ", Min: " + fun_flag.Value.lifeMin * mod + ", Max: " + fun_flag.Value.lifeMax * mod + "\n";
					}
				}

				result += "\n\n";
			}

			print(result);
		}
	}

	internal class Statistics
	{
		internal int lifeInstances = 0;
		
		internal float lifeMin = Mathf.Infinity;
		internal float lifeTotal = 0;
		internal float lifeMax = 0;
		
		internal float lifeAverage
		{ get { return lifeTotal / lifeInstances; } }
		
		public void Add(float t)
		{
			if(t < lifeMin)
			{
				lifeMin = t;
			}
			if(t > lifeMax)
			{
				lifeMax = t;
			}
			
			lifeInstances++;
			lifeTotal += t;
		}
	}

	public class Function
	{
		internal Statistics stats;
		internal SortedList<string, Statistics> flagStats;
		
		internal Function()
		{
			stats = new Statistics();
			flagStats = new SortedList<string, Statistics>();
		}

		internal void AddInstance(Function_Instance inst)
		{
			stats.Add (inst.totalTime);
			foreach(string key in inst.flag_times.Keys)
			{
				if(!flagStats.ContainsKey(key))
				{
					flagStats.Add(key, new Statistics());
				}
				flagStats[key].Add(inst.flag_times[key]);
			}
		}
	}

	public class Function_Instance
	{
		internal float ref_frame;
		internal Function parent;

		private float startTime;

		private SortedList<string, float> flag_starts;
		internal SortedList<string, float> flag_times;
		
		internal float totalTime = 0;
		
		public Function_Instance(Function parent)
		{
			ref_frame = Time.time;
			this.parent = parent;

			startTime = Time.realtimeSinceStartup;

			flag_starts = new SortedList<string, float>();
			flag_times = new SortedList<string, float>();
		}

		public void Pause()
		{
			totalTime += Time.realtimeSinceStartup - startTime;
			startTime = -1;

			foreach(string flagid in flag_starts.Keys)
			{
				if(!flag_times.ContainsKey(flagid))
				{
					flag_times.Add(flagid, 0);
				}
				
				flag_times[flagid] += Time.realtimeSinceStartup - flag_starts[flagid];
			}
		}

		public void Resume()
		{
			startTime = Time.realtimeSinceStartup;
			foreach(string flagid in flag_starts.Keys)
			{
				flag_starts[flagid] = Time.realtimeSinceStartup;
			}
		}

		public void Start_Flag(string flagid)
		{
			flag_starts [flagid] = Time.realtimeSinceStartup;
		}

		public void End_Flag(string flagid)
		{
			if(flag_starts.ContainsKey(flagid))
			{
				if(!flag_times.ContainsKey(flagid))
				{
					flag_times.Add(flagid, 0);
				}

				flag_times[flagid] += Time.realtimeSinceStartup - flag_starts[flagid];
				flag_starts.Remove(flagid);
			}
		}
		
		public void Throw(string error)
		{
			End ();
			throw new UnityEngine.UnityException (error);
		}
		
		public void End()
		{
			totalTime += (Time.realtimeSinceStartup - startTime);

			foreach(string flag in flag_starts.Keys)
			{
				End_Flag(flag);
			}

			parent.AddInstance(this);
		}
	}
}
