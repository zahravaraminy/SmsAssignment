using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace SmsRateLimiter
{
    [ApiController]
    [Route("api/[controller]")]
    public class SmsController : ControllerBase
    {
        
        private readonly ConcurrentDictionary<string, SlidingWindow> PerNumberLimits;
        private readonly SlidingWindow AccountLimit; // Example limit: 10 messages/second
        private readonly int NumberLimit; // Example limit: 5 messages/second per number

         public SmsController(
        ConcurrentDictionary<string, SlidingWindow>? perNumberLimits = null,
        SlidingWindow? accountLimit = null,
        int numberLimit = 5) // Default limit: 5 messages per second
    {
        PerNumberLimits = perNumberLimits ?? new ConcurrentDictionary<string, SlidingWindow>();
        AccountLimit = accountLimit ?? new SlidingWindow(10); // Default limit: 10 messages per second
        NumberLimit = numberLimit;
    }
        [HttpPost("can-send")]
        public IActionResult CanSend([FromBody] SmsRequest request)
        {
            var timestamp = DateTime.UtcNow;
            Console.WriteLine($"[{timestamp}] Received request for phone number: {request.PhoneNumber}");
            // Check per-number limit
            var numberWindow = PerNumberLimits.GetOrAdd(request.PhoneNumber, _ => new SlidingWindow(NumberLimit));
            if (!numberWindow.TryAddMessage())
                return BadRequest("Per-number limit exceeded.");

            // Check account-wide limit
            if (!AccountLimit.TryAddMessage())
            {
                numberWindow.Rollback(); // Revert number counter if account-wide fails
                return BadRequest("Account-wide limit exceeded.");
            }

            return Ok("Message can be sent.");
        }

        [HttpGet("monitor/account")]
        public IActionResult GetAccountStats()
        {
            return Ok(AccountLimit.GetStats());
        }

        [HttpGet("monitor/number")]
        public IActionResult GetPerNumberStats()
        {
            var stats = PerNumberLimits.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetStats());
            return Ok(stats);
        }
    }

    public class SmsRequest
    {
        public string PhoneNumber { get; set; }
    }

    public class SlidingWindow
    {
        private readonly int _limit;
        private readonly ConcurrentQueue<DateTime> _timestamps = new();

        public SlidingWindow(int limit) => _limit = limit;

        public bool TryAddMessage()
        {
            var now = DateTime.UtcNow;

            lock (_timestamps)
            {
                while (_timestamps.TryPeek(out var ts) && (now - ts).TotalSeconds >= 1)
                    _timestamps.TryDequeue(out _);

                if (_timestamps.Count >= _limit) return false;

                _timestamps.Enqueue(now);
                return true;
            }
        }

        public void Rollback()
        {
            lock (_timestamps)
            {
                _timestamps.TryDequeue(out _);
            }
        }

        public int GetStats()
    {
        lock (_timestamps)
        {
            return _timestamps.Count;
        }
    }
    }
}
