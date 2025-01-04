namespace FEZRepacker.Interface.Actions
{
    internal class UnpackConvertAction : UnpackAction
    {
        protected override UnpackingMode Mode => UnpackingMode.Converted;
        public override string Name => "--unpack";
        public override string[] Aliases => new[] { "-u" };
        public override string Description =>
            "Unpacks entire .PAK package into specified directory (creates one if doesn't exist) " +
            "and attempts to convert XNB assets into their corresponding format in the process.";
    }
}
