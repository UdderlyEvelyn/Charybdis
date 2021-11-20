using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public static class RandomExtensions
    {
        //Adapted from https://bitbucket.org/Superbest/superbest-random/src/f067e1dc014c31be62c5280ee16544381e04e303/Superbest%20random/RandomExtensions.cs?at=master&fileviewer=file-view-default on 5/26/17
        public static double NextGaussian(this Random r, double desiredMean = 0, double desiredStandardDeviation = 1)
        {            
            return desiredMean + desiredStandardDeviation * Math.Sqrt(-2.0 * Math.Log(r.NextDouble())) * Math.Sin(2.0 * Math.PI * r.NextDouble());
        }

        public static bool Chance(this Random r, double chance)
        {
            if (chance > 1)
                throw new ArgumentException("You can't have a chance greater than 1, which represents 100%.");
            else if (chance == 1)
                return true;
            else if (chance == 0)
                return false;
            else if (chance < 0)
                throw new ArgumentException("You can't have a negative chance.");
            else
                return r.NextDouble() < chance;
        }
    }
}
