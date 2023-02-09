namespace FEZRepacker.Converter.Definitions.FezEngine.Structure.Input
{
    [XnbType("FezEngine.Structure.Input.CodeInput")]
    [Flags]
    internal enum CodeInput
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,
        SpinLeft = 16,
        SpinRight = 32,
        Jump = 64,
    }
}