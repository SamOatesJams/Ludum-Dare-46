using System;
using SamOatesGames.Systems;
using UnityEngine;

public class TutorialConversationUI : SubscribableMonoBehaviour
{
    [Header("UI Components")]
    public GameObject ConversationUi;
    public TMPro.TMP_Text SpeakerNameText;
    public TMPro.TMP_Text ConversationText;
    public UnityEngine.UI.Image SpeakerImage;

    [Header("Profile Images")]
    public Sprite DrFrankensteinImage;
    public Sprite MonsterImage;

    private enum ConversationStage
    {
        Dr_1,
        Dr_2,
        Monster_1,
        Dr_3,
        Monster_2,
        Dr_4
    }

    private EventAggregator m_eventAggregator;
    private ConversationStage m_stage;

    public void Start()
    {
        m_eventAggregator = EventAggregator.GetInstance();

        m_stage = ConversationStage.Dr_1;
        SetConversation("Dr Frankenstein", DrFrankensteinImage, "I can't believe I've done it! After all these years I've finally recreated life!");

        ConversationUi.SetActive(true);
    }

    public void ContinueConversation()
    {
        switch (m_stage)
        {
            case ConversationStage.Dr_1:
                SetConversation("Dr Frankenstein", DrFrankensteinImage, "It's such a shame that the village people want to destroy you and my laboratory!");
                m_stage = ConversationStage.Dr_2;
                break;
            case ConversationStage.Dr_2:
                SetConversation("Monster", MonsterImage, "Me Sad...");
                m_stage = ConversationStage.Monster_1;
                break;
            case ConversationStage.Monster_1:
                SetConversation("Dr Frankenstein", DrFrankensteinImage, "Through the day, I will craft traps and place them using resources I've collected.");
                m_stage = ConversationStage.Dr_3;
                break;
            case ConversationStage.Dr_3:
                SetConversation("Dr Frankenstein", DrFrankensteinImage, "Then through the night, the traps will stop those pesky villagers! And maybe you could also encourage them to leave?");
                m_stage = ConversationStage.Monster_2;
                break;
            case ConversationStage.Monster_2:
                SetConversation("Monster", MonsterImage, "Me Smash!");
                m_stage = ConversationStage.Dr_4;
                break;
            case ConversationStage.Dr_4:
                ConversationUi.SetActive(false);
                m_eventAggregator.Publish(new TutorialCompleteEvent());
                break;
        }
    }

    private void SetConversation(string speakerName, Sprite image, string conversation)
    {
        SpeakerNameText.text = speakerName;
        SpeakerImage.sprite = image;
        ConversationText.text = conversation;
    }
}
