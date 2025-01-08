using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SnapTranslator.Services.Impl
{
    public class GoogleTranslateService : ITranslateSevice
    {
        private string _APIKey;
        private string _url;

        public GoogleTranslateService(string apiKey,
            string url)
        {
            _APIKey = apiKey;
            _url = url;
        }

        public async Task<string?> Translate(string source, string target, string text)
        {
            using (var httpClient = new HttpClient())
            {
                var form = new Dictionary<string, string>()
                {
                    {"target", target },
                    {"source", source},
                    {"q", text},
                    {"key",  _APIKey}
                };


                var content = new FormUrlEncodedContent(form);
                HttpResponseMessage response = await httpClient.PostAsync(_url, content);

                string responseString = await response.Content.ReadAsStringAsync();

                var node = JsonNode.Parse(responseString);
                var translationText = node?["data"]?["translations"]
                    ?.AsArray()?.FirstOrDefault()?["translatedText"]
                    ?.ToString();

                return translationText;
            }
        }
    }
}
