using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace Machine.VSTestAdapter.Navigation
{
    public class NavigationMethod
    {
        private readonly List<NavigationSequencePoint> sequencePoints = new List<NavigationSequencePoint>();

        public NavigationMethod(string type, string method, MethodDefinitionHandle handle)
        {
            Type = type;
            Method = method;
            Handle = handle;
        }

        public string Type { get; }

        public string Method { get; }

        public MethodDefinitionHandle Handle { get; }

        public List<Instruction> Instructions { get; } = new List<Instruction>();

        public void AddSequencePoints(NavigationSequencePoint[] points)
        {
            sequencePoints.AddRange(points);
        }

        public NavigationSequencePoint GetSequencePoint(Instruction instruction)
        {
            return sequencePoints.FirstOrDefault(x => x.Offset == instruction.Offset);
        }
    }
}
