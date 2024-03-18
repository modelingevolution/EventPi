using EventPi.Abstractions;
using Microsoft.Extensions.Logging;

namespace EventPi.Services.MachineWorkTime;

public interface IRfidState
{
    bool IsMachineWorking { get; set; }
    DateTime? LastSwipeDate { get; }
    bool ShouldSwipe(string cardId);
    void ClearCardId();
    void SetCardId(string cardId);
    string GetCardId();
}

class RfidState : IRfidState
{
    private IDateTimeProvider _time;
    private ILogger<RfidState> _logger;
    public bool IsMachineWorking { get; set; } = false;
    public DateTime? LastSwipeDate { get; private set; } = null;
    
    private string? _cardId;

    public RfidState(IDateTimeProvider provider, ILogger<RfidState> logger)
    {
        _logger = logger;
        _time = provider;
    }
    public bool ShouldSwipe(string cardId)
    {
        _logger.LogInformation($"Current time: {_time.Now}");
        if(LastSwipeDate.HasValue)
            _logger.LogInformation($"LastSwipeDate time: {LastSwipeDate.Value}");
        return _cardId != cardId ||
               (LastSwipeDate.HasValue &&
                _time.Now.Subtract(LastSwipeDate.Value) > TimeSpan.FromSeconds(5));
    }

    public void ClearCardId()
    {
        _logger.LogInformation($"SetCardId time: {_time.Now}");
        _cardId = string.Empty;
        LastSwipeDate = _time.Now;
    }
    public void SetCardId(string cardId)
    {
        _logger.LogInformation($"SetCardId time: {_time.Now}");
        _cardId = cardId;
        LastSwipeDate = _time.Now;
    }
    public string GetCardId()
    {
        return string.IsNullOrEmpty(_cardId) ? string.Empty : _cardId;
    }
}