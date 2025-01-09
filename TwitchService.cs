///----------------------------------------------------------------------------
///   Module:       Twitch Service
///   Author:       NuboHeimer (https://live.vkvideo.ru/nuboheimer)
///   Email:        nuboheimer@yandex.ru
///   Telegram:     t.me/nuboheimer
///   Version:      1.1.0
///----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Threading;

public class CPHInline
{
    public void Init()
    {
        CPH.LogInfo("TwitchService started.");
        if (CPH.GetGlobalVar<List<string>>("twitch_todays_viewers", true) == null)
        {
            CPH.SetGlobalVar("twitch_todays_viewers", new List<string>(), true);
            CPH.LogInfo("[TwitchService] Global variable twitch_todays_viewers created.");
        }
    }

    public bool ClearTodaysViewers()
    {
        CPH.SetGlobalVar("twitch_todays_viewers", new List<string>(), true);
        return true;
    }

    public bool GetNewViewers()
    {
        List<string> twitch_todays_viewers = CPH.GetGlobalVar<List<string>>("twitch_todays_viewers", true);
        try
        {
            CPH.LogInfo("[TwitchService] try to get new viewers");
            var viewers = (List<Dictionary<string, object>>)args["users"];
            if (viewers.Count == 0)
            {
                CPH.LogInfo("[TwitchService] Viewers not found.");
                return true;
            }

            for (int i = 0; i < viewers.Count; i++)
            {
                if (!twitch_todays_viewers.Contains(viewers[i]["userName"].ToString()))
                {
                    twitch_todays_viewers.Add(viewers[i]["userName"].ToString());
                    CPH.SetGlobalVar("twitch_todays_viewers", twitch_todays_viewers, true);
                    CPH.SetArgument("service", "Twitch");
                    CPH.SetArgument("title", "Новый зритель");
                    CPH.SetArgument("message", viewers[i]["userName"].ToString());
                    CPH.ExecuteMethod("MiniChat Method Collection", "CreateCustomEvent");
                    Thread.Sleep(200); // если убрать задержку, то при большом количестве одновременно зашедших зрителей некоторые оповещения могут не отобразиться.
                }
            }
        }
        catch (Exception e)
        {
            CPH.LogError("[TwitchService] Some error was happend.");
        }

        return true;
    }

    public bool AddFirstWordViewer()
    {
        List<string> twitch_todays_viewers = CPH.GetGlobalVar<List<string>>("twitch_todays_viewers", true);
        twitch_todays_viewers.Add(args["userName"].ToString());
        CPH.SetGlobalVar("twitch_todays_viewers", twitch_todays_viewers, true);
        return true;
    }

    public bool GetPresentViewersNameList()
    {
        try
        {
            CPH.LogInfo("[TwitchService] try to get viewers");
            var viewers = (List<Dictionary<string, object>>)args["users"];
            if (viewers.Count == 0)
            {
                CPH.LogInfo("[TwitchService] Viewers not found.");
                return true;
            }

            List<string> lastTwitchViewersNameList = new List<string>();
            for (int i = 0; i < viewers.Count; i++)
            {
                lastTwitchViewersNameList.Add(viewers[i]["userName"].ToString());
                CPH.SetGlobalVar("lastTwitchViewersNameList", lastTwitchViewersNameList, true);
            }
        }
        catch (Exception e)
        {
            CPH.LogError("[TwitchService] Some error was happend.");
        }

        return true;
    }
}