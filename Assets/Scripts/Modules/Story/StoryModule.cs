public class StoryModule : BaseModule
{
    private StoryEventManager storyEventManager;

    protected override void OnInit()
    {
        RegisterController();
    }

    protected override void OnRelease()
    {
    }

    public void RegisterController()
    {
        storyEventManager = new StoryEventManager();
        storyEventManager.Init();

        storyEventManager.RegisterControl<BackgroundComponentControl>(StoryDefine.ComponentType.Background); //背景控制器
    }

    public void PlayTestStory()
    {
    }

    public void PlayStory(string storyId)
    {
    }
}