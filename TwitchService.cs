///----------------------------------------------------------------------------
///   Module:       Twitch Service
///   Author:       NuboHeimer (https://live.vkvideo.ru/nuboheimer)
///   Email:        nuboheimer@yandex.ru
///   Telegram:     t.me/nuboheimer
///   Version:      1.2.0
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

        if (CPH.GetGlobalVar<List<string>>("twitchLastViewersNameList", true) == null)
        {
            CPH.SetGlobalVar("twitchLastViewersNameList", new List<string>(), true);
            CPH.LogInfo("[TwitchService] Global variable twitchLastViewersNameList created.");
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
            var currentViewers = (List<Dictionary<string, object>>)args["users"];
            if (currentViewers.Count == 0)
            {
                CPH.LogInfo("[TwitchService] Viewers not found.");
                return false;
            }

            foreach(var viewer in currentViewers)
            {
                if (!twitch_todays_viewers.Contains(viewer["userName"].ToString()))
                {
                    twitch_todays_viewers.Add(viewer["userName"].ToString());
                    CPH.SetGlobalVar("twitch_todays_viewers", twitch_todays_viewers, true);
                    CPH.SetArgument("service", "Twitch");
                    CPH.SetArgument("title", "Новый зритель");
                    CPH.SetArgument("message", viewer["userName"].ToString());
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
            var currentViewers = (List<Dictionary<string, object>>)args["users"];
            if (currentViewers.Count == 0)
            {
                CPH.LogInfo("[TwitchService] Viewers not found.");
                return false;
            }

            List<string> twitchLastViewersNameList = new List<string>();

            foreach (var viewer in currentViewers)
            {
                twitchLastViewersNameList.Add(viewer["userName"].ToString().ToLower());
            }

            CPH.SetGlobalVar("twitchLastViewersNameList", twitchLastViewersNameList, false);
        }
        catch (Exception e)
        {
            CPH.LogError("[TwitchService] Some error was happend.");
        }

        return true;
    }

        public bool ClearPresentViewersNameList()
    {
        CPH.SetGlobalVar("twitchLastViewersNameList", new List<string>(), true);
        return true;
    }

}