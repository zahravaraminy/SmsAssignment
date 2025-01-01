using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace SmsRateLimiter
{
    [ApiController]
    [Route("api/[controller]")]
    public class SmsController : ControllerBase
    {
        
        private readonly ConcurrentDictionary<string, SlidingWindow>  _perNumberLimits;
        private readonly SlidingWindow _accountLimit; // Example limit: 10 messages/second
        private readonly int _numberLimit; // Example limit: 5 messages/second per number
        private readonly ConcurrentDictionary<string, AccountMonitorModel> _realTimeAccountData;

        private readonly ConcurrentDictionary<string, List<NumberMonitorModel>> _realTimeNumberData =
            new ConcurrentDictionary<string, List<NumberMonitorModel>>();



         public SmsController( 
            ConcurrentDictionary<string, List<NumberMonitorModel>> realTimeNumberData,
            ConcurrentDictionary<string, AccountMonitorModel> realTimeAccountData,
            ConcurrentDictionary<string, SlidingWindow>? perNumberLimits = null,
            SlidingWindow? accountLimit = null,
            int numberLimit = 5) // Default limit: 5 messages per second
        {
            _perNumberLimits = perNumberLimits ?? new ConcurrentDictionary<string, SlidingWindow>();
            _accountLimit = accountLimit ?? new SlidingWindow(10); // Default limit: 10 messages per second
            _numberLimit = numberLimit;
            _realTimeAccountData = realTimeAccountData;
            _realTimeNumberData=  realTimeNumberData; 
        }

        [HttpPost("can-send")]
        public IActionResult CanSend([FromBody] SmsRequest request)
        {
             if (request == null)
            {
                return BadRequest("Invalid request payload.");
            }

            var timestamp = DateTime.UtcNow;
            var accountId = request.AccountId; // Assume this is part of the request model

            // Log the request
            Console.WriteLine($"[{timestamp}] Received request for account: {accountId}, phone number: {request.PhoneNumber}");

            // Check per-number limit
            var numberWindow = _perNumberLimits.GetOrAdd(request.PhoneNumber, _ => new SlidingWindow(_numberLimit));
            if (!numberWindow.TryAddMessage())
                return BadRequest("Per-number limit exceeded.");


            // Check account-wide limit
            if (!_accountLimit.TryAddMessage())
            {
                numberWindow.Rollback(); // Rollback per-number counter
                return BadRequest("Account-wide limit exceeded." );

            }

            // Update account data
            _realTimeAccountData.AddOrUpdate(
                accountId,
             new AccountMonitorModel
            {
                AccountId = accountId,
                Date = timestamp,
                MessagesSent = 1
            },
            (key, existing) =>
            {
                existing.MessagesSent += 1; // Increment message count
                existing.Date = timestamp;  // Update timestamp
                return existing;
            });

        
            // Update real-time number data
            _realTimeNumberData.AddOrUpdate(
                request.PhoneNumber,
                new List<NumberMonitorModel>
                {
                    new NumberMonitorModel
                {
                    PhoneNumber = request.PhoneNumber,
                    Date = timestamp,
                    MessagesSent = 1
                }
            },
            (key, existingList) =>
            {
                var existingEntry = existingList.FirstOrDefault(x => x.Date.Minute == timestamp.Minute);
                if (existingEntry != null)
                {
                    existingEntry.MessagesSent += 1; // Increment the message count
                }
                else
                {
                    existingList.Add(new NumberMonitorModel
                    {
                        PhoneNumber = request.PhoneNumber,
                        Date = timestamp,
                        MessagesSent = 1
                    });
                }
                return existingList;
            });

            return Ok("Message can be sent.");
        }

        [HttpGet("monitor/account")]
        public IActionResult GetAccountStats()
        {
            return Ok(_accountLimit.GetStats());
        }

        [HttpGet("monitor/number")]
        public IActionResult GetPerNumberStats()
        {
            var stats = _perNumberLimits.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetStats());
            return Ok(stats);
        }
    }

    public class SmsRequest
    {
    public string PhoneNumber { get; set; } // Existing property
    public string AccountId { get; set; }   // Add this property
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
        public List<DateTime> GetWindowData(DateTime startDate, DateTime endDate)
        {
            lock (_timestamps)
            {
            return _timestamps.Where(ts => ts >= startDate && ts <= endDate).ToList();
            }
        }
    }
}
