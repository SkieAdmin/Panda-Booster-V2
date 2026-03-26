using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;

namespace Panda
{
    public class ValidationResult
    {
        public bool Success { get; set; }
        public bool IsPremium { get; set; }
        public string ExpireDate { get; set; }
        public string Message { get; set; }
    }

    public class PandaKeySystem
    {
        private static readonly HttpClient _http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        private readonly string _serviceId;
        public const string BaseUrl = "https://new.pandadevelopment.net";

        public PandaKeySystem(string serviceId)
        {
            _serviceId = serviceId;
        }

        public ValidationResult Validate(string key, string hwid, bool premiumVerification = false)
        {
            try
            {
                var body = JsonConvert.SerializeObject(new
                {
                    ServiceID = _serviceId,
                    Key = key,
                    HWID = hwid
                });

                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = _http.PostAsync($"{BaseUrl}/api/v1/keys/validate", content).Result;
                var json = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ServerResponse>(json);

                bool isAuthenticated = result.Authenticated_Status == "Success";
                bool isPremium = result.Key_Premium;

                bool isValid = isAuthenticated;
                string message = result.Note ?? (isAuthenticated ? "Key validated!" : "Invalid key");

                if (premiumVerification && isAuthenticated && !isPremium)
                {
                    isValid = false;
                    message = "Premium key required";
                }

                return new ValidationResult
                {
                    Success = isValid,
                    Message = message,
                    IsPremium = isPremium,
                    ExpireDate = result.Expire_Date
                };
            }
            catch
            {
                return new ValidationResult
                {
                    Success = false,
                    Message = "Failed to connect to server",
                    IsPremium = false,
                    ExpireDate = null
                };
            }
        }

        public string GetKeyURL(string hwid)
        {
            return $"{BaseUrl}/getkey/{_serviceId}?hwid={hwid}";
        }

        private class ServerResponse
        {
            public string Authenticated_Status { get; set; }
            public string Note { get; set; }
            public string Expire_Date { get; set; }
            public bool Key_Premium { get; set; }
        }
    }

    public static class Auth
    {
        public static void LaunchSecureBrowser(string serviceId, string hwid)
        {
            var url = $"{PandaKeySystem.BaseUrl}/getkey/{serviceId}?hwid={hwid}";

            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            try
            {
                process.StartInfo.FileName = "msedge.exe";
                process.StartInfo.Arguments = $"--guest \"{url}\"";
                process.Start();
            }
            catch
            {
                process.StartInfo.FileName = url;
                process.StartInfo.Arguments = "";
                process.Start();
            }
        }

        public static bool Validate(string serviceId, string key, string hwid, bool premiumOnly = false)
        {
            var panda = new PandaKeySystem(serviceId);
            var result = panda.Validate(key, hwid, premiumVerification: premiumOnly);

            if (result.Success)
            {
                Console.WriteLine("Authenticated!");
                Console.WriteLine($"Premium: {result.IsPremium}");
                Console.WriteLine($"Expires: {result.ExpireDate ?? "Never"}");
                return true;
            }

            Console.WriteLine($"Failed: {result.Message}");
            return false;
        }
    }
}
