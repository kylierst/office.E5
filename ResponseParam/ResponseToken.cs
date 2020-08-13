using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace office365.ResponseParam
{
    public class ResponseToken
    {
        public string token_type { get; set; }
        public string scope { get; set; }
        public int expires_in { get; set; }
        public int ext_expires_in { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
    }

}
