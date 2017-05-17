using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIExtensions;
using UnityEngine.UI;
using System.Diagnostics;

public class RelaxingRhythmsGameView : InGameView 
{
    private static RelaxingRhythmsGameView mInstance;
    public static RelaxingRhythmsGameView Instance { get { return mInstance; } }

    public string[]                 timeChoices;
    public string[]                 scrolling;
    public bool                     introDone = false;
    public AudioSource              meditationTrack;
	public GameInit					gameInit;
    static string                   SKIPINTRO           = "Skip Intro";
    static string                   SELECTLENGTH        = "Select a Length";
    static string                   INTRO               = "Introduction";
    static string                   INTRODONE           = "Introduction Complete";
    static string                   CONGRATS            = "Congratulations!";
	static string                   INFOTEXT            = "In the next event, follow the <b>Breathing Indicator</b>" +
		" to increase your <b>Coherence</b> score";
    static string                   INTRONEXT           = "Next:  Guided Meditation with ";
    static string                   GUIDEDNEXT          = "Next:  Play ";
    static string                   GAMENEXT            = "Next: Step ";
    static string                   STEP10TEXT1         = "You've completed the final step of Relaxing Rhythms 2! We hope you're becoming familiar with your own relaxation and natural breathing cycles and have begun to incorporate these lessons into your daily life.";
    static string                   STEP10TEXT2         = "Now that you know what to do, you should be able to better maintain the self-discipline to guide your mediation practice with a firm but gentle hand.";
    static string                   STEP10TEXT3         = "Maintain and grow your mediation practice by repeating the steps to reinforce the guided lessons as well as continuing to play the games, perhaps with increased difficulty settings.";
    static string                   STEP10END           = "Be Well!";

	MeditationLevelSetup.EVENT      mChoice;
	GameObject                      mEventScene;

    bool							mAudioStarted;
	bool							mPaused;

	int                             mStepNumber;
	EventData                       mChosenEventData;
	string                          mEventName;

	PlayMakerFSM                    mScene;
	AudioSource                     mPlaymakerAudio;

    WindowView                      mIntroEndWindow;
    WindowView                      mStep10EndWindow;

	float 							mTotalTime;
	bool 							mStarted = false;
	bool 							mAfterMetrics = false;
	Stopwatch 						mStopwatch;

    void Start()
    {
        if (mInstance == null)
        {
            mInstance = this;
        }

		Iom.IomEngine.Instance.RecievedDataPack += HandleBPMRecieved;
		if (gameInit != null) 
		{
			gameInit.Setup ();
			UnityEngine.Debug.Log ("================ Setup");
		}
    }

    public override void SetUpPage ()
	{
		base.SetUpPage ();
		SetupMeditation ();
	}

	public void SetupMeditation()
	{
        if (MeditationLevelSetup.instance) 
		{
            mChoice = MeditationLevelSetup.instance.GetEventType();
            mStepNumber = MeditationLevelSetup.instance.GetStepIndex();

            mChosenEventData = MeditationLevelSetup.instance.ChosenEventData();
            mEventScene = mChosenEventData.EventPrefab;
            mEventName = mChosenEventData.EventName;

            //start tracking data to send to server
            StartTracking(mChosenEventData.EventTitle);

            if (meditationTrack == null)
            {
                meditationTrack = gameObject.AddComponent<AudioSource>();
            }

            //set volume to volume from settings
            meditationTrack.volume = AppController.instance.GetVolume();

            switch (mChoice)
            {
                case MeditationLevelSetup.EVENT.INTRO:
                    mEventName += " Introduction";
                    needIom = false;
                    Instantiate(mChosenEventData.IntroScenePrefab);
                    ShowDrawer(null, null, mChosenEventData.EventName + ": " + mChosenEventData.EventTitle, INTRO, "", 0, true, false);
                    StartCoroutine(PlayIntro());
                    break;
                case MeditationLevelSetup.EVENT.GUIDED:
                    mEventName += " Meditation";
                    CreateScene();
                    ShowDrawer(null, mChosenEventData.MentorImage, mChosenEventData.MeditationTitle, mChosenEventData.MentorName, "", 0, true, false);
                    StartCoroutine(PlayGuidedMeditation());
                    break;
                case MeditationLevelSetup.EVENT.GAME:
                    mEventName += " Game";
                    allowIgnore = false;
                    CreateScene();
                    ShowDrawer(SkipIntro, null, mChosenEventData.EventGame, mChosenEventData.EventName, SKIPINTRO, 0, false, true);
                    StartCoroutine(PlayEventIntro());
                    break;
            }
        }
    }

    void SkipIntro ()
    {
		StopCoroutine(FadeOutAudio(meditationTrack));
		StopCoroutine(PlayIntro());
		StopCoroutine(PlayGuidedMeditation());
		StopCoroutine(PlayEventIntro());
        PlayEventMusic();
    }

	void StartDurationGame (float duration)
    {
        mScene.enabled = true;
        introDone = true;

		mTotalTime = duration;

        meditationTrack.clip = mChosenEventData.GameBackgroundMusic;
        meditationTrack.loop = true;
        meditationTrack.Play();
    }

    void CreateScene ()
    {
        mScene = GameObjectExtensions.AddPrefabChildAndGetComponent<PlayMakerFSM>(gameObject, mEventScene);
        mScene.enabled = false;

        //set scene noises to correct volume
        if (mScene.GetComponent<AudioSource>() != null)
        {
            mPlaymakerAudio = mScene.GetComponent<AudioSource>();
            mPlaymakerAudio.volume = AppController.instance.GetVolume();
        }
    }

    //selects the duration for step 7 game
    public override void HandleSelectionChanged(int selection)
    {
		int minutes = 3;

        switch (selection)
        {
            case 1:
				minutes = 5;
                break;
            case 2:
				minutes = 10;
                break;
            case 3:
				minutes = 15;
                break;
        }
		MeditationLevelSetup.instance.SetDuration(minutes);
        base.HandleSelectionChanged(selection);
        drawerWithSelection.HideNoDelay();
		SetUpTime(minutes * 60.0f);
		StartDurationGame (minutes * 60.0f);
		if (mStopwatch == null)
		{
			mStopwatch = new Stopwatch ();
            UnityEngine.Debug.Log("NewStopWatch");
        }
		mStopwatch.Start ();
    }

    public override void BeforeDestory ()
	{
		StopCoroutine(FadeOutAudio(meditationTrack));
		StopCoroutine(PlayIntro());
		StopCoroutine(PlayGuidedMeditation());
		StopCoroutine(PlayEventIntro());
		Iom.IomEngine.Instance.RecievedDataPack -= HandleBPMRecieved;
		base.BeforeDestory ();
	}

	public override void AfterUpdate ()
	{
		base.AfterUpdate ();

        if (mChoice == MeditationLevelSetup.EVENT.GAME && !meditationTrack.isPlaying && !introDone && !mPaused)
        {
            UnityEngine.Debug.Log("should be playing, waiting for skip");
            if (drawerWithSelection == null)
            {
                PlayEventMusic();
            }
        }

        //even if there is no iom data, still update the time UI
        if ((mChosenEventData.DurationGame && introDone) || mChoice != MeditationLevelSetup.EVENT.GAME
			&&!mPaused)
        {
            UpdateHUDTime();
        }

		if(!mStarted && meditationTrack!=null && playing)
		{
			mStarted = true;
			Iom.IomEngine.Instance.Data.Paused = false;
			meditationTrack.Play();
			mAudioStarted = true;
			return;
		}

		if (mAudioStarted
			&&!Iom.IomEngine.Instance.Data.Paused && !mPaused) 
		{
			bool ended = false;
            if (meditationTrack != null)
            {
				if (mStopwatch != null) 
				{
					if (mStopwatch.ElapsedMilliseconds * 0.001f >= mTotalTime) 
					{
						ended = true;
					}
				}
                else if (!meditationTrack.isPlaying && mChoice != MeditationLevelSetup.EVENT.GAME 
					&& !AppController.instance.backFromPause)
                {
                    ended = true;
                }

            }

            if (ended) 
			{
				Iom.IomEngine.Instance.Data.Paused = true;
				StopAudioEndGame (null);
				mAudioStarted = false;
			}
		}
		if(Iom.IomEngine.Instance.Data.Paused)
		{
			return;
		}

		Iom.IomEngine.Instance.Data.Elapsed += Time.deltaTime;

		if(!Iom.IomEngine.Instance.Data.IsConnected)
		{	
			UpdateCoherence (0);
			UpdateBPM (0, false);

		}
	}

	void HandleBPMRecieved(string msg)
	{
		if (!Iom.IomEngine.Instance.Data.Paused) 
		{
			int value = Mathf.RoundToInt (Iom.IomEngine.Instance.Data.Coherence * 100.0f);
			int heartRate = Mathf.RoundToInt (Iom.IomEngine.Instance.Data.CurrentSmoothedHeartRate);

			UpdateCoherence (value);
			UpdateBPM (heartRate, true);
		}
	}

	void UpdateHUDTime()
	{
		if (Iom.IomEngine.Instance != null) 
		{
			if (meditationTrack != null && meditationTrack.clip != null && mAudioStarted) 
			{
				if (mStopwatch == null) 
				{
					UpdateHUDTime (meditationTrack.time / mTotalTime);
				} 
				else 
				{
					UpdateHUDTime (mStopwatch.ElapsedMilliseconds*0.001f/ mTotalTime);
				}
			}
		}
	}

    IEnumerator PlayGuidedMeditation ()
    {
        meditationTrack.clip = mChosenEventData.GuidedMeditation;
        mTotalTime = meditationTrack.clip.length;
        SetUpTime(mTotalTime);
        meditationTrack.Play();
        yield return new WaitForSeconds(meditationTrack.clip.length);
    }

    IEnumerator PlayIntro()
    {
        meditationTrack.clip = mChosenEventData.Intro;
        mTotalTime = meditationTrack.clip.length;
        SetUpTime(mTotalTime);
		ShowMatrix (false, false, true, true);
        meditationTrack.Play();
        yield return new WaitForSeconds(meditationTrack.clip.length);
    }

    IEnumerator PlayEventIntro()
    {
        if (mChosenEventData.DurationGame)
            HideTime(false, true);
        else
            HideTime(true, true);

        meditationTrack.clip = mChosenEventData.GameIntro;
        meditationTrack.Play();
        yield return new WaitForSeconds(meditationTrack.clip.length);
        //PlayEventMusic();
    }

    void PlayEventMusic()
    {
        UnityEngine.Debug.Log("play event music");
        if(drawerWithIcon != null)
        {
            drawerWithIcon.buttonOnBanner.gameObject.SetActive(false);
            drawerWithIcon.HideNoDelay();
        }
        meditationTrack.Stop();

		if (mChosenEventData.DurationGame) 
		{
			ShowDrawer (SELECTLENGTH, timeChoices, false, true);
		}
        else
        {
            mScene.enabled = true;
            introDone = true;

            meditationTrack.clip = mChosenEventData.GameBackgroundMusic;
            meditationTrack.loop = true;
            meditationTrack.Play();
        }
    }

	public override void HandleVolumeChanged (float volume)
	{
        if (meditationTrack != null)
        {
            meditationTrack.volume = volume;
        }
        if (mPlaymakerAudio != null)
        {
            mPlaymakerAudio.volume = volume;
        }

        base.HandleVolumeChanged (volume);
	}

	public override void HandleTimeChange (float value)
	{
		if (meditationTrack != null) 
		{
			float length = meditationTrack.clip.length;

			//if we scroll to the very end, we recieve compile error
			if (value <= 0.99f) 
			{
				meditationTrack.time = length * value;
			}
		}
		base.HandleTimeChange (value);
	}

    public override void HandlePause (bool value)
	{
		mPaused = !value;
        if (value)
        {
            if (meditationTrack != null)
            {
                meditationTrack.UnPause();
            }
            if (introDone)
            {
                mScene.enabled = true;
            }
			if (mStopwatch != null) 
			{
				mStopwatch.Start ();
			}
        } 
		else 
		{
            if (meditationTrack != null) 
			{
                meditationTrack.Pause ();
                UnityEngine.Debug.Log(meditationTrack.clip.name + " paused");
				UpdateTime (meditationTrack.time / mTotalTime);
            }
            if (introDone)
            {
                mScene.enabled = false;
            }
			if (mStopwatch != null) 
			{
				mStopwatch.Stop ();
			}
        }

		base.HandlePause (value);
	}

    public override void OnErrorShowed()
    {
        mPaused = true;
        if (meditationTrack != null)
        {
            meditationTrack.Pause();
        }
        if (introDone)
        {
            mScene.enabled = false;
        }
		if (mStopwatch != null) 
		{
			mStopwatch.Stop ();
		}
        base.OnErrorShowed();
    }

    public override void OnErrorSolved()
    {
        mPaused = false;
        if (meditationTrack != null)
        {
            meditationTrack.UnPause();
        }
        if (introDone)
        {
            mScene.enabled = true;
        }
		if (mStopwatch != null) 
		{
			mStopwatch.Start ();
		}
        base.OnErrorSolved();
    }

    public override void Menu()
    {
        base.Menu();
        meditationTrack.Stop();
        LoadPage("LevelSelection");
    }

    public override void AfterLoadSettings()
    {
        if (mChoice == MeditationLevelSetup.EVENT.GAME)
            mScene.enabled = false;

        base.AfterLoadSettings();
    }

    public override void HandleSettingsBackClicked()
    {
        /*
         * This code introduces a bug that while playing the game, if the player
         * presses pause then goes to settings, then presess the back button
         * the game starts playing again, even though the game is supposed to
         * still be paused.
         * 
        if (mChoice == MeditationLevelSetup.EVENT.GAME && introDone) { 
            mScene.enabled = true;
        }
        */

        AppController.instance.StopBackgroundMusic();
        Iom.IomEngine.Instance.Data.Difficulty = (AppController.instance.GetDifficulty() + 1) * 10;
        Iom.IomEngine.Instance.Data.PreferredHeartCycle = AppController.instance.GetBreathingCycleSeconds();

        base.HandleSettingsBackClicked();
    }

    public override void Continue()
    {
        base.Continue();
        switch (mChoice)
        {
            case MeditationLevelSetup.EVENT.INTRO:
                MeditationLevelSetup.instance.SaveEventIndex(1);
                break;
            case MeditationLevelSetup.EVENT.GUIDED:
                if (mChosenEventData.HasEvent)
                    MeditationLevelSetup.instance.SaveEventIndex(2);
                else
                {
                    if (mAfterMetrics)
                    {
                        MeditationLevelSetup.instance.SaveStepIndex(0);
                        LoadPage("LevelSelection");
                    }
                    else
                    {
                        HideEndWindow();
                        Step10Modal();
                        mStep10EndWindow.gameObject.SetActive(true);
                        mAfterMetrics = true;
                    }
                    return;
                }
                break;
            case MeditationLevelSetup.EVENT.GAME:
                MeditationLevelSetup.instance.SaveStepIndex(mStepNumber + 1);
                LoadPage("LevelSelection");
                return;
        }
        LoadPage("Main");
    }

    public void StopAudioEndGame(AudioClip endSound)
    {
        if (mChoice == MeditationLevelSetup.EVENT.GAME)
        {
			SetupEffect();
        }
        if (meditationTrack != null && meditationTrack.clip != null)
        {
            StartCoroutine(FadeOutAudio(meditationTrack));
        }
		if (mStopwatch != null) 
		{
			mStopwatch.Reset ();
            UnityEngine.Debug.Log("TimerReset");
        }
        EndGame(endSound);
    }

    IEnumerator FadeOutAudio(AudioSource player)
    {
        float currentVolume = player.volume;

        player.volume = Mathf.Lerp(currentVolume, 0.0f, 4.0f);
        yield return new WaitForSeconds(4.0f);
        player.Stop();
    }


	public override void UpdateEndScreen ()
	{
        EventTracker.instance.TrackStepCompleted(mEventName, DataTracker.instance.GetAverageHeartRate(), DataTracker.instance.GetAverageCoherence(),
            AppController.instance.GetDifficulty(), AppController.instance.GetBreathingCycleSeconds());
        if (mChoice == MeditationLevelSetup.EVENT.INTRO)
        {
            HideEndWindow();
            CreateIntroModal();
            mIntroEndWindow.gameObject.SetActive(true);
            NextText();
        }
        else
        {
			float meditationDuration = 0;
			if (mStopwatch == null) 
			{
				meditationDuration = Iom.IomEngine.Instance.Data.Elapsed;
			} 
			else 
			{
				// if the event is time based
				mStopwatch = null;
                UnityEngine.Debug.Log("StopWatchNULL");
                meditationDuration = mTotalTime;
			}
			timeChart.UpdateValue(StringFormater.FormateTimeToString(meditationDuration), meditationDuration/ mTotalTime);

            NextText();
            base.UpdateEndScreen();
        }
	}

    void NextText()
    {
        string message = "";
        switch (mChoice)
        {
            case MeditationLevelSetup.EVENT.INTRO:
                message = INTRONEXT + mChosenEventData.MentorName;
                break;
            case MeditationLevelSetup.EVENT.GUIDED:
                if (mChosenEventData.HasEvent)
                {
                    message = GUIDEDNEXT + mChosenEventData.EventGame;
                }
                break;
            case MeditationLevelSetup.EVENT.GAME:
                int n = mChosenEventData.EventNumber + 1;
                message = GAMENEXT + n;
                break;
        }
        UpdateSessionText(message);
    }

    void CreateIntroModal()
    {
        mIntroEndWindow = CreateWindowView();
        VerticalLayoutGroup windowGrid = mIntroEndWindow.Init(INTRODONE, currentTheme, currentTheme.titleFont, new Vector2(800, 0));
        windowGrid.childControlHeight = true;

        LayoutGroup textGrid = UIObjectExtensions.AddLayout(windowGrid.gameObject, currentTheme.verticalTablePrefab, TextAnchor.MiddleCenter, new Vector2(0, 0));
        textGrid.padding = new RectOffset(0, 0, 0, -20);
        textGrid.childAlignment = TextAnchor.MiddleCenter;
        VerticalLayoutGroup textGridV = textGrid as VerticalLayoutGroup;
        textGridV.childControlHeight = true;
		textGridV.childControlWidth = true;

        LayoutGroup infoSection = UIObjectExtensions.AddLayout(textGrid.gameObject,
			currentTheme.horizontalLayoutPrefab, TextAnchor.MiddleCenter, new Vector2(0, 0));
		HorizontalLayoutGroup ChartsH = infoSection as HorizontalLayoutGroup;
        ChartsH.padding = new RectOffset(0, 0, 0, 0);
        ChartsH.spacing = 5;
        ChartsH.childControlHeight = true;
		ChartsH.childForceExpandWidth = true;
		ChartsH.childControlWidth = true;

		LayoutGroup infoTextLine = UIObjectExtensions.AddLayout(infoSection.gameObject,
			currentTheme.verticalTablePrefab, TextAnchor.UpperRight, new Vector2(0, 0));
		VerticalLayoutGroup verticalInfoText = infoTextLine.GetComponent<VerticalLayoutGroup> ();
		verticalInfoText.childForceExpandHeight = true;
		verticalInfoText.childControlHeight = true;
		verticalInfoText.childControlWidth = false;
		verticalInfoText.padding = new RectOffset (0, 0, -60, 0);
		LayoutGroup infoSectionLine1 = UIObjectExtensions.AddLayout(infoSection.gameObject,
			currentTheme.verticalTablePrefab, TextAnchor.MiddleLeft, new Vector2(0, 0));

        VerticalLayoutGroup inforVertical = infoSectionLine1 as VerticalLayoutGroup;
		inforVertical.childControlHeight = true;
		inforVertical.childForceExpandHeight = true;
        inforVertical.childControlWidth = false;
        inforVertical.spacing = -20;

		Text infoText = UIObjectExtensions.AddText(infoTextLine.gameObject, currentTheme.textPrefab, INFOTEXT, 22, new Vector2(320, 30));
		infoText.alignment = TextAnchor.MiddleLeft;

        Image breathing = GameObjectExtensions.AddPrefabChildAndGetComponent<Image>(infoSectionLine1.gameObject, currentTheme.imagePrefab);
        breathing.sprite = currentTheme.breathingExample;
        breathing.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 200);
        breathing.preserveAspect = true;

        Image coherence = GameObjectExtensions.AddPrefabChildAndGetComponent<Image>(infoSectionLine1.gameObject, currentTheme.imagePrefab);
		coherence.sprite = currentTheme.coherenceExample;
		coherence.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 200);
        coherence.preserveAspect = true;

        EndGameButtons(windowGrid, true);

        RectTransform endWindowRect = mIntroEndWindow.GetComponent<RectTransform>();
        endWindowRect.anchorMax = new Vector2(1, 1);
        endWindowRect.anchorMin = new Vector2(0, 0);
        endWindowRect.sizeDelta = new Vector2(300, -40);

		mIntroEndWindow.container.GetComponent<RectTransform> ().sizeDelta = new Vector2 (-80, -80);
    }

    void Step10Modal ()
    {
        mStep10EndWindow = CreateWindowView();
        VerticalLayoutGroup windowGrid = mStep10EndWindow.Init(CONGRATS, currentTheme, currentTheme.titleFont, new Vector2(800, 0));
        windowGrid.spacing = 0;
        windowGrid.childControlHeight = true;

        LayoutGroup infoSection = UIObjectExtensions.AddLayout(windowGrid.gameObject,
            currentTheme.horizontalLayoutPrefab, TextAnchor.MiddleCenter, new Vector2(500, 100));
        HorizontalLayoutGroup ChartsH = infoSection as HorizontalLayoutGroup;
        ChartsH.padding = new RectOffset(-5, 0, 0, 0);
        ChartsH.spacing = 0;
        ChartsH.childControlHeight = false;

        UIObjectExtensions.AddText(infoSection.gameObject, currentTheme.textPrefab, STEP10TEXT1, 20, new Vector2(600, 100), 
			TextAnchor.MiddleLeft);
        Image icon = GameObjectExtensions.AddPrefabChildAndGetComponent<Image>(infoSection.gameObject, currentTheme.imagePrefab);
        icon.sprite = currentTheme.gameIcon;
        icon.preserveAspect = true;

        UIObjectExtensions.AddText(windowGrid.gameObject, currentTheme.textPrefab, STEP10TEXT2, 20, new Vector2(700, 100), TextAnchor.MiddleLeft);
        UIObjectExtensions.AddText(windowGrid.gameObject, currentTheme.textPrefab, STEP10TEXT3, 20, new Vector2(700, 100), TextAnchor.MiddleLeft);
        UIObjectExtensions.AddText(windowGrid.gameObject, currentTheme.textPrefab, STEP10END, 20, new Vector2(700, 50), TextAnchor.MiddleLeft);

        EndGameButtons(windowGrid, false);

        RectTransform endWindowRect = mStep10EndWindow.GetComponent<RectTransform>();
        endWindowRect.anchorMax = new Vector2(1, 1);
        endWindowRect.anchorMin = new Vector2(0, 0);
        endWindowRect.sizeDelta = new Vector2(300, -40);
    }
}
