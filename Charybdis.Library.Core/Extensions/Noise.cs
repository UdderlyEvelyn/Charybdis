using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public static class Noise
    {
        public static Array2<bool> Random(this Array2<bool> data)
        {
            //Random random = new Random();
            for (int i = 0; i < data.Count; i++)
                data.Set(i, data.Random.Next(1) == 1);
            return data;
        }

        public static Array2<byte> RandomStatic(this Array2<byte> data, int min = 0, int max = 255)
        {
            //Random random = new Random();
            int maxBound = max + 1;
            for (int i = 0; i < data.Count; i++)
            {
                data.Set(i, (byte)data.Random.Next(min, maxBound));
            }
            return data;
        }

        public static Array2<byte> PercentageStatic(this Array2<byte> data, int percent = 10, int min = 100, int max = 255)
        {
            //Random random = new Random();
            int maxBound = max + 1;
            for (int i = 0; i < data.Count; i++)
            {
                if (data.Random.Next(0, percent) == 0)
                    data.Set(i, (byte)data.Random.Next(min, maxBound));
            }
            return data;
        }

        //public static void Perlin(this Array2<float> data, int octaves = 11, float maxValue = 1)
        //{
        //    Array2<float> perlin = GeneratePerlinNoise(data.Width, data.Height, octaves);
        //    for (int i = 0; i < data.Count; i++)
        //        data.Set(i, perlin.Get(i) * maxValue);
        //}

        //public static void Perlin(this Array2<float> data, int xOffset, int yOffset, int octaves = 11)
        //{
        //    Array2<float> baseNoise = new Array2<float>(data.Width, data.Height).RawSimplexNoise(xOffset, yOffset, 0, octaves);
        //    Array2<float> perlin = GeneratePerlinNoise(baseNoise, octaves / 2);
        //    for (int i = 0; i < data.Count; i++)
        //        data.Set(i, perlin.Get(i));
        //}

        public static Array2<byte> Perlin(this Array2<byte> data, int octaves = 11, byte maxValue = 255)
        {
            Array2<float> perlin = GeneratePerlinNoise(data.Width, data.Height, octaves);
            for (int i = 0; i < data.Count; i++)
            {
                data.Set(i, (byte)(perlin.Get(i) * maxValue));
            }
            return data;
        }

        public static Array2<byte> Perlin(this Array2<byte> data, int xOffset, int yOffset, int octaves = 11)
        {
            Array2<float> baseNoise = new Array2<float>(data.Width, data.Height).RawSimplexNoise(xOffset, yOffset, 0, octaves);
            Array2<float> perlin = GeneratePerlinNoise(baseNoise, octaves / 2);
            for (int i = 0; i < data.Count; i++)
            {
                data.Set(i, (byte)(perlin.Get(i) * 255));
            }
            return data;
        }

        public static Array2<byte> Perlin2(this Array2<byte> data, int xOffset, int yOffset, int octaves = 11)
        {
            Array2<float> perlin = PerlinNoise(data.Width, data.Height, xOffset, yOffset, octaves);
            for (int i = 0; i < data.Count; i++)
            {
                data.Set(i, (byte)(perlin.Get(i) * 255));
            }
            return data;
        }

        public static Array2<byte> SmoothNoise(this Array2<byte> data, int octaves = 3)
        {
            return GenerateSmoothNoise(GenerateWhiteNoise(data.Width, data.Height), octaves).ToByteArray();
        }

        /// <summary>
        /// Garbage
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="octaveCount"></param>
        /// <returns></returns>
        private static Array2<float> PerlinNoise(int width, int height, int xOffset, int yOffset, int octaveCount)
        {
            Array2<float> baseNoise = new Array2<float>(width, height).RawSimplexNoise(xOffset, yOffset, 0, 11);

            Array2<float>[] smoothNoise = new Array2<float>[octaveCount]; //an array of 2D arrays containing

            float persistance = 0.9f;

            //generate smooth noise
            for (int i = 0; i < octaveCount; i++)
            {
                smoothNoise[i] = GenerateSmoothNoise(baseNoise, i);
            }

            Array2<float> perlinNoise = new Array2<float>(width, height); //an array of floats initialised to 0

            float amplitude = 1.0f;
            float totalAmplitude = 0.0f;

            //blend noise together
            for (int octave = octaveCount - 1; octave >= 0; octave--)
            {
                amplitude *= persistance;
                totalAmplitude += amplitude;

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        perlinNoise.Set(i, j, perlinNoise.Get(i, j) + smoothNoise[octave].Get(i, j) * amplitude);
                    }
                }
            }

            //normalisation
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    perlinNoise.Set(i, j, ((perlinNoise.Get(i, j) / totalAmplitude) + baseNoise.Get(i, j)) / 2);
                }
            }

            return perlinNoise;
        }

        #region Perlin Backend
        //This region contains code from http://devmag.org.za/2009/04/25/perlin-noise/ adapted to the Array2 class rather than the jagged arrays used in the original (done 7/1/14).

        private static Array2<float> GenerateWhiteNoise(int width, int height, int xOffset = 0, int yOffset = 0)
        {
            Array2<float> noise = new Array2<float>(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    noise.Set(i, j, (float)noise.Random.NextDouble() % 1);
                }
            }

            return noise;
        }

        private static float Interpolate(float x0, float x1, float alpha)
        {
            return x0 * (1 - alpha) + alpha * x1;
        }

        private static Array2<float> GenerateSmoothNoise(Array2<float> baseNoise, int octave)
        {
            int width = baseNoise.Width;
            int height = baseNoise.Height;

            Array2<float> smoothNoise = new Array2<float>(width, height);

            int samplePeriod = 1 << octave; // calculates 2 ^ k
            float sampleFrequency = 1.0f / samplePeriod;

            for (int i = 0; i < width; i++)
            {
                //calculate the horizontal sampling indices
                int sample_i0 = (i / samplePeriod) * samplePeriod;
                int sample_i1 = (sample_i0 + samplePeriod) % width; //wrap around
                float horizontal_blend = (i - sample_i0) * sampleFrequency;

                for (int j = 0; j < height; j++)
                {
                    //calculate the vertical sampling indices
                    int sample_j0 = (j / samplePeriod) * samplePeriod;
                    int sample_j1 = (sample_j0 + samplePeriod) % height; //wrap around
                    float vertical_blend = (j - sample_j0) * sampleFrequency;

                    //blend the top two corners
                    float top = Interpolate(baseNoise.Get(sample_i0, sample_j0),
                        baseNoise.Get(sample_i1, sample_j0), horizontal_blend);

                    //blend the bottom two corners
                    float bottom = Interpolate(baseNoise.Get(sample_i0, sample_j1),
                        baseNoise.Get(sample_i1, sample_j1), horizontal_blend);

                    //final blend
                    smoothNoise.Set(i, j, Interpolate(top, bottom, vertical_blend));
                }
            }

            return smoothNoise;
        }

        private static Array2<float> GeneratePerlinNoise(Array2<float> baseNoise, int octaveCount)
        {
            int width = baseNoise.Width;
            int height = baseNoise.Height;

            Array2<float>[] smoothNoise = new Array2<float>[octaveCount]; //an array of 2D arrays containing

            float persistance = 0.9f;

            //generate smooth noise
            for (int i = 0; i < octaveCount; i++)
            {
                smoothNoise[i] = GenerateSmoothNoise(baseNoise, i);
            }

            Array2<float> perlinNoise = new Array2<float>(width, height); //an array of floats initialised to 0

            float amplitude = 1.0f;
            float totalAmplitude = 0.0f;

            //blend noise together
            for (int octave = octaveCount - 1; octave >= 0; octave--)
            {
                amplitude *= persistance;
                totalAmplitude += amplitude;

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        perlinNoise.Set(i, j, perlinNoise.Get(i, j) + smoothNoise[octave].Get(i, j) * amplitude);
                    }
                }
            }

            //normalisation
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    perlinNoise.Set(i, j, perlinNoise.Get(i, j) / totalAmplitude);
                }
            }

            return perlinNoise;
        }

        private static Array2<float> GeneratePerlinNoise(int width, int height, int octaveCount)
        {
            Array2<float> baseNoise = GenerateWhiteNoise(width, height);
            return GeneratePerlinNoise(baseNoise, octaveCount);
        }
        #endregion

        //public static Array2<byte> HybridNoise(this Array2<byte> data, int slice = 0, int octaves = 5, int translationFactor = 255, int bias = 0)
        //{
        //    Array2<float> simplex = new Array2<float>(data.Width, data.Height);
        //    for (int y = 0; y < data.Height; y++)
        //    {
        //        for (int x = 0; x < data.Width; x++)
        //        {
        //            simplex.Set(x, y, GetOctaveNoise(x, y, slice, octaves) * translationFactor + bias);
        //        }
        //    }
        //    Array2<float> perlin = GeneratePerlinNoise(simplex, 11);
        //    for (int i = 0; i < data.Count; i++)
        //    {
        //        data.Set(i, (byte)(perlin.Get(i) * 255));
        //    }
        //    return data;
        //}

        //public static Array2<byte> HybridNoise(this Array2<byte> data, int xOffset, int yOffset, int slice = 0, int octaves = 5, int translationFactor = 255, int bias = 0)
        //{
        //    Array2<float> simplex = new Array2<float>(data.Width, data.Height);
        //    for (int y = 0; y < data.Height; y++)
        //    {
        //        for (int x = 0; x < data.Width; x++)
        //        {
        //            simplex.Set(x, y, GetOctaveNoise(x + xOffset, y + yOffset, slice, octaves) * translationFactor + bias);
        //        }
        //    }
        //    Array2<float> perlin = GeneratePerlinNoise(simplex, 11);
        //    for (int i = 0; i < data.Count; i++)
        //    {
        //        data.Set(i, (byte)(perlin.Get(i) * 255));
        //    }
        //    return data;
        //}

        public static Array2<byte> SimplexNoise(this Array2<byte> data, NoiseArgs na, int slice = 0, int translationFactor = 255, int bias = 0)
        {
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    data.Set(x, y, (byte)(Math.Max(0, Math.Min(255, GetOctaveNoise(x, y, na, slice) * translationFactor + bias))));
                }
            }
            return data;
        }

        public static Array2<float> RawSimplexNoise(this Array2<float> data, int xOffset, int yOffset, int slice = 0, int octaves = 5)
        {
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    data.Set(x, y, GetOctaveNoise(x + xOffset, y + yOffset, new NoiseArgs { octaves = octaves }, slice));
                }
            }
            return data;
        }

        #region Simplex Backend
        /*
         * A speed-improved simplex noise algorithm for 2D, 3D and 4D in Java.
         *
         * Based on example code by Stefan Gustavson (stegu@itn.liu.se).
         * Optimisations by Peter Eastman (peastman@drizzle.stanford.edu).
         * Better rank ordering method by Stefan Gustavson in 2012.
         *
         * This could be speeded up even further, but it's useful as it is.
         *
         * Version 2012-03-09
         *
         * This code was placed in the public domain by its original author,
         * Stefan Gustavson. You may use it as you see fit, but
         * attribution is appreciated.
         *
         * Update by NightCabbage (2013-11-05) NightCabbage@gmail.com
         * 
         * Working with Stefan (thanks!) I have compiled all of the
         * improvements I could find and put them into this code.
         * 
         * Note that for corner contribution I have made the decision here to
         * use 0.6 instead of 0.5, as I believe it looks a bit better for 2d
         * purposes (0.5 made it a bit more grey, and also had more pulsating for
         * integral inputs). If you're using it for bumpmaps or similar, feel
         * free to change it - and the final scale factor is 76 (as opposed to 32).
         */

        public static double XScale = 0.02;
        public static double YScale = 0.02;
        public static double ZScale = 1;

        public static double Scale
        {
            set
            {
                XScale = value;
                YScale = value;
            }
        }

        private static Vec4[] grad3 = new Vec4[] {
            new Vec4(1,1,0,0), new Vec4(-1,1,0,0), new Vec4(1,-1,0,0), new Vec4(-1,-1,0,0),
            new Vec4(1,0,1,0), new Vec4(-1,0,1,0), new Vec4(1,0,-1,0), new Vec4(-1,0,-1,0),
            new Vec4(0,1,1,0), new Vec4(0,-1,1,0), new Vec4(0,1,-1,0), new Vec4(0,-1,-1,0)
        };

        private static byte[] p = new byte[] {
            151,160,137,91,90,15,131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,190,6,148,
            247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,88,237,149,56,87,174,20,125,136,171,168,68,175,
            74,165,71,134,139,48,27,166,77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,102,143,54,
            65,25,63,161,1,216,80,73,209,76,132,187,208,89,18,169,200,196,135,130,116,188,159,86,164,100,109,198,173,186,3,64,
            52,217,226,250,124,123,5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,223,183,170,213,
            119,248,152,2,44,154,163,70,221,153,101,155,167,43,172,9,129,22,39,253,19,98,108,110,79,113,224,232,178,185,112,104,
            218,246,97,228,251,34,242,193,238,210,144,12,191,179,162,241,81,51,145,235,249,14,239,107,49,192,214,31,181,199,106,157,
            184,84,204,176,115,121,50,45,127,4,150,254,138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
        };

        // To remove the need for index wrapping, double the permutation table length
        private static byte[] perm = new byte[512];
        private static byte[] permMod12 = new byte[512];

        private static bool init = false;
        private static void Initialize()
        {
            if (init == true) return;
            else init = true;

            for(int i = 0; i < 512; i++)
            {
                perm[i] = p[i & 255];
                permMod12[i] = (byte)(perm[i] % 12);
            }
        }

        // Skewing and unskewing factors for 2, 3, and 4 dimensions
        private static double F3 = 1.0 / 3.0;
        private static double G3 = 1.0 / 6.0;

        // This method is a *lot* faster than using (int)Math.floor(x)
        private static int fastfloor(double x)
        {
            int xi = (int)x;
            return x < xi ? xi - 1 : xi;
        }

        private static double dot(Vec4 g, double x, double y, double z)
        {
            return g.X * x + g.Y * y + g.Z * z;
        }

        // 3D simplex noise
        public static float GetNoise(double xin, double yin, double zin, float persistance = .5f, float cornerContribution = .6f)
        {
            //float persistance = .4f; //Default .5f
            //float cornerContribution = .7f; //Default .6f
            Initialize();
            double n0, n1, n2, n3; // Noise contributions from the four corners
            // Skew the input space to determine which simplex cell we're in
            double s = (xin+yin+zin)*F3; // Very nice and simple skew factor for 3D
            int i = fastfloor(xin+s);
            int j = fastfloor(yin+s);
            int k = fastfloor(zin+s);
            double t = (i+j+k)*G3;
            double X0 = i-t; // Unskew the cell origin back to (x,y,z) space
            double Y0 = j-t;
            double Z0 = k-t;
            double x0 = xin-X0; // The x,y,z distances from the cell origin
            double y0 = yin-Y0;
            double z0 = zin-Z0;
            // For the 3D case, the simplex shape is a slightly irregular tetrahedron.
            // Determine which simplex we are in.
            int i1, j1, k1; // Offsets for second corner of simplex in (i,j,k) coords
            int i2, j2, k2; // Offsets for third corner of simplex in (i,j,k) coords
            if(x0>=y0) {
            if(y0>=z0)
            { i1=1; j1=0; k1=0; i2=1; j2=1; k2=0; } // X Y Z order
            else if(x0>=z0) { i1=1; j1=0; k1=0; i2=1; j2=0; k2=1; } // X Z Y order
            else { i1=0; j1=0; k1=1; i2=1; j2=0; k2=1; } // Z X Y order
            }
            else { // x0<y0
            if(y0<z0) { i1=0; j1=0; k1=1; i2=0; j2=1; k2=1; } // Z Y X order
            else if(x0<z0) { i1=0; j1=1; k1=0; i2=0; j2=1; k2=1; } // Y Z X order
            else { i1=0; j1=1; k1=0; i2=1; j2=1; k2=0; } // Y X Z order
            }
            // A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z),
            // a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z), and
            // a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z), where
            // c = 1/6.
            double x1 = x0 - i1 + G3; // Offsets for second corner in (x,y,z) coords
            double y1 = y0 - j1 + G3;
            double z1 = z0 - k1 + G3;
            double x2 = x0 - i2 + 2.0*G3; // Offsets for third corner in (x,y,z) coords
            double y2 = y0 - j2 + 2.0*G3;
            double z2 = z0 - k2 + 2.0*G3;
            double x3 = x0 - 1.0 + 3.0*G3; // Offsets for last corner in (x,y,z) coords
            double y3 = y0 - 1.0 + 3.0*G3;
            double z3 = z0 - 1.0 + 3.0*G3;
            // Work out the hashed gradient indices of the four simplex corners
            int ii = i & 255;
            int jj = j & 255;
            int kk = k & 255;
            int gi0 = permMod12[ii+perm[jj+perm[kk]]];
            int gi1 = permMod12[ii+i1+perm[jj+j1+perm[kk+k1]]];
            int gi2 = permMod12[ii+i2+perm[jj+j2+perm[kk+k2]]];
            int gi3 = permMod12[ii+1+perm[jj+1+perm[kk+1]]];
            // Calculate the contribution from the four corners
            double t0 = cornerContribution - x0 * x0 - y0 * y0 - z0 * z0; // change to 0.5 if you want
            if(t0<0) n0 = 0.0;
            else {
            t0 *= t0;
            n0 = t0 * t0 * dot(grad3[gi0], x0, y0, z0);
            }
            double t1 = cornerContribution - x1 * x1 - y1 * y1 - z1 * z1; // change to 0.5 if you want
            if(t1<0) n1 = 0.0;
            else {
            t1 *= t1;
            n1 = t1 * t1 * dot(grad3[gi1], x1, y1, z1);
            }
            double t2 = cornerContribution - x2 * x2 - y2 * y2 - z2 * z2; // change to 0.5 if you want
            if(t2<0) n2 = 0.0;
            else {
            t2 *= t2;
            n2 = t2 * t2 * dot(grad3[gi2], x2, y2, z2);
            }
            double t3 = cornerContribution - x3 * x3 - y3 * y3 - z3 * z3; // change to 0.5 if you want
            if(t3<0) n3 = 0.0;
            else {
            t3 *= t3;
            n3 = t3 * t3 * dot(grad3[gi3], x3, y3, z3);
            }
            // Add contributions from each corner to get the final noise value.
            // The result is scaled to stay just inside [-1,1] (now [0, 1])
            return (float)(32.0 * (n0 + n1 + n2 + n3) + 1) * persistance;; // change to 76.0 if you want
        }

        // get multiple octaves of noise at once
        /*public static float GetOctaveNoise(double pX, double pY, double pZ, int pOctaves)
        {
            float value = 0;
            float divisor = 0;
            float currentHalf = 0;
            float currentDouble = 0;

            for(int i = 0; i < pOctaves; i++)
            {
                currentHalf = (float)Math.Pow(0.5f, i);
                currentDouble = (float)Math.Pow(2, i);
                value += GetNoise(pX * currentDouble, pY * currentDouble, pZ) * currentHalf;
                divisor += currentHalf;
            }

            return value / divisor;
        }*/
        #endregion

        public static float GetOctaveNoise(double x, double y, NoiseArgs na, double slice = 0)
        {
            float sum = 0;
            float curWeight = 1f;
            for (int i = 0; i < na.octaves; i++)
            {
                float exp = 1 << i;
                float div = (float)Math.Pow(.5f, i);
                sum += curWeight * GetNoise(x * exp, y * exp, slice + i, na.persistance, na.cornerContribution);
                curWeight *= na.octavePersistance;
            }
            return sum / (float)Math.Pow(na.octaves, na.sumExponent);
        }

        public static float GetOctaveNoise(double x, double y, double z, NoiseArgs na)
        {
            float sum = 0;
            float curWeight = 1f;
            for (int i = 0; i < na.octaves; i++)
            {
                float exp = 1 << i;
                float div = (float)Math.Pow(.5f, i);
                sum += curWeight * GetNoise(x * exp, y * exp, z * exp, na.persistance, na.cornerContribution);
                curWeight *= na.octavePersistance;
            }
            return sum / (float)Math.Pow(na.octaves, na.sumExponent);
        }

        /*public static Array2<float> tStaticNoise(this Array2<float> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                data.Set(i, (float)Globals.Random.NextDouble());
            }
            return data;
        }*/

        /*public static Array2<float> tNormalizedNoise(this Array2<float> data, double seed, int xOffset = 0, int yOffset = 0)
        {
            int radiusX = data.Width / 2;
            int radiusY = data.Height / 2;
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    float nx = (x + xOffset - radiusX) / (float)radiusX;
                    float ny = (y + yOffset - radiusY) / (float)radiusY;
                    data.Set(x, y, GetOctaveNoise(nx, ny, seed, 10));
                }
            }
            return data;
        }*/

        public static Array3<float> NormalizedNoise(this Array3<float> data, NoiseArgs na, int xOffset = 0, int yOffset = 0, int projectedWidth = 0, int projectedHeight = 0)
        {
            int radiusX = data.Width / 2;
            int radiusY = data.Height / 2;
            if (projectedWidth > 0 && projectedHeight > 0)
            {
                radiusX = projectedWidth / 2;
                radiusY = projectedHeight / 2;
            }
            for (int z = 0; z < data.Depth; z++)
                for (int y = 0; y < data.Height; y++)
                    for (int x = 0; x < data.Width; x++)
                    {
                        float nx = (x + xOffset - radiusX) / (float)radiusX;
                        float ny = (y + yOffset - radiusY) / (float)radiusY;
                        data.Set(x, y, z, GetOctaveNoise(nx, ny, z, na));
                    }
            return data;
        }

        public static Array2<float> NormalizedNoise(this Array2<float> data, double seed, NoiseArgs na, int xOffset = 0, int yOffset = 0, int projectedWidth = 0, int projectedHeight = 0)
        {
            int radiusX = data.Width / 2;
            int radiusY = data.Height / 2;
            if (projectedWidth > 0 && projectedHeight > 0)
            {
                radiusX = projectedWidth / 2;
                radiusY = projectedHeight / 2;
            }
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    float nx = (x + xOffset - radiusX) / (float)radiusX;
                    float ny = (y + yOffset - radiusY) / (float)radiusY;
                    data.Set(x, y, GetOctaveNoise(nx, ny, na, seed));
                }
            }
            return data;
        }

        /*public static Array2<byte> tTestNoise(this Array2<byte> data, double seed, int xOffset = 0, int yOffset = 0)
        {
            Array2<float> temp = new Array2<float>(data.Width, data.Height).tNormalizedNoise(seed, xOffset, yOffset);
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    data.Set(x, y, (byte)(Math.Abs(temp.Get(x, y)) * 255));
                }
            }
            return data;
        }*/

        //Note: this is not set up to cope with non-square arrays. It works fine, but there will be some amount of distortion/stretching.
        public static Array2<byte> SimplexNoise(this Array2<byte> data, double seed, NoiseArgs? na = null, int xOffset = 0, int yOffset = 0, int projectedWidth = 0, int projectedHeight = 0)
        {
            if (na == null) na = DefaultNoiseArgs;
            Array2<float> temp = new Array2<float>(data.Width, data.Height).NormalizedNoise(seed, na.Value, xOffset, yOffset, projectedWidth, projectedHeight);
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    data.Set(x, y, (byte)(Math.Abs(temp.Get(x, y)) * 255));
                }
            }
            return data;
        }

        public struct NoiseArgs
        {
            public float persistance;
            public float cornerContribution;
            public int octaves;
            public float octavePersistance;
            public double sumExponent;

            public NoiseArgs(float persistance = .5f, float cornerContribution = .6f, int octaves = 11, float octavePersistance = .7f, double sumExponent = .4)
            {
                this.persistance = persistance;
                this.cornerContribution = cornerContribution;
                this.octaves = octaves; 
                this.octavePersistance = octavePersistance; 
                this.sumExponent = sumExponent;
            }
        }

        public static NoiseArgs DefaultNoiseArgs = new Noise.NoiseArgs(.6f, .6f, 7, .5f, .4);
    }
}
