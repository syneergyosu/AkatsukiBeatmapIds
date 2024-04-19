using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Enter the user ID:");
        string userId = Console.ReadLine();

        string url = $"https://akatsuki.gg/api/v1/users/scores/best?id={userId}&rx=1";
        HttpClient httpClient = new HttpClient();

        try
        {
            int currentPage = 1;
            const int scoresPerPage = 100;
            bool hasNextPage = true;
            int totalBeatmapCount = 0;

            string fileName = $"{userId}_beatmap_ids.txt"; // Constructing file name using user ID

            // Create a StreamWriter to write to a file
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                while (hasNextPage)
                {
                    string pageUrl = $"{url}&p={currentPage}&l={scoresPerPage}";
                    var httpResponseMessage = await httpClient.GetAsync(pageUrl);
                    string jsonResponse = await httpResponseMessage.Content.ReadAsStringAsync();
                    var responseObj = JObject.Parse(jsonResponse);
                    var scoresArray = responseObj["scores"] as JArray;

                    if (scoresArray != null)
                    {
                        Console.WriteLine($"Page {currentPage}:");
                        foreach (var score in scoresArray)
                        {
                            var beatmap = score["beatmap"];
                            var beatmapId = beatmap?["beatmap_id"]?.Value<int>();

                            if (beatmapId != null)
                            {
                                Console.WriteLine(beatmapId);
                                totalBeatmapCount++;
                                // Write beatmap ID to file
                                writer.WriteLine(beatmapId);
                            }
                        }

                        Console.WriteLine();

                        hasNextPage = scoresArray.Count == scoresPerPage;
                        currentPage++;
                    }
                    else
                    {
                        hasNextPage = false;
                    }
                }
            }

            Console.WriteLine($"Total beatmap count: {totalBeatmapCount}");
            Console.WriteLine($"Beatmap IDs written to file: {fileName}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}