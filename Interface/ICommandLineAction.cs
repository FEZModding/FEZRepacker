using System.CommandLine;

namespace FEZRepacker.Interface
{
    internal interface ICommandLineAction
    {
        string Name { get; }
        
        string[] Aliases { get; }
        
        string Description { get; }
        
        Argument[] Arguments { get; }
        
        Option[] Options { get; }
        
        void Execute(ParseResult result);
    }
}
