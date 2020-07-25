using frame8.ScrollRectItemsAdapter.Classic.Util;
using UnityEngine;
using UnityEngine.UI;

namespace frame8.ScrollRectItemsAdapter.Classic.Examples.Common
{
    public class ReadyListModel<TClientModel> : CAbstractViewsHolder where TClientModel : SimpleClientModel
	{
		public LayoutElement layoutElement;
		public Image bg;

		public Text nameText;
		public Text readyText;
		public GameObject dots;


		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			layoutElement = root.GetComponent<LayoutElement>();
			bg = root.GetComponent<Image>();

			var mainPanel = root.GetChild(0);
			nameText = mainPanel.Find("NameText").GetComponent<Text>();
			readyText = mainPanel.Find("ReadyText").GetComponent<Text>();
			dots = mainPanel.Find("Dots").gameObject;
		}

		public virtual void UpdateViews(TClientModel dataModel)
		{
			bg.color = dataModel.player.isReady ? new Color32(215, 215, 215, 255) : new Color32(145, 145, 145, 255);
			nameText.text = dataModel.player.playerName;
			readyText.text = dataModel.player.isReady ? "READY" : "";
			dots.gameObject.SetActive(!dataModel.player.isReady);
		}


	}

	public class ReadyListModel_SimpleClientViewsHolder : ReadyListModel<SimpleClientModel>
	{

	}


	public class ReadyList_SimpleExpandableClientViewsHolder : ReadyListModel<ExpandableSimpleClientModel>
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