using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace HuiZ.Makai.Rpc
{
    public interface IWorker : IObservable<Unit>
    {

    }
    public class Worker : IWorker
    {
        private readonly IObservable<Unit> _producer;
        public Worker()
        {
            _producer = Observable.Using(() => new DealerSocket(">tcp://127.0.0.1:9998"), worker => Observable.Create<Unit>(o =>
            {
                worker.Options.Identity = Encoding.Default.GetBytes(Protocol.Worker);
                var heartbeat = Observable.Interval(Protocol.HeartbeatInterval)
                    .Do(_ => Send(worker, Protocol.Heartbeat));
                var liveness = 3;
                var recv = NewThreadScheduler.Default.Schedule(self =>
                {
                    NetMQMessage msg = null;
                    if (worker.TryReceiveMultipartMessage(Protocol.HeartbeatInterval, ref msg))
                    {
                        liveness = 3;
                        ProcessRecv(msg);
                    }
                    else if (liveness-- == 0)
                    {
                        Send(worker, Protocol.Disconnect);
                        throw new ApplicationException("server down, reconnect");
                    }
                    self();
                });
                return new CompositeDisposable(heartbeat.Subscribe(), recv);
            })).Retry();
        }
        private void ProcessRecv(NetMQMessage msg)
        {
            var type = msg.Pop().ConvertToString();
            switch(type)
            {
                case Protocol.Message:
                    break;
                case Protocol.Heartbeat:
                    break;
                case Protocol.Disconnect:
                    throw new ApplicationException("server down, reconnect");
            }
        }

        private void Send(NetMQSocket worker, string msgType, NetMQMessage payload = null)
        {
            var msg = payload ?? new NetMQMessage();
            msg.Push(msgType);
            worker.SendMultipartMessage(msg);
        }

        public IDisposable Subscribe(IObserver<Unit> observer)
        {
            throw new NotImplementedException();
        }
    }
}
