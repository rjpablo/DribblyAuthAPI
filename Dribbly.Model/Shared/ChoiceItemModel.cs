namespace Dribbly.Model.Shared
{
    public class ChoiceItemModel<TValue>
    {
        public string Text { get; set; }

        public TValue Value { get; set; }

        public string IconUrl { get; set; }
    }
}
