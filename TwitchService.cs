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
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;


public class TwitchUsersResponse
{
    public List<TwitchUser> data { get; set; }
}

public class TwitchUser
{
    public string id { get; set; }
    public string login { get; set; }
    public string display_name { get; set; }
}

public class TwitchSubscriptionsResponse
{
    public List<TwitchSubscription> data { get; set; }
    public TwitchPagination pagination { get; set; }
}

public class TwitchSubscription
{
    public string broadcaster_id { get; set; }
    public string broadcaster_login { get; set; }
    public string broadcaster_name { get; set; }
    public string user_id { get; set; }
    public string user_login { get; set; }
    public string user_name { get; set; }
    public string tier { get; set; }
    public bool is_gift { get; set; }
}

public class TwitchPagination
{
    public string cursor { get; set; }
}

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

    public bool GetPaidSubscribers()
    {
        return TwitchServiceInternal.GetPaidSubscribers(CPH);
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
                CPH.LogDebug($"{LogPrefix}[GetNewViewers] Viewers not found (argument 'users' is missing).");
                return false;
            }

            var currentViewers = usersObj as List<Dictionary<string, object>>;

            if (currentViewers == null || currentViewers.Count == 0)
            {
                CPH.LogDebug($"{LogPrefix}[GetNewViewers] Viewers list is empty.");
                return false;
            }

            var twitchTodaysViewers = CPH.GetGlobalVar<HashSet<string>>("twitchTodaysViewers", true);

            CPH.LogDebug($"{LogPrefix}[GetNewViewers] Try to get new viewers.");
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
            CPH.LogError($"{LogPrefix}[GetNewViewers] Error, {e.Message}");
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
                CPH.LogDebug($"{LogPrefix}[GetInOutViewers] Viewers not found (argument 'users' is missing).");
                return false;
            }

            var currentViewers = usersObj as List<Dictionary<string, object>>;
            if (currentViewers == null || currentViewers.Count == 0)
            {
                CPH.LogDebug($"{LogPrefix}[GetInOutViewers] Viewers list is empty.");
                return false;
            }

            CPH.LogDebug($"{LogPrefix}[GetInOutViewers] Try to get viewers.");

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
            CPH.LogError($"{LogPrefix}[GetInOutViewers] Error, {e.Message}");
            return false;
        }
    }

    public static bool AddFirstWordViewer(IInlineInvokeProxy CPH)
    {
        try
        {
            if (!CPH.TryGetArg("userName", out string userName) || string.IsNullOrEmpty(userName))
            {
                CPH.LogError($"{LogPrefix}[AddFirstWordViewer] Argument 'userName' is missing or empty.");
                return false;
            }
            var twitchTodaysViewers = CPH.GetGlobalVar<HashSet<string>>("twitchTodaysViewers", true);

            twitchTodaysViewers.Add(userName);
            CPH.LogDebug($"{LogPrefix}[AddFirstWordViewer] User added to todays viewers: {userName}");

            CPH.SetGlobalVar("twitchTodaysViewers", twitchTodaysViewers, true);

            return true;
        }
        catch (Exception e)
        {
            CPH.LogError($"{LogPrefix}[AddFirstWordViewer] Error, {e.Message}");
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

    public static bool GetPaidSubscribers(IInlineInvokeProxy CPH)
    {
        try
        {
            var clientId = (string)CPH.TwitchClientId;
            var oauthToken = (string)CPH.TwitchOAuthToken;

            if (string.IsNullOrWhiteSpace(clientId))
            {
                CPH.LogError($"{LogPrefix}[GetPaidSubscribers] Twitch ClientId is empty. Check Streamer.bot Twitch connection.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(oauthToken))
            {
                CPH.LogError($"{LogPrefix}[GetPaidSubscribers] Twitch OAuth token is empty. Check Streamer.bot Twitch connection.");
                return false;
            }

            oauthToken = NormalizeBearerToken(oauthToken);

            string broadcasterId;
            string broadcasterLogin;
            string broadcasterDisplayName;

            if (!TryGetBroadcasterInfo(CPH, clientId, oauthToken, out broadcasterId, out broadcasterLogin, out broadcasterDisplayName))
            {
                CPH.LogError($"{LogPrefix}[GetPaidSubscribers] Failed to resolve broadcaster info.");
                return false;
            }

            var detailedSubs = GetAllSubscriptionsDetailed(CPH, clientId, oauthToken, broadcasterId, broadcasterLogin, broadcasterDisplayName);
            var subscribers = new List<Dictionary<string, object>>();

            foreach (var sub in detailedSubs)
            {
                if (!sub.TryGetValue("user_login", out object loginObj))
                    continue;

                var login = loginObj as string;
                if (string.IsNullOrEmpty(login))
                    continue;

                var dict = new Dictionary<string, object>
                {
                    { "userName", login },
                    { "userId", sub.ContainsKey("user_id") ? sub["user_id"] as string : null },
                    { "displayName", sub.ContainsKey("user_name") ? sub["user_name"] as string : null },
                    { "tier", sub.ContainsKey("tier") ? sub["tier"] as string : null },
                    { "isGift", sub.ContainsKey("is_gift") && sub["is_gift"] is bool b && b }
                };

                subscribers.Add(dict);
            }

            CPH.SetArgument("twitchPaidSubscribers", subscribers);

            CPH.LogInfo($"{LogPrefix}[GetPaidSubscribers] Loaded paid subscribers (argument 'twitchPaidSubscribers'): {subscribers.Count}");
            return true;
        }
        catch (WebException wex)
        {
            var details = TryReadWebExceptionBody(wex);
            CPH.LogError($"{LogPrefix}[GetPaidSubscribers] HTTP error: {wex.Message}{(string.IsNullOrEmpty(details) ? "" : $", body: {details}")}");
            return false;
        }
        catch (Exception e)
        {
            CPH.LogError($"{LogPrefix}[GetPaidSubscribers] Error, {e.Message}");
            return false;
        }
    }

    private static bool TryGetBroadcasterInfo(IInlineInvokeProxy CPH, string clientId, string bearerToken, out string id, out string login, out string displayName)
    {
        id = null;
        login = null;
        displayName = null;

        var url = "https://api.twitch.tv/helix/users";
        var json = HttpGet(url, clientId, bearerToken);

        CPH.LogDebug($"{LogPrefix}[GetPaidSubscribers] /helix/users raw response: {json}");

        var users = JsonConvert.DeserializeObject<TwitchUsersResponse>(json);
        if (users?.data == null || users.data.Count == 0)
        {
            CPH.LogError($"{LogPrefix}[GetPaidSubscribers] /users response does not contain broadcaster data.");
            return false;
        }

        var me = users.data[0];
        id = me.id;
        login = me.login;
        displayName = me.display_name;

        if (string.IsNullOrEmpty(id))
        {
            CPH.LogError($"{LogPrefix}[GetPaidSubscribers] /users response has empty 'id'.");
            return false;
        }

        return true;
    }

    private static List<Dictionary<string, object>> GetAllSubscriptionsDetailed(IInlineInvokeProxy CPH, string clientId, string bearerToken, string broadcasterId, string broadcasterLogin, string broadcasterDisplayName)
    {
        var all = new List<Dictionary<string, object>>();
        string cursor = null;

        while (true)
        {
            var url = $"https://api.twitch.tv/helix/subscriptions?broadcaster_id={Uri.EscapeDataString(broadcasterId)}&first=100";
            if (!string.IsNullOrEmpty(cursor))
                url += $"&after={Uri.EscapeDataString(cursor)}";

            var json = HttpGet(url, clientId, bearerToken);

            CPH.LogDebug($"{LogPrefix}[GetPaidSubscribers] /helix/subscriptions raw response (page): {json}");

            var page = JsonConvert.DeserializeObject<TwitchSubscriptionsResponse>(json);
            if (page?.data != null)
            {
                foreach (var sub in page.data)
                {
                    if (!string.IsNullOrEmpty(broadcasterLogin) &&
                        !string.IsNullOrEmpty(sub.user_login) &&
                        sub.user_login.Equals(broadcasterLogin, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    if (!string.IsNullOrEmpty(broadcasterDisplayName) &&
                        !string.IsNullOrEmpty(sub.user_name) &&
                        sub.user_name.Equals(broadcasterDisplayName, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var dict = new Dictionary<string, object>();
                    if (!string.IsNullOrEmpty(sub.user_id)) dict["user_id"] = sub.user_id;
                    if (!string.IsNullOrEmpty(sub.user_login)) dict["user_login"] = sub.user_login;
                    if (!string.IsNullOrEmpty(sub.user_name)) dict["user_name"] = sub.user_name;
                    if (!string.IsNullOrEmpty(sub.tier)) dict["tier"] = sub.tier;
                    dict["is_gift"] = sub.is_gift;

                    all.Add(dict);
                }
            }

            cursor = page?.pagination?.cursor;
            if (string.IsNullOrEmpty(cursor))
                break;
        }

        return all;
    }

    private static string NormalizeBearerToken(string token)
    {
        token = token.Trim();
        if (token.StartsWith("oauth:", StringComparison.OrdinalIgnoreCase))
            token = token.Substring("oauth:".Length);
        return token;
    }

    private static string HttpGet(string url, string clientId, string bearerToken)
    {
        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        request.Accept = "application/json";
        request.Headers["Client-Id"] = clientId;
        request.Headers["Authorization"] = $"Bearer {bearerToken}";

        using (var response = (HttpWebResponse)request.GetResponse())
        using (var stream = response.GetResponseStream())
        using (var reader = new StreamReader(stream, Encoding.UTF8))
        {
            return reader.ReadToEnd();
        }
    }

    private static string TryReadWebExceptionBody(WebException wex)
    {
        try
        {
            var resp = wex.Response as HttpWebResponse;
            if (resp == null)
                return null;

            using (var stream = resp.GetResponseStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
        catch
        {
            return null;
        }
    }
}