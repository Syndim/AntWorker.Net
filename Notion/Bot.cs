using global::Notion.Client;

namespace AntWorker.Net.Notion
{
    internal class Bot
    {
        private NotionClient _client;

        public Bot(string apiKey)
        {
            _client = NotionClientFactory.Create(new ClientOptions
            {
                AuthToken = apiKey
            });
        }

        public async Task CreateTaskPageAsync(string parentId)
        {
            var today = Today;
            Logging.LogInfo($"Creating task page for {parentId}, date: {today}");
            var page = await _client.Pages.CreateAsync(new PagesCreateParameters
            {
                Parent = new DatabaseParentInput
                {
                    DatabaseId = parentId
                },
                Properties = new Dictionary<string, PropertyValue>
                {
                    {
                        "Name",
                        new TitlePropertyValue
                         {
                            Title = new List<RichTextBase>
                            {
                                 new RichTextText
                                 {
                                     Text = new Text
                                     {
                                         Content = today
                                     }
                                 }
                            }
                        }
                    }
                }
            });

            Logging.LogInfo($"Page created: {page.Url}");
        }

        public async Task ArchiveAsync(string databaseId)
        {
            var filter = new SelectFilter("Status", equal: "Completed");
            var pages = await _client.Databases.QueryAsync(databaseId, new DatabasesQueryParameters { Filter = filter });
            var properties = new Dictionary<string, PropertyValue>
            {
                { "Status", new SelectPropertyValue { Select = new SelectOption { Name = "Archived" } } }
            };

            foreach (var page in pages.Results)
            {
                Logging.LogInfo($"Archiving {page.Id}");
                await _client.Pages.UpdatePropertiesAsync(page.Id, properties);
            }
        }

        public async Task SetCompleteDateAsync(string databaseId)
        {
            var filter = new CompoundFilter(and: new List<Filter>
                    {
                        new SelectFilter("Status", equal: "Completed"),
                        new DateFilter("Date Completed", isEmpty: true)
                    });
            var today = DateTime.Today;
            var properties = new Dictionary<string, PropertyValue>
            {
                { "Date Completed", new DatePropertyValue { Date = new Date { Start = DateTime.Parse($"{today.Year}-{today.Month}-{today.Day}") } } }
            };

            var pages = await _client.Databases.QueryAsync(databaseId, new DatabasesQueryParameters { Filter = filter });
            foreach (var page in pages.Results)
            {
                Logging.LogInfo($"Updating complete date for {page.Id}");
                await _client.Pages.UpdatePropertiesAsync(page.Id, properties);
            }
        }

        private static string Today
        {
            get
            {
                var now = DateTime.UtcNow;
                var cst = TimeZoneInfo.ConvertTime(now, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));

                return cst.ToString("yyyy年MM月dd日");
            }
        }
    }
}
