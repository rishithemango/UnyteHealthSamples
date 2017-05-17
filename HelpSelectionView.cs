using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIExtensions;
using UnityEngine.UI;

public class HelpSelectionView : LevelSelectionView
{
    static      string  VIEW            = "VIEW   ";
    static      string  HELP            = "help";
    static      string  BACK            = "BACK";
    List<HelpCategory>  mCategories;
    List<HelpLevel>     mLevels;
    STAGE               mStage;
    RectTransform       mContentSize;
    HelpCategory        mCategory;
    HelpLevel           mLevel;
    WindowView          mWindow;
    Text                mText;
    Scrollbar           mScrollBar;
    bool                mForceScroll;

    enum STAGE
    {
        MAIN,
        SUB,
        SPECIFIC,
    }

    public override void AfterSetUp()
    {
        startString = VIEW;
        base.AfterSetUp();
        LoadCategory();
    }

    public override void Play()
    {
        List<Level> convertLevels = new List<Level>();
        switch (mStage)
        {
            case STAGE.MAIN:
                mStage = STAGE.SUB;
                HelpMenuSetup.instance.SaveCategory(selectedIndex);
                mLevels = HelpMenuSetup.instance.GetTitles();
                mCategory = HelpMenuSetup.instance.GetCategory();

                UpdateTitle("help..." + mCategory.info.levelTitle.ToLower());

                if (mLevels.Count == 0)
                {
                    SetupInfo();
                    break;
                }

                if (mLevels[0].type == HelpLevel.TYPE.CREDITS)
                {
                    HelpMenuSetup.instance.SaveTitle(0);
                    mLevel = HelpMenuSetup.instance.GetLevel();
                    ShowWindow();
                    break;
                }

                foreach (HelpLevel l in mLevels)
                {
                    Level le = new Level();
                    le.levelIcon = l.levelIcon;
                    le.levelTitle = l.levelTitle;
                    convertLevels.Add(le);
                }  
                LoadLevels(convertLevels.ToArray(), false);
                break;
            case STAGE.SUB:
                HelpMenuSetup.instance.SaveTitle(selectedIndex);
                mLevel = HelpMenuSetup.instance.GetLevel();

                if (mLevel.hasSubSections)
                {
                    UpdateTitle("help..." + mCategory.info.levelTitle.ToLower() + "..." + mLevel.levelTitle.ToLower());
                    LoadLevels(HelpMenuSetup.instance.GetExtraHelp().ToArray(), false);
                    mStage = STAGE.SPECIFIC;
                }
                else
                {
                    SetupInfo();
                }
                break;
            case STAGE.SPECIFIC:
                HelpMenuSetup.instance.SaveStepHelp(selectedIndex);
                mLevel = HelpMenuSetup.instance.GetLevel();
                SetupInfo();
                break;
            default:
                base.Play();
                break;
        }
    }

    public override void GoBack()
    {
        switch (mStage)
        {
            case STAGE.MAIN:
                base.GoBack();
                break;
            case STAGE.SUB:
                LoadCategory();
                break;
            default:
                LoadTitles();
                break;
        }
    }

    void LoadCategory ()
    {
        mStage = STAGE.MAIN;
        mCategories = HelpMenuSetup.instance.categories;

        UpdateTitle(HELP);

        List<Level> convertLevels = new List<Level>();
        foreach (HelpCategory l in mCategories)
        {
            Level le = new Level();
            le.levelIcon = l.info.levelIcon;
            le.levelTitle = l.info.levelTitle;
            convertLevels.Add(le);
        }
        LoadLevels(convertLevels.ToArray(), false, HelpMenuSetup.instance.GetCategoryIndex());
    }

    void LoadTitles ()
    {
        mStage = STAGE.SUB;
        mLevels = HelpMenuSetup.instance.GetTitles();

        UpdateTitle("help..." + mCategory.info.levelTitle.ToLower());
        List<Level> convertLevels = new List<Level>();
        foreach (HelpLevel l in mLevels)
        {
            Level le = new Level();
            le.levelIcon = l.levelIcon;
            le.levelTitle = l.levelTitle;
            convertLevels.Add(le);
        }
        LoadLevels(convertLevels.ToArray(), false, HelpMenuSetup.instance.GetLevelIndex());
    }

    void ShowWindow()
    {
        if (mWindow == null) 
        {
            mWindow = GameObjectExtensions.AddPrefabChildAndGetComponent<WindowView>
            (overlay.transform.parent.gameObject, currentTheme.windowPrefab);
            //contril the width of window buttons
            mWindow.title.GetComponent<RectTransform> ().sizeDelta = new Vector2 (200, 65);
            mWindow.title.resizeTextForBestFit = true;

            VerticalLayoutGroup windowGrid = mWindow.Init (mLevel.levelTitle.ToLower(), currentTheme, currentTheme.titleFont, 
                new Vector2 (800,-10));
            Destroy (windowGrid);

            mWindow.container.GetComponent<RectTransform> ().sizeDelta = new Vector2 (-40, -100);
            ScrollRect scroll = GameObjectExtensions.AddPrefabChildAndGetComponent<ScrollRect>
                (windowGrid.gameObject, currentTheme.scrollViewVerticalPrefab);
            scroll.name = "scroll";
            mContentSize = scroll.content.GetComponent<RectTransform> ();
            RectTransform scrollRect =  scroll.GetComponent<RectTransform> ();
            scrollRect.anchorMax    = new Vector2 (1, 1);
            scrollRect.anchorMin    = new Vector2 (0, 0);
            scrollRect.sizeDelta    = new Vector2 (0, -60);
            scrollRect.pivot        = new Vector2 (0.5f, 1);
            //scroll.verticalScrollbar.value = 0;

            mText = UIObjectExtensions.AddText (scroll.content.gameObject, currentTheme.textPrefab, mLevel.content, 
                20, new Vector2 (700, 100), TextAnchor.UpperLeft);
            RectTransform textRect = mText.GetComponent<RectTransform> ();
            textRect.anchorMax  = new Vector2 (1, 1);
            textRect.anchorMin  = new Vector2 (0, 0);
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;

            ButtonWithIcon back = TemplateUIObject.AddButtonWithIcon (windowGrid.gameObject, currentTheme.singleLineButtonPrefab,
                BACK, 30, CloseWindow, new Vector2 (225, 50), currentTheme.menuIcon, currentTheme.themeColor);
            RectTransform backRect = back.GetComponent<RectTransform> ();
            backRect.anchorMax = new Vector2 (0.5f, 0);
            backRect.anchorMin = new Vector2 (0.5f, 0);
            backRect.pivot =    new Vector2 (0.5f, 0);

            mScrollBar = scroll.verticalScrollbar;
        }

        mWindow.title.text = mLevel.levelTitle.ToLower();
        mText.text = mLevel.content;
        mContentSize.sizeDelta = new Vector2 (0, mLevel.contentHeight);
        mWindow.gameObject.SetActive (true);
        ShowAlphaOverlay ();

        mForceScroll = true;
    }

    void CloseWindow()
    {
        mWindow.gameObject.SetActive (false);
        HideAlphaOverlay ();
        if (mLevel.type == HelpLevel.TYPE.CREDITS)
        {
            LoadCategory();
        }
    }

    void SetupInfo()
    {
        if (HelpMenuSetup.instance)
        {
            switch (mLevel.type)
            {
            case HelpLevel.TYPE.VIDEO:
                PlayVideo (mLevel.videoClip);
                break;
            default:
                ShowWindow ();
                break;
            }
        }
    }

    public override void AfterUpdate ()
    {
        base.AfterUpdate ();

        if (mForceScroll && mScrollBar != null) 
        {
            if (mScrollBar.value != 1) 
            {
                mForceScroll = false;
                mScrollBar.value = 1;
            }
        }
    }
}
