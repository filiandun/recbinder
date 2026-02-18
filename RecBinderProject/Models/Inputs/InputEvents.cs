namespace RecBinderProject.Models.Inputs
{
    public class InputEvents
    {
        public LinkedList<InputEvent> InputEventsList { get; init; }

        public InputEvents()
        {
            this.InputEventsList = new LinkedList<InputEvent>();
        }

        public void Add(InputEvent userEvent) => this.InputEventsList.AddLast(userEvent);
        public void Clear() => this.InputEventsList.Clear();
    }
}
