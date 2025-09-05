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

    // TODO: добавить метод для удаления глобалки twitch_todays_viewers 
    public void Init()
    {
        CPH.LogInfo("[TwitchService] loaded.");
        if (CPH.GetGlobalVar<List<string>>("twitchTodaysViewers", true) == null)
        {
            CPH.SetGlobalVar("twitchTodaysViewers", new List<string>(), true);
            CPH.LogInfo("[TwitchService] Global variable twitchTodaysViewers created.");
        }

        if (CPH.GetGlobalVar<List<string>>("twitchLastViewersNameList", true) == null)
        {
            CPH.SetGlobalVar("twitchLastViewersNameList", new List<string>(), true);
            CPH.LogInfo("[TwitchService] Global variable twitchLastViewersNameList created.");
        }
    }

    public bool ClearTodaysViewers()
    {
        CPH.SetGlobalVar("twitchTodaysViewers", new List<string>(), true);
        return true;
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
                    CPH.SetGlobalVar("twitchTodaysViewers", twitchTodaysViewers, true);
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
        List<string> twitchTodaysViewers = CPH.GetGlobalVar<List<string>>("twitchTodaysViewers", true);
        twitchTodaysViewers.Add(args["userName"].ToString());
        CPH.SetGlobalVar("twitchTodaysViewers", twitchTodaysViewers, true);
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

    public bool GetViewersCount()
    {
        try
        {
            CPH.LogInfo("[TwitchService] try to get viewers count");
            var currentViewers = (List<Dictionary<string, object>>)args["users"];

            CPH.SetGlobalVar("twitchViewersCount", currentViewers.Count, false);
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