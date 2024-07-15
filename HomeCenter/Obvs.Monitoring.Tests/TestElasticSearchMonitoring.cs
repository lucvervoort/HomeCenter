using System;
using System.Collections.Generic;
using Elastic.Clients.Elasticsearch;
using FakeItEasy;
using Microsoft.Reactive.Testing;
//using Nest;
using Obvs.Monitoring.ElasticSearch;
using Xunit;

namespace Obvs.Monitoring.Tests
{
    public class TestElasticSearchMonitoring
    {
        [Fact]
        public void ShouldBeAbleToCreateMonitor()
        {
            /*IElasticClient*/ElasticsearchClient elasticClient = A.Fake</*IElasticClient*/ElasticsearchClient>();
            TestScheduler testScheduler = new TestScheduler();

            IMonitorFactory<TestMessage> factory = new ElasticSearchMonitorFactory<TestMessage>("indexName", new List<Type>(), "instanceName", TimeSpan.FromSeconds(1), testScheduler, elasticClient);

            IMonitor<TestMessage> monitor = factory.Create("SomeName");

            Assert.NotNull(monitor);
        }

        [Fact]
        public void ShouldNotAttemptToSaveIfNoMessages()
        {
            /*IElasticClient*/ElasticsearchClient elasticClient = A.Fake<ElasticsearchClient/*IElasticClient*/>();
            TestScheduler testScheduler = new TestScheduler();

            IMonitorFactory<TestMessage> factory = new ElasticSearchMonitorFactory<TestMessage>("indexName", new List<Type>(), "instanceName", TimeSpan.FromSeconds(1), testScheduler, elasticClient);

            IMonitor<TestMessage> monitor = factory.Create("SomeName");

            Assert.NotNull(monitor);

            testScheduler.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);

            A.CallTo(() => elasticClient.Bulk(A</*I*/BulkRequest>._)).MustNotHaveHappened();
        }

        [Fact]
        public void ShouldAttemptToSaveIfMessagesSent()
        {
            /*IElasticClient*/
            ElasticsearchClient elasticClient = A.Fake</*IElasticClient*/ElasticsearchClient>();
            TestScheduler testScheduler = new TestScheduler();

            IMonitorFactory<TestMessage> factory = new ElasticSearchMonitorFactory<TestMessage>("indexName", new List<Type>(), "instanceName", TimeSpan.FromSeconds(1), testScheduler, elasticClient);

            IMonitor<TestMessage> monitor = factory.Create("SomeName");

            monitor.MessageSent(new TestMessage(), TimeSpan.FromMilliseconds(1));

            testScheduler.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);

            A.CallTo(() => elasticClient.Bulk(A</*I*/BulkRequest>._)).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void ShouldAttemptToSaveIfMessagesReceived()
        {
            /*IElasticClient*/
            ElasticsearchClient elasticClient = A.Fake</*IElasticClient*/ElasticsearchClient>();
            TestScheduler testScheduler = new TestScheduler();

            IMonitorFactory<TestMessage> factory = new ElasticSearchMonitorFactory<TestMessage>("indexName", new List<Type>(), "instanceName", TimeSpan.FromSeconds(1), testScheduler, elasticClient);

            IMonitor<TestMessage> monitor = factory.Create("SomeName");

            monitor.MessageReceived(new TestMessage(), TimeSpan.FromMilliseconds(1));

            testScheduler.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);

            A.CallTo(() => elasticClient.Bulk(A</*I*/BulkRequest>._)).MustHaveHappened(1, Times.Exactly);
        }


        [Fact]
        public void ShouldAttemptToSaveMultipleCountersIfMessagesSent()
        {
            /*IElasticClient*/
            ElasticsearchClient elasticClient = A.Fake</*IElasticClient*/ElasticsearchClient>();
            TestScheduler testScheduler = new TestScheduler();

            var types = new List<Type> { typeof(TestMessage) };
            IMonitorFactory<TestMessage> factory = new ElasticSearchMonitorFactory<TestMessage>("indexName", types, "instanceName", TimeSpan.FromSeconds(1), testScheduler, elasticClient);

            IMonitor<TestMessage> monitor = factory.Create("SomeName");

            monitor.MessageSent(new TestMessage(), TimeSpan.FromMilliseconds(1));

            testScheduler.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);

            A.CallTo(() => elasticClient.Bulk(A</*I*/BulkRequest>.That.Matches(request => request.Operations.Count == 1 + types.Count))).MustHaveHappened(1, Times.Exactly);
        }
        
        [Fact]
        public void ShouldAttemptToSaveMultipleCountersIfMessagesReceived()
        {
            /*IElasticClient*/
            ElasticsearchClient elasticClient = A.Fake</*IElasticClient*/ElasticsearchClient>();
            TestScheduler testScheduler = new TestScheduler();

            var types = new List<Type> {typeof(TestMessage)};
            IMonitorFactory<TestMessage> factory = new ElasticSearchMonitorFactory<TestMessage>("indexName", types, "instanceName", TimeSpan.FromSeconds(1), testScheduler, elasticClient);

            IMonitor<TestMessage> monitor = factory.Create("SomeName");

            monitor.MessageReceived(new TestMessage(), TimeSpan.FromMilliseconds(1));

            testScheduler.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);

            A.CallTo(() => elasticClient.Bulk(A</*I*/BulkRequest>.That.Matches(request => request.Operations.Count == 1 + types.Count))).MustHaveHappened(1, Times.Exactly);
        }
    
        [Fact]
        public void ShouldDisposeCleanly()
        {
            /*IElasticClient*/
            ElasticsearchClient elasticClient = A.Fake</*IElasticClient*/ElasticsearchClient>();
            TestScheduler testScheduler = new TestScheduler();

            IMonitorFactory<TestMessage> factory = new ElasticSearchMonitorFactory<TestMessage>("indexName", new List<Type>(), "instanceName", TimeSpan.FromSeconds(1), testScheduler, elasticClient);

            IMonitor<TestMessage> monitor = factory.Create("SomeName");

            monitor.Dispose();
        }
    }
}
