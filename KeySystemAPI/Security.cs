using System;
using System.Net;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

namespace Panda
{
    public static class Auth
    {

        public static void LaunchSecureBrowser(string URL, string HWID, string Exploit)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = "msedge.exe";
                process.StartInfo.Arguments = $"--guest \"{URL}/getkey?service={Exploit}&hwid={HWID}\"";

                try
                {
                    process.Start();
                }
                catch
                {
                    process.StartInfo.FileName = $"{URL}/getkey?service={Exploit}&hwid={HWID}";
                    process.Start();
                }
            }
        }

        public static bool Validate(string URL, string HWID, string Exploit, string Key)
        {
            using (var WB = new WebClient() { Proxy = null })
            {
                WB.Headers.Add("User-Agent", "Auth-Client/1.0");

                var Result = "";
                var Blob = $"{Guid.NewGuid()}{Guid.NewGuid()}";

                try
                {
                    //Result = WB.DownloadString($"{URL}/validate?service={Exploit}&key={Key}&blob={Blob}&hwid={HWID}");
                    WebClient webkit = new WebClient();
                    //webkit.Headers.Add("User-Agent", "Auth-Client/1.0");
                    webkit.Proxy = null;
                    string url = URL + "/validate?service=" + Exploit + "&key=" + Key + "&blob=" + Blob + "&hwid=" + HWID;
                    Clipboard.SetText(url);
                    Result = webkit.DownloadString(url);
                }
                catch
                {
                    MessageBox.Show($"Connection to {URL} has failed!", "Authentication Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                MessageBox.Show(Result);
                //Clipboard.SetText(Blob);
                var Buffer = Result.Split('#');
                var Parsed = new List<int>();

                if (Result.Length > 5)
                {
                    for (int i = 0; i < Buffer.Length; i += 2)
                    {
                        try
                        {
                            Parsed.Add(Int32.Parse(Buffer[i]));
                        }
                        catch
                        {
                            MessageBox.Show($"Server has returned malformed data!", "Authentication Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }

                    var Integer = 0;
                    foreach (int Value in Parsed)
                    {
                        if (((int)Key[Integer] * (int)Blob[Integer]) - ((int)Blob[Integer] + (int)Key[Integer]) != Value)
                            return false;

                        ++Integer;
                    }

                    return true;

                }
                else
                    return false;
            };
        }
    }
}