﻿using System.Collections.Generic;
using AutoGenerated.DB;
using UnityEngine;

namespace Hello
{
	public class main : MonoBehaviour
	{
		void Start()
		{
			DataService ds = new DataService("Assets/output/DB.db", "helloworld");
			List<SampleCharacter> characters = ds.Gets<SampleCharacter>();
			foreach (SampleCharacter c in characters)
			{
				Debug.Log($"{c.id} | {c.character_id} | {c.level} | {c.name}");
			}
		}
	}
}