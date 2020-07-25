using System;
using frame8.ScrollRectItemsAdapter.Classic.Util;
using UnityEngine;
using UnityEngine.UI;

namespace frame8.ScrollRectItemsAdapter.Classic.Examples.Common
{
    public class ResultsListModel<TClientModel> : CAbstractViewsHolder where TClientModel : ExpandableSimpleClientModel
	{
		GP6_REVEALS_BASE gp6;

		// Prefabs
		GameObject childPrefab;

		public LayoutElement layoutElement;
		public IconsLoader icons;

		private Sprite voter_blank;
		private Sprite voter_correct;
		private Sprite voter_wrong;
		private Sprite reader;
		private Sprite reader_bronze;
		private Sprite reader_silver;
		private Sprite reader_gold;
		private Sprite sabo;
		private Sprite sabo_bad;
		private Sprite sabo_silver;
		private Sprite sabo_gold;

		public GameObject rootPanel;
		public GameObject mainPanel;

		public Text nameText;
		public Text scoreText;
        private Text oldScoreText;
        private Text newScoreText;

        /// <inheritdoc/>
        public override void CollectViews()
		{
			base.CollectViews();


			layoutElement = root.GetComponent<LayoutElement>();

            rootPanel = root.gameObject;

			mainPanel = root.Find("MainParent").gameObject;
			nameText = mainPanel.transform.Find("NameText").GetComponent<Text>();
			scoreText = mainPanel.transform.Find("ScorePanel/ScoreText").GetComponent<Text>();

			oldScoreText = mainPanel.transform.Find("ScorePanel/BreakDownPanel/Text_OldPoints").GetComponent<Text>();
			newScoreText = mainPanel.transform.Find("ScorePanel/BreakDownPanel/Text_NewPoints").GetComponent<Text>();

			childPrefab = Resources.Load<GameObject>("ListViews/ResultsList/ChildItem");

			icons = rootPanel.gameObject.GetComponent<IconsLoader>();
			voter_correct = icons.icons[0];
			voter_wrong = icons.icons[1];
			voter_blank = icons.icons[2];
			reader = icons.icons[3];
			reader_bronze = icons.icons[4];
			reader_gold = icons.icons[5];
			reader_silver = icons.icons[6];
			sabo = icons.icons[7];
			sabo_bad = icons.icons[8];
			sabo_silver = icons.icons[9];
			sabo_gold = icons.icons[10];
		}

		Color32 blue_soft = new Color32(121, 210,255,255);
		Color32 red_soft = new Color32(255,178,160, 255);

		public virtual void UpdateViews(TClientModel dataModel)
		{
            // Data objs
			RoundDataInterpretter rdi = dataModel.player.rdi;

			string myName = dataModel.player.playerName;
			int currentScore = rdi.resultsData.newScores[myName];

			int oldScore = rdi.resultsData.oldScores[myName];
			int scoreDiff = currentScore - oldScore;

            // Set header vals
			nameText.text = myName;
			scoreText.text = currentScore.ToString();
			oldScoreText.text = "(" + oldScore + ")";
			newScoreText.text =
				"<color=#" +
				((scoreDiff > 0) ? ColorUtility.ToHtmlStringRGB(blue_soft) : ColorUtility.ToHtmlStringRGB(red_soft))
				+ ">" +
				((scoreDiff > 0) ? "+" : "-") + Math.Abs(scoreDiff)+
				 "</color>";

			// Generate list children
			GameObject layoutChild;
            Text Text_Top;
            Text Text_Ach;
			Image Img;


				for (int i = 0; i < rdi.turnsData.Count; i++)
				{
					// This turn
					TurnData td = rdi.turnsData[i];

					// Results child prefab views
					layoutChild = UnityEngine.Object.Instantiate(childPrefab, rootPanel.transform);
					layoutChild.transform.SetSiblingIndex(i + 1);

					Button button = layoutChild.GetComponent<Button>();
					Debug.Log("button == null: " + (button == null) + ", gp6 == null: " + (gp6 == null));
					button.onClick.AddListener(() => TryJumpToPage(button.gameObject.transform.GetSiblingIndex()));

					Text_Top = layoutChild.transform.Find("Panel_Text/Text_Top").GetComponent<Text>();
					Text_Ach = layoutChild.transform.Find("Panel_Text/Text_Ach").GetComponent<Text>();
					Img = layoutChild.transform.Find("Image").GetComponent<Image>();

					// Set data - top text
					Text_Top.text = "#" + (i + 1) + " " + td.readersName + "'s ";
					Text_Top.text = Text_Top.text
						+ "<color=#"
						+ (td.wasInFact ? ColorUtility.ToHtmlStringRGB(MainScript.true_blue) + ">truth" : ColorUtility.ToHtmlStringRGB(MainScript.bull_red) + ">lie")
						+ "</color>";

					// Set data - ach text
					string scoreString = "";
					if (td.achievementsUnlocked[myName].Count > 0)
					{
						foreach (Achievement ach in td.achievementsUnlocked[myName])
						{
							scoreString = scoreString + ach.simpleColouredString + "\n";
						}
					}
					else
					{
						scoreString = "<color=#ff0000>No points scored.</color>" + "\n";
					}

					scoreString = scoreString.Substring(0, scoreString.Length - 1);

					Text_Ach.text = scoreString;

					// Determine appropriate image TODO
					bool isReader = (myName == td.readersName);
					bool isSabo = (td.saboName != null && (td.saboName == myName));
					bool isVoter = !isReader && !isSabo;

					if (isReader)
					{
						if (td.p_allEarned) Img.sprite = reader_gold;
						else if (td.p_mostEarned) Img.sprite = reader_silver;
						else if (td.p_fiftyfiftyEarned) Img.sprite = reader_bronze;
						else Img.sprite = reader;
					}
					else if (isSabo)
					{
						if (td.s_allEarned) Img.sprite = sabo_gold;
						else if (td.s_mostEarned) Img.sprite = sabo_silver;
						else if (td.s_badSabo) Img.sprite = sabo_bad;
						else Img.sprite = sabo;

					}
					else
					{
						if (td.correctVoters.Contains(myName)) Img.sprite = voter_correct;
						else if (td.incorrectVoters.Contains(myName)) Img.sprite = voter_wrong;
						else if (td.xVoters.Contains(myName)) Img.sprite = voter_blank;
					}

					// Set active to false
					layoutChild.SetActive(false);
				}
			
			//catch(Exception e)
   //         {
			//	Debug.Log("RESULTS VIEWS ERROR: " + e.Message);

			//	// Kill children
			//	for (int i = 0; i < rootPanel.transform.childCount; i++)
			//	{
   //                 UnityEngine.Object.Destroy(rootPanel.transform.GetChild(i).gameObject);
			//	}

   //         }


			// Measurements for expansion/contraction
			dataModel.nonExpandedSize =
				mainPanel.GetComponent<RectTransform>().rect.height
				// + ext.GetComponent<RectTransform>().rect.height
				;

			dataModel.expandedSize =
				rootPanel.GetComponent<RectTransform>().rect.height
				;

			

		}

        private void TryJumpToPage(int i)
        {
            try
			{
				root.parent.GetComponent<GP8_ListContent>().gp6.JumpToPage(i-1);

            }catch(Exception e)
            {

            }
		}
    }

	public class ResultsListModel_SimpleClientViewsHolder : ResultsListModel<ExpandableSimpleClientModel>
	{

	}


	public class ResultsList_SimpleExpandableClientViewsHolder : ResultsListModel<ExpandableSimpleClientModel>
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
				expandCollapseComponent.expandedSize = dataModel.expandedSize;
			}
		}
	}
}