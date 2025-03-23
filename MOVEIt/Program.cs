using System.Net.Http.Headers;
using System.Text.Json;

namespace MOVEIt;

class MOVEIt
{
    private static readonly Config config = ConfigReader.LoadConfig();

    private static readonly string moveItBaseUrl = config.MoveItBaseUrl;
    private static readonly string username = config.Username;
    private static readonly string password = config.Password;
    private static readonly string localFolderPath = config.LocalFolderPath;
    private static string? accessToken;
    private static FileSystemWatcher watcher;
    
    private static readonly HttpClient client = new();
    private static string homeFolderId;

    private static async Task Main()
    {
        await Authenticate();
        SetupFolderWatcher();
        Console.WriteLine("Monitoring folder. Press Enter to exit.");
        Console.ReadLine();
    }
    
    private static async Task Authenticate()
    {
        var content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", password)
        ]);

        HttpResponseMessage response = await client.PostAsync($"{moveItBaseUrl}/token", content);

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
            accessToken = tokenData.GetProperty("access_token").GetString();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            Console.WriteLine("Authentication successful.");

            await GetUserHomeFolder();
        }
        else
        {
            string errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Authentication failed: {response.StatusCode}, {errorDetails}");
            Environment.Exit(1);
        }
    }
    
    private static async Task GetUserHomeFolder()
    {
        HttpResponseMessage response = await client.GetAsync($"{moveItBaseUrl}/users/self");

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var userData = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
            homeFolderId = userData.GetProperty("homeFolderID").GetInt32().ToString();
        }
        else
        {
            string errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Failed to retrieve user details: {response.StatusCode}, {errorDetails}");
        }
    }


    private static void SetupFolderWatcher()
    {
        watcher = new FileSystemWatcher(localFolderPath)
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            Filter = "*.*",
            EnableRaisingEvents = true
        };
        watcher.Created += async (sender, e) => await UploadFile(e.FullPath);
    }

    private static async Task UploadFile(string filePath)
    {
        if (string.IsNullOrEmpty(homeFolderId))
        {
            Console.WriteLine("Home folder ID is not set. Cannot upload file.");
            return;
        }

        try
        {
            await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(fileStream), "file", Path.GetFileName(filePath));

            HttpResponseMessage response = await client.PostAsync($"{moveItBaseUrl}/folders/{homeFolderId}/files", content);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Uploaded: {Path.GetFileName(filePath)}");
                Console.WriteLine($"Uploading to: {moveItBaseUrl}/folders/{homeFolderId}/files");
            }
            else
            {
                string errorDetails = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to upload: {Path.GetFileName(filePath)} - {response.StatusCode} - {errorDetails}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error uploading file: {ex.Message}");
        }
    }
}