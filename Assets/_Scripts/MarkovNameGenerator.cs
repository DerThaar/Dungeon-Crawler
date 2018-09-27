using System;
using System.Collections.Generic;

public class MarkovNameGenerator
{
	Dictionary<string, List<char>> chains = new Dictionary<string, List<char>>();
	List<string> samples = new List<string>();
	List<string> used = new List<string>();
	Random random = new Random();
	int order;
	int minLength;


	public MarkovNameGenerator(IEnumerable<string> sampleNames, int order, int minLength)
	{
		//clamp parameters
		if (order < 1) { order = 1; }
		if (minLength < 1) { minLength = 1; }
		this.order = order;
		this.minLength = minLength;

		//splitting and formating
		foreach (string line in sampleNames)
		{
			string[] tokens = line.Split(',');
			foreach (string word in tokens)
			{
				string upper = word.Trim().ToUpper();
				if (upper.Length < order + 1)
				{
					continue;
				}
				samples.Add(upper);
			}
		}

		//build chains
		foreach (string word in samples)
		{
			for (int letter = 0; letter < word.Length - order; letter++)
			{
				string token = word.Substring(letter, order);
				List<char> entry = null;
				if (chains.ContainsKey(token))
				{
					entry = chains[token];
				}
				else
				{
					entry = new List<char>();
					chains[token] = entry;
				}
				entry.Add(word[letter + order]);
			}
		}
	}

	public string NextName
	{
		get
		{
			string s = "";
			do
			{
				int n = random.Next(samples.Count);
				int nameLength = samples[n].Length;
				s = samples[n].Substring(random.Next(0, samples[n].Length - order), order);
				while (s.Length < nameLength)
				{
					string token = s.Substring(s.Length - order, order);
					char c = GetLetter(token);
					if (c != '?') { s += c; }
					else { break; }
				}

				if (s.Contains(" "))
				{
					string[] tokens = s.Split(' ');
					s = "";
					for (int t = 0; t < tokens.Length; t++)
					{
						if (tokens[t] == "") { continue; }
						//first letter upper case, rest lower case
						else { tokens[t] = tokens[t].Substring(0, 1) + tokens[t].Substring(1).ToLower(); }
						if (s != "") { s += " "; }
						s += tokens[t];
					}
				}
				//first letter upper case, rest lower case
				else { s = s.Substring(0, 1) + s.Substring(1).ToLower(); }
			}
			while (used.Contains(s) || s.Length < minLength);
			used.Add(s);
			return s;
		}
	}

	public void Reset()
	{
		used.Clear();
	}

	char GetLetter(string token)
	{
		if (!chains.ContainsKey(token))
		{
			return '?';
		}
		List<char> letters = chains[token];
		int n = random.Next(letters.Count);
		return letters[n];
	}
}