//============================================================================
//   Module:       Twitch Service
//   Author:       NuboHeimer (https://live.vkvideo.ru/nuboheimer)
//   Email:        nuboheimer@yandex.ru
//   Help:         https://t.me/nuboheimersb/30
//   Version:      1.2.0
//============================================================================

using System;
using System.Collections.Generic;
using System.Threading;
using System; //дублирование нужно, чтобы при find refs стримербот добавил System.Core.dll, необходимый для HashSet. Иначе его надо добавлять руками. Я не знаю, почему это так работает.

// Содержит публичные методы для вызова из Streamer.bot
public class CPHInline
{
    private Logger _logger;
    private Logger Logger => _logger ??= new Logger(CPH, "[TwitchService]: ");

    // Инициализация модуля.
    // Запускается при компилляции кода.
    // Проверяет наличие глобальных переменных и создает их, если они отсутствуют.
    public void Init()
    {
        if (CPH.GetGlobalVar<HashSet<string>>("twitchTodaysViewers", true) == null)
        {
            CPH.SetGlobalVar("twitchTodaysViewers", new HashSet<string>(), true);
            Logger.Debug("Global variable twitchTodaysViewers created.");
        }

        if (CPH.GetGlobalVar<HashSet<string>>("twitchPreviousPresentViewers", true) == null)
        {
            CPH.SetGlobalVar("twitchPreviousPresentViewers", new HashSet<string>(), true);
            Logger.Debug("Global variable twitchPreviousPresentViewers created.");
        }
        Logger.Info("initialized.");
    }

    private bool ErrorHandler(Func<bool> action)
    {
        try
        {
            return action();
        }
        catch (Exception e)
        {
            Logger.Error("Error", e.Message);
            return false;
        }
    }

    public bool GetNewViewers()
    {
        return ErrorHandler(() =>
        {
            return Internal.GetNewViewers(CPH, args);
        });
    }

    public bool GetInOutViewers()
    {
        return ErrorHandler(() =>
        {
            return Internal.GetInOutViewers(CPH, args);
        });
    }

    public bool AddFirstWordViewer()
    {
        return ErrorHandler(() =>
        {
            return Internal.AddFirstWordViewer(CPH, args);
        });
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

public class Internal
{
    private const string LogPrefix = "[TwitchService]: ";

    public static bool GetNewViewers(IInlineInvokeProxy CPH, IDictionary<string, object> args)
    {
        var logger = new Logger(CPH, LogPrefix);

        if (!args.ContainsKey("users"))
        {
            logger.Debug("[GetNewViewers] Viewers not found.");
            return false;
        }

        var currentViewers = args["users"] as List<Dictionary<string, object>>;

        if (currentViewers == null || currentViewers.Count == 0)
        {
            logger.Debug("[GetNewViewers] Viewers list is empty.");
            return false;
        }

        var twitchTodaysViewers = CPH.GetGlobalVar<HashSet<string>>("twitchTodaysViewers", true);

        logger.Debug("[GetNewViewers] try to get new viewers");
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

    public static bool GetInOutViewers(IInlineInvokeProxy CPH, IDictionary<string, object> args)
    {
        var logger = new Logger(CPH, LogPrefix);
        var twitchPreviousPresentViewers = CPH.GetGlobalVar<HashSet<string>>("twitchPreviousPresentViewers", true);
        var twitchTodaysViewers = CPH.GetGlobalVar<HashSet<string>>("twitchTodaysViewers", true);

        if (!args.ContainsKey("users"))
        {
            logger.Debug("[GetInOutViewers] Viewers not found.");
            return false;
        }

        var currentViewers = args["users"] as List<Dictionary<string, object>>;
        if (currentViewers == null || currentViewers.Count == 0)
        {
            logger.Debug("[GetInOutViewers] Viewers list is empty.");
            return false;
        }

        logger.Debug("[GetInOutViewers] try to get viewers");

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

    public static bool AddFirstWordViewer(IInlineInvokeProxy CPH, IDictionary<string, object> args)
    {
        var logger = new Logger(CPH, LogPrefix);
        var twitchTodaysViewers = CPH.GetGlobalVar<HashSet<string>>("twitchTodaysViewers", true);
        string userName = args["userName"].ToString();

        twitchTodaysViewers.Add(userName);

        CPH.SetGlobalVar("twitchTodaysViewers", twitchTodaysViewers, true);

        return true;
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

public class Logger
{
    private readonly IInlineInvokeProxy _cph;
    private readonly string _prefix;

    public Logger(IInlineInvokeProxy cph, string prefix)
    {
        _cph = cph;
        _prefix = prefix;
    }

    public void Verbose(string message)
    {
        message = string.Format("{0} {1}", _prefix, message);
        _cph.LogVerbose(message);
    }

    public void Verbose(string message, params object[] additional)
    {
        string finalMessage = message;
        foreach (var line in additional)
        {
            finalMessage += ", " + line;
        }
        Verbose(finalMessage);
    }

    public void Debug(string message)
    {
        message = string.Format("{0} {1}", _prefix, message);
        _cph.LogDebug(message);
    }

    public void Debug(string message, params object[] additional)
    {
        string finalMessage = message;
        foreach (var line in additional)
        {
            finalMessage += ", " + line;
        }
        Debug(finalMessage);
    }

    public void Info(string message)
    {
        message = string.Format("{0} {1}", _prefix, message);
        _cph.LogInfo(message);
    }

    public void Info(string message, params object[] additional)
    {
        string finalMessage = message;
        foreach (var line in additional)
        {
            finalMessage += ", " + line;
        }
        Info(finalMessage);
    }

    public void Warn(string message)
    {
        message = string.Format("{0} {1}", _prefix, message);
        _cph.LogWarn(message);
    }

    public void Warn(string message, params object[] additional)
    {
        string finalMessage = message;
        foreach (var line in additional)
        {
            finalMessage += ", " + line;
        }
        Warn(finalMessage);
    }

    public void Error(string message)
    {
        message = string.Format("{0} {1}", _prefix, message);
        _cph.LogError(message);
    }

    public void Error(string message, params object[] additional)
    {
        string finalMessage = message;
        foreach (var line in additional)
        {
            finalMessage += ", " + line;
        }
        Error(finalMessage);
    }
}