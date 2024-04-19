using Castle.Core.Logging;
using EventPi.Abstractions;
using EventPi.Events.MachineWork;
using Microsoft.Extensions.Logging;
using ModelingEvolution.Plumberd;
using NSubstitute;
using TechTalk.SpecFlow;

namespace EventPi.Services.MachineWorkTime.Tests
{
    [Binding]
    public class CardSwipesSteps
    {
        private IRfidHandler _sut;
        private IEventStoreStream _eventStoreStreamMock;
        private IDateTimeProvider _dateTimeProviderMock;
        private string _card = "!23;";
        private DateTime _now = DateTime.Now.Date;

        private void ClearEventStoreMock()
        {
            this._eventStoreStreamMock.ClearReceivedCalls();
        }

        [Given(@"system is running")]
        public void GivenSystemIsRunning()
        {
            this._eventStoreStreamMock = NSubstitute.Substitute.For<IEventStoreStream>();
            this._dateTimeProviderMock = NSubstitute.Substitute.For<IDateTimeProvider>();
            this._dateTimeProviderMock.Now.Returns(_now);
            var logger = NSubstitute.Substitute.For<ILogger<RfidHandler>>();
            var rfidModel = new RfidState(_dateTimeProviderMock, NSubstitute.Substitute.For<ILogger<RfidState>>());
            _sut = new RfidHandler(_eventStoreStreamMock, rfidModel, logger, new HostEnvironment());
        }
        [When(@"I swipe a card")]
        [When(@"I swipe the card again")]
        public async Task WhenISwipeACard()
        {
            await _sut.Post(new RfidRequest() { CardId = _card });
        }
        [Then(@"WorkOnMachineStarted event appeared")]
        public void ThenWorkOnMachineStartedEventAppeared()
        {
            this._eventStoreStreamMock.Received(1).Write(Arg.Any<Guid>(),
                Arg.Is<WorkOnMachineStarted>(x => x.CardNumber == _card));
            ClearEventStoreMock();
        }
        [Then(@"WorkOnMachineStopped event appeared")]
        public void ThenWorkOnMachineStoppedEventAppeared()
        {
            this._eventStoreStreamMock.Received(1).Write(Arg.Any<Guid>(),
                Arg.Is<WorkOnMachineStopped>(x => x.SwipedWithCard == _card));
            ClearEventStoreMock();

        }
        [Then(@"I waited (.*) more seconds")]
        [When(@"I waited (.*) seconds")]
        public void WhenIWaitedSeconds(int p0)
        {
            var last = _dateTimeProviderMock.Now;
            _dateTimeProviderMock.Now.Returns(last.AddSeconds(p0));
        }

        [Then(@"no event should appear")]
        public void ThenNoEventShouldAppear()
        {
            this._eventStoreStreamMock.DidNotReceiveWithAnyArgs().Write(Arg.Any<Guid>(), Arg.Any<IEvent>());
            ClearEventStoreMock();
        }



    }
}