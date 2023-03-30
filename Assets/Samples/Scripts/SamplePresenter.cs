namespace UuIiView.Sample
{
    public class SamplePresenter : UuIiView.UIBasePresenter
    {
        UIPanel questList;
        public void StartPanel()
        {
            var uiPanels = OpenGroup("Main");
            questList = Open("QuestList");
            Open("QuestDetail");

            //questList.UpdateData();
        }

        public void SetTestData(string json)
        {
            questList.UpdateData(json);
        }

    }
}