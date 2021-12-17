using System.Threading.Tasks;

namespace RublikNativeAndroid.Contracts
{
    public interface IFragmentViewCreateListener
    {
        Task UpdateUI(AndroidX.Fragment.App.Fragment fragment);
    }
}
