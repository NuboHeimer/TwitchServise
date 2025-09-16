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
using System; //дублирование нужно, что бы при find refs стримербот добавил System.Core.dll, необходимый для HashSet. Иначе его надо добавлять руками. Я не знаю, почему это так работает.

// ============================================================================
// ОСНОВНОЙ КЛАСС CPHInline
// ============================================================================

// Содержит публичные методы для вызова из Streamer.bot
public class CPHInline
{
    // Инициализация модуля.
    // Запускается при компилляции кода.
    // Проверяет наличие глобальных переменных и создает их, если они отсутствуют.
    public void Init()
    {
        CPH.LogInfo("[TwitchService] initialized.");
        if (CPH.GetGlobalVar<HashSet<string>>("twitchTodaysViewers", true) == null)
        {
            CPH.SetGlobalVar("twitchTodaysViewers", new HashSet<string>(), true);
            CPH.LogDebug("[TwitchService] Global variable twitchTodaysViewers created.");
        }

        if (CPH.GetGlobalVar<HashSet<string>>("twitchPreviousPresentViewers", true) == null)
        {
            CPH.SetGlobalVar("twitchPreviousPresentViewers", new HashSet<string>(), true);
            CPH.LogDebug("[TwitchService] Global variable twitchPreviousPresentViewers created.");
        }
    }
    
    // Получение новых зрителей
    // Получает список текущих зрителей, добавляет их в список и отправляет событие в minichat.

    public bool GetNewViewers()
    {
        HashSet<string> twitchTodaysViewers = CPH.GetGlobalVar<HashSet<string>>("twitchTodaysViewers", true);
        try
        {
            CPH.LogDebug("[TwitchService][GetNewViewers] try to get new viewers");
            List<Dictionary<string, object>> currentViewers = (List<Dictionary<string, object>>)args["users"];
            if (currentViewers.Count == 0)
            {
                CPH.LogDebug("[TwitchService][GetNewViewers] Viewers not found.");
                return false;
            }

            foreach (var viewer in currentViewers)
            {
                if (!twitchTodaysViewers.Contains(viewer["userName"].ToString()))
                {
                    CreateViewerEvent(viewer["userName"].ToString(), "Зашел сегодня на трансляцию первый раз.");
                    twitchTodaysViewers.Add(viewer["userName"].ToString());
                }
            }
            CPH.SetGlobalVar("twitchTodaysViewers", twitchTodaysViewers, true);
        }
        catch (Exception e)
        {
            CPH.LogError("[TwitchService][GetNewViewers] Some error was happened." + e.Message);
        }

        return true;
    }

    //Получение пришедших и ушедших зрителей.
    //Получает список текущих зрителей, сравнивает его с предыдущим списком и отправляет события в minichat.
    public bool GetInOutViewers()
    {
        HashSet<string> twitchPreviousPresentViewers = CPH.GetGlobalVar<HashSet<string>>("twitchPreviousPresentViewers", true);
        try
        {
            CPH.LogDebug("[TwitchService][GetInOutViewers] try to get viewers");
            List<Dictionary<string, object>> currentViewers = (List<Dictionary<string, object>>)args["users"];

            if (currentViewers.Count == 0)
            {
                CPH.LogDebug("[TwitchService][GetInOutViewers] Viewers not found.");
                return false;
            }

            HashSet<string> currentViewersNames = new HashSet<string>();

            foreach (var viewer in currentViewers)
            {
                currentViewersNames.Add(viewer["userName"].ToString());
            }

            foreach (var viewerName in currentViewersNames)
            {
                if (!twitchPreviousPresentViewers.Contains(viewerName))
                {
                    CreateViewerEvent(viewerName, "Обнаружен(а) в списке зрителей.");
                }
            }

            foreach (var previousViewer in twitchPreviousPresentViewers)
            {
                if (!currentViewersNames.Contains(previousViewer))
                {
                    CreateViewerEvent(previousViewer, "Пропал(а) из списка зрителей.");
                }
            }

            CPH.SetGlobalVar("twitchPreviousPresentViewers", currentViewersNames, true);
        }
        catch (Exception e)
        {
            CPH.LogError("[TwitchService][GetInOutViewers] Some error was happened." + e.Message);
        }

        return true;
    }

    //Добавление зрителя в списки новых и пришедших зрителей.
    public bool AddFirstWordViewer()
    {
        HashSet<string> twitchTodaysViewers = CPH.GetGlobalVar<HashSet<string>>("twitchTodaysViewers", true);
        string userName = args["userName"].ToString();

        twitchTodaysViewers.Add(userName);

        CPH.SetGlobalVar("twitchTodaysViewers", twitchTodaysViewers, true);

        return true;
    }

    //Очистка списка новых зрителей.
    public bool ClearTodaysViewers()
    {
        CPH.SetGlobalVar("twitchTodaysViewers", new HashSet<string>(), true);
        return true;
    }

    //Очистка кэшированного списка present viewers зрителей.
    public bool ClearPreviousPresentViewers()
    {
        CPH.SetGlobalVar("twitchPreviousPresentViewers", new HashSet<string>(), true);
        return true;
    }

    //Удаление глобальной переменной twitch_todays_viewers.
    public bool RemoveTwitchTodaysViewersVariable()
    {
        CPH.UnsetGlobalVar("twitch_todays_viewers", true);
        return true;
    }

    //Удаление глобальной переменной twitchLastViewersNameList.
    public bool RemoveTwitchLastViewersNameListVariable()
    {
        CPH.UnsetGlobalVar("twitchLastViewersNameList", true);
        return true;
    }

    //Создание события в minichat.
    //Создает событие в minichat с указанными параметрами.
    private void CreateViewerEvent(string viewerName, string eventType)
    {
        CPH.SetArgument("service", "Twitch");
        CPH.SetArgument("title", viewerName);
        CPH.SetArgument("message", eventType);
        CPH.ExecuteMethod("MiniChat Method Collection", "CreateCustomEvent");
        Thread.Sleep(200); // если убрать задержку, то при большом количестве одновременно зашедших зрителей некоторые оповещения могут не отобразиться.
    }
}