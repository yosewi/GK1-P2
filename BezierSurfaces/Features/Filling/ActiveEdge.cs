namespace BezierSurfaces.Features.Filling
{
    public sealed class ActiveEdge
    {
        public int UpperVertex { get; }
        public int LowerVertex { get; }
        public float X { get; set; }
        public float InverseSlope { get; }

        public ActiveEdge(int upperVertex, int lowerVertex, float x, float inverseSlope)
        {
            UpperVertex = upperVertex;
            LowerVertex = lowerVertex;
            X = x;
            InverseSlope = inverseSlope;
        }
    }
}
