using System;

namespace FlatPhysics
{
    public static class Collisions
    {
        public static bool IntersectCircles(
            FlatVector centerA, float radiusA, 
            FlatVector centerB, float radiusB, 
            out FlatVector normal, out float depth)
        {
            normal = FlatVector.Zero;
            depth = 0f;

            float distance = FlatMath.Distance(centerA, centerB);
            float radii = radiusA + radiusB;

            if(distance >= radii)
            {
                return false;
            }

            normal = FlatMath.Normalize(centerB - centerA);
            depth = radii - distance;

            return true;
        }
    }
}
