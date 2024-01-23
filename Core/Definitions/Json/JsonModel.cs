namespace FEZRepacker.Core.Definitions.Json
{
    internal interface JsonModel<T>
    {
        public void SerializeFrom(T data);
        public T Deserialize();
    }
}
