using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public static class Blur
    {
        public struct BlurWeights
        {
            public const double Low = (double)1 / 18;
            public const double Medium = (double)1 / 9;
            public const double High = (double)3 / 9;
        }

        public struct LayeredBlurWeights
        {
            public const double Base = .75 / 3 / 3 / 3;
            public const double Low = .75 / 3 / 3;
            public const double Medium = .75 / 3;
            public const double High = .75;
        }

        public static Array2<byte> CheapBlur(this Array2<byte> data, int passes = 1)
        {
            Array2<byte> temp = new Array2<byte>(data.Width, data.Height);
            for (int p = 0; p < passes; p++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    for (int x = 0; x < data.Width; x++)
                    {
                        double gridTotal = data.Get(x, y); //X (Self)
                        double gridCount = 1;

                        if (y > 0) //Not first row.
                        {
                            if (x > 0) //Not left column.
                            {
                                gridTotal += data.Get(x - 1, y - 1); //Up/Left from X
                                gridCount++;
                            }

                            if (x < (data.Width - 1)) //Not right column.
                            {
                                gridTotal += data.Get(x + 1, y - 1); //Up/Right from X
                                gridCount++;
                            }

                            gridTotal += data.Get(x, y - 1); //Up from X
                            gridCount++;
                        }

                        if (y < (data.Height - 1)) //Not last row.
                        {
                            if (x > 0) //Not left column.
                            {
                                gridTotal += data.Get(x - 1, y + 1); //Down/Left from X
                                gridCount++;
                            }

                            if (x < (data.Width - 1)) //Not right column.
                            {
                                gridTotal += data.Get(x + 1, y + 1); //Down/Right from X
                                gridCount++;
                            }

                            gridTotal += data.Get(x, y + 1); //Down from X
                            gridCount++;
                        }

                        if (x > 0) //Not left column.
                        {
                            gridTotal += data.Get(x - 1, y); //Left of X
                            gridCount++;
                        }

                        if (x < (data.Width - 1)) //Not right column.
                        {
                            gridTotal += data.Get(x + 1, y); //Right of X
                            gridCount++;
                        }
                        temp.Set(x, y, (byte)(gridTotal / gridCount));
                    }
                }
            }
            return temp;
        }

        public static Array2<byte> WeightedBlur(this Array2<byte> data, int passes = 1, int startX = 0, int startY = 0)
        {
            Array2<byte> temp = new Array2<byte>(data.Width, data.Height);
            for (int p = 0; p < passes; p++)
            {
                for (int y = startY; y < data.Height; y++)
                {
                    for (int x = startX; x < data.Width; x++)
                    {
                        double gridTotal = data.Get(x, y) * BlurWeights.High; //X (Self)
                        double gridCount = BlurWeights.High;

                        if (y > 0) //Not first row.
                        {
                            if (x > 0) //Not left column.
                            {
                                gridTotal += data.Get(x - 1, y - 1) * BlurWeights.Low; //Up/Left from X
                                gridCount += BlurWeights.Low;
                            }

                            if (x < (data.Width - 1)) //Not right column.
                            {
                                gridTotal += data.Get(x + 1, y - 1) * BlurWeights.Low; //Up/Right from X
                                gridCount += BlurWeights.Low;
                            }

                            gridTotal += data.Get(x, y - 1) * BlurWeights.Medium; //Up from X
                            gridCount += BlurWeights.Medium;
                        }

                        if (y < (data.Height - 1)) //Not last row.
                        {
                            if (x > 0) //Not left column.
                            {
                                gridTotal += data.Get(x - 1, y + 1) * BlurWeights.Low; //Down/Left from X
                                gridCount += BlurWeights.Low;
                            }

                            if (x < (data.Width - 1)) //Not right column.
                            {
                                gridTotal += data.Get(x + 1, y + 1) * BlurWeights.Low; //Down/Right from X
                                gridCount += BlurWeights.Low;
                            }

                            gridTotal += data.Get(x, y + 1) * BlurWeights.Medium; //Down from X
                            gridCount += BlurWeights.Medium;
                        }

                        if (x > 0) //Not left column.
                        {
                            gridTotal += data.Get(x - 1, y) * BlurWeights.Medium; //Left of X
                            gridCount += BlurWeights.Medium;
                        }

                        if (x < (data.Width - 1)) //Not right column.
                        {
                            gridTotal += data.Get(x + 1, y) * BlurWeights.Medium; //Right of X
                            gridCount += BlurWeights.Medium;
                        }
                        temp.Set(x, y, (byte)(gridTotal / gridCount));
                    }
                }
            }
            return temp;
        }

        public static Array2<byte> LayeredBlur(this Array2<byte> data)
        {
            Array2<byte> l1 = new Array2<byte>(data.Width, data.Height);
            Array2<byte> l2 = new Array2<byte>(data.Width, data.Height);
            Array2<byte> l3 = new Array2<byte>(data.Width, data.Height);
            l1 = data.WeightedBlur();
            l2 = l1.WeightedBlur();
            l3 = l2.WeightedBlur();
            Array2<byte> result = new Array2<byte>(data.Width, data.Height);
            for (int i = 0; i < data.Count; i++)
            {
                result.Set(i, (byte)Math.Max(0, Math.Min(255, ((data.Get(i) * LayeredBlurWeights.Base) + (l1.Get(i) * LayeredBlurWeights.Low) + (l2.Get(i) * LayeredBlurWeights.Medium) + (l3.Get(i) * LayeredBlurWeights.High)))));
            }
            return result;
        }

        public static Array2<float> CheapBlur(this Array2<float> data, int passes = 1)
        {
            Array2<float> temp = new Array2<float>(data.Width, data.Height);
            for (int p = 0; p < passes; p++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    for (int x = 0; x < data.Width; x++)
                    {
                        double gridTotal = data.Get(x, y); //X (Self)
                        double gridCount = 1;

                        if (y > 0) //Not first row.
                        {
                            if (x > 0) //Not left column.
                            {
                                gridTotal += data.Get(x - 1, y - 1); //Up/Left from X
                                gridCount++;
                            }

                            if (x < (data.Width - 1)) //Not right column.
                            {
                                gridTotal += data.Get(x + 1, y - 1); //Up/Right from X
                                gridCount++;
                            }

                            gridTotal += data.Get(x, y - 1); //Up from X
                            gridCount++;
                        }

                        if (y < (data.Height - 1)) //Not last row.
                        {
                            if (x > 0) //Not left column.
                            {
                                gridTotal += data.Get(x - 1, y + 1); //Down/Left from X
                                gridCount++;
                            }

                            if (x < (data.Width - 1)) //Not right column.
                            {
                                gridTotal += data.Get(x + 1, y + 1); //Down/Right from X
                                gridCount++;
                            }

                            gridTotal += data.Get(x, y + 1); //Down from X
                            gridCount++;
                        }

                        if (x > 0) //Not left column.
                        {
                            gridTotal += data.Get(x - 1, y); //Left of X
                            gridCount++;
                        }

                        if (x < (data.Width - 1)) //Not right column.
                        {
                            gridTotal += data.Get(x + 1, y); //Right of X
                            gridCount++;
                        }
                        temp.Set(x, y, (float)(gridTotal / gridCount));
                    }
                }
            }
            return temp;
        }

        public static Array2<float> WeightedBlur(this Array2<float> data, int passes = 1, int startX = 0, int startY = 0)
        {
            Array2<float> temp = new Array2<float>(data.Width, data.Height);
            for (int p = 0; p < passes; p++)
            {
                for (int y = startY; y < data.Height; y++)
                {
                    for (int x = startX; x < data.Width; x++)
                    {
                        double gridTotal = data.Get(x, y) * BlurWeights.High; //X (Self)
                        double gridCount = BlurWeights.High;

                        if (y > 0) //Not first row.
                        {
                            if (x > 0) //Not left column.
                            {
                                gridTotal += data.Get(x - 1, y - 1) * BlurWeights.Low; //Up/Left from X
                                gridCount += BlurWeights.Low;
                            }

                            if (x < (data.Width - 1)) //Not right column.
                            {
                                gridTotal += data.Get(x + 1, y - 1) * BlurWeights.Low; //Up/Right from X
                                gridCount += BlurWeights.Low;
                            }

                            gridTotal += data.Get(x, y - 1) * BlurWeights.Medium; //Up from X
                            gridCount += BlurWeights.Medium;
                        }

                        if (y < (data.Height - 1)) //Not last row.
                        {
                            if (x > 0) //Not left column.
                            {
                                gridTotal += data.Get(x - 1, y + 1) * BlurWeights.Low; //Down/Left from X
                                gridCount += BlurWeights.Low;
                            }

                            if (x < (data.Width - 1)) //Not right column.
                            {
                                gridTotal += data.Get(x + 1, y + 1) * BlurWeights.Low; //Down/Right from X
                                gridCount += BlurWeights.Low;
                            }

                            gridTotal += data.Get(x, y + 1) * BlurWeights.Medium; //Down from X
                            gridCount += BlurWeights.Medium;
                        }

                        if (x > 0) //Not left column.
                        {
                            gridTotal += data.Get(x - 1, y) * BlurWeights.Medium; //Left of X
                            gridCount += BlurWeights.Medium;
                        }

                        if (x < (data.Width - 1)) //Not right column.
                        {
                            gridTotal += data.Get(x + 1, y) * BlurWeights.Medium; //Right of X
                            gridCount += BlurWeights.Medium;
                        }
                        temp.Set(x, y, (float)(gridTotal / gridCount));
                    }
                }
            }
            return temp;
        }

        public static Array2<float> LayeredBlur(this Array2<float> data)
        {
            Array2<float> l1 = new Array2<float>(data.Width, data.Height);
            Array2<float> l2 = new Array2<float>(data.Width, data.Height);
            Array2<float> l3 = new Array2<float>(data.Width, data.Height);
            l1 = data.WeightedBlur();
            l2 = l1.WeightedBlur();
            l3 = l2.WeightedBlur();
            Array2<float> result = new Array2<float>(data.Width, data.Height);
            for (int i = 0; i < data.Count; i++)
            {
                result.Set(i, (float)Math.Max(0, Math.Min(255, ((data.Get(i) * LayeredBlurWeights.Base) + (l1.Get(i) * LayeredBlurWeights.Low) + (l2.Get(i) * LayeredBlurWeights.Medium) + (l3.Get(i) * LayeredBlurWeights.High)))));
            }
            return result;
        }
    }
}
