using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace SS.Data.Error
{
    using Elements;
    
    public class SSGroupErrorData 
    {
        public SSErrorData ErrorData { get; set; }
        public List<SSGroup> Groups { get; set; }

        public SSGroupErrorData()
        {
            ErrorData = new SSErrorData();
            Groups = new List<SSGroup>();
        }
    }
}