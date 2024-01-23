namespace FEZRepacker.Core.Definitions.Json
{
    public interface JsonModel<T>
    {
        public void SerializeFrom(T data);
        public T Deserialize();
    }
}
