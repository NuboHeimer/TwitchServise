using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

///----------------------------------------------------------------------------
///   Module:       Twitch Service
///   Author:       NuboHeimer (https://vkplay.live/nuboheimer)
///   Email:        nuboheimer@yandex.ru
///   Telegram:     t.me/nuboheimer
///   Version:      1.0.3
///----------------------------------------------------------------------------
public class CPHInline
{
    public void Init()
    {
        CPH.LogInfo("TwitchService инициализирован");
        if (CPH.GetGlobalVar<List<string>>("twitch_todays_viewers", true) == null) {
            CPH.SetGlobalVar("twitch_todays_viewers", new List<string>(), true);
            CPH.LogInfo("Создана глобальная переменная twitch_todays_viewers");
        }
    }
    public bool ClearTodayViewers()
    {
        CPH.SetGlobalVar("twitch_todays_viewers", new List<string>(), true);
        return true;
    }

    public bool GetNewViewers()
    {

        List<string> twitch_todays_viewers = CPH.GetGlobalVar<List<string>>("twitch_todays_viewers", true);
        try
        {
            CPH.LogInfo("Попытка получить нового зрителя на твиче");
            var viewers = (List<Dictionary<string, object>>)args["users"];
            if (viewers.Count == 0)
            {
                CPH.LogInfo("Зрителей в списке нет");
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
                    CPH.LogInfo("Новый зритель: " + viewers[i]["userName"].ToString());
                    Thread.Sleep(200); // если убрать задержку, то при большом количестве одновременно зашедших зрителей некоторые оповещения могут не отобразиться.
                }
            }
        }
        catch (Exception e)
        {
            CPH.LogError("Не удалось получить новых зрителей с твича");
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
}