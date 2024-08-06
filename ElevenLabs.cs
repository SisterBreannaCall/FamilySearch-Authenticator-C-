using System.Text;
using NAudio.Wave;
using Newtonsoft.Json;
using ElevenLabsResource;
using System.Net.Http.Headers;

public class ElevenLabs
{
    private Uri _baseUri;
    private string _clientID;
    private HttpClient _httpClient = new HttpClient();

    private static ElevenLabs _instance = new ElevenLabs();

    private ElevenLabs()
    {

    }

    public static ElevenLabs Instance
    {
        get { return _instance; }
    }

    public async void GetAudio(string textToSpeak)
    {
        _baseUri = new Uri("https://api.elevenlabs.io/v1/text-to-speech/TxGEqnHWrfWFTfGW9XjX");
        _clientID = "";

        ElevenLabsJSON elevenLabs = new ElevenLabsJSON();
        elevenLabs.text = textToSpeak;
        elevenLabs.model_id = "eleven_monolingual_v1";

        string jsonString = JsonConvert.SerializeObject(elevenLabs);
        HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("audio/mpeg"));

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _baseUri);
        request.Content = content;
        request.Content.Headers.Add("xi-api-key", _clientID);

        HttpResponseMessage response = await _httpClient.SendAsync(request);

        Stream audioStream = await response.Content.ReadAsStreamAsync();
        
        Mp3FileReader reader = new Mp3FileReader(audioStream);

        WaveOutEvent waveOut = new WaveOutEvent();

        waveOut.Init(reader);
        waveOut.Play();
    }
}