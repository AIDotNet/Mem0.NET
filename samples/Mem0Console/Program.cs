using Mem0.NET;

var client = new Mem0Client("", "https://api.mem0.ai/v2");

// messages = [
//     {"role": "user", "content": "Hi, I'm Alex. I'm a vegetarian and I'm allergic to nuts."},
//     {"role": "assistant", "content": "Hello Alex! I've noted that you're a vegetarian and have a nut allergy. I'll keep this in mind for any food-related recommendations or discussions."}
// ]
// client.add(messages, user_id="alex")

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
// client.search(query, user_id="alex")

var response = await client.SearchAsync(query, userId: "alex");

foreach (var memory in response)
{
    Console.WriteLine($"Memory ID: {memory.Id}");
}