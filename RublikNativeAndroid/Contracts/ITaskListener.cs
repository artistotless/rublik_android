
namespace RublikNativeAndroid.Contracts
{

    public interface ITaskListener<T, E>
    {
        void OnPrepare();
        void OnSuccess(T result);
        void OnError(E error);
    }
}
