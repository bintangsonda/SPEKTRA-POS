using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace IDS.Web.UI.Models
{
    public class CustomDataManagerRequest
    {
        public Dictionary<string, object> Params { get; set; }
    }
}
