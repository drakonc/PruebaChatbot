namespace PruebaChatbot.Models
{
    public class Payload
    {
        public string @object { get; set; }
        public List<Entry> entry { get; set; }
    }

    public class Entry
    {
        public string id { get; set; }
        public List<Changes> changes { get; set; }
    }

    public class Changes
    {
        public string field { get; set; }
        public Values value { get; set; }
    }

    public class Values
    {
        public string messaging_product { get; set; }
        public Metadata metadata { get; set; }
        public List<contacts> contacts { get; set; }
        public List<Messages> messages { get; set; }
    }

    public class Metadata
    {
        public string phone_number_id { get; set; }

    }

    public class contacts
    {
        public string wa_id { get; set; }
        public Profile profile { get; set; }
    }

    public class Profile
    {
        public string name { get; set; }
    }

    public class Messages
    {
        public string from { get; set; }
        public string id { get; set; }
        public string timestamp { get; set; }
        public string type { get; set; }
        public Text text { get; set; }
    }

    public class Text
    {
        public string body { get; set; }
    }
}
 