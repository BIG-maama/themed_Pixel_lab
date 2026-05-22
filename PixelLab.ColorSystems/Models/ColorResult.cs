namespace PixelLab.ColorSystems.Models
{
    public class ColorResult
    {
        public string SystemName { get; }
        public string Channel1Name { get; }
        public string Channel2Name { get; }
        public string Channel3Name { get; }
        public double Channel1 { get; }
        public double Channel2 { get; }
        public double Channel3 { get; }

        public ColorResult(
            string systemName,
            string ch1Name, double ch1,
            string ch2Name, double ch2,
            string ch3Name, double ch3)
        {
            SystemName = systemName;
            Channel1Name = ch1Name; Channel1 = ch1;
            Channel2Name = ch2Name; Channel2 = ch2;
            Channel3Name = ch3Name; Channel3 = ch3;
        }

        public override string ToString()
        {
            return SystemName + " -> " +
                   Channel1Name + ": " + Channel1.ToString("F2") + ", " +
                   Channel2Name + ": " + Channel2.ToString("F2") + ", " +
                   Channel3Name + ": " + Channel3.ToString("F2");
        }
    }
}