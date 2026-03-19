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

public class TwitchSubscriberDto
{
    public string UserId { get; set; }
    public string Login { get; set; }
    public string DisplayName { get; set; }
    public string Tier { get; set; }
    public bool IsGift { get; set; }
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

                var dto = new TwitchSubscriberDto
                {
                    UserId = sub.ContainsKey("user_id") ? sub["user_id"] as string : null,
                    Login = login,
                    DisplayName = sub.ContainsKey("user_name") ? sub["user_name"] as string : null,
                    Tier = sub.ContainsKey("tier") ? sub["tier"] as string : null,
                    IsGift = sub.ContainsKey("is_gift") && sub["is_gift"] is bool b && b
                };

                var dict = new Dictionary<string, object>
                {
                    { "userName", dto.Login },
                    { "userId", dto.UserId },
                    { "displayName", dto.DisplayName },
                    { "tier", dto.Tier },
                    { "isGift", dto.IsGift }
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

        var marker = "\"id\":\"";
        var idx = json.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
        {
            CPH.LogError($"{LogPrefix}[GetPaidSubscribers] Cannot find 'id' in /users response.");
            return false;
        }
        idx += marker.Length;
        var end = json.IndexOf('\"', idx);
        if (end < 0)
        {
            CPH.LogError($"{LogPrefix}[GetPaidSubscribers] Unexpected /users response (unterminated 'id').");
            return false;
        }

        id = json.Substring(idx, end - idx);

        var loginMarker = "\"login\":\"";
        var loginIdx = json.IndexOf(loginMarker, StringComparison.OrdinalIgnoreCase);
        if (loginIdx >= 0)
        {
            loginIdx += loginMarker.Length;
            var loginEnd = json.IndexOf('\"', loginIdx);
            if (loginEnd > loginIdx)
                login = json.Substring(loginIdx, loginEnd - loginIdx);
        }

        var nameMarker = "\"display_name\":\"";
        var nameIdx = json.IndexOf(nameMarker, StringComparison.OrdinalIgnoreCase);
        if (nameIdx >= 0)
        {
            nameIdx += nameMarker.Length;
            var nameEnd = json.IndexOf('\"', nameIdx);
            if (nameEnd > nameIdx)
                displayName = json.Substring(nameIdx, nameEnd - nameIdx);
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

            ParseSubscriptionsPage(json, all, broadcasterLogin, broadcasterDisplayName);

            cursor = null;
            var paginationMarker = "\"cursor\":\"";
            var pIdx = json.IndexOf(paginationMarker, StringComparison.OrdinalIgnoreCase);
            if (pIdx >= 0)
            {
                pIdx += paginationMarker.Length;
                var pEnd = json.IndexOf('"', pIdx);
                if (pEnd > pIdx)
                    cursor = json.Substring(pIdx, pEnd - pIdx);
            }

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

    private static void ParseSubscriptionsPage(string json, List<Dictionary<string, object>> target, string broadcasterLogin, string broadcasterDisplayName)
    {
        var searchMarker = "\"broadcaster_id\":\"";
        var startIndex = 0;

        while (true)
        {
            var idx = json.IndexOf(searchMarker, startIndex, StringComparison.OrdinalIgnoreCase);
            if (idx < 0)
                break;

            var objStart = json.LastIndexOf('{', idx);
            if (objStart < 0)
                break;

            var endObj = json.IndexOf('}', idx);
            if (endObj < 0)
                break;

            var obj = json.Substring(objStart, endObj - objStart + 1);

            var userId = ExtractField(obj, "\"user_id\":\"");
            var userLogin = ExtractField(obj, "\"user_login\":\"");
            var userName = ExtractField(obj, "\"user_name\":\"");
            var tier = ExtractField(obj, "\"tier\":\"");
            var isGiftStr = ExtractField(obj, "\"is_gift\":");
            bool isGift = string.Equals(isGiftStr, "true", StringComparison.OrdinalIgnoreCase);

            if (!string.IsNullOrEmpty(broadcasterLogin) &&
                !string.IsNullOrEmpty(userLogin) &&
                userLogin.Equals(broadcasterLogin, StringComparison.OrdinalIgnoreCase))
            {
                startIndex = endObj + 1;
                continue;
            }
            if (!string.IsNullOrEmpty(broadcasterDisplayName) &&
                !string.IsNullOrEmpty(userName) &&
                userName.Equals(broadcasterDisplayName, StringComparison.OrdinalIgnoreCase))
            {
                startIndex = endObj + 1;
                continue;
            }

            var dict = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(userId)) dict["user_id"] = userId;
            if (!string.IsNullOrEmpty(userLogin)) dict["user_login"] = userLogin;
            if (!string.IsNullOrEmpty(userName)) dict["user_name"] = userName;
            if (!string.IsNullOrEmpty(tier)) dict["tier"] = tier;
            dict["is_gift"] = isGift;

            target.Add(dict);

            startIndex = endObj + 1;
        }
    }

    private static string ExtractField(string source, string marker)
    {
        var idx = source.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
            return null;

        idx += marker.Length;

        if (marker.EndsWith("\":"))
        {
            var end = source.IndexOfAny(new[] { ',', '}' }, idx);
            if (end < 0)
                end = source.Length;
            return source.Substring(idx, end - idx).Trim();
        }
        else
            var end = source.IndexOf('"', idx);
            if (end < 0)
                return null;
            return source.Substring(idx, end - idx);
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