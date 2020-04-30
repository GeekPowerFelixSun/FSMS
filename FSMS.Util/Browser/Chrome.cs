using System;
using System.Collections.Generic;
using System.Text;

namespace FSMS.Util.Browser
{
    public class Chrome : BaseBrowser
    {
        private readonly string _agent;

        public Chrome(string agent)
        {
            _agent = agent.ToLower();
            var chrome = BrowserType.Chrome.ToString().ToLower();

            if (_agent.Contains(chrome))
            {
                var first = _agent.IndexOf(chrome);
                var cut = _agent.Substring(first + chrome.Length + 1);
                var version = cut.Substring(0, cut.IndexOf(' '));
                Version = ToVersion(version);
                Type = BrowserType.Chrome;
            }
        }
    }
}
