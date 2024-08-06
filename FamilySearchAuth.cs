using JWT;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using JWT.Serializers;
using System.Diagnostics;
using AccessTokenResource;
using CurrentUserResource;
using IdentityTokenResource;
using System.Net.Http.Headers;
using System.Security.Cryptography;

public class FamilySearchAuth
{
    private Uri _baseUri;
    private string _clientID;
    private string _outState;
    private string _codeVerifier;
    private string _codeChallenge;
    private string _responseString;
    private bool _finishedProcessing;
    private string _authorizationCode;
    private HttpClient _httpClient = new HttpClient();
    private AccessTokenJSON _accessToken;

    private static FamilySearchAuth _instance = new FamilySearchAuth();

    private FamilySearchAuth()
    {

    }

    public static FamilySearchAuth Instance
    {
        get { return _instance; }
    }

    private static string GenerateRandom(uint length)
    {
        byte[] bytes = new byte[length];
        RandomNumberGenerator.Create().GetBytes(bytes);
        return EncodeNoPadding(bytes);
    }

    private static string EncodeNoPadding(byte[] buffer)
    {
        string toEncode = Convert.ToBase64String(buffer);

        toEncode = toEncode.Replace("+", "-");
        toEncode = toEncode.Replace("/", "_");
        toEncode = toEncode.Replace("=", "");

        return toEncode;
    }

    private static byte[] GenerateSha256(string inputString)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(inputString);
        SHA256 sha256 = SHA256.Create();
        return sha256.ComputeHash(bytes);
    }

    public void InitAuth()
    {
        _codeVerifier = GenerateRandom(32);
        _codeChallenge = EncodeNoPadding(GenerateSha256(_codeVerifier));

        _clientID = "";
        _outState = "237589753";

        string redirectUri = "http://127.0.0.1:5000";
        string redirectUriListener = "http://127.0.0.1:5000/";
        _baseUri = new Uri("http://ident.familysearch.org/cis-web/oauth2/v3/authorization");

        string authRequest = string.Format("{0}?client_id={1}&redirect_uri={2}&response_type=code&state={3}&code_challenge={4}&code_challenge_method=S256&scope=openid%20profile%20email%20qualifies_for_affiliate_account%20country",
        _baseUri,
        _clientID,
        redirectUri,
        _outState,
        _codeChallenge);

        HttpListener httpListener = new HttpListener();
        httpListener.Prefixes.Add(redirectUriListener);

        httpListener.Start();

        Process process = new Process();
        process.StartInfo.FileName = authRequest;
        process.StartInfo.UseShellExecute = true;
        process.Start();

        HttpListenerContext context = httpListener.GetContext();
        HttpListenerResponse response = context.Response;

        string responseString = "<HTML><HEAD><SCRIPT>window.close();</SCRIPT></HEAD><BODY></BODY></HTML>";
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();

        httpListener.Stop();

        _authorizationCode = context.Request.QueryString.Get("code");
        string inState = context.Request.QueryString.Get("state");

        if (inState == _outState)
        {
            ExchangeCodeForToken();
        }
    }

    private async void ExchangeCodeForToken()
    {
        _baseUri = new Uri("https://ident.familysearch.org/cis-web/oauth2/v3/token");

        Dictionary<string, string> formData = new Dictionary<string, string>();
        formData.Add("code", _authorizationCode);
        formData.Add("grant_type", "authorization_code");
        formData.Add("client_id", _clientID);
        formData.Add("code_verifier", _codeVerifier);

        FormUrlEncodedContent content = new FormUrlEncodedContent(formData);

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _baseUri);
        request.Content = content;
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        HttpResponseMessage response = await _httpClient.SendAsync(request);
       
        _responseString = await response.Content.ReadAsStringAsync();
        _accessToken = JsonConvert.DeserializeObject<AccessTokenJSON>(_responseString);
        DecodeJWT();
    }

    private void DecodeJWT()
    {
        IJsonSerializer serializer = new JsonNetSerializer();
        IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
        IJwtDecoder decoder = new JwtDecoder(serializer, urlEncoder);

        string jwtString = decoder.Decode(_accessToken.id_token, false);

        IdentityTokenJSON identityToken = JsonConvert.DeserializeObject<IdentityTokenJSON>(jwtString);

        if (identityToken.qualifies_for_affiliate_account == "true")
        {
            if (identityToken.gender == "M")
            {
                ElevenLabs.Instance.GetAudio($"Hello and welcome Brother {identityToken.family_name}");
            }
            else if (identityToken.gender == "F")
            {
                ElevenLabs.Instance.GetAudio($"Hello and welcome Sister {identityToken.family_name}");
            }
        }
        else if (identityToken.qualifies_for_affiliate_account == "false")
        {
            ElevenLabs.Instance.GetAudio($"Hello and welcome {identityToken.given_name}");
        }
    }

    public void GetCurrentUser()
    {
        _baseUri = new Uri("https://api.familysearch.org/");

        SendRequest("platform/users/current", "User");

        DelayTillProcessed(1);

        CurrentUserJSON currentUser = JsonConvert.DeserializeObject<CurrentUserJSON>(_responseString);

        GetAncestry(currentUser.users[0].personId);
    }

    private void GetAncestry(string userPid)
    {
        string endPoint = "platform/tree/ancestry";
        string person = "?person=" + userPid;
        string generations = "&generations=4";
        string apiRequest = $"{endPoint}{person}{generations}";

        SendRequest(apiRequest, "Ancestry");

        DelayTillProcessed(1);

        AncestryManager.Instance.SetAncestors(_responseString);
    }

    public void GetMemories()
    {
        string endPoint = "platform/tree/persons/";
        string memoryCount = "/memories?start=";
        string countString = MemoriesManager.Instance.GetMemoryCount();

        string apiRequest = $"{endPoint}{AncestryManager.Instance.GetSelectedAncestorPID()}{memoryCount}{countString}";

        SendRequest(apiRequest, "Memories");

        DelayTillProcessed(1);
    }

    private void DelayTillProcessed(int seconds)
    {
        do
        {
            Task.Delay(seconds);
        }
        while(!_finishedProcessing);
    }

    private async void SendRequest(string apiRoute, string mode)
    {
        _finishedProcessing = false;

        string requestString = $"{_baseUri}{apiRoute}";

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken.access_token}");
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestString);

        HttpResponseMessage response = await _httpClient.SendAsync(request);
       
        if (mode == "User")
        {
            _responseString = await response.Content.ReadAsStringAsync();
        }
        else if (mode == "Ancestry")
        {
            _responseString = await response.Content.ReadAsStringAsync();
        }
        else if (mode == "Memories")
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                _responseString = await response.Content.ReadAsStringAsync();
                MemoriesManager.Instance.SetMemorySet(_responseString);
                GetMemories();
            }
            else
            {
                MemoriesManager.Instance.ParseMemoryData();
            }
        }
        _finishedProcessing = true;
    }

    public void GetMemory(string memoryLocation)
    {
        GetTextMemory(memoryLocation);

        DelayTillProcessed(1);
    }

    private async void GetTextMemory(string memoryLocation)
    {
        _finishedProcessing = false;

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Clear();

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, memoryLocation);

        HttpResponseMessage response = await _httpClient.SendAsync(request);

        _responseString = await response.Content.ReadAsStringAsync();

        Console.WriteLine();
        Console.WriteLine("Text Memory:");
        Console.WriteLine();
        Console.WriteLine(_responseString);

        ElevenLabs.Instance.GetAudio(_responseString);

        Console.WriteLine();
        Console.WriteLine("Press enter to go back to the main menu");
        string userInput = Console.ReadLine();

        if (userInput == "")
        {
            _finishedProcessing = true;
        }
    }
}