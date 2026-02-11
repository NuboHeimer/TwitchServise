//============================================================================
//   Module:       Twitch Service
//   Author:       NuboHeimer (https://live.vkvideo.ru/nuboheimer)
//   Email:        nuboheimer@yandex.ru
//   Help:         https://t.me/nuboheimersb/30
//   Version:      1.1.0
//============================================================================

using System;
using System.Collections.Generic;
using System.Threading;
using System; //дублирование нужно, чтобы при find refs стримербот добавил System.Core.dll, необходимый для HashSet. Иначе его надо добавлять руками. Я не знаю, почему это так работает.

public class CPHInline
{
    public void Init()
    {
        if (CPH.GetGlobalVar<HashSet<string>>("twitchTodaysViewers", true) == null)
        {
            CPH.SetGlobalVar("twitchTodaysViewers", new HashSet<string>(), true);
            CPH.LogDebug("[TwitchService]: Global variable twitchTodaysViewers created.");
        }

        if (CPH.GetGlobalVar<HashSet<string>>("twitchPreviousPresentViewers", true) == null)
        {
            CPH.SetGlobalVar("twitchPreviousPresentViewers", new HashSet<string>(), true);
            CPH.LogDebug("[TwitchService]: Global variable twitchPreviousPresentViewers created.");
        }
        CPH.LogInfo("[TwitchService]: initialized.");
    }

    public bool GetNewViewers()
    {
        return TwitchServiceInternal.GetNewViewers(CPH);
    }

    public bool GetInOutViewers()
    {
        return TwitchServiceInternal.GetInOutViewers(CPH);
    }

    public bool AddFirstWordViewer()
    {
        return TwitchServiceInternal.AddFirstWordViewer(CPH);
    }

    public bool ClearTodaysViewers()
    {
        CPH.SetGlobalVar("twitchTodaysViewers", new HashSet<string>(), true);
        return true;
    }

    public bool ClearPreviousPresentViewers()
    {
        CPH.SetGlobalVar("twitchPreviousPresentViewers", new HashSet<string>(), true);
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

public class TwitchServiceInternal
{
    private const string LogPrefix = "[TwitchService]: ";

    public static bool GetNewViewers(IInlineInvokeProxy CPH)
    {
        try
        {
            if (!CPH.TryGetArg("users", out object usersObj))
            {
                CPH.LogDebug($"{LogPrefix} [GetNewViewers] Viewers not found (argument 'users' is missing).");
                return false;
            }

            var currentViewers = usersObj as List<Dictionary<string, object>>;

            if (currentViewers == null || currentViewers.Count == 0)
            {
                CPH.LogDebug($"{LogPrefix} [GetNewViewers] Viewers list is empty.");
                return false;
            }

            var twitchTodaysViewers = CPH.GetGlobalVar<HashSet<string>>("twitchTodaysViewers", true);

            CPH.LogDebug($"{LogPrefix} [GetNewViewers] Try to get new viewers.");
            foreach (var viewer in currentViewers)
            {
                var userName = viewer["userName"].ToString();
                if (!twitchTodaysViewers.Contains(userName))
                {
                    CreateViewerEvent(CPH, userName, "Обнаружен(а) впервые на текущей трансляции.");
                    twitchTodaysViewers.Add(userName);
                }
            }
            CPH.SetGlobalVar("twitchTodaysViewers", twitchTodaysViewers, true);
            return true;
        }
        catch (Exception e)
        {
            CPH.LogError($"{LogPrefix} [GetNewViewers] Error, {e.Message}");
            return false;
        }
    }

    public static bool GetInOutViewers(IInlineInvokeProxy CPH)
    {
        try
        {
            var twitchPreviousPresentViewers = CPH.GetGlobalVar<HashSet<string>>("twitchPreviousPresentViewers", true);
            var twitchTodaysViewers = CPH.GetGlobalVar<HashSet<string>>("twitchTodaysViewers", true);

            if (!CPH.TryGetArg("users", out object usersObj))
            {
                CPH.LogDebug($"{LogPrefix} [GetInOutViewers] Viewers not found (argument 'users' is missing).");
                return false;
            }

            var currentViewers = usersObj as List<Dictionary<string, object>>;
            if (currentViewers == null || currentViewers.Count == 0)
            {
                CPH.LogDebug($"{LogPrefix} [GetInOutViewers] Viewers list is empty.");
                return false;
            }

            CPH.LogDebug($"{LogPrefix} [GetInOutViewers] Try to get viewers.");

            var currentViewersNames = new HashSet<string>();

            foreach (var viewer in currentViewers)
            {
                currentViewersNames.Add(viewer["userName"].ToString());
            }

            var tempViewersNamesList = new HashSet<string>();
            var currentViewersNamesForSaving = new HashSet<string>(currentViewersNames);

            foreach (var viewerName in currentViewersNames)
            {
                if (!twitchTodaysViewers.Contains(viewerName))
                {
                    CreateViewerEvent(CPH, viewerName, "Обнаружен(а) впервые на текущей трансляции.");
                    twitchTodaysViewers.Add(viewerName);
                    tempViewersNamesList.Add(viewerName);
                }
            }
            CPH.SetGlobalVar("twitchTodaysViewers", twitchTodaysViewers, true);

            foreach (var viewerName in tempViewersNamesList)
            {
                currentViewersNames.Remove(viewerName);
            }

            foreach (var viewerName in currentViewersNames)
            {
                if (!twitchPreviousPresentViewers.Contains(viewerName))
                {
                    CreateViewerEvent(CPH, viewerName, "Обнаружен(а) в списке зрителей.");
                }
            }

            foreach (var previousViewer in twitchPreviousPresentViewers)
            {
                if (!currentViewersNames.Contains(previousViewer))
                {
                    CreateViewerEvent(CPH, previousViewer, "Пропал(а) из списка зрителей.");
                }
            }

            CPH.SetGlobalVar("twitchPreviousPresentViewers", currentViewersNamesForSaving, true);

            return true;
        }
        catch (Exception e)
        {
            CPH.LogError($"{LogPrefix} [GetInOutViewers] Error, {e.Message}");
            return false;
        }
    }

    public static bool AddFirstWordViewer(IInlineInvokeProxy CPH)
    {
        try
        {
            if (!CPH.TryGetArg("userName", out string userName) || string.IsNullOrEmpty(userName))
            {
                CPH.LogError($"{LogPrefix} [AddFirstWordViewer] Argument 'userName' is missing or empty.");
                return false;
            }
            var twitchTodaysViewers = CPH.GetGlobalVar<HashSet<string>>("twitchTodaysViewers", true);

            twitchTodaysViewers.Add(userName);
            CPH.LogDebug($"{LogPrefix} [AddFirstWordViewer] User added to todays and previous present viewers: {userName}");

            CPH.SetGlobalVar("twitchTodaysViewers", twitchTodaysViewers, true);

            return true;
        }
        catch (Exception e)
        {
            CPH.LogError($"{LogPrefix} [AddFirstWordViewer] Error, {e.Message}");
            return false;
        }
    }
    private static void CreateViewerEvent(IInlineInvokeProxy CPH, string viewerName, string eventType)
    {
        CPH.SetArgument("service", "Twitch");
        CPH.SetArgument("title", viewerName);
        CPH.SetArgument("message", eventType);
        CPH.ExecuteMethod("MiniChat Method Collection", "CreateCustomEvent");
        Thread.Sleep(200); // если убрать задержку, то при большом количестве одновременно зашедших зрителей некоторые оповещения могут не отобразиться.
    }
}