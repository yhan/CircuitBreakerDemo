namespace WebAPI.Controllers
{
    public class CircuitBreakerSimulationHelper
    {
        private bool _closed = true;

        public bool Closed()
        {
            var nowClosed = _closed;
            _closed = !_closed;
            return nowClosed;
        }
    }
}