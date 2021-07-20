using System;
using System.Collections.Generic;

namespace FlatPhysics
{
    public sealed class FlatWorld
    {
        public static readonly float MinBodySize = 0.01f * 0.01f;
        public static readonly float MaxBodySize = 64f * 64f;

        public static readonly float MinDensity = 0.5f;     // g/cm^3
        public static readonly float MaxDensity = 21.4f;

        private FlatVector gravity;
        private List<FlatBody> bodyList;

        public int BodyCount
        {
            get { return this.bodyList.Count; }
        }

        public FlatWorld()
        {
            this.gravity = new FlatVector(0f, 9.81f);
            this.bodyList = new List<FlatBody>();
        }

        public void AddBody(FlatBody body)
        {
            this.bodyList.Add(body);
        }

        public bool RemoveBody(FlatBody body)
        {
            return this.bodyList.Remove(body);
        }

        public bool GetBody(int index, out FlatBody body)
        {
            body = null;

            if(index < 0 || index >= this.bodyList.Count)
            {
                return false;
            }

            body = this.bodyList[index];
            return true;
        }

        public void Step(float time)
        {
            // Movement step
            for (int i = 0; i < this.bodyList.Count; i++)
            {
                this.bodyList[i].Step(time);
            }

            // collision step
            for (int i = 0; i < this.bodyList.Count - 1; i++)
            {
                FlatBody bodyA = this.bodyList[i];

                for (int j = i + 1; j < this.bodyList.Count; j++)
                {
                    FlatBody bodyB = this.bodyList[j];

                    if(this.Collide(bodyA, bodyB, out FlatVector normal, out float depth))
                    {
                        bodyA.Move(-normal * depth / 2f);
                        bodyB.Move(normal * depth / 2f);
                    }
                }
            }
        }

        public bool Collide(FlatBody bodyA, FlatBody bodyB, out FlatVector normal, out float depth)
        {
            normal = FlatVector.Zero;
            depth = 0f;

            ShapeType shapeTypeA = bodyA.ShapeType;
            ShapeType shapeTypeB = bodyB.ShapeType;

            if(shapeTypeA is ShapeType.Box)
            {
                if (shapeTypeB is ShapeType.Box)
                {
                    return Collisions.IntersectPolygons(bodyA.GetTransformedVertices(), bodyB.GetTransformedVertices(),
                        out normal, out depth);
                }
                else if (shapeTypeB is ShapeType.Circle)
                {
                    bool result = Collisions.IntersectCirclePolygon(bodyB.Position, bodyB.Radius, bodyA.GetTransformedVertices(),
                        out normal, out depth);

                    normal = -normal;
                    return result;
                }
            }
            else if(shapeTypeA is ShapeType.Circle)
            {
                if (shapeTypeB is ShapeType.Box)
                {
                    return Collisions.IntersectCirclePolygon(bodyA.Position, bodyA.Radius, bodyB.GetTransformedVertices(),
                        out normal, out depth);
                }
                else if (shapeTypeB is ShapeType.Circle)
                {
                    return Collisions.IntersectCircles(bodyA.Position, bodyA.Radius, bodyB.Position, bodyB.Radius,
                        out normal, out depth);
                }
            }

            return false;
        }
    }
}
