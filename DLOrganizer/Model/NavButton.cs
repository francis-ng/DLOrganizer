using System.Windows.Input;

namespace DLOrganizer.Model
{
    public class NavButton
    {
        public string Label
        {
            get; set;
        }

        public ICommand Command
        {
            get; set;
        }

        public int PageNumber
        {
            get; set;
        }

        public NavButton(string label, ICommand command, int page)
        {
            Label = label;
            Command = command;
            PageNumber = page;
        }
    }
}
