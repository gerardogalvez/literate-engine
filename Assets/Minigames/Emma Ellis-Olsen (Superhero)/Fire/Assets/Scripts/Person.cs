using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MoodyBlues.Fire
{
    public class Person : MonoBehaviour
    {
        private readonly float landingPositionY = 1.0f;
        private readonly float EPSILON = 0.001f;

        private Queue<Vector2> personPath;
        private Transform startingPosition;
        private Transform endingPosition;
        private GameObject owner;
        private float tickSpeed;
        private float[] landingPositionsX;
        private List<int> landingPositionIndices;

        private float[] GetQuadraticCoefficients(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            // y = ax^2 + bx + c
            float[] coefficients = new float[3];
            float x0 = p0.x; float y0 = p0.y;
            float x1 = p1.x; float y1 = p1.y;
            float x2 = p2.x; float y2 = p2.y;

            float a = (y1 - y2 - (y0 - y1) * (x1 - x2) / (x0 - x1)) / (Mathf.Pow(x1, 2) - Mathf.Pow(x2, 2) - (x0 + x1) * (x1 - x2));
            float b = (y0 - y1 - a * (Mathf.Pow(x0, 2) - Mathf.Pow(x1, 2))) / (x0 - x1);
            float c = y0 - a * Mathf.Pow(x0, 2) - b * x0;

            coefficients[0] = a; coefficients[1] = b; coefficients[2] = c;

            return coefficients;
        }

        private float EvaluateParabola(float[] coefficients, float x)
        {
            return coefficients[0] * Mathf.Pow(x, 2) + coefficients[1] * x + coefficients[2];
        }

        private List<Vector2> GetParabolaPoints(int numPoints, float[] coefficients, float xStart, float xEnd)
        {
            float delta = (xEnd - xStart) / numPoints;
            List<Vector2> points = new List<Vector2>
        {
            new Vector2(xStart, this.EvaluateParabola(coefficients, xStart))
        };

            for (int i = 1; i <= numPoints; ++i)
            {
                float x = xStart + i * delta;
                points.Add(new Vector2(x, this.EvaluateParabola(coefficients, x)));
            }

            points.Add(new Vector2(xEnd, this.EvaluateParabola(coefficients, xEnd)));

            return points;
        }

        private List<Vector2> GetPath()
        {
            List<Vector2> path = new List<Vector2>();
            Vector2 p0, p1, p2;
            RangeInt heightRange = GameManager.instance.BounceHeightRange;
            float[] coefficients = new float[3];

            // Starting position to first landing position
            p1 = startingPosition.position;
            p2 = new Vector2(this.landingPositionsX[this.landingPositionIndices[0]], this.landingPositionY);
            p0 = new Vector2(2 * p1.x - p2.x, this.landingPositionY);
            coefficients = this.GetQuadraticCoefficients(p0, p1, p2);
            path.AddRange(this.GetParabolaPoints(GameManager.instance.TicksPerBounce / 2, coefficients, p1.x, p2.x));

            // Landing position to next landing position
            for (int i = 1; i < this.landingPositionIndices.Count; ++i)
            {
                p0 = p2;
                p2 = new Vector2(this.landingPositionsX[this.landingPositionIndices[i]], this.landingPositionY);
                p1 = new Vector2((p0.x + p2.x) / 2.0f, Random.Range(GameManager.instance.BounceHeightRange.start, GameManager.instance.BounceHeightRange.end));
                coefficients = this.GetQuadraticCoefficients(p0, p1, p2);
                path.AddRange(this.GetParabolaPoints(GameManager.instance.TicksPerBounce, coefficients, p0.x, p2.x));
            }

            // Last landing position to endingPosition
            p0 = p2;
            p2 = this.endingPosition.position;
            p1 = new Vector2((p0.x + p2.x) / 2.0f, Random.Range(GameManager.instance.BounceHeightRange.start, GameManager.instance.BounceHeightRange.end));
            coefficients = this.GetQuadraticCoefficients(p0, p1, p2);
            path.AddRange(this.GetParabolaPoints(GameManager.instance.TicksPerBounce, coefficients, p0.x, p2.x));

            return path;
        }

        private void FollowPath()
        {
            if (this.personPath.Count >= 2)
            {
                this.transform.position = this.personPath.Dequeue();
                if (this.personPath.Count > 1)
                {
                    this.transform.GetChild(0).position = this.personPath.Peek();
                }
                else
                {
                    this.transform.GetChild(0).gameObject.SetActive(false);
                }

                if (ShouldCheckBounce())
                {
                    if (!ShouldBounce())
                    {
                        Destroy(this.gameObject);
                    }
                }
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private bool ShouldCheckBounce()
        {
            return Mathf.Abs(this.transform.position.y - this.landingPositionY) < EPSILON;
        }

        private bool ShouldBounce()
        {
            return Mathf.Abs(this.owner.transform.position.x - this.transform.position.x) < EPSILON;
        }

        private void GetBouncePositionIndices(int amountBouncePoints)
        {
            this.landingPositionIndices = new List<int>();
            List<int> indices = Enumerable.Range(0, this.landingPositionsX.Length).ToList();
            for (int i = 0; i < amountBouncePoints; ++i)
            {
                int randomIndex = Random.Range(0, indices.Count);
                this.landingPositionIndices.Add(indices[randomIndex]);
                indices.RemoveAt(randomIndex);
            }

            this.landingPositionIndices.Sort();
        }

        public void Initialize(PersonParameters args)
        {
            this.owner = args.owner;
            this.tickSpeed = args.tickSpeed;
            this.startingPosition = args.startingPoint;
            this.endingPosition = args.endingPoint;

            int maxLandingPosition = GameManager.instance.MaxPlayerHorizontalMovement;
            this.landingPositionsX = new float[2 * maxLandingPosition + 1];
            for (int i = -1 * maxLandingPosition, j = 0; i <= maxLandingPosition; ++i, ++j)
            {
                this.landingPositionsX[j] = i;
            }

            this.GetBouncePositionIndices(Random.Range(2, 4));
        }

        public void StartPath()
        {
            Debug.Assert(this.owner != null);
            Debug.Assert(this.tickSpeed > 0.0f);
            Debug.Assert(this.startingPosition != null);
            Debug.Assert(this.endingPosition != null);
            Debug.Assert(this.landingPositionsX != null);
            Debug.Assert(this.landingPositionIndices != null);

            this.personPath = new Queue<Vector2>(this.GetPath());
            this.transform.position = personPath.Dequeue();

            InvokeRepeating("FollowPath", 0.0f, this.tickSpeed);
        }

    }
}
