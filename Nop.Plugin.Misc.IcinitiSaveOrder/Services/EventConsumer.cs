using Nop.Core.Domain.Orders;
using Nop.Services.Events;

namespace Nop.Plugin.Misc.IcinitiSaveOrder.Services
{
    /// <summary>
    /// Represents event consumer
    /// </summary>
    public class EventConsumer : IConsumer<OrderPlacedEvent>
    {
        #region Fields

        private readonly IcinitiService _icinitiService;

        #endregion

        #region Ctor

        public EventConsumer(IcinitiService icinitiService)
        {
            _icinitiService = icinitiService;
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
            _icinitiService.HandleOrderPlacedEvent(eventMessage.Order);
        }

        #endregion
    }
}