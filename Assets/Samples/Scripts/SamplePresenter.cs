namespace UuIiView.Sample
{
    public class SamplePresenter : UuIiView.UIBasePresenter
    {
        public void StartPanel()
        {
            var uiPanels = OpenGroup("Main");
            var questList = Open("QuestList");
            Open("QuestDetail");

            //questList.UpdateData();
        }

    }
}