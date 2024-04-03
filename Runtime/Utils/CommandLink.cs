using System.Collections.Generic;
using System;

namespace UuIiView
{
    public class CommandLink
    {
        public string Id;
        public string EventName;
        public UuIiView.EventType EventType;
        public UuIiView.ActionType ActionType;
        public string PanelName;
        public Dictionary<string, string> param;
        string source = string.Empty;
        public CommandLink(string commandLink)
        {
            source = commandLink;

            var arr = commandLink.Split("/");
            PanelName = arr[0];
            EventType = (UuIiView.EventType)Enum.Parse(typeof(UuIiView.EventType), arr[1]);
            ActionType = (UuIiView.ActionType)Enum.Parse(typeof(UuIiView.ActionType), arr[2]);
            EventName = arr[3];
            Id = arr[4];
            param = new Dictionary<string, string>();
            for (int i = 5; i < arr.Length; i++)
            {
                var sep = arr[i].Split("=");
                param[sep[0]] = sep[1];
            }
        }

        public override string ToString() => source;

        public string Log()
        {
            var paramStr = "";
            foreach (var kv in param)
            {
                paramStr += $",{kv.Key}={kv.Value}";
            }
            if (paramStr.Length > 0)
                paramStr = paramStr.Substring(1);

            return ($"<color=cyan>[UuIiView] CommandLink (Id = {Id} : PanelName={PanelName} : EventName={EventName} : EventType={EventType} : ActionType={ActionType} : param=({paramStr})</color>\n{this.ToString()}");
        }
    }
}