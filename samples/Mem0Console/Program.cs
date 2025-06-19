using Mem0.NET;

var client = new Mem0Client("", "http://0.0.0.0:8000/");

await client.AddAsync(new List<Message>()
{
    new Message()
    {
        Role = "user",
        Content = "Hi, I'm Alex. I'm a vegetarian and I'm allergic to nuts."
    },
    new Message()
    {
        Role = "assistant",
        Content =
            "Hello Alex! I've noted that you're a vegetarian and have a nut allergy. I'll keep this in mind for any food-related recommendations or discussions."
    }
}, "alex");

var query = "What can I cook for dinner tonight?";
var response = await client.SearchAsync(new SearchRequest()
{
    Query = query,
    UserId = "alex"
});

foreach (var memory in response.results)
{
    Console.WriteLine($"Memory ID: {memory.Content}");
}