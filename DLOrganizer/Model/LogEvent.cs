using System;

namespace DLOrganizer.Model
{
    class LogEvent : EventArgs
    {
        private string _source;
        private string _destination;

        public enum EventType
        {
            FileMove,
            FolderCreate,
            FileRename
        }

        public EventType Type { get; set; }
        public string Source
        {
            get
            {
                return _source ?? "";
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _source = "";
                }
                else
                {
                    _source = value;
                }
            }
        }
        public string Destination
        {
            get
            {
                return _destination ?? "";
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _source = "";
                }
                else
                {
                    _source = value;
                }
            }
        }
        public bool Success { get; set; }

        public string LogMessage
        {
            get
            {
                switch (Type)
                {
                    case EventType.FileMove:
                        return string.Format("Moved {0} to {1}", Source, Destination);
                    case EventType.FolderCreate:
                        return string.Format("Created folder {0}", Destination);
                    case EventType.FileRename:
                        return string.Format("Renamed {0} to {1}", Source, Destination);
                    default:
                        return "Unknown event found";
                }
            }
        }
    }
}
