using System.Collections.Generic;
using AndroidX.Fragment.App;

namespace RublikNativeAndroid.Adapters
{

    class AuthenticationPagerAdapter : FragmentPagerAdapter
    {
        private List<Fragment> fragmentList = new List<Fragment>();

        public override int Count => fragmentList.Count;

        public AuthenticationPagerAdapter(FragmentManager fm) : base(fm)
        {

        }

        public override Fragment GetItem(int i)
        {
            return fragmentList[i];
        }


        public void AddFragment(Fragment fragment)
        {
            fragmentList.Add(fragment);
        }
    }
}
