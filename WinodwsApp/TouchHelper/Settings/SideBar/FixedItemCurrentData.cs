using System.Collections.Generic;

namespace TouchHelper.SideBar;
internal class FixedItem
{
    public List<FixedItem> RootFixedItem { get; set; } = [];
}