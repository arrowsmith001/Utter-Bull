

namespace frame8.ScrollRectItemsAdapter.Classic.Examples.Common
{
	public class SimpleClientModel
	{
		public Player player;
	}


	public class ExpandableSimpleClientModel : SimpleClientModel
	{
		// View size related
		public bool expanded = true;
		public float nonExpandedSize;
		public float expandedSize;
	}
}
