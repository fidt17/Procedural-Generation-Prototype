using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomNoise
{
	private System.Random random;

	public RandomNoise(int seed)
	{
		random = new System.Random(seed);
	}

	///return random value between 0 and maxValue(not included), which can be reachieved with the same seed.
    public int RandomNoise1D(int maxValue) {

		return (int) (random.Next() % maxValue);
	}
}
