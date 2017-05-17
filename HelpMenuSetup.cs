using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public class HelpCategory
{
    public string name;
    public Level info;
    public List<HelpLevel> sections = new List<HelpLevel>();

    public HelpCategory(Level l, List<HelpLevel> levelSections)
    {
        name = l.levelTitle;
        info = l;
        sections = levelSections;
    }
}

public class HelpMenuSetup : SetupBase
{
    private static HelpMenuSetup mInstance;
    public  static HelpMenuSetup instance { get { return mInstance; } }

    public List<HelpCategory>   categories              = new List<HelpCategory>();
    public List<HelpLevel>      titles                  = new List<HelpLevel>();
    public List<string>         stepContent;
    public List<float>          stepHeight;
    public List<Level>          extraHelp               = new List<Level>();
    public List<EventData>      info                    = new List<EventData>();

    HelpCategory                mCategory;
    HelpLevel                   mTitle;
    HelpLevel                   mStepLevel;
    int                         mCategoryIndex;
    int                         mStepIndex;
    int                         mLevelIndex;

    void Start()
    {
        if (mInstance == null)
        {
            mInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetUp()
    {
        CreateDictionary ();
        categories.Clear ();
        StepSpecificHelp();

        HelpCategory video = AddSection("Intro Videos");

        AddATitle("Welcome Video", video, HelpLevel.TYPE.VIDEO, false, "IntroVideo.mp4");
        AddATitle("How To Play Video", video, HelpLevel.TYPE.VIDEO, false, "HelpVideo.mp4");
        categories.Add(video);

        HelpCategory game = AddSection("Relaxing Rhythms 2");
        AddATitle("How To Play", game, HelpLevel.TYPE.GAME, false, "Relaxing Rhythms 2 consists of ten steps, " +
            "with each step containing an introduction, a guided meditation and a fun, sometimes challenging, biofeedback " +
            "event.\n\nSimply follow the navigation to progress through these steps and events.\n\nRelaxing Rhythms 2 is " +
            "best experienced using an Iom device. The Iom device is optional for some events and mandatory for others - you’ll " +
            "be prompted appropriately.\n\nVery simply, the goal is to learn how to meditate and/or improve your meditation" +
            " practice by focussing on your breathing and following the lesson instructions.  \n\nWhenever you see the Breathing" +
            " Indicator, try and follow its pattern with your own in and out breathing cycles to maximise your Coherence score" +
            " which is a measure of your meditative state.\n\nYou can adjust the rate of the Breathing Indicator in the Settings" +
            " menu.\n\nYour goal is to maximise your Coherence score. Your Coherence score is used to progress in the biofeedback" +
            " events as well as to give you an indication of your meditative state.\n\nDuring the meditations and biofeedback" +
            " events, using your Iom device, sit still, avoid talking, chewing, drinking, etc. and practice slow, deep regular" +
            " and rhythmic breaths. As you do this you will see your scores increase as you increase your heart rate variability" +
            " and your Coherence. \n\nNOTE: Breathing Cycle is the total number of seconds in your “heart breath” which is" +
            " determined by adding the number of seconds breathing in added to the number of seconds breathing out. For most" +
            " people this is roughly 10 seconds total.\n\nWhen you are breathing in this program to clear your mind, the" +
            " biofeedback component is taking your heart rate signals and interpreting them to show you how your breathing" +
            " cycle can be trained to become deeper, slower and more regular. This type of “breathwork” has profound benefits" +
            " to your mind and body, as well as to your mental and emotional state.\n\nThis biofeedback training will" +
            " help you in your daily life as your learn to react to challenges and tasks in a more calm, clear" +
            " and relaxed manner.\n\nThe more you use Relaxing Rhythms 2 the more you will become familiar and" +
            " in tune with your body’s state of relaxation and your breathing cycle.\n", 1200);
        AddATitle("What You Will Learn", game, HelpLevel.TYPE.GAME, false,"It’s especially important to say that" +
            " your needs and journey will be individual. Take what works for you, and leave what doesn’t. What matters" +
            " most is that you use this program to enrich and expand your awareness, while deepening your sense of calm" +
            " and peace. \n\nThere is no “wrong” or “right” way to approach these lessons - simply begin and adjust as" +
            " you go. You’ll be amazed at how simply tuning into the body and its rhythms can bring about a significantly" +
            " increased sense of health and well-being. \n\nAs you use the different programs from Wild Divine you may" +
            " find some activities more challenging, while others will come naturally to you - this is normal. As you" +
            " learn to control your energy and state of mind, you may experience an internal shift – an “a-ha” experience" +
            " where you’ll just intuitively know how to do things. \n\nYou’ll develop a natural sense of when to breathe" +
            " more slowly and deeply, or how to connect with a positive emotion. \n\nRemember, the most important tools" +
            " you will take away from the Wild Divine programs are not just those you use when seated at your computer," +
            " but those that you take with you into your everyday life.\n", 600);
        AddATitle("The Ten Steps", game, HelpLevel.TYPE.GAME, true,"", 300);
        categories.Add(game);

        HelpCategory meditation = AddSection("Meditation");
        AddATitle("Mind Body Training", meditation, HelpLevel.TYPE.MEDITATION, false, "<b>What is Mind Body Training?</b>" +
            "\nThe concept of mind/body wellness springs from the widely held belief that your mental, emotional, and spiritual" +
            " self are not separate from your physical, flesh - and - blood self.They are all deeply interconnected on levels you may not even be aware of yet." + 
            "\n\nWhile we may more easily accept the idea that physical pain can have emotional repercussions, the reverse is also true: " + 
            "Emotional disturbances can, and most likely will, have an impact on our physical state.For example, have you ever ended up with a cold during a time of acute stress ?" +
            "\n\nIn this holistic philosophy, all components of the “self” are considered— it says that we are more than what we think, what we feel, " + 
            "or what happens to us physically—that the “self” is the sum of all of these components woven together." + 
            "\n\n<b>How to Achieve It?</b>\nThe more attention and awareness you bring to what’s going on inside and around you, the less likely you are to be " + 
            "driven by unconscious habits and thought patterns that may have a negative impact on your physical and mental health.One easy way to begin to " + 
            "cultivate this connection between mind and body is to bring yourself back to your physical self through “breathwork” and meditative relaxation." + 
            "The simple act of being aware of your breath or energy level on a daily basis can have a considerable impact on how you think, feel, and act." +
            "In fact, with regular practice, you may find you are less susceptible to stress, fatigue, and illness." +
            "The lessons in this program will show you a range of different ways to strengthen your mind/body connection—and thus, your overall health and well-being. ",800);
        AddATitle("Meditation & Breathwork", meditation, HelpLevel.TYPE.MEDITATION, false,"<b>What is Meditation?</b>" +
            "\nThe age-old practice of meditation is an integral part of many traditions, both religious and non-religious. While there are thousands of different ways to meditate, " +
            "at the core of each is the practice of focusing and quieting the mind so that we can uncover and re-experience the eternal quiet and deep wisdom within each of us." +
            "\n\nIn this day and age, our busy lives are filled with so much sound, movement, and activity that meditation is essential to help us go within, tune out the distractions, and reconnect with our authentic self. " +
            "You don’t need any special training, religious background, or prior knowledge of meditation to experience its benefits in your life. Everything you need to know you’ll learn right here. " +
            "\n\nSometimes people who are new to meditation wonder if there’s some secret or trick to it, or if they’re doing it right. The goal is not to do it right," +
            " but to do it regularly—because it’s with consistency that the unlimited benefits of meditation are discovered.\n\n<b>What is Breathwork?</b>" +
            "\nIt’s been said that the breath is the link between the physical and spiritual. The simple act of tuning in to your breath can have a profound effect on your mind and body; it can alter your brainwaves, lower your blood pressure, and slow your heart rate." +
            " Breath is also a vital tool in meditation. Paying attention to the natural in-and-out flow of each breath gives your mind a place to return to and focus your awareness. " +
            "\n\nIn this program you will learn different breathing techniques to help you relax your mind, release physical tension, and create a sense of calm. " +
            "By engaging in even just a few minutes of conscious breathing, you help undo and even prevent the negative effects that stress can have on your health. " +
            "\n\nIf you leave this program only having learned how to use your breath to experience a few minutes of rich and utter quiet, you will have given yourself an invaluable gift to use for the rest of your life. ", 900);
        categories.Add(meditation);

        HelpCategory device = AddSection("Iom Device & Biofeedback");
        AddATitle("Biofeedback", device, HelpLevel.TYPE.DEVICE, false,"<b>What is Biofeedback?</b>" +
            "\nBiofeedback technology uses sophisticated electronic equipment to measure and monitor changes in your internal physiological states. " +
            "The measurements offer insight into what’s going on energetically in your body, and provide a platform on which you can make positive changes to your mental, physical and emotional states. " +
            "\n\nThough biofeedback is usually done in conjunction with a professional biofeedback therapist, the Relaxing Rhythms 2 program from Wild Divine is designed so that you can take advantage of this " +
            "effective technology on your own. Many biofeedback therapists have endorsed the Wild Divine programs because they believe they provide powerful and effective ways to experience first-hand, " +
            "the connection between your mind and body. They also believe the knowledge that stems from that experience can improve your overall sense of well-being. \n\n<b>The Biofeedback in Wild Divine</b>\n" +
            "The biofeedback hardware from Wild Divine, when plugged into your computer, allows you to interact with the software. You will be able to witness the positive effects of the practices taught in " +
            "this program as they are displayed in different ways to you on the screen. The heart rate signals taken from the IomPro, IomPE or Lightstone, are then transferred into the program. " +
            "\n\nWith each inhalation of breath your heart rate slightly increases and with each exhalation it slightly decreases. So if you were to chart your heart rate onto a graph, as you breathe in and out a sine" +
            " wave pattern would begin to form. Now, the length of time for each breath cycle is what the program measures.\n\nWhen you have those points at an equal length of time, for example, of 10 seconds between each" +
            " breath cycle, you form a pattern of breathing called Coherence. This is commonly referred to as the most stabilizing frequency and is the source of many positive changes in the body. Coherence " +
            "is also the best tool for recovery from stress and creating a happier mind and body. Meditation tends to quiet the busy mind and body and helps produce that state of Coherence. \n\n" +
            "In the Relaxing Rhythms 2 program are biofeedback events. Each event is meant to give you a different sort of training experience through a visual game format. Based upon the level of " +
            "Coherence difficulty measurement, as you breathe, relax and lower your heart rate, you will experience a change or event onscreen in response to those same signals. In this way, the game " +
            "becomes your personal coach to aid you in your efforts to meditate and restore your peace of mind.", 1200);
        AddATitle("Iom Troubleshooting", device, HelpLevel.TYPE.DEVICE, false,"<b>Iom Sensors</b>" +
            "\nRelaxing Rhythms 2 will work with both the IomPE and the IomPro sensors on your Mac or Windows computer." +
            "\n\nIf you are using the IomPro sensors (white housing) you will need to use the IomDriver program.  This is the same IomDriver program required for other Wild Divine software that use the IomPro sensors." +
            "\n\n<b>Important Notes</b>\n<b>IomPro Users</b> - If using IomPro sensors please start the IomDriver BEFORE you launch Relaxing Rhythms 2 to ensure proper connectivity with the IomPro sensors and the program." +
            "\n\n<b>IomPE Users</b> – Please make sure you DO NOT have the IomDriver program running! It will interfere with the IomPE sensors and Relaxing Rhythms 2 will not function properly." +
            "\n\n<b>NOTE:</b> If you are using the IomPE sensors (green housing) with Relaxing Rhythms 2 no additional driver software is required. \n\n<b>Wearing the Iom PE Ear Clip</b>" +
            "\nThe Iom PE ear clip must be properly placed on the center part of your ear lobe in order to control the events in Wild Divine software. It can also be worn on the hand, in the flesh between the thumb and fore-finger." +
            "\n\nPlease be sure that you remove any hair from the area, as it can interfere with the integrity of the signal. You will see a blinking red light in time with your "+
            "heart beat, and this should be beating steadily to display a good connection. Adjust accordingly until you see this display reliably.\n\n<b>System Requirements</b>\n" +
            "Windows – Windows Vista/Win7/Win8/Win10 \nMac – OS X 10.7 or greater \nHardware Needed: INTEL Core Duo2 (or equivalent) or greater \n4GB Main Memory, Graphics processor with 256 MB VRAM or SDRAM, 250 MB Free Disk Space, USB 2.0 Port (no USB Hub)" +
            "\n\n<b>Please visit <i>support.wilddivine.com </i> for more detailed support information.</b>", 1000);
        categories.Add(device);

        HelpCategory credits = AddSection("Credits");
        AddATitle("Credits", credits, HelpLevel.TYPE.CREDITS, false, "We would like to express our deepest gratitude to each of the generous and compassionate authors, artists, software developers and advisors who helped us in the development of Relaxing Rhythms 2." +
            "\n\nThich Nhat Hanh\nJon Kabat-Zinn\nPema Chodron\nAdyashanti\nRick Hanson\nGangaji\nShinzen Young\nJonathan Foust\nSally Kempton\nAri Shomair\nMacy Kuang\nRishi Ilangomaran\nAnthony Kanfer\nAnd the rest of the Wild Divine team!\n\n" +
            "Their word comes directly from the heart and will provide you with hours of inspiration and motivation as you head down the path toward greater balance, peace and happiness in your life." +
            "\n\nAlso deep thanks to our friends at Sounds True without whom this program could not have happened as it has.", 600);
        categories.Add(credits);

        extraHelp = GameSpecificHelp();
    }

    HelpCategory AddSection (string title)
    {
        Level level = new Level();
        level.levelTitle = title;
        List<HelpLevel> sections = new List<HelpLevel>();
        HelpCategory mainSection = new HelpCategory(level, sections);

        if (allIcons.Count>0) 
        {
            mainSection.info.levelIcon = GetSpriteByName ("help_icon_1");
        }
        Debug.Log("sections in " + mainSection.name + " = " + mainSection.sections.Count);

        return mainSection;
    }

    void AddATitle(string title, HelpCategory category, HelpLevel.TYPE type, bool subSections,
        string text, float height)
    {
        Sprite icon = null;

        if (allIcons.Count > 0)
        {
            icon = GetSpriteByName("help_icon_1");
        }

        HelpLevel l = new HelpLevel(title, null, icon, type, subSections, null, text, height);
        category.sections.Add(l);
    }

    void AddATitle(string title, HelpCategory category, HelpLevel.TYPE type, bool subSections,
        string video)
    {
        Sprite icon = null;
        if (allIcons.Count > 0)
        {
            icon = GetSpriteByName("help_icon_1");
        }

        HelpLevel l = new HelpLevel(title, null, icon, type, subSections, video, "", 0);
        category.sections.Add(l);
    }

    List<Level> GameSpecificHelp()
    {
        List<Level> steps = new List<Level>();

        foreach (EventData data in info)
        {
            Level le = new Level();
            le.levelIcon = data.EventImage;
            le.levelTitle = data.EventName + ": " + data.EventTitle;
            steps.Add(le);
        }

        return steps;
    }

    public List<Level> GetExtraHelp()
    {
        return extraHelp;
    }

    public void SaveCategory (int index)
    {
        mCategory = categories[index];
        titles = mCategory.sections;
        mCategoryIndex = index;
    }

    public void SaveTitle (int index)
    {
        mTitle = titles[index];
        mLevelIndex = index;
    }

    public void SaveStepHelp (int index)
    {
        mStepIndex = index;

        HelpLevel step = new HelpLevel(info[index].EventName, null, null, HelpLevel.TYPE.GAME, false, null, stepContent[index], stepHeight[index]);
        mTitle = step;
    }

    public HelpCategory GetCategory()
    {
        return mCategory;
    }

    public HelpLevel GetLevel()
    {
        return mTitle;
    }

    public int GetStepIndex()
    {
        return mStepIndex;
    }

    public int GetCategoryIndex()
    {
        return mCategoryIndex;
    }

    public int GetLevelIndex()
    {
        return mLevelIndex;
    }

    public List<HelpLevel> GetTitles()
    {
        return titles;
    }

    void StepSpecificHelp()
    {
        stepContent.Clear();
        stepHeight.Clear();
        //Step 1
        stepHeight.Add(1300);
        stepContent.Add("Our breathing has been automatic, since the first gasp of air we took at our moment of birth into this world. So why now, are we suggesting it’s something that needs to be found? "+
            "\n\nBecause breath is the life force that moves through us unceasingly, it’s one of the most amazing parts of life we take for granted. Have you ever really noticed how often you breathe, how deeply you breathe or where the initial breath begins?" +
            "\n\nSome of us breathe from the belly, others from the chest, still others almost from the throat. This is what is meant by ‘finding your breath.”" +
            "\n\nFinding your breath is a lesson in concentration. We begin by focusing on the ins and outs of the breath and gain an intimate awareness of our breathing patterns. Once we can identify the how " +
            "and where we can overlay that focus into the rest of our lives. Everyday occurrences we may have taken for granted, become more important." +
            "\n\nAs we develop our awareness of how each breath affects us, we may develop a deeper awareness of how certain people affect us." +
            "\n\n<b>Notice Where Your Breath Comes From</b>\nIn everyday life, whenever we are fearful or stressed, our breath rate tends to quicken and each breath we take gets more shallow. Each quick intake of air coming from somewhere in the chest " +
            "area. Yet, watch a person deeply asleep and notice how the breath comes from a far deeper place. Not only are the inhalations stronger, but each breath lasts longer than they do when they are awake" +
            "\n\nHow can the rhythm of breath affect our lives? What if, in a stressful situation, the first thing you did was slow your breathing – take deeper, longer breaths? Would it be possible to reduce the stress of that situation?" +
            "\n\nCan you change your breathing pattern, make it more like a person who is asleep to relieve the stress? Scholars, teachers and meditators say ‘yes’." +
            "\n\nNext we focus on where the breath goes and what sensations we feel as it reaches its destination.  As we use a laser like focus on the breath, we notice the usual pattern of distracting thoughts that " +
            "regularly cross our minds, begin to dissipate. We don’t have time to worry about the past or the future. The only thing important is the steady in and out of our breath in this moment right here and right now." +
            "\n\n<b>What You Will Learn</b>\nAchieving that focus is the reason for meditation. In this step, these exercises are meant to " +
            "observe your breath and become more aware of it. With deep, rhythmic breaths, we go within, leave the outside world behind, and stay totally present in the here and now." +
            "\n\nBreath is the entryway to your highest, most profound self – the remarkable being you were destined to be.");
        //Step 2
        stepHeight.Add(1000);
        stepContent.Add("Here’s a situation most of us have experienced. You are driving down the highway, listening to music or just thinking about nothing in particular and suddenly realize you’ve passed your turnoff. Your body has been sitting in the driver’s seat, but who and what was driving?"+
            "\n\nThis body/mind separation is a challenge we have in many parts of our lives, whether we are playing with our children but distracted by our phones, talking to a colleague or loved one while thinking we’re hungry or tired, or driving aimlessly down a road." +
            "\n\n<b>How to Integrate the Mind and the Body</b>\nMindfulness is the key to having all parts of the self in one place. If your mind is thinking about what happened in the past, it’s not in the present. If you are worrying about what will happen tomorrow, you are not being present." +
            "\n\nThe easiest way to practice mindfulness is with something that is always there – your breath. Breathing is usually taken for granted – an automatic body response. What if you decided to be" +
            " fully present for each breath, thinking about that precious life force entering your body, cleansing everything in its path, then taking what isn’t necessary out of your body?" +
            "\n\nBreath is the miracle of life. Be mindful of this amazing life force and consciously celebrate its existence.\n\n<b>What You Will Learn</b>\n" +
            "Learning mindful breathing has far reaching benefits. Once you discover the joys and benefits of mindfulness, you can take the practice out into other parts of your life." +
            "\n\nMindfulness can build stronger relationships. Your loved ones know that when you are with them, you are with them body, mind and soul." +
            "\n\nMindfulness makes the world a more beautiful place. When you are taking in the beauty of a sunset, colors seem brighter because your whole being is involved. " +
            "\n\nPracticing mindful breathing keeps you out of the past and the future, and allows you to be in the present, the only time, the only place that matters.");
        //Step 3
        stepHeight.Add(900);
        stepContent.Add("There’s a great paradox in meditation. On one hand you’re instructed to sit up straight, shut out all distractions, breathe rhythmically and stop any thoughts going through your mind. On the other" +
            " hand, you’re taught that meditation is about letting go. How can you follow conflicting instructions about time, place, posture and mind for meditation and let go?" +
            "\n\nThe answer is you cannot, nor is it necessary. Proper meditation is whatever takes you, personally, into the silence and gives you peace.\n\n<b>Meditation is Letting Go</b>\n" +
            "When you come to the main purpose of meditation, it’s really about letting go – giving up control of whatever is going on. Control is a central issue in many of our lives, a struggle most of us go through." +
            "\n\nYou trust people at work, yet you want things done right, so it’s difficult to delegate and let go. You want your children to be safe, but want to allow them to grow, so control becomes a delicate balance. We even try to control things we have no control over." +
            "\n\nTry as you may, it’s almost impossible to change someone’s mind, how they feel or their beliefs. You can only control your reaction to outside forces." +
            "In meditation, you learn to release control, to let go, and watch what happens. \n\n<b>What You Will Learn</b>\n" +
            "Another way to think about ‘letting go’ is to ‘allow everything to be as it is.’ Letting go of control in any part of life means acceptance of the way things are." +
            "\n\nYou’ll find that when you are in deep, restful meditation, you no longer judge right or wrong, no longer wonder if you are doing it right, no longer try to push away any thoughts – simply accept the time, the place, the meditation and the meditator. " +
            "There you will discover the deep pool of stillness meditation offers you, an exploration not to attain something, not to change anything, but simply to be.");
        //Step 4
        stepHeight.Add(1000);
        stepContent.Add("Your ancestors, in part, gave you certain hair and eye color, body type and may have passed down some innate talent in your DNA, but is that who you are? " +
            "\n\nA common, true phrase is that we stand on the shoulders of our ancestors. But the choices we make in life, the paths we take, the people we associate with, the beliefs and attitudes we grow up with – those are the details that create our destiny." +
            "\n\n<b>Who Are You?</b>\nWhen we pose this question to ourselves, or someone else asks us, we usually answer not with who we are, our true identity, but with what we do or what roles we play in life. In truth, whatever label you or others have placed upon you is not who you are." +
            "\n\nTry asking yourself, “Who am I”, and notice what jumps out at you – a father, a teacher, a spouse, etc. Those are roles you play in life but none of them reveal your true identity." +
            "\n\nThe only way to discover who you are is to go into the depths of your inner being and, in the quiet, ask again.\n\nIn “Discovering Uncharted Territory”, this guided meditation from Gangaji leads you on an inner journey to help you awaken to your true nature." +
            "\n\n<b>What You Will Learn</b>\nThis step shows you how to slow down, step away from the outside world and go within. Once you are in the quiet space of meditation, you can repeat the question “Who am I?” " +
            "\n\nA variety of answers may, at first, drift across your mind. Allow them to appear, then release them into the ether. Ask the question again and a deeper layer of answers will arise." +
            "\n\nAs you allow the obvious answers to appear, then drift away, deep insights will eventually come to you, definitions of who you are, what your true role in life is and how you can express the person you were meant to be." +
            "\n\nGoing within is the place where your true identity will be revealed.");
        //Step 5
        stepHeight.Add(1000);
        stepContent.Add("When it comes to beliefs, there are two schools of thought. One says, “When I see it I’ll believe it,” while the other says, “Whatever I believe, that’s what I see.”" +
            "\n\nIn spiritual teachings you are taught that what is going on inside of your mind is out-pictured in your world. In other words, your beliefs create your reality." +
            "\n\n<b>What Do You Believe?</b>\nBeliefs are usually long held opinions we learned from parents, teachers and other important people in our lives. " +
            "As children, we are taught certain things which we seldom question. The stove will burn our hand. The light socket will hurt. Strangers may be dangerous." +
            "\n\nThey become so embedded in our consciousness, we seldom think about them until we notice patterns repeated in our lives. We’ve had conflict with co-workers in several jobs, our relationships don’t end well, and we “catch” cold every winter." +
            "\n\nWhen we choose to delve into our deepest nature, discover exactly what we believe, they can surface into the light and we can then re-choose our personal reality." +
            "\n\n<b>What You Will Learn</b>\nAs we move into the meditative portion of this lesson, we’re asked to turn our attention to a place in our lives where we may feel stuck. With careful guidance we’re led to discover where, exactly, that sense resides in our body, and to identify what it feels like. " +
            "\n\nThe next step is to ask the questions, “What would I have to believe to experience this discomfort?”, “Where did this belief come from,”… “Is this true?” We seldom question whether a long held belief is a habit or a truth. " +
            "\n\nOnce the answer comes, we get to choose whether to keep the belief or release it, freeing us from the consequences of holding onto that which no longer serves, creating different outcomes than in " +
            "the past. In this lesson, you get to re-choose whether you want to continue living in a reality based on long held beliefs, or move into a new, brighter reality that defines the truth of your awakened self.");
        //Step 6
        stepHeight.Add(900);
        stepContent.Add("<b>The Past is Over</b>\nMindfulness – being present in each moment – is the key to happiness and fulfillment.If your mind is filled with regrets, hurts, offenses, resentments or disappointments from the past, you carry those negative emotions into every situation that arises." +
            "\n\nA great opportunity in business comes up but you remember how disappointed you were when a similar opportunity fell through in the past. You meet a new prospective partner but you still carry the hurt from the last failed relationship. It’s as if you are carrying a heavy sack filled with long held pain. Are you willing to release the past?" +
            "\n\n<b>The Future is a Clean Slate</b>\nWhen you hold onto past hurts, the pain keeps you from trusting the future will be different.You may begin to worry about your ability to succeed, to win, to feel good.Held fears from the past become worries about the future." +
            "\n\nInstead of taking one day at a time, living in the present, your past becomes a projection for your future. Yet, most of what we worry about never happens. Worry is totally unproductive. It just keeps you from the thrill of moving into the next beautiful moment of life." +
            "\n\nAre you willing to release your worries?" +
            "\n\n<b>What You Will Learn</b>\nIn this lesson you may discover past hurts and resentments you thought you let go of long ago and know that you don’t have to carry them into today.Since the past cannot be changed, why drag it into your present ? " +
            "\n\nYou will also see that worry can weigh heavily on you as well. As you learn to let go, release the past and the future, you will begin to feel lighter, as if a heavy weight has been lifted from you. "+
            " Each moment becomes new, exciting and you move into your newfound lightness with excitement, knowing that you can create whatever you choose – today.");
        //Step 7
        stepHeight.Add(1100);
        stepContent.Add("When the driver in the adjoining lane cuts in front of you, or when a server in a restaurant acts rudely, it’s not only easy to get annoyed, it’s part of being human. The challenge comes when you continue to hold onto anger or annoyance. It’s not only unhealthy for you, it doesn’t take into consideration the situation of the other person." +
            "\n\nWhat if the server woke up with a migraine headache she can’t shake? Perhaps the driver was on his way to the hospital to see someone gravely ill. What if, instead of reacting negatively to some people, we could change their behaviors by extending kindness and understanding." +
            "\n\n<b>We All Have Challenges</b>\nWe see only the outer layer of people, what they want to show the world.You no doubt have had times when you felt depressed, sad or anxious, yet went on with your day, not sharing those feelings.Instead, you may have tried to put up a brave front. " +
            "\n\nAll of humanity experiences positive and negative feelings. And sometimes anger or hostility is a cry for help. Without revealing your pain, you would love for someone to understand." +
            "\n\nAs soon as you become willing to help heal one person’s pain, you begin to heal yourself." +
            "\n\n<b>What You Will Learn</b>\nIn this lesson, through a Buddhist meditation called Tonglen, you will practice compassion or empathy by recalling someone close to you, perhaps someone who has caused you pain in the past." +
            "\n\nFrom the deep peace of your meditation you will begin to take away their personal pain and send out love and forgiveness. As difficult as this may be, when you are willing to view the world from another’s eyes and help alleviate their distress, you will be the one who is rewarded the most. Your life will change for the better. " +
            "\n\nYou’ll find yourself feeling lighter, more peaceful, while moving through the world with a softened, more compassionate heart.");
        //Step 8
        stepHeight.Add(1200);
        stepContent.Add("The word “forgiveness” is one that carries great emotion. Most religions teach that we are to forgive – no matter what – yet it is a directive most difficult to follow. When we feel someone has wronged us, waves of feelings well up inside." +
            "\n\nSome of us refuse to forgive, others find forgiveness easy. Many people believe that forgiving someone else is letting them off the hook, when, in fact, it is simply a practice that frees us to move on with our lives." +
            "\n\n<b>Forgive Whom?</b>\nThere are cases, of course, where certain random acts affect our lives and we have no control over the occurrence.In other cases, hurt feelings come from missteps or misunderstanding in relationships.Yet, any feeling of resentment, regardless of what happened, seldom hurts the perpetrator." +
            "\n\nThe bad feelings fester inside of you and can create stress, depression and deep resentment. You are the one suffering the consequences of non-forgiveness. And when you explore the situation more, you may find that the one you are resenting is yourself. “How could I let myself get roped into that?” “Why did I subject myself to him/her – again?”" +
            "\n\nWhen you think of forgiveness as letting your own self off the hook, the picture appears different." +
            "\n\n<b>What You Will Learn</b>\nWe are all human and as such, make mistakes.It’s important to allow yourself to make mistakes, learn from them and move on." +
            "\n\nIn this lesson you will discover whom it is you are really unwilling to forgive and hold the intention to, at least, move towards forgiveness. In the silence you will ask yourself where this resentment has located itself in your body, what it feels like and what you believe will happen if you let it go." +
            "\n\nFinally, you will end the lesson with the affirmation, “Please allow me to make mistakes.” Once you realize everyone makes mistakes, you can more easily accept your own errors and those of others." +
            "\n\nWith forgiveness comes a newfound freedom and lightness of being you may never have had before.");
        //Step 9
        stepHeight.Add(900);
        stepContent.Add("Compassion is less a concept that needs to be learned, than it is an innate quality to be revealed. It’s the warm feeling you get when you’re watching a young wife greet her soldier husband back from a long deployment. " +
            "\n\nYou sense how difficult the separation was while you feel the love of their reconnection. This is an example of compassion." +
            "\n\n<b>Compassion Servers Others and You as Well</b>\nAll religious traditions teach love, forgiveness and compassion.The practice of these qualities touches both sides.Forgiving others can affect the perpetrator, but more so the one who forgives." +
            "\n\nLove creates good feelings in the lover and the beloved. The thought and act of compassion is similar. That’s why the Dalai Lama said, “If you want others to be happy, practice compassion. If you want to be happy, practice compassion.”" +
            "\n\n<b>What You Will Learn</b>\nYou cannot give to others what you cannot give to yourself.To live a compassionate life, you first must be patient, loving and compassionate with you. " +
            "\n\nIn this step we will meditate and affirm self-acceptance, inviting the thought, “May I be loved and may I be at peace,” moving outward to “May I be loving and may I be at peace.”" +
            "\n\nIt’s easier to feel and extend compassion to people who treat you with love and kindness. However, selective compassion is not living a compassionate life. " +
            "\n\nA compassionate life is one that is all-inclusive - for everyone, everywhere, all the time. As with all behaviors and attitudes, compassion begins from within." +
            "\n\nThis is the value of contemplation, affirmation and meditation.");
        //Step 10
        stepHeight.Add(800);
        stepContent.Add("Philosophers and cosmologists tell us we are not only living within the universe, but we are the universe, made of the same stardust that created all the galaxies. How much more valuable would you feel in the great scheme of things if you not only believed that premise, but really felt and knew it to be true?" +
            "\n\nIn the busy, material world of good and bad, big and little, it’s difficult to step up to our true nature. But in the quiet space of meditation, that’s where we can reveal who and what we really are." +
            "\n\n<b>Your Brain on Meditation</b>\nIn order not to injure ourselves as we move through the day, we have to be aware of where our bodies stop and objects begin.But in the inner world of meditation, there is no need for separation." +
            "\n\nBrain scans of meditators have shown the part of the brain that is aware of separation, shuts off. No longer do you feel separated from any part of the universe. Instead, the powerful nature that is you, is revealed." +
            "\n\n<b>What You Will Learn</b>\nFor this lesson, you will relax enough to go beyond the mind that separates, into the space that feels like an integral part of everything.You may visualize yourself walking through a forest, feeling as if you are one of the trees, or flowers or gentle waterfalls that pours into a cool pond." +
            "\n\nYou not only feel the power of the cascading water, but know that beneath the surface is an abiding calm." +
            "\n\nThe beauty of meditation is that with enough practice, you become adept at calling forth that calm, that peace and that feeling of being not in the universe, but of it, anytime and anywhere.");
    }
}
