using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CWLEngine.GameImpl.Util
{
    public class MathTools
    {
        public static float Distance(Vector3 vec1, Vector3 vec2)
        {
            return (float) Math.Sqrt((vec1.x - vec2.x) * (vec1.x - vec2.x)
                + (vec1.y - vec2.y) * (vec1.y - vec2.y)
                + (vec1.z - vec2.z) * (vec1.z - vec2.z));
        }

    }
}
