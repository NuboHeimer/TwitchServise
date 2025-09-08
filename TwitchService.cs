///----------------------------------------------------------------------------
///   Module:       Twitch Service
///   Author:       NuboHeimer (https://live.vkvideo.ru/nuboheimer)
///   Email:        nuboheimer@yandex.ru
///   Help:         https://t.me/nuboheimersb/30
///   Version:      1.2.0
///----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;

public class CPHInline
{
    public void Init()
    {
        CPH.LogInfo("[TwitchService] initialized.");
        if (CPH.GetGlobalVar<List<string>>("twitchTodaysViewers", true) == null)
        {
            CPH.SetGlobalVar("twitchTodaysViewers", new List<string>(), true);
            CPH.LogInfo("[TwitchService] Global variable twitchTodaysViewers created.");
        }

        if (CPH.GetGlobalVar<List<string>>("twitchPreviousPresentViewers", true) == null)
        {
            CPH.SetGlobalVar("twitchPreviousPresentViewers", new List<string>(), true);
            CPH.LogInfo("[TwitchService] Global variable twitchPreviousPresentViewers created.");
        }
    }

    public bool GetNewViewers()
    {
        List<string> twitchTodaysViewers = CPH.GetGlobalVar<List<string>>("twitchTodaysViewers", true);
        try
        {
            CPH.LogInfo("[TwitchService] try to get new viewers");
            var currentViewers = (List<Dictionary<string, object>>)args["users"];
            if (currentViewers.Count == 0)
            {
                CPH.LogInfo("[TwitchService] Viewers not found.");
                return false;
            }

            foreach (var viewer in currentViewers)
            {
                if (!twitchTodaysViewers.Contains(viewer["userName"].ToString()))
                {
                    twitchTodaysViewers.Add(viewer["userName"].ToString());
                    CPH.SetArgument("service", "Twitch");
                    CPH.SetArgument("title", "Новый зритель");
                    CPH.SetArgument("message", viewer["userName"].ToString());
                    CPH.ExecuteMethod("MiniChat Method Collection", "CreateCustomEvent");
                    Thread.Sleep(200); // если убрать задержку, то при большом количестве одновременно зашедших зрителей некоторые оповещения могут не отобразиться.
                }
            }
            CPH.SetGlobalVar("twitchTodaysViewers", twitchTodaysViewers, true);
        }
        catch (Exception e)
        {
            CPH.LogError("[TwitchService] Some error was happend.");
        }

        return true;
    }

    public bool AddFirstWordViewer()
    {
        List<string> twitchTodaysViewers = CPH.GetGlobalVar<List<string>>("twitchTodaysViewers", true);
        twitchTodaysViewers.Add(args["userName"].ToString());
        CPH.SetGlobalVar("twitchTodaysViewers", twitchTodaysViewers, true);
        return true;
    }
    public bool ClearTodaysViewers()
    {
        CPH.SetGlobalVar("twitchTodaysViewers", new List<string>(), true);
        return true;
    }

    public bool RemoveTwitchTodaysViewersVariable()
    {
        CPH.UnsetGlobalVar("twitch_todays_viewers", true);
        return true;
    }

    public bool RemoveTwitchLastViewersNameListVariable()
    {
        CPH.UnsetGlobalVar("twitchLastViewersNameList", true);
        return true;
    }
}