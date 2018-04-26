using System;
using System.Collections.Generic;
using System.Text;

namespace app_sys 
{  
    public sealed class Publisher<T> 
    {
        public void Publish(T message)
        {
            var copy = OnMessage;
            copy?.Invoke(this, new GenericEventArgs<T>(message));
        }

        public void PublishKey(string key, T message)
        {
            var copy = OnMessage;
            copy?.Invoke(this, new GenericEventArgs<T>(key, message));
        }

        public event EventHandler<GenericEventArgs<T>> OnMessage;
    }

    public sealed class GenericEventArgs<T> : EventArgs
    {
        public GenericEventArgs(T message)
        {
            Message = message;
            Key = string.Empty;
        }

        public GenericEventArgs(string key, T message)
        {
            Key = key;
            Message = message;
        }

        public string Key { get; }
        public T Message { get; }
    }

    public sealed class Subscriber<T>
    {
        private readonly Publisher<T> _publisher;

        public Subscriber(Publisher<T> publisher)
        {
            _publisher = publisher;
        }

        public void Subscribe(Action<GenericEventArgs<T>> onMessage)
        {
            _publisher.OnMessage += (sender, msg) => onMessage(msg);
        }
    }
}
