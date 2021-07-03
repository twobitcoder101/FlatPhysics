﻿using System;

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

        private readonly FlatVector[] vertices;
        public readonly int[] Triangles;
        private FlatVector[] transformedVertices;

        private bool transformUpdateRequired;

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

            if(this.ShapeType is ShapeType.Box)
            {
                this.vertices = FlatBody.CreateBoxVertices(this.Width, this.Height);
                this.Triangles = FlatBody.CreateBoxTriangles();
                this.transformedVertices = new FlatVector[this.vertices.Length];
            }
            else
            {
                this.vertices = null;
                Triangles = null;
                this.transformedVertices = null;
            }

            this.transformUpdateRequired = true;
        }

        private static FlatVector[] CreateBoxVertices(float width, float height)
        {
            float left = -width / 2f;
            float right = left + width;
            float bottom = -height / 2f;
            float top = bottom + height;

            FlatVector[] vertices = new FlatVector[4];
            vertices[0] = new FlatVector(left, top);
            vertices[1] = new FlatVector(right, top);
            vertices[2] = new FlatVector(right, bottom);
            vertices[3] = new FlatVector(left, bottom);

            return vertices;
        }

        private static int[] CreateBoxTriangles()
        {
            int[] triangles = new int[6];
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 0;
            triangles[4] = 2;
            triangles[5] = 3;
            return triangles;
        }

        public FlatVector[] GetTransformedVertices()
        {
            if(this.transformUpdateRequired)
            {
                FlatTransform transform = new FlatTransform(this.position, this.rotation);

                for(int i = 0; i < this.vertices.Length; i++)
                {
                    FlatVector v = this.vertices[i];
                    this.transformedVertices[i] = FlatVector.Transform(v, transform);
                }
            }

            return this.transformedVertices;
        }

        public void Move(FlatVector amount)
        {
            this.position += amount;
            this.transformUpdateRequired = true;
        }

        public void MoveTo(FlatVector position)
        {
            this.position = position;
            this.transformUpdateRequired = true;
        }

        public void Rotate(float amount)
        {
            this.rotation += amount;
            this.transformUpdateRequired = true;
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