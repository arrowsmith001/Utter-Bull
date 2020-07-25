using UnityEngine;
using UnityEngine.UI;
using frame8.ScrollRectItemsAdapter.Classic.Util;

namespace frame8.ScrollRectItemsAdapter.Classic.Examples.Common
{
	public class WaitingRoomModel<TClientModel> : CAbstractViewsHolder where TClientModel : SimpleClientModel
	{
		public LayoutElement layoutElement;
		public Text nameText;
		public Text hostText;
		public Text scoreText;


		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			layoutElement = root.GetComponent<LayoutElement>();

			var mainPanel = root.GetChild(0);
			nameText = mainPanel.Find("NameText").GetComponent<Text>();
			hostText = mainPanel.Find("HostText").GetComponent<Text>();
			scoreText = mainPanel.Find("ScoreText").GetComponent<Text>();
		}

		public virtual void UpdateViews(TClientModel dataModel)
		{
            nameText.text = dataModel.player.playerName;
            hostText.text = dataModel.player.isHosting ? "HOST" : "";
            scoreText.text = dataModel.player.points.ToString();

        }

		
	}

	public class WaitingRoomModel_SimpleClientViewsHolder : WaitingRoomModel<SimpleClientModel>
	{

	}


	public class WaitingRoom_SimpleExpandableClientViewsHolder : WaitingRoomModel<ExpandableSimpleClientModel>
	{
		public CExpandCollapseOnClick expandCollapseComponent;


		public override void CollectViews()
		{
			base.CollectViews();

			expandCollapseComponent = root.GetComponent<CExpandCollapseOnClick>();
		}

		public override void UpdateViews(ExpandableSimpleClientModel dataModel)
		{
			base.UpdateViews(dataModel);

			if (expandCollapseComponent)
			{
				expandCollapseComponent.expanded = dataModel.expanded;
				expandCollapseComponent.nonExpandedSize = dataModel.nonExpandedSize;
			}
		}
	}
}
