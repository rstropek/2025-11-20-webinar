Console.WriteLine("Extension Members");

var longText = "This is a very long text that needs to be truncated.";
var shortText = longText.TruncateWithSuffix(20, "...");
//var shortText = MyExtensions.TruncateWithSuffix(longText, 20, "...");
Console.WriteLine(shortText);
Console.WriteLine(shortText.AsUpperCase);

var vec1 = new Vector2D(3, 4);
var vec2 = new Vector2D(1, 2);
vec1 += vec2;
Console.WriteLine($"Vector after addition: ({vec1.X}, {vec1.Y})");

var batman = string.GenerateBatman(16);
Console.WriteLine(batman);

var t = Task.Run(async () =>
{
    await Task.Delay(2000);
    return 42;
});
var result = await t.WithDefaultOnTimeout(TimeSpan.FromSeconds(1));
Console.WriteLine($"Task completed with result: {result}");

var tasks = new List<Task<int>>
{
    Task.Run(async () => { await Task.Delay(500); return 1; }),
    Task.Run<int>(async () => { await Task.Delay(1500); throw new Exception("Failed task"); }),
    Task.Run(async () => { await Task.Delay(300); return 3; })
};
var successfulResults = await tasks.WhenAllSuccessful();
Console.WriteLine($"Successful task results: {string.Join(", ", successfulResults)}");

var tasks2 = new List<Task<int>>
{
    Task.Run(async () => { await Task.Delay(200); return 10; }),
    Task.Run(async () => { await Task.Delay(400); return 20; }),
    Task.Run(async () => { await Task.Delay(600); return 30; })
};
var doubledResults = await tasks2.WhenAllConverted(x => x * 2);
Console.WriteLine($"Doubled task results: {string.Join(", ", doubledResults)}");

static class MyExtensions
{
    extension (string str)
    {
        public string TruncateWithSuffix(int maxLength, string suffix)
        {
            if (str.Length <= maxLength) return str;
            return string.Concat(str.AsSpan(0, maxLength), suffix);
        }

        public string AsUpperCase => str.ToUpperInvariant();
    }

    extension (DateOnly date)
    {
        public string FormatToGermanDate()
        {
            return date.ToString("dd.MM.yyyy");
        }
    }

    extension (Vector2D vector)
    {
        public void operator +=(Vector2D other)
        {
            vector.X += other.X;
            vector.Y += other.Y;
        }
    }
}

class Vector2D(double x, double y)
{
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
}

static class BatmanGenerator
{
    extension(string)
    {
        public static string GenerateBatman(int numberOfNas)
        {
            const string batman = "Batman!";

            // Generates something like "NaNaNaNaNaNaNaNaNaNaNaNaNaNaNaNa Batman!"
            // See https://youtu.be/kK4H-LkrQjQ
            if (numberOfNas < 1)
            {
                return batman;
            }

            // Each "Na" is 2 characters, so total Na characters = numberOfNas * 2
            // Total length = (numberOfNas * 2) + 1 space + batman.Length
            return String.Create(numberOfNas * 2 + batman.Length + 1, numberOfNas, (span, numberOfNas) =>
            {
                // Generate the "Na" pairs
                for (int i = 0; i < numberOfNas; i++)
                {
                    span[i * 2] = 'N';
                    span[i * 2 + 1] = 'a';
                }

                // Add space between Nas and Batman
                span[numberOfNas * 2] = ' ';

                // Copy Batman to the end
                batman.AsSpan().CopyTo(span[(numberOfNas * 2 + 1)..]);
            });
        }
    }
}

static class TaskExtensions
{
    extension<T>(Task<T> task)
    {
        public async Task<T> WithTimeout(TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);
            var completedTask = await Task.WhenAny(task, Task.Delay(timeout, cts.Token));

            if (completedTask == task)
            {
                cts.Cancel(); // Cancel the delay task
                return await task;
            }

            throw new TimeoutException($"Task timed out after {timeout}");
        }

        public async Task<T?> WithDefaultOnTimeout(TimeSpan timeout, T? defaultValue = default)
        {
            try
            {
                return await task.WithTimeout(timeout);
            }
            catch (TimeoutException)
            {
                return defaultValue;
            }
        }
    }

    extension<T>(IEnumerable<Task<T>> tasks)
    {
        public async Task<IEnumerable<T>> WhenAllSuccessful()
        {
            // Similar to Task.WhenAll, but returns only successful results

            var todo = tasks.ToList();
            var results = new List<T>();

            while (todo.Count != 0)
            {
                var completed = await Task.WhenAny(todo);
                todo.Remove(completed);

                try
                {
                    results.Add(await completed);
                }
                catch
                {
                    // Log error if needed, but continue with other tasks
                    Console.Error.WriteLine("Something bad has happened...");
                }
            }

            return results;
        }

        public async Task<IEnumerable<TResult>> WhenAllConverted<TResult>(Func<T, TResult> converter)
        {
            var results = await Task.WhenAll(tasks);
            return results.Select(converter);
        }
    }
}
