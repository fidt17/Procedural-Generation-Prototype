using UnityEngine;
using System.Collections;

public class PerlinNoise
{
	int seed;
	
	public PerlinNoise(int seed)
	{
		this.seed = seed;
	}
	
	private int random(long x, int range){
		return (int)(((x+seed)^5) % range);
	}
	
	private int random(long x, long y, int range){
		return (int)(((x+y*65536+seed)^5) % range);
	}
	
	public void Get1DPerlinNoise(int size, int nOctaves, float fBias, ref float[] perlinArray)
	{
		Random.seed = seed;
		float [] seedArray = new float[size];

		for(int i = 0; i < size; i++) {
			seedArray[i] = Random.Range(0.0f, 1.0f);
		}

		PerlinNoise1D(size, seedArray, nOctaves, fBias, ref perlinArray);
	}

	private void PerlinNoise1D(int nCount, float[] seedArray, int nOctaves, float fBias, ref float[] perlinArray)
	{
		for(int x = 0; x < nCount; x++)
		{
			float fNoise = 0.0f;
			float fScaleAcc = 0.0f;
			float fScale = 1.0f;

			for(int i = 0; i < nOctaves; i++) {

				int nPitch = nCount >> i;

				//index of current pitch * pitch size
				int nSample1 = (x / nPitch) * nPitch;

				//finding start of the next sample. and wrap it around in case it overflows size of needed array
				int nSample2 = (nSample1 + nPitch) % nCount;

				//Liniar interpolation between nSample1 and nSample2
				float fBlend = (float)(x - nSample1) / (float)nPitch;
				float fSample = (1.0f - fBlend)*seedArray[nSample1] + fBlend * seedArray[nSample2];

				fScaleAcc += fScale;
				fNoise += fSample * fScale;

				fScale = fScale / fBias;
			}

			//We are dividing fNoise by fScaleAcc to ensure that the result lies between 0.0f and 1.0f
			perlinArray[x] = fNoise / fScaleAcc;
		}
		
	}
	
	public void Get2DPerlinNoise(int xSize, int ySize, int nOctaves, float fBias, ref float[,] perlinArray)
	{
		Random.seed = seed;
		float [,] seedArray = new float[xSize, ySize];

		for(int x = 0; x < xSize; x++) {
			for(int y = 0; y < ySize; y++) {
				seedArray[x,y] = Random.Range(0.0f, 1.0f);
			}
		}

		PerlinNoise2D(xSize, ySize, seedArray, nOctaves, fBias, ref perlinArray);
	}

	private void PerlinNoise2D(int nWidth, int nHeight, float[,] fSeed, int nOctaves, float fBias, ref float[,] perlinArray) 
	{

		for (int x = 0; x < nWidth; x++) {
			for (int y = 0; y < nHeight; y++)
			{				
				float fNoise = 0.0f;
				float fScaleAcc = 0.0f;
				float fScale = 1.0f;

				for (int o = 0; o < nOctaves; o++)
				{
					int nPitch = nWidth >> o;
					int nSampleX1 = (x / nPitch) * nPitch;
					int nSampleY1 = (y / nPitch) * nPitch;
					
					int nSampleX2 = (nSampleX1 + nPitch) % nWidth;					
					int nSampleY2 = (nSampleY1 + nPitch) % nHeight;

					float fBlendX = (float)(x - nSampleX1) / (float)nPitch;
					float fBlendY = (float)(y - nSampleY1) / (float)nPitch;

					fBlendX = fade(fBlendX);
					fBlendY = fade(fBlendY);

					float fSampleA = lerp(fBlendX, fSeed[nSampleX1, nSampleY1], fSeed[nSampleX2, nSampleY1]);
					float fSampleB = lerp(fBlendX, fSeed[nSampleX1, nSampleY2], fSeed[nSampleX2, nSampleY2]);

					fScaleAcc += fScale;
					fNoise += lerp(fBlendY, fSampleA, fSampleB) * fScale;
					fScale = fScale / fBias;
				}

				// Scale to seed range
				perlinArray[x, y] = fNoise / fScaleAcc;
			}
		}
	}

	public void Get3DPerlinNoise(int xSize, int ySize, int zSize, int nOctaves, float fBias, ref float[,,] perlinArray)
	{
		Random.seed = seed;
		float [,,] seedArray = new float[xSize, ySize, zSize];

		for(int x = 0; x < xSize; x++) {
			for(int y = 0; y < ySize; y++) {
				for(int z = 0; z < zSize; z++) {
					seedArray[x,y,z] = Random.Range(0.0f, 1.0f);
				}
			}
		}

		PerlinNoise3D(xSize, ySize, zSize, seedArray, nOctaves, fBias, ref perlinArray);
	}

	private void PerlinNoise3D(int nWidth, int nHeight, int nDepth, float[,,] fSeed, int nOctaves, float fBias, ref float[,,] perlinArray) 
	{

		for (int x = 0; x < nWidth; x++) {
			for (int y = 0; y < nHeight; y++) {
				for (int z = 0; z < nDepth; z++) {

					float fNoise = 0.0f;
					float fScaleAcc = 0.0f;
					float fScale = 1.0f;

					for (int o = 0; o < nOctaves; o++)
					{
						int nPitch = nWidth >> o;
						int nSampleX1 = (x / nPitch) * nPitch;
						int nSampleY1 = (y / nPitch) * nPitch;
						int nSampleZ1 = (z / nPitch) * nPitch;///
					
						int nSampleX2 = (nSampleX1 + nPitch) % nWidth;					
						int nSampleY2 = (nSampleY1 + nPitch) % nHeight;
						int nSampleZ2 = (nSampleZ1 + nPitch) % nDepth;///

						float fBlendX = (float)(x - nSampleX1) / (float)nPitch;
						float fBlendY = (float)(y - nSampleY1) / (float)nPitch;
						float fBlendZ = (float)(z - nSampleZ1) / (float)nPitch;///

						fBlendX = fade(fBlendX);
						fBlendY = fade(fBlendY);
						fBlendZ = fade(fBlendZ);///

						float fSampleA  = lerp(fBlendX, fSeed[nSampleX1, nSampleY1, nSampleZ1], fSeed[nSampleX2, nSampleY1, nSampleZ1]);
						float fSampleB  = lerp(fBlendX, fSeed[nSampleX1, nSampleY2, nSampleZ1], fSeed[nSampleX2, nSampleY2, nSampleZ1]);
						float plane1 = lerp(fBlendY, fSampleA, fSampleB);

						float fSampleC  = lerp(fBlendX, fSeed[nSampleX1, nSampleY1, nSampleZ2], fSeed[nSampleX2, nSampleY1, nSampleZ2]);
						float fSampleD  = lerp(fBlendX, fSeed[nSampleX1, nSampleY2, nSampleZ2], fSeed[nSampleX2, nSampleY2, nSampleZ2]);
						float plane2 = lerp(fBlendY, fSampleC, fSampleD);


						fScaleAcc += fScale;
						fNoise += lerp(fBlendZ, plane1, plane2) * fScale;
						fScale = fScale / fBias;
					}

					// Scale to seed range
					perlinArray[x, y, z] = fNoise / fScaleAcc;
				}
			}
		}
	}

	private float fade(float t)
	{ 
		return t * t * t * (t * (t * 6 - 15) + 10);
	}

	private float lerp(float t, float a, float b)
	{
		return a + t * (b - a); 
	}
}
