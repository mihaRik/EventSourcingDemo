using System;

namespace EventSourcingDemo.ReadModel
{
    public interface IReadModel
    {
        Guid Id {  get; set; }
    }
}
