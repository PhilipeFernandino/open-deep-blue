using System.Collections;
using UnityEngine;

namespace Core.ProcGen
{
    public interface IMapCreator
    {
        public float[,] CreateMap(int dimensions, System.Random random);
    }
}