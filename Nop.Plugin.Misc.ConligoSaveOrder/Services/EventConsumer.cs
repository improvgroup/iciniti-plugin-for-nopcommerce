using Nop.Core.Domain.Orders;
using Nop.Services.Events;

namespace Nop.Plugin.Misc.ConligoSaveOrder.Services
{
    /// <summary>
    /// Represents event consumer
    /// </summary>
    public class EventConsumer : IConsumer<OrderPlacedEvent>
    {
        #region Fields

        private readonly ConligoService _conligoService;

        #endregion

        #region Ctor

        public EventConsumer(ConligoService conligoService)
        {
            _conligoService = conligoService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle the order placed event
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public void HandleEvent(OrderPlacedEvent eventMessage)
        {
            //handle event
            _conligoService.HandleOrderPlacedEvent(eventMessage.Order);
        }

        #endregion
    }
}