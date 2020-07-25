using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using frame8.ScrollRectItemsAdapter.Classic.Examples.Common;
using frame8.ScrollRectItemsAdapter.Classic.Util;
using System;
using System.Collections;

namespace frame8.ScrollRectItemsAdapter.Classic.Examples
{
	/// <summary>Class (initially) implemented during this YouTube tutorial: https://youtu.be/aoqq_j-aV8I (which is now too old to relate). It demonstrates a simple use case with items that expand/collapse on click</summary>
	public class VerticalListView_ResultsList : ClassicSRIA<ResultsList_SimpleExpandableClientViewsHolder>, CExpandCollapseOnClick.ISizeChangesHandler
	{
		public RectTransform itemPrefab;
		//public string[] sampleFirstNames;//, sampleLastNames;
		//public string[] sampleLocations;

		public List<Player> playerList = new List<Player>();

		//public DemoUI demoUI;

		public List<ExpandableSimpleClientModel> Data { get; private set; }

		public GP8_RESULTS gp8;

		LayoutElement _PrefabLayoutElement;
		// Used to quickly retrieve the views holder given the gameobject
		Dictionary<RectTransform, ResultsList_SimpleExpandableClientViewsHolder> _MapRootToViewsHolder = new Dictionary<RectTransform, ResultsList_SimpleExpandableClientViewsHolder>();

		Interpolator decel = new DecelerateInterpolator(2);

		// DEMO UI REPLACEMENTS
		bool freezeContentEndEdge_isOn = true; // TODO Change these all to true

		#region ClassicSRIA implementation
		protected override void Awake()
		{
			base.Awake();

			playerList = gp8.list;
			//playerList = new List<Player>();
			//playerList.Add(new Player("P1", true, 10));
			//playerList.Add(new Player("P2", true, 20));
			//playerList.Add(new Player("P3", true, 310));

			Data = new List<ExpandableSimpleClientModel>();
			_PrefabLayoutElement = itemPrefab.GetComponent<LayoutElement>();
		}

		protected override void Start()
		{
			base.Start();

			ChangeModelsAndReset(playerList.Count);
			
			//demoUI.setCountButton.onClick.AddListener(OnItemCountChangeRequested);
			//demoUI.scrollToButton.onClick.AddListener(OnScrollToRequested);
			//demoUI.addOneTailButton.onClick.AddListener(() => OnAddItemRequested(true));
			//demoUI.addOneHeadButton.onClick.AddListener(() => OnAddItemRequested(false));
			//demoUI.removeOneTailButton.onClick.AddListener(() => OnRemoveItemRequested(true));
			//demoUI.removeOneHeadButton.onClick.AddListener(() => OnRemoveItemRequested(false));

			//StartCoroutine(DelayedClick());
		}

		public void RefreshList()
		{
            //Debug.Log("RefreshList: CALLED");

           
			    playerList = gp8.list;

			//playerList = new List<Player>();
			//playerList.Add(new Player("P1", true, 10));
			//playerList.Add(new Player("P2", true, 20));
			//playerList.Add(new Player("P3", true, 310));



			////Debug.Log("PLAYERLIST LENGTH:" + playerList.Count.ToString());

			Data = new List<ExpandableSimpleClientModel>();
			_PrefabLayoutElement = itemPrefab.GetComponent<LayoutElement>();

			ChangeModelsAndReset(playerList.Count);

			Refresh();

		}

		IEnumerator DelayedClick()
		{
			yield return new WaitForSeconds(.4f);

			//if (viewsHolders.Count > 0)
			//	viewsHolders[Mathf.Min(1, Data.Count - 1)].expandCollapseComponent.OnClicked();
		}

		protected override ResultsList_SimpleExpandableClientViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new ResultsList_SimpleExpandableClientViewsHolder();
			instance.Init(itemPrefab, itemIndex);
			instance.expandCollapseComponent.sizeChangesHandler = this;
			_MapRootToViewsHolder[instance.root] = instance;

			//print("Created instance" + itemIndex);

			return instance;
		}

		protected override void UpdateViewsHolder(ResultsList_SimpleExpandableClientViewsHolder vh) { vh.UpdateViews(Data[vh.ItemIndex]); }
		#endregion

		#region events from DrawerCommandPanel
		void OnAddItemRequested(bool atEnd)
		{
			int index = atEnd ? Data.Count : 0;
			Data.Insert(index, CreateNewModel(index));
			InsertItems(index, 1, freezeContentEndEdge_isOn);
		}
		void OnRemoveItemRequested(bool fromEnd)
		{
			if (Data.Count == 0)
				return;

			int index = fromEnd ? Data.Count - 1 : 0;

			Data.RemoveAt(index);
			RemoveItems(index, 1, freezeContentEndEdge_isOn);
		}
		void OnItemCountChangeRequested()
		{
			// ChangeModelsAndReset(demoUI.SetCountValue);
		}
		void OnScrollToRequested()
		{
			//if (demoUI.ScrollToValue >= Data.Count)
			//	return;

			//demoUI.scrollToButton.interactable = false;
			//bool started = SmoothScrollTo(demoUI.ScrollToValue, .75f, .5f, .5f, () => demoUI.scrollToButton.interactable = true);
			//if (!started)
			//	demoUI.scrollToButton.interactable = true;
		}
		#endregion

		#region CExpandCollapseOnClick.ISizeChangesHandler implementation
		public bool HandleSizeChangeRequest(RectTransform rt, float newSize)
		{
            for(int i = 1; i < rt.transform.childCount - 1; i++)
            {
				rt.GetChild(i).gameObject.SetActive(_MapRootToViewsHolder[rt].layoutElement.preferredHeight < newSize);
            }
            
			_MapRootToViewsHolder[rt].layoutElement.preferredHeight = newSize;
			return true;
		}

		public void OnExpandedStateChanged(RectTransform rt, bool expanded)
		{
			var itemIndex = _MapRootToViewsHolder[rt].ItemIndex;
			Data[itemIndex].expanded = expanded;
		}

		public void ExpandItems()
		{
			foreach (RectTransform rt in _MapRootToViewsHolder.Keys)
			{
				if (_MapRootToViewsHolder[rt].expandCollapseComponent.expanded)
				{
					_MapRootToViewsHolder[rt].expandCollapseComponent.OnClicked();
				}
			}
		}

		
		public void CollapseItems()
		{
			foreach (RectTransform rt in _MapRootToViewsHolder.Keys)
			{
				if (!_MapRootToViewsHolder[rt].expandCollapseComponent.expanded)
				{
					_MapRootToViewsHolder[rt].expandCollapseComponent.OnClicked();
				}
			}
		}
		#endregion

		void ChangeModelsAndReset(int newCount)
		{
			Data.Clear();
			Data.Capacity = newCount;
			for (int i = 0; i < newCount; i++)
			{
				var model = CreateNewModel(i);
				Data.Add(model);
			}

			ResetItems(Data.Count);
		}

		ExpandableSimpleClientModel CreateNewModel(int index)
		{
			var model = new ExpandableSimpleClientModel()
			{
				player = playerList[index],
				nonExpandedSize = _PrefabLayoutElement.preferredHeight
			};

			return model;
		}
	}
}