using System;
namespace RublikNativeAndroid.Services
{
    public class UnsubscriberService : IDisposable
    {
        private IDisposable _liveData;

        public UnsubscriberService(IDisposable liveData)
        {
            _liveData = liveData;
        }

        public void Dispose()
        {
            _liveData.Dispose();
        }
    }
}
