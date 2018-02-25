using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace Draven
{
    public class NullPropertiesConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var jsonExample = new Dictionary<string, object>();
            foreach (var prop in obj.GetType().GetProperties())
            {
                //check if decorated with ScriptIgnore attribute
                bool ignoreProp = prop.IsDefined(typeof(ScriptIgnoreAttribute), true);

                var value = prop.GetValue(obj, BindingFlags.Public, null, null, null);
                if (value != null && !ignoreProp)
                    jsonExample.Add(prop.Name, value);
            }

            return jsonExample;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return GetType().Assembly.GetTypes(); }
        }
    }

    class AuthServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, Task<object>> _responderMethod;

        public AuthServer(Func<HttpListenerRequest, Task<object>> method, params string[] prefixes)
        {
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            if (method == null)
                throw new ArgumentException("method");

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);

            _responderMethod = method;
        }

        public void Start()
        {
            _listener.Start();

            ThreadPool.QueueUserWorkItem((o) =>
            {
                while (_listener.IsListening)
                {
                    try
                    {
                        HttpListenerContext context = _listener.GetContext();
                        context.Response.Headers[HttpResponseHeader.ContentType] = SetContentType(context.Request.RawUrl);

                        Task<object> responseTask = _responderMethod(context.Request);
                        Task.WaitAny(responseTask);
                        object response = responseTask.Result;
                        byte[] buf;

                        if (response is string[])
                        {
                            string[] temp = response as string[];
                            response = temp[0];
                            context.Response.StatusCode = Convert.ToInt32(temp[1]);
                            buf = Encoding.UTF8.GetBytes((string)response);
                        }
                        else if (response is string)
                            buf = Encoding.UTF8.GetBytes((string)response);
                        else
                            buf = (byte[])response;
                        
                        context.Response.ContentLength64 = buf.Length;
                        using (var Stream = context.Response.OutputStream)
                        {
                            Stream.Write(buf, 0, buf.Length);
                        }
                    }
                    catch (Exception e) { Console.WriteLine(e.Message); }
                }
            });
        }

        public static string SetContentType(string RawUrl)
        {
            if (RawUrl.EndsWith(".png"))
                return "image/png";
            else if (RawUrl.EndsWith(".jpg"))
                return "image/jpeg";
            else if (RawUrl.EndsWith(".css"))
                return "text/css";
            else if (RawUrl.EndsWith(".js"))
                return "text/javascript";
            else if (RawUrl.EndsWith("/") || RawUrl.EndsWith(".html"))
                return "text/html";

            return "application/json";
        }
        public static string GetRequestPostData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                return null;
            }
            using (System.IO.Stream body = request.InputStream) // here we have data
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(body, request.ContentEncoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public class AuthTokenResponse
        {

            public string access_token { get; set; }

            public string TokenType { get; set; }

            public int ExpiresIn { get; set; }

            public string RefreshToken { get; set; }

            public string IdToken { get; set; }

            public string Scope { get; set; }

            public string GasToken { get; set; }
        }

        public class LQResponse
        {
            public int rate { get; set; }
            public string reason { get; set; }
            public string status { get; set; }
            public LQt lqt { get; set; }
            public int delay { get; set; }
            public InGameCreds inGameCredentials { get; set; }
            public string user { get; set; }
            public long? banned { get; set; }
        }

        public class InGameCreds
        {
            public bool inGame { get; set; }
            public long summonerId { get; set; }
            public string serverIp { get; set; }
            public int serverPort { get; set; }
            public string encryptionKey { get; set; }
            public string handshakeToken { get; set; }
            public string user { get; set; }
        }

        public class LQt
        {
            public long account_id { get; set; }
            public string account_name { get; set; }
            public string other { get; set; }
            public string fingerprint { get; set; }
            public string signature { get; set; }
            public long timestamp { get; set; }
            public string uuid { get; set; }
            public string ip { get; set; }
            public string partner_token { get; set; }
            public string resource { get; set; }
        }

        public static async Task<object> HandleWebServ(HttpListenerRequest request)
        {
            Console.WriteLine("[LOG] Get Request for: " + request.RawUrl);
            if (request.RawUrl == ("/authenticate"))
            {
                string[] response = { "", "" };
                try
                {
                    string payloader = GetRequestPostData(request);
                    string username = payloader.Split(',')[0].Split('=')[1];
                    string password = payloader.Split(',')[1].Split('=')[1];
                    if (DatabaseManager.checkAccount(username, password))
                    {
                        Dictionary<string, string> data = DatabaseManager.getAccountData(username, password);
                        if (data["banned"] == "1")
                        {
                            Console.WriteLine("Login from: " + username + " but banned.");
                            response[1] = "403";
                            LQResponse lqRes = new LQResponse()
                            {
                                rate = 0,
                                reason = "account_banned",
                                status = "FAILED",
                                delay = 5000,
                                banned = 7357299742000
                            };
                            var serializer = new JavaScriptSerializer();
                            serializer.RegisterConverters(new JavaScriptConverter[] { new NullPropertiesConverter() });
                            response[0] = serializer.Serialize(lqRes);
                        }
                        else
                        {
                            Console.WriteLine("Successful Login from: " + username);
                            response[1] = "200";
                            LQResponse lqRes = new LQResponse()
                            {
                                rate = 325,
                                reason = "login_rate",
                                status = "LOGIN",
                                lqt = new LQt
                                {
                                    account_id = Convert.ToInt64(data["id"]),
                                    account_name = username,
                                    other = "lMnzmeRQXfuSO7-E8gyIC2njt28aZDiQ3WE2EC55o3m2tnfadjIvDzAmEB4oRyoZfApyQ+HBMbpu5yvY2Wl2XzHKrjp0W-V4",
                                    fingerprint = "7823ae2957be6d04243330e71143cf98",
                                    signature = "HDNlMYyci+N1GvZlYDqV/38Qco9BIEP+xI3K/trtHqIdv/53XaUR5l03pIQV0K6jiq/XulOYySRRbknr1rq7qMRgTsbTw/quzZ+wTFS9Kz7qIC1Ekkt7+BUsr6C+rBmUXwH137xP9BoxNrCM/pFgxVdDDg38YlzLNKvxK3Q1kE0=",
                                    timestamp = Convert.ToInt64((TimeZoneInfo.ConvertTimeToUtc(DateTime.UtcNow.Date) -
           new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds),
                                    partner_token = "eyJraWQiOiJzMSIsImFsZyI6IlJTMjU2In0.eyJzdWIiOiI5NzYwNjdlYi05YmEzLTVlZTgtOWI5ZC0wOGJhNzdjZTZlODIiLCJzY3AiOlsib3BlbmlkIl0sImNsbSI6WyJvcGVuaWQiLCJyZ25fTkExIl0sImRhdCI6eyJ1IjoyNDU1MTUxOTYsInIiOiJOQTEifSwiaXNzIjoiaHR0cHM6XC9cL2F1dGgucmlvdGdhbWVzLmNvbSIsImV4cCI6MTUxOTU3OTkwOSwiaWF0IjoxNTE5NTc5MzA5LCJqdGkiOiJsaDRDaC1zWVpqayIsImNpZCI6ImxvbCJ9.PSNqUFQ7kGBIfgxnhYAYCuT0HZ7J7CDhH4XMTeK5CVZBZSHVkihZyljksNYALehilf_S0h56k_GPr5IfeAKhRysgmndcNBDtPALK6ttmvN3ikO83Swb3PjgaDpA1yJUNAVnIN7lcElxoLe_B2W_pYjKoRb3IWQIVSASFHx-zOoI",
                                    uuid = "32b79186-bb8a-4f5c-8701-34e94078d2ba",
                                    ip = "127.0.0.1",

                                },
                                delay = 5000,
                                inGameCredentials = new InGameCreds() {
                                    user = username,
                                    inGame = true,
                                    summonerId = 1
                                }
                            };
                            var serializer = new JavaScriptSerializer();
                            serializer.RegisterConverters(new JavaScriptConverter[] { new NullPropertiesConverter() });
                            response[0] = serializer.Serialize(lqRes);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid Login from: " + username);
                        response[1] = "200";
                        LQResponse lqRes = new LQResponse()
                        {
                            rate = 0,
                            reason = "invalid_credentials",
                            status = "FAILED",
                            delay = 5000
                        };
                        var serializer = new JavaScriptSerializer();
                        serializer.RegisterConverters(new JavaScriptConverter[] { new NullPropertiesConverter() });
                        response[0] = serializer.Serialize(lqRes);
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
                return response;
            }
            else if (request.RawUrl == ("/token"))
            {
                string[] response = { "", "" };
                AuthTokenResponse atr = new AuthTokenResponse();
                try
                {
                    using (System.IO.Stream body = request.InputStream) // here we have data
                    {
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(body, request.ContentEncoding))
                        {
                            string payloader = Uri.UnescapeDataString(reader.ReadToEnd());
                            atr.access_token = "TUzZDcxYWQxZmYwNTU0ZTg2M2MyMDk5ZmUyZWI2ZQ";
                            atr.ExpiresIn = 90000;
                            atr.TokenType = "bearer";
                            atr.Scope = null;

                            response[1] = "200";
                            response[0] = new JavaScriptSerializer().Serialize(atr); ;
                        }
                    }
                } catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return response;
            }
            else if (request.RawUrl.StartsWith("/api"))
            {
                return await HandleAPI(request);
            }
            else
            {

                string ReadURL = request.RawUrl;
                if (ReadURL == "/")
                    ReadURL = "/index.html";
                if (ReadURL == "/favicon.ico")
                    return "";

                string ContentType = AuthServer.SetContentType(request.RawUrl);
                string RequestedFile = ReadURL.Split('/').Last();

#if !FILESYSTEM
                /*using (var db = new LiteEngine("poro.dat"))
                {
                    var file = db.FileStorage.FindById(RequestedFile);

                    if (file == null)
                        return "404";

                    var stream = file.OpenRead();

                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        byte[] bytes = memoryStream.ToArray();
                        if (ContentType.StartsWith("image"))
                        {
                            return bytes;
                        }
                        else
                        {
                            return Encoding.Default.GetString(bytes);
                        }
                    }
                }*/
                return "<html><head><meta charset=\"utf-8\"><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"><title>LoL Lobby</title><base target=\"_blank\"><link rel=\"stylesheet\" href=\"https://lolstatic-a.akamaihd.net/frontpage/apps/prod/lol_client/de_DE/18f69d3ef970a03a43c86181a3d620f12ac208f6/assets/css/lk3.css\"><script type=\"text/javascript\">var childInterface = {};var spectateDataURL;window.childSandboxBridge = childInterface;if (typeof window.parentSandboxBridge !== \'undefined\') {window.parentSandboxBridge.loaded();window.location.clientAssetPath = window.parentSandboxBridge.clientAssetPath;}spectateDataURL = window.parentSandboxBridge.featuredGamesURL;var locale = \'de_DE\';var versionedAssetPath = \'https://lolstatic-a.akamaihd.net/frontpage/apps/prod/lol_client/de_DE/18f69d3ef970a03a43c86181a3d620f12ac208f6/assets\';var templateVersion = \'18f69d3ef970a03a43c86181a3d620f12ac208f6\';var pagesBasePath = \'https://lolstatic-a.akamaihd.net/frontpage/apps/prod/lol_client/de_DE/18f69d3ef970a03a43c86181a3d620f12ac208f6\';var assetMagickPath = \'https://am-a.akamaihd.net\';/*@TODO: construct templateReferenceBase on the server side, and pass it along with the contextso that the build process can own the definition of how to reference templates.*/var templateReferenceBase = \'lol_client/\' + locale + \'/\' + templateVersion;/*set a default for assetMagickPath, so we don\'t break staging before Harbinger changes get depolyed.*/if (assetMagickPath === \'\') {assetMagickPath = \'https://am-a.akamaihd.net\';}(function() {var rs = document.createElement(\'script\');rs.type = \'text/javascript\';rs.setAttribute(\'data-main\', \'https://lolstatic-a.akamaihd.net/frontpage/apps/prod/lol_client/de_DE/18f69d3ef970a03a43c86181a3d620f12ac208f6/assets/js/main-require.js\');rs.src = window.location.clientAssetPath + \'/htmlTemplates/js/require-2.1.11.min.js\';document.getElementsByTagName(\'head\')[0].appendChild(rs);})();</script><script type=\"text/javascript\">var clientLocale;var clientRegion;var summonerInfo;var gasToken;var accountId;if (typeof window.parentSandboxBridge !== \'undefined\'){clientLocale = window.parentSandboxBridge.locale || \'unknown\';clientRegion = window.parentSandboxBridge.region;if (typeof window.parentSandboxBridge.getSummonerInfo === \'function\') {summonerInfo = window.parentSandboxBridge.getSummonerInfo();gasToken = JSON.parse(summonerInfo.summonerGasToken);accountId = gasToken.pvpnet_account_id;}}window.pCfg = {appname: \'lol_client\',meta: {locale: clientLocale}};if (typeof accountId !== \'undefined\') {pCfg.account = {locale: clientLocale,region: clientRegion,accountId: accountId};}</script></head><body id=\"frontpage\" class=\"i18n-de_DE landing-oembeds\" data-rodeo-concurrency=\"false\" data-lasso-endpoint=\"https://oembed.leagueoflegends.com/oembed\"><div class=\"cbox cbox-r-client\"><div class=\"gsc-fill margin-small\"><div class=\"gsc gsc-fill gsc-gutter-small\"><div class=\"gst w-2-3 h-2-3\"><lasso-embed url=\"http://news-oembed.leagueoflegends.com/v1/euw/de/news/landing-page/uuid/90cf3c12-f005-454e-a8ea-5b3f553a46a8?viewMode=card-tier-1\"></lasso-embed></div><div class=\"gst w-1-3 h-1-3 l-2-3\"><div class=\"gsc gsc-fill gsc-gutter-small\"><div class=\"gst w-1-2 h-1-1\"><div class=\"ct-wr ct-t-store ct-s-card tier-3\" data-ping-meta=\"cardTier=3|cardType=store\"><div class=\"ct-bd\"><img class=\"store-image-portrait\" data-client-image-src=\"/images/champions/Lissandra_3.jpg\" /></div><div class=\"gsc-fill\"><div class=\"overlay pos-bottom\"><h2 class=\"ct-title stacktext\">Programm Lissandra</h2><div class=\"item-cost\"><span class=\"cost-rp\">1350</span></div></div></div><a class=\"gsc-fill action store-unlock\" data-air-navigate-json=\"{&quot;type&quot;:&quot;store&quot;, &quot;relativeUrl&quot;:&quot;/store/tabs/view/skins&quot;, &quot;queryString&quot;:&quot;showItemId=championsskin_127003&quot; }\" data-analytics-event=\"store:unlock\"></a></div></div><div class=\"gst w-1-2 h-1-1 l-1-2\"><div class=\"ct-wr ct-t-store ct-s-card tier-3\" data-ping-meta=\"cardTier=3|cardType=store\"><div class=\"ct-bd\"><img class=\"store-image-portrait\" data-client-image-src=\"/images/champions/Soraka_6.jpg\" /></div><div class=\"gsc-fill\"><div class=\"overlay pos-bottom\"><h2 class=\"ct-title stacktext\">Programm Soraka</h2><div class=\"item-cost\"><span class=\"cost-rp\">1350</span></div></div></div><a class=\"gsc-fill action store-unlock\" data-air-navigate-json=\"{&quot;type&quot;:&quot;store&quot;, &quot;relativeUrl&quot;:&quot;/store/tabs/view/skins&quot;, &quot;queryString&quot;:&quot;showItemId=championsskin_16006&quot; }\" data-analytics-event=\"store:unlock\"></a></div></div></div></div><div class=\"gst w-1-3 h-1-3 l-2-3 t-1-3\"><lasso-embed url=\"http://news-oembed.leagueoflegends.com/v1/euw/de/news/landing-page/uuid/44f06393-f567-4481-aea5-9e46d1d533a5?viewMode=card-tier-2\"></lasso-embed></div><div class=\"gst w-1-3 h-1-3 t-2-3\"><lasso-embed url=\"http://news-oembed.leagueoflegends.com/v1/euw/de/news/landing-page/uuid/75dd4c82-e3d1-4211-9689-95edd9c1cf2d?viewMode=card-tier-2\"></lasso-embed></div><div class=\"gst w-1-3 h-1-3 l-1-3 t-2-3\" data-player-survey=\"50\"><lasso-embed url=\"http://news-oembed.leagueoflegends.com/v1/euw/de/news/landing-page/uuid/d93fe2ef-de21-4251-a523-56f46d3c1e0f?viewMode=card-tier-2\"></lasso-embed></div><div class=\"gst w-1-3 h-1-3 l-2-3 t-2-3\"><lasso-embed url=\"http://news-oembed.leagueoflegends.com/v1/euw/de/news/landing-page/uuid/68dcc063-c318-446e-b747-5e68a78d000f?viewMode=card-tier-2\"></lasso-embed></div></div></div></div><script type=\"text/javascript\" src=\"https://lolstatic-a.akamaihd.net/ping/ping-0.1.238.min.js\"></script><script type=\"text/javascript\">(function(){window.addEventListener(\'error\', errorHandler);window.addEventListener(\"load\", function() {var elements = document.getElementsByTagName(\'link\');for (var i = 0; i < elements.length; i++) {var element = elements[i];if (element.rel == \'stylesheet\') {var elementRules = element.sheet.rules;if (elementRules && elementRules.length == 0) {notifyPageError(\'STYLESHEET\', element.href);}}}});function getTargetErrorDescription(element) {if (element.nodeName == \"SCRIPT\") {return element.src;}else {var estr = \"\";for (var p in element) {if (element.hasOwnProperty(p)) {estr += \" | \" + p + \": \" + element[p];}}return estr;}}function errorHandler (err) {notifyPageError(err.target.nodeName, getTargetErrorDescription(err.target));}window.notifyPageError = function notifyPageError(type, message) {/*alert(\'Error: type:\' + type + \' message: \' + message);*/window.ping(\'error\', {\'meta.error_type\': type,\'meta.error_message\': message});}})();</script><script type=\"text/javascript\" src=\"https://lolstatic-a.akamaihd.net/lassojs/0.1.4/lasso.js\"></script><script>(function(i,s,o,g,r,a,m){i[\'GoogleAnalyticsObject\']=r;i[r]=i[r]||function(){(i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)})(window,document,\'script\',\'//www.google-analytics.com/analytics.js\',\'ga\');ga(\'create\', \'UA-5859958-26\', \'leagueoflegends.com\');ga(\'send\', \'pageview\');</script></body></html>";
#endif

#if FILESYSTEM
                //Uncomment to create poro.dat
                /*var x = File.OpenRead(FileURL);
                using (var db = new LiteEngine("poro.dat"))
                {
                    var file = db.FileStorage.FindById(RequestedFile);

                    if (file == null)
                    {
                        db.FileStorage.Upload(RequestedFile, x);
                    }
                }*/

                string FileURL = string.Format("app/web{0}", ReadURL);

                if (ContentType.StartsWith("image"))
                {
                    return File.ReadAllBytes(FileURL);
                }
                else
                {
                    return File.ReadAllText(FileURL);
                }
#endif
            }
        }

        public static async Task<object> HandleAPI(HttpListenerRequest request)
        {
            if (request.RawUrl.StartsWith("/api/users"))
            {
                using (System.IO.Stream body = request.InputStream) // here we have data
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(body, request.ContentEncoding))
                    {
                        Console.WriteLine(reader.ReadToEnd());
                    }
                }
                return null;
            }
            else if (request.RawUrl.StartsWith("/api/register"))
            {
                //if (request.QueryString == null && request.QueryString.Count != 4)
                    return "400";

                /*_users.AddUser(new User
                {
                    Username = request.QueryString["Username"],
                    Password = request.QueryString["Password"],
                    Region = request.QueryString["Region"],
                    SummonerName = request.QueryString["Username"]
                });
                return JsonConvert.SerializeObject(_users.GetUserList());*/
            }
            else if (request.RawUrl.StartsWith("/api/delete"))
            {
                //if (request.QueryString == null && request.QueryString.Count != 2)
                    return "400";

                /*User u = _users.GetUser(request.QueryString["Username"], request.QueryString["Region"]);
                _users.RemoveUser(u);*/

                //return JsonConvert.SerializeObject(_users.GetUserList());
            }
            else if (request.RawUrl.StartsWith("/api/regions"))
            {
                //return JsonConvert.SerializeObject(Shards.GetStatus());
                return "400";
            }
            else if (request.RawUrl.StartsWith("/api/login"))
            {
                if (request.QueryString == null && request.QueryString.Count != 2)
                    return "400";

                string Username = request.QueryString["Username"];
                string Region = request.QueryString["Region"];

                Console.WriteLine("Username: " + Username + " Region: " + Region);
                return await Task.FromResult<string>(DateTime.Now.DayOfWeek.ToString());
                /*var ShardList = Shards.GetInstances<BaseShard>();
                BaseShard shard = null;
                foreach (BaseShard s in ShardList)
                    if (s.Name == Region)
                        shard = s;

                User user = _users.GetUser(Username, Region);
                ForwardPlayer player = new ForwardPlayer(user, shard, _context);
                bool Connected = await player.Connect();

                foreach (RtmpClient client in _server._clients)
                {
                    StoreAccountBalanceNotification notification = new StoreAccountBalanceNotification
                    {
                        IP = player._packet.IpBalance,
                        RP = player._packet.RpBalance
                    };

                    client.InvokeDestReceive("cn-1", "cn-1", "messagingDestination", notification);
                }

                _forwarder.Assign(player);

                Console.WriteLine(string.Format("[LOG] Forwarding to {0} ({1})", Username, Region));*/

                //return JsonConvert.SerializeObject("OK");
            }
            else if (request.RawUrl.StartsWith("/api/check"))
            {
                return JsonConvert.SerializeObject("Not logged in");
            }
            else
            {
                return "404";
            }
        }

    }
}
