using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using RabbitMQ.Client.Events;

namespace Obvs.RabbitMQ.Extensions
{
    internal static class RabbitExtensions
    {
        public static IObservable<BasicDeliverEventArgs> ToObservable(this AsyncEventingBasicConsumer consumer)
        {
            return Observable.Create<BasicDeliverEventArgs>(
                observer =>
                {
                    EventHandler<BasicDeliverEventArgs> onReceived = (sender, args) => observer.OnNext(args);
                    EventHandler<ConsumerEventArgs> consumerCancelled = (sender, args) => observer.OnCompleted();
                    
                    consumer.ReceivedAsync += onReceived;
                    consumer.ConsumerCancelled += consumerCancelled;

                    return Disposable.Create(
                        () =>
                        {
                            consumer.ReceivedAsync -= onReceived;
                            consumer.ConsumerCancelled -= consumerCancelled;
                        });
                });
        }
    }
}