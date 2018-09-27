using System;
using System.Collections.Generic;
using UnityEngine;

public class NamesTester : MonoBehaviour
{
	//public string[] names;
	public List<string> namesFromAsset;
	public int order;
	public int minLength;
	MarkovNameGenerator generator;


	void Update()
	{
		if (Input.GetKey(KeyCode.KeypadEnter))
		{
			TextAsset nameAsset = Resources.Load("Names") as TextAsset;
			namesFromAsset = new List<string>(nameAsset.text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
			//namesFromAsset = new List<string>(nameAsset.text.Split(new[] { '\n', ','}));
			generator = new MarkovNameGenerator(namesFromAsset, order, minLength);
			for (int i = 0; i < 10; i++)
			{
				var name = generator.NextName;
				Debug.Log(name);
			} 
		}
	}
}
