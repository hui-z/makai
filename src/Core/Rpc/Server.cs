using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace HuiZ.Makai.Rpc
{
    public interface IServer : IDisposable
    {
        IObservable<NetMQMessage> Call(NetMQMessage msg);
    }
    public class Server : IServer
    {
        private readonly NetMQSocket _router = new RouterSocket("@tcp://127.0.0.1:9998");
        private readonly EventLoopScheduler _scheduler = new EventLoopScheduler();
        private readonly ISubject<NetMQMessage> _messages = new Subject<NetMQMessage>();
        private NetMQFrame _worker;
        private IDisposable _disposal;

        public Server()
        {
            _disposal = Observable.Create<Unit>(o =>
            {
                var liveness = 3;
                return _scheduler.Schedule(self =>
                {
                    NetMQMessage msg = null;
                    if (_router.TryReceiveMultipartMessage(Protocol.HeartbeatInterval, ref msg))
                    {
                        liveness = 3;
                        ProcessRecv(msg);
                    }
                    else if(liveness-- == 0)
                        _worker = null;
                    self();
                });
            }).Subscribe();
        }

        public void Dispose()
        {
            _scheduler.Dispose();
            _router.Dispose();
        }

        public IObservable<NetMQMessage> Call(NetMQMessage msg) => Observable.Defer(() =>
        {
            if (_worker == null) return Observable.Empty<NetMQMessage>();
            var reqId = new Guid().ToString();
            msg.Push(reqId);
            _scheduler.Schedule(() => Send(Protocol.Message, msg));
            return _messages.SelectMany(reply =>
            {
                var repId = reply.Pop().ConvertToString();
                return Observable.If(() => repId == reqId, Observable.Return(reply), Observable.Empty(reply));
            })
            .Take(1);
        })
        .SubscribeOn(_scheduler);

        private void ProcessRecv(NetMQMessage msg)
        {
            _worker = msg.Pop();
            if (_worker.ConvertToString() != Protocol.Worker)
                throw new InvalidOperationException();
            var empty = msg.Pop();
            if (!empty.IsEmpty)
                throw new InvalidOperationException();
            var type = msg.Pop().ConvertToString();
            switch(type)
            {
                case Protocol.Message:
                    _messages.OnNext(new NetMQMessage(msg));
                    break;
                case Protocol.Heartbeat:
                    Send(Protocol.Heartbeat);
                    break;
                case Protocol.Disconnect:
                    Send(Protocol.Disconnect);
                    _worker = null;
                    break;
                default:
                    throw new InvalidOperationException($"invalid type: {type}");
            }
        }
        private void Send(string msgType, NetMQMessage payload = null)
        {
            if (_worker == null) return;
            var msg = payload ?? new NetMQMessage();
            msg.Push(msgType);
            msg.PushEmptyFrame();
            msg.Push(_worker);
            _router.SendMultipartMessage(msg);
        }
    }
}
