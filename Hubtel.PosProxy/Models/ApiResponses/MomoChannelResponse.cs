using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Responses
{
    public class MomoChannelResponse
    {
        public string ResponseCode { get; set; }
        public List<MomoChannel> Data { get; set; }
    }

    public class MomoChannel
    {
        public string ChannelName { get; set; }
        public string Country { get; set; }
        public bool Enabled { get; set; }
    }
}
