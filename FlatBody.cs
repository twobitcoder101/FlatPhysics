using System;

namespace FlatPhysics
{
    public enum ShapeType
    {
        Circle = 0, 
        Box = 1
    }

    public sealed class FlatBody
    {
        private FlatVector position;
        private FlatVector linearVelocity;
        private float rotation;
        private float rotationalVelocity;

        public readonly float Density;
        public readonly float Mass;
        public readonly float Restitution;
        public readonly float Area;

        public readonly bool IsStatic;

        public readonly float Radius;
        public readonly float Width;
        public readonly float Height;

        public readonly ShapeType ShapeType;

        public FlatVector Position
        {
            get { return this.position; }
        }

        private FlatBody(FlatVector position, float density, float mass, float restitution, float area, 
            bool isStatic, float radius, float width, float height, ShapeType shapeType)
        {
            this.position = position;
            this.linearVelocity = FlatVector.Zero;
            this.rotation = 0f;
            this.rotationalVelocity = 0f;

            this.Density = density;
            this.Mass = mass;
            this.Restitution = restitution;
            this.Area = area;

            this.IsStatic = isStatic;
            this.Radius = radius;
            this.Width = width;
            this.Height = height;
            this.ShapeType = shapeType;
        }

        public void Move(FlatVector amount)
        {
            this.position += amount;
        }

        public void MoveTo(FlatVector position)
        {
            this.position = position;
        }

        public static bool CreateCircleBody(float radius, FlatVector position, float density, bool isStatic, float restitution, out FlatBody body, out string errorMessage)
        {
            body = null;
            errorMessage = string.Empty;

            float area = radius * radius * MathF.PI;

            if(area < FlatWorld.MinBodySize)
            {
                errorMessage = $"Circle radius is too small. Min circle area is {FlatWorld.MinBodySize}.";
                return false;
            }

            if(area > FlatWorld.MaxBodySize)
            {
                errorMessage = $"Circle radius is too large. Max circle area is {FlatWorld.MaxBodySize}.";
                return false;
            }

            if (density < FlatWorld.MinDensity)
            {
                errorMessage = $"Density is too small. Min density is {FlatWorld.MinDensity}";
                return false;
            }

            if (density > FlatWorld.MaxDensity)
            {
                errorMessage = $"Density is too large. Max density is {FlatWorld.MaxDensity}";
                return false;
            }

            restitution = FlatMath.Clamp(restitution, 0f, 1f);

            // mass = area * depth * density
            float mass = area * density;

            body = new FlatBody(position, density, mass, restitution, area, isStatic, radius, 0f, 0f, ShapeType.Circle);
            return true;
        }

        public static bool CreateBoxBody(float width, float height, FlatVector position, float density, bool isStatic, float restitution, out FlatBody body, out string errorMessage)
        {
            body = null;
            errorMessage = string.Empty;

            float area = width * height;

            if (area < FlatWorld.MinBodySize)
            {
                errorMessage = $"Area is too small. Min area is {FlatWorld.MinBodySize}.";
                return false;
            }

            if (area > FlatWorld.MaxBodySize)
            {
                errorMessage = $"Area is too large. Max area is {FlatWorld.MaxBodySize}.";
                return false;
            }

            if (density < FlatWorld.MinDensity)
            {
                errorMessage = $"Density is too small. Min density is {FlatWorld.MinDensity}";
                return false;
            }

            if (density > FlatWorld.MaxDensity)
            {
                errorMessage = $"Density is too large. Max density is {FlatWorld.MaxDensity}";
                return false;
            }

            restitution = FlatMath.Clamp(restitution, 0f, 1f);

            // mass = area * depth * density
            float mass = area * density;

            body = new FlatBody(position, density, mass, restitution, area, isStatic, 0f, width, height, ShapeType.Box);
            return true;
        }
    }
}
