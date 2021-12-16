using System;
namespace RublikNativeAndroid.Services
{
    public class UnsubscriberService : IDisposable
    {
        private IDisposable _service;
        private IDisposable _liveData;

        public UnsubscriberService(IDisposable service, IDisposable liveData)
        {
            _service = service;
            _liveData = liveData;
        }

        public void Dispose()
        {
            _liveData.Dispose();
            _service.Dispose();
        }
    }
}
