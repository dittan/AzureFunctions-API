using System.Collections.Generic;

namespace Functions.Core.Models
{
    public class Email
    {
        private string _to;
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public string ToAddress
        {
            get => _to; set
            {
                if (value.Contains(","))
                {
                    var tos = value.Split(",");
                    var list = new List<string>();
                    foreach (var to in tos)
                    {
                        list.Add(to.Trim());
                    }
                    Tos = list;
                }
                else
                    _to = value;
            }
        }
        public string ToName { get; set; }
        public string Subject { get; set; }
        public string PlainTextContent { get; set; }
        public string HtmlContent { get; set; }
        public IEnumerable<string> Tos { get; set; }
    }
}
