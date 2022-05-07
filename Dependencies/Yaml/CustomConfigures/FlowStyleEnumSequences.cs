using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace FEZRepacker.Dependencies.Yaml.CustomConfigures
{
    class FlowStyleEnumSequences : ChainedEventEmitter
    {
        public FlowStyleEnumSequences(IEventEmitter nextEmitter)
            : base(nextEmitter) { }

        public override void Emit(SequenceStartEventInfo eventInfo, IEmitter emitter)
        {
            Type sourceType = eventInfo.Source.Type;

            bool isEnumList = false;
            if (sourceType.IsArray && sourceType.GetElementType()!.IsEnum) isEnumList = true;
            if (sourceType.GenericTypeArguments.Length == 1 && sourceType.GenericTypeArguments[0].IsEnum) isEnumList = true;

            if (isEnumList)
            {
                eventInfo = new SequenceStartEventInfo(eventInfo.Source)
                {
                    Style = SequenceStyle.Flow
                };
            }

            nextEmitter.Emit(eventInfo, emitter);
        }
    }
}
