using SmartCoinOS.Domain.AuditLogs;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Clients.Events;
using SmartCoinOS.Domain.SeedWork;


namespace SmartCoinOS.Domain.Orders.Events;

public abstract class OrderAuditEventBase : ClientAuditEventBase
{
    protected OrderAuditEventBase(OrderId orderId, ClientId clientId, UserInfo auditedBy) : base(clientId, auditedBy)
    {
        Parameters.Add(new AuditLogParameter(AuditParameters.OrderId, orderId.Value.ToString()));
    }

    private static readonly object _orderLock = new();
    private static IEnumerable<string> _orderEvents = [];
    
    public static IEnumerable<string> OrderEventNames
    {
        get
        {
            if (_orderEvents.Any()) return _orderEvents;
            
            lock (_orderLock)
            {
                var assembly = typeof(OrderAuditEventBase).Assembly;

                _orderEvents = from t in assembly.GetTypes()
                    where t.IsSubclassOf(typeof(OrderAuditEventBase))
                    select t.Name;
            }

            return _orderEvents;
        }
    }
}
