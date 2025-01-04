namespace FEZRepacker.Interface.Actions
{
    internal class UnpackRawAction : UnpackAction
    {
        protected override UnpackingMode Mode => UnpackingMode.Raw;
        public override string Name => "--unpack-raw";
        public override string[] Aliases => new string[] {};
        public override string Description =>
            "Unpacks entire .PAK package into specified directory (creates one " +
            "if doesn't exist) leaving XNB assets in their original form.";
    }
}
