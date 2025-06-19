using Mem0.NET;

var client = new Mem0Client("", "http://127.0.0.1:8000/");

await client.AddAsync([
    new()
    {
        Role = "user",
        Content = "Hi, I'm Alex. I'm a vegetarian and I'm allergic to nuts."
    },

    new()
    {
        Role = "assistant",
        Content =
            "Hello Alex! I've noted that you're a vegetarian and have a nut allergy. I'll keep this in mind for any food-related recommendations or discussions."
    }
], "alex");

var response = await client.SearchAsync(new SearchRequest()
{
    Query = "What can I cook for dinner tonight?",
    UserId = "alex"
});

foreach (var memory in response.Results)
{
    Console.WriteLine($"Memory ID: {memory.Content}");
}