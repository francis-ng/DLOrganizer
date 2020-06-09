namespace DLOrganizer.Model
{
    public class SanitizeType
    {
        public SanitizeType() { }

        public SanitizeType(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public string Name
        {
            get; set;
        }

        public int Value
        {
            get; set;
        }
    }
}
