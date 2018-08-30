using System;
using System.Collections.Generic;

public class MarkovNameGenerator
{
	Dictionary<string, List<char>> chains = new Dictionary<string, List<char>>();
	List<string> sample = new List<string>();
	List<string> used = new List<string>();
	Random random = new Random();
	int order;
	int minLength;

	public MarkovNameGenerator(IEnumerable<string> sampleNames, int order, int minLength)
	{
		//clamp parameters
		if(order < 1) { order = 1; }
		if(minLength < 1) { minLength = 1; }

	}
}