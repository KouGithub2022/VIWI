using System.Collections.Generic;
using VIWI.UI.Pages;

namespace VIWI.UI
{
    public static class DashboardRegistry
    {
        private static readonly List<IDashboardPage> pages = new();

        public static IReadOnlyList<IDashboardPage> Pages => pages;

        public static void Register(IDashboardPage page)
        {
            if (!pages.Contains(page))
                pages.Add(page);
        }
    }
}