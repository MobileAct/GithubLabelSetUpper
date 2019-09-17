using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Core;
using Utf8Json;

namespace GiteaLabelSetUpper
{
    public class GiteaApi : IApi<Label>
    {
        private const string defaultGiteaHost = "https://try.gitea.io";
        private const string userAgent = nameof(GiteaLabelSetUpper);
        private const int timeOutSeconds = 60;

        private readonly HttpClient client = new HttpClient();
        private readonly string baseUrl;
        private readonly string owner;
        private readonly string repository;

        public GiteaApi(string? giteaHost, string? giteaToken, string owner, string repository)
        {
            this.client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(timeOutSeconds),
            };
            client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
            if (giteaToken is { })
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", giteaToken);
            }
            this.baseUrl = $"{giteaHost ?? defaultGiteaHost}/api/v1";
            this.owner = owner;
            this.repository = repository;
        }

        public async Task CreateLabelAsync(Label newLabel)
        {
            var request = new LabelRequest(
                newLabel.Name ?? throw new ArgumentException("name is null"),
                newLabel.Description,
                newLabel.Color ?? throw new ArgumentException("color is null")
            );
            string json = JsonSerializer.ToJsonString(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = await client.PostAsync($"{baseUrl}/repos/{owner}/{repository}/labels", data);
            checkError(httpResponseMessage);
        }

        public async Task DeleteLabelAsync(Label oldLabel)
        {
            long id = oldLabel.Id ?? throw new ArgumentException("id is null");
            HttpResponseMessage httpResponseMessage = await client.DeleteAsync($"{baseUrl}/repos/{owner}/{repository}/labels/{id}");
            checkError(httpResponseMessage);
        }

        public async Task<IReadOnlyList<Label>> GetLabelsAsync()
        {
            HttpResponseMessage httpResponseMessage = await client.GetAsync($"{baseUrl}/repos/{owner}/{repository}/labels");
            checkError(httpResponseMessage);
            string content = await httpResponseMessage.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IReadOnlyList<Label>>(content);
        }

        public async Task UpdateLabelAsync(Label oldLabel, Label newLabel)
        {
            long id = newLabel.Id ?? oldLabel.Id ?? throw new ArgumentException("id is null");
            var request = new LabelRequest(
                newLabel.Name ?? throw new ArgumentException("name is null"),
                newLabel.Description,
                newLabel.Color ?? throw new ArgumentException("color is null")
            );
            string json = JsonSerializer.ToJsonString(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponseMessage = await client.PatchAsync($"{baseUrl}/repos/{owner}/{repository}/labels/{id}", data);
            checkError(httpResponseMessage);
        }

        private void checkError(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.IsSuccessStatusCode is false)
            {
                Console.WriteLine($"{httpResponseMessage.StatusCode} {httpResponseMessage.ReasonPhrase}");
            }
        }

        public class LabelRequest
        {
            [DataMember(Name = "name")]
            public string Name { get; }

            [DataMember(Name = "description")]
            public string? Description { get; }

            [DataMember(Name = "color")]
            public string Color { get; }

            public LabelRequest(string name, string? description, string color)
            {
                Name = name;
                Description = description;
                Color = $"#{color}";
            }
        }
    }
}
