using System.Collections.Generic;

namespace SS.Data.Error
{
    using Elements;
    
    public class SSNodeErrorData
    {
        public SSErrorData ErrorData { get; set; }
        public List<SSNode> Nodes { get; set; }

        public SSNodeErrorData()
        {
            ErrorData = new SSErrorData();
            Nodes = new List<SSNode>();
        }
    }
}