using System.Collections.Generic;
using DefaultNamespace.Managers;

namespace DefaultNamespace.UI.Popup
{
    public class PopUpComparer : IComparer<PopUpBase>
    {
        public int Compare(PopUpBase x, PopUpBase y)
        {
            if (x == null && y == null)
                return 0;

            if (x == null && y != null)
                return 1;

            if (x != null && y == null)
                return -1;

            return x.Priority - y.Priority;
        }
    }
}