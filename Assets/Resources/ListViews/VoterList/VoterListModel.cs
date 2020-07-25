


using UnityEngine;
using UnityEngine.UI;
using frame8.ScrollRectItemsAdapter.Classic.Util;
using System;

namespace frame8.ScrollRectItemsAdapter.Classic.Examples.Common
{
	public class VoterListModel<TClientModel> : CAbstractViewsHolder where TClientModel : SimpleClientModel
	{
		public LayoutElement layoutElement;
		public Text nameText;
		public Text timeText;
		public Image image_fastestTime;

		public Image sabo;

		public Sprite img_default;
		public Sprite img_sabo;

		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			layoutElement = root.GetComponent<LayoutElement>();

			var mainPanel = root.GetChild(0);
			nameText = mainPanel.Find("NameText").GetComponent<Text>();
			timeText = mainPanel.Find("TimeText").GetComponent<Text>();
			image_fastestTime = mainPanel.Find("Image_FastestTime").GetComponent<Image>();
			sabo = mainPanel.Find("Image_Sabo").GetComponent<Image>();

			SpriteHolder sh = layoutElement.gameObject.transform.GetComponent<SpriteHolder>();

			img_default = sh.list[0];
			img_sabo = sh.list[1];
		}

		public virtual void UpdateViews(TClientModel dataModel)
		{
			try
			{
				nameText.text = dataModel.player.playerName;
				timeText.text = " in " + dataModel.player.voteTime.ToString() + "s";
				image_fastestTime.gameObject.SetActive(dataModel.player.isAmongFastest ?? false);

				sabo.gameObject.SetActive(dataModel.player.fakeVoter ?? false);

				layoutElement.transform.GetComponent<Image>().sprite =
					(dataModel.player.fakeVoter ?? false) ? img_sabo : img_default;

			}catch(Exception e) { Debug.Log("UPDATE VIEWS PROBLEM: " + e.Message); }

		}


	}

	public class VoterListModel_SimpleClientViewsHolder : VoterListModel<SimpleClientModel>
	{

	}


	public class VoterList_SimpleExpandableClientViewsHolder : VoterListModel<ExpandableSimpleClientModel>
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
